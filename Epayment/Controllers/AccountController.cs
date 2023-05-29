using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCXN.Data;
using BCXN.Models;
using BCXN.ViewModels;
using Epayment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace BCXN.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;
        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, RoleManager<ApplicationRole> roleManager, ILogger<HomeController> logger, ApplicationDbContext context)
        {
            this.signInManager = signInManager;
            this._userManager = userManager;
            _context = context;
            this._configuration = configuration;
            this._roleManager = roleManager;
            this._logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignOff()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Route("LoginApiDVTT")]
        [AllowAnonymous]
        public async Task<object> LoginApiDVTT([FromBody] LoginDVTTViewModel model)
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
                        { "deviceId", model.deviceId },
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
                            LoginResultViewModel loginResult = new LoginResultViewModel();
                            loginResult.Token = null;
                            loginResult.Username = model.Username;
                            loginResult.AccessFailedCount = user.AccessFailedCount;
                            return loginResult;
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
                                                        where cn.Type == 2
                                                        select cn.ClaimValue;
                                var listChucNang = listChucNangQuery.ToList();
                                string tokenString = await GenerateJwtToken(model.Username, appUser, appUser.NhomQuyenId, listChucNang);
                                return new LoginResultViewModel(appUser, tokenString, listChucNang, chucNangDefault);
                            }
                            else
                            {
                                LoginResultViewModel loginResult = new LoginResultViewModel();
                                loginResult.Token = null;
                                loginResult.Username = model.Username;
                                return loginResult;
                            }
                        }
                    }
                    else
                    {
                        LoginResultViewModel loginResult = new LoginResultViewModel();
                        loginResult.Token = null;
                        loginResult.Username = model.Username;
                        return loginResult;
                    }
                }
            }
            catch (Exception ex)
            { _logger.LogInformation("username login: " + ex.Message); }
            return null;
        }

        [HttpPost]
        [Route("LoginApi")]
        [AllowAnonymous]
        public async Task<object> LoginApi([FromBody] LoginViewModel model)
        {
            //var appUser1 = _userManager.Users.SingleOrDefault(r => r.UserName == model.Email);
            //string hash = _userManager.PasswordHasher.HashPassword(appUser1,model.Password);
            _logger.LogInformation("username login: " + model.Email);
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            var checkActiveUser = _userManager.Users.FirstOrDefault(x => x.UserName == model.Email);
            if (result.Succeeded)
            {
                if (checkActiveUser != null && checkActiveUser.IsActive == true)
                {
                    // var appUser = _userManager.Users.SingleOrDefault(r => r.UserName == model.Email);
                    var appUser = _userManager.Users.Where(x => x.UserName == model.Email).Select(u => new ApplicationUser
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
                                            where cn.Type == 2
                                            select cn.ClaimValue;
                    var listChucNang = listChucNangQuery.ToList();
                    string tokenString = await GenerateJwtToken(model.Email, appUser, appUser.NhomQuyenId, listChucNang);

                    return new LoginResultViewModel(appUser, tokenString, listChucNang, chucNangDefault);
                }
                return new Response("DANG NHAP KHONG THANH CONG", null, "01", false);
            }
            else
            {
                var user = _userManager.Users.Where(x => x.UserName == model.Email).FirstOrDefault();
                if (user != null)
                {
                    user.AccessFailedCount = user.AccessFailedCount + 1;
                    await _userManager.UpdateAsync(user);
                }                
                LoginResultViewModel loginResult = new LoginResultViewModel();
                loginResult.Token = null;
                loginResult.Username = model.Email;
                loginResult.AccessFailedCount = user.AccessFailedCount;
                return loginResult;
            }

            throw new ApplicationException("INVALID_LOGIN_ATTEMPT");
        }

        [HttpPost]
        [Route("SmartEvnLoginApi")]
        [AllowAnonymous]
        public async Task<object> SmartEvnLoginApi([FromBody] LoginViewModel model)
        {
            string username = null;
            var keyBytes = Convert.FromBase64String(_configuration["SmartEVN:PublicKey"]);
            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(keyBytes);
            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
            RSAParameters rsaParameters = new RSAParameters
            {
                Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned(),
                Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned()
            };
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(rsaParameters);

            var handler = new JwtSecurityTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                RequireSignedTokens = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                IssuerSigningKey = new RsaSecurityKey(rsa),
            };
            try
            {
                var result = handler.ValidateToken(model.token, validationParameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                username = jwtToken.Subject;

                var appUser = _userManager.Users.Where(x => x.UserName == username).Select(u => new ApplicationUser
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    DonViId = u.DonViId,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    ChangePassWordDate = u.ChangePassWordDate
                }).FirstOrDefault();

                if (appUser == null)
                {
                    LoginResultViewModel loginResult = new LoginResultViewModel();
                    loginResult.Token = null;
                    loginResult.Username = username;
                    return loginResult;
                }

                var user = _userManager.Users.FirstOrDefault(u => u.Id == appUser.Id);
                IList<string> listRole = await _userManager.GetRolesAsync(user);
                if (listRole != null && listRole.Count > 0)
                    appUser.NhomQuyenId = _roleManager.Roles.Single(r => r.Name == listRole.Single()).Id;
                var chucNangDefault = _roleManager.Roles.Single(r => r.Name == listRole.Single()).ChucNangDefault;
                var listChucNangQuery = from cnn in _context.ChucNangNhom
                                        join cn in _context.ChucNang on cnn.ChucNangId equals cn.Id
                                        where cnn.NhomQuyenId == appUser.NhomQuyenId
                                        where cn.Type == 2
                                        select cn.ClaimValue;
                var listChucNang = listChucNangQuery.ToList();
                string tokenString = await GenerateJwtToken(username, appUser, appUser.NhomQuyenId, listChucNang);

                return new LoginResultViewModel(appUser, tokenString, listChucNang, chucNangDefault);
            }
            catch (Exception e)
            {
                LoginResultViewModel loginResult = new LoginResultViewModel();
                loginResult.Token = null;
                loginResult.Username = username;
                loginResult.Message = e.Message;
                return loginResult;
            }
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
        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }

    }

    public class LoginDVTTViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string deviceId { get; set; }
    }
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
        public string token { get; set; }
    }

    public class XacNhanChuyenTienViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class LoginResultViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public int? DonViId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string NhomQuyenId { get; set; }
        public string chucNangDefault { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? ChangePassWordDate { get; set; }
        public List<string> ListChucNang { get; set; }
        public string Message { get; set; }

        public LoginResultViewModel()
        { }
        public LoginResultViewModel(ApplicationUser user, string token, List<string> listChucNang, string chucNangDefaultParam, string message = "")
        {
            Id = user.Id;
            DonViId = user.DonViId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            Username = user.UserName;
            Token = token;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePassWordDate = user.ChangePassWordDate;
            ListChucNang = listChucNang;
            chucNangDefault = chucNangDefaultParam;
            Message = message;
        }
    }

}