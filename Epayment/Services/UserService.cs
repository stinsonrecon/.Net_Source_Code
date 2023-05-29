using Microsoft.Extensions.Configuration;
using BCXN.Data;
using BCXN.Models;
using Epayment.Controllers;
using Epayment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCXN.Controllers;
using BCXN.ViewModels;
using System.Net.Http;
using Newtonsoft.Json;

namespace Epayment.Services
{
    public interface IUserService
    {
        Task<Response> Authenticate(AuthenticateRequest model, string ipAddress);
        Task<Response> AuthenticateID(AuthenticateRequest model, string ipAddress);
        
        Task<Response> RefreshToken(string token, string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetById(int id);
    }

    public class UserService : IUserService
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;

        public UserService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<ApplicationRole> roleManager, ILogger<UserService> logger, ApplicationDbContext context)
        {
            this.signInManager = signInManager;
            this._userManager = userManager;
            _context = context;
            this._configuration = configuration;
            this._roleManager = roleManager;
            this._logger = logger;
        }

        public async Task<Response> Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var result = await signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);

            if (result.Succeeded)
            {
                var checkActiveUser = _userManager.Users.FirstOrDefault(x => x.UserName == model.Username);
                if (checkActiveUser != null && checkActiveUser.IsActive == true)
                {
                    // var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.Email);
                    var appUser = _userManager.Users.Where(x => x.UserName == model.Username).Select(u => new ApplicationUser
                    {
                        Id = u.Id,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        DonViId = u.DonViId,
                        UserName = u.UserName,
                        PhoneNumber = u.PhoneNumber,
                        // NhomQuyenId = u.NhomQuyenId
                        ChangePassWordDate = u.ChangePassWordDate
                    }).FirstOrDefault();

                    // foreach (var m in appUser)
                    // {
                    var user = _userManager.Users.FirstOrDefault(u => u.Id == appUser.Id);
                    IList<string> listRole = await _userManager.GetRolesAsync(user);
                    if (listRole != null && listRole.Count > 0)
                        appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                    var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;
                    // m.ApplicationRoles = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Name;
                    // }
                    var listChucNangQuery = from cnn in _context.ChucNangNhom
                                            join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                            where cnn.NhomQuyenId == appUser.NhomQuyenId
                                            select cn.ClaimValue;
                    var listChucNang = listChucNangQuery.ToList();
                    string tokenString = await GenerateJwtToken(model.Username, appUser, appUser.NhomQuyenId, listChucNang);
                    var refreshToken = generateRefreshToken(ipAddress);

                    // save refresh token
                    checkActiveUser.RefreshTokens.Add(refreshToken);
                    _context.Update(checkActiveUser);
                    _context.SaveChanges();

                    return new Response("Dang nhap thanh cong", new AuthenticateResponse(appUser, tokenString, refreshToken.Token, listChucNang, chucNangDefault), "00", true);
                }
                return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);

            }
            return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);


        }
        public async Task<Response> AuthenticateID(AuthenticateRequest model, string ipAddress)
        {

            try
            {
                //gọi login tới api của hrms
                Dictionary<string, object> values = new Dictionary<string, object>
                {
                    { "username", model.Username.Replace(@"\\", @"\") },
                    { "password", model.Password },
                    { "expiration", 60},
                    { "deviceInfo", new Dictionary<string, string>{
                        { "deviceId", model.DeviceId },
                        { "deviceType", "windows-10/desktop/MS-Edge-Chromium"},
                        { "appId", _configuration["appId"]},
                        { "appVersion", _configuration["appVersion"]}
                    } }
                };
                using (var client = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(values), Encoding.UTF8, "application/json");
                    var resultResponse = client.PostAsync(_configuration["TableauSetting:hrmAuthenticationEndpoint"], stringContent);
                    if (resultResponse.Result.Content != null)
                    {
                        var responseContent = resultResponse.Result.Content.ReadAsStringAsync();
                        resultResponse.Wait();
                        var output = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent.Result);
                        if ((bool)output["state"] == false)
                        {
                            var user = _userManager.Users.Where(x => x.UserName == model.Username).FirstOrDefault();
                            if (user != null)
                            {
                                user.AccessFailedCount = user.AccessFailedCount + 1;
                            }
                            await _userManager.UpdateAsync(user);
                            //LoginResultViewModel loginResult = new LoginResultViewModel();
                            //loginResult.Token = null;
                            //loginResult.Username = model.Username;
                            //loginResult.AccessFailedCount = user.AccessFailedCount;
                            //return loginResult;

                            new Response("DANG NHAP KHONG THANH CONG", null, "01", false);
                        }
                        else
                        {
                            // nếu ok, trả token, chức năng của user
                            var checkActiveUser = _userManager.Users.FirstOrDefault(x => x.UserName == model.Username);
                            if (checkActiveUser != null && checkActiveUser.IsActive == true)
                            {                                
                                if (checkActiveUser.AccessFailedCount > 0)
                                {
                                    checkActiveUser.AccessFailedCount = 0;
                                    await _userManager.UpdateAsync(checkActiveUser);
                                }

                                var appUser = new ApplicationUser
                                {
                                    Id = checkActiveUser.Id,
                                    FirstName = checkActiveUser.FirstName,
                                    LastName = checkActiveUser.LastName,
                                    Email = checkActiveUser.Email,
                                    DonViId = checkActiveUser.DonViId,
                                    UserName = checkActiveUser.UserName,
                                    PhoneNumber = checkActiveUser.PhoneNumber,
                                    ChangePassWordDate = checkActiveUser.ChangePassWordDate
                                };

                                // foreach (var m in appUser)
                                // {
                                //var user = _userManager.Users.FirstOrDefault(u => u.Id == appUser.Id);
                                //IList<string> listRole = await _userManager.GetRolesAsync(user);
                                IList<string> listRole = await _userManager.GetRolesAsync(checkActiveUser);
                                if (listRole != null && listRole.Count > 0)
                                    appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                                var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;
                                // m.ApplicationRoles = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Name;
                                // }
                                var listChucNangQuery = from cnn in _context.ChucNangNhom
                                                        join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                                        where cnn.NhomQuyenId == appUser.NhomQuyenId
                                                        where cn.Type == 1
                                                        select cn.ClaimValue;
                                var listChucNang = listChucNangQuery.ToList();
                                string tokenString = await GenerateJwtToken(model.Username, appUser, appUser.NhomQuyenId, listChucNang);                                
                                var refreshToken = generateRefreshToken(ipAddress);

                                // save refresh token
                                checkActiveUser.RefreshTokens.Add(refreshToken);
                                _context.Update(checkActiveUser);
                                _context.SaveChanges();

                                return new Response("Dang nhap thanh cong", new AuthenticateResponse(appUser, tokenString, refreshToken.Token, listChucNang, chucNangDefault), "00", true);
                            }
                            else
                            {
                                //LoginResultViewModel loginResult = new LoginResultViewModel();
                                //loginResult.Token = null;
                                //loginResult.Username = model.Username;
                                //return loginResult;
                                return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);

                            }
                        }
                    }
                    else
                    {
                        //LoginResultViewModel loginResult = new LoginResultViewModel();
                        //loginResult.Token = null;
                        //loginResult.Username = model.Username;
                        //return loginResult;
                        return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);

                    }
                }
            }
            catch (Exception ex)
            { _logger.LogInformation("username login: " + ex.Message); }
            return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);           
        }
        private async Task<string> GenerateJwtToken(string userName, ApplicationUser user, string permissions, List<string> listChucNang)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                // new Claim(ClaimTypes.Role, permissions),
            };

            if (listChucNang.Count() > 0)
            {
                foreach (var chucNang in listChucNang)
                {
                    if (chucNang != null)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, chucNang));
                    }
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration["JwtIssuer"],
                _configuration["JwtIssuer"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<Response> RefreshToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            _context.Update(user);
            _context.SaveChanges();

            // var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.Email);
            var appUser = _userManager.Users.Where(x => x.UserName == user.UserName).Select(u => new ApplicationUser
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                DonViId = u.DonViId,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                // NhomQuyenId = u.NhomQuyenId
                ChangePassWordDate = u.ChangePassWordDate
            }).FirstOrDefault();

            // foreach (var m in appUser)
            // {

            IList<string> listRole = await _userManager.GetRolesAsync(appUser);
            if (listRole != null && listRole.Count > 0)
                appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
            var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;
            // m.ApplicationRoles = roleManager.Roles.Single(r => r.Name == userManager.GetRolesAsync(user).Result.Single()).Name;
            // }
            var listChucNangQuery = from cnn in _context.ChucNangNhom
                                    join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                    where cnn.NhomQuyenId == appUser.NhomQuyenId
                                    select cn.ClaimValue;
            var listChucNang = listChucNangQuery.ToList();
            string tokenString = await GenerateJwtToken(user.UserName, appUser, appUser.NhomQuyenId, listChucNang);

            // generate new jwt
            //var jwtToken = generateJwtToken(user);

            return new Response("Lay token thanh cong", new AuthenticateResponse(user, tokenString, newRefreshToken.Token, listChucNang, chucNangDefault), "00", true);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _context.Users.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));

            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            _context.Update(user);
            _context.SaveChanges();

            return true;
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return _userManager.Users;
        }

        public ApplicationUser GetById(int id)
        {
            return _context.Users.Find(id);
        }

        // helper methods

        private string generateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._configuration["JwtKey"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"])).AddMinutes(5),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
