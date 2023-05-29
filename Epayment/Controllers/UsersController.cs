using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BCXN.Models;
using Epayment.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Epayment.Controllers
{
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public async Task<object> Authenticate([FromBody] AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model, ipAddress());

            if (response == null)
                return Ok(new { message = "Username or password is incorrect" });
            if (response.Data != null)
            {
                AuthenticateResponse resAuth = (AuthenticateResponse)response.Data;
                setTokenCookie(resAuth.RefreshToken);
            }
            return Ok(response);
        }
        [HttpPost("authenticateID")]
        public async Task<object> AuthenticateID([FromBody] AuthenticateRequest model)
        {
            var response = await _userService.AuthenticateID(model, ipAddress());

            if (response == null)
                return Ok(new { message = "Username or password is incorrect" });
            if (response.Data != null)
            {
                AuthenticateResponse resAuth = (AuthenticateResponse)response.Data;
                setTokenCookie(resAuth.RefreshToken);
            }
            return Ok(response);
        }
        [HttpPost("refresh-token")]
        public async Task<object> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response =await _userService.RefreshToken(refreshToken, ipAddress());

            if (response == null)
                return Unauthorized(new { message = "Invalid token" });

            if (response.Data != null)
            {
                AuthenticateResponse resAuth = (AuthenticateResponse)response.Data;
                setTokenCookie(resAuth.RefreshToken);
            }            

            return Ok(response);
        }

        [HttpPost("revoke-token")]
        public IActionResult RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            var response = _userService.RevokeToken(token, ipAddress());

            if (!response)
                return NotFound(new { message = "Token not found" });

            return Ok(new { message = "Token revoked" });
        }

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user);
        }

        [HttpGet("{id}/refresh-tokens")]
        public IActionResult GetRefreshTokens(int id)
        {
            var user = _userService.GetById(id);
            if (user == null) return NotFound();

            return Ok(user.RefreshTokens);
        }

        // helper methods

        private void setTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string ipAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
    public class RevokeTokenRequest
    {
        public string Token { get; set; }
    }
    public class AuthenticateRequest
    {
        public string UserType { get; set; }
        public string DeviceId { get; set; }
        public string Username { get; set; }

        public string Password { get; set; }
    }
    public class AuthenticateResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string JwtToken { get; set; }

        public int? DonViId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string NhomQuyenId { get; set; }
        public string chucNangDefault { get; set; }
        public int AccessFailedCount { get; set; }
        public DateTime? ChangePassWordDate { get; set; }
        public List<string> ListChucNang { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(ApplicationUser user, string jwtToken, string refreshToken, List<string> listChucNang, string chucNangDefaultParam)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Email;
            //JwtToken = jwtToken;
            RefreshToken = refreshToken;
            Email = user.Email;
            Username = user.UserName;
            Token = jwtToken;
            DonViId = user.DonViId;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePassWordDate = user.ChangePassWordDate;
            ListChucNang = listChucNang;
            chucNangDefault = chucNangDefaultParam;
        }
        public AuthenticateResponse(ApplicationUser user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.Email;
            JwtToken = jwtToken;
            RefreshToken = refreshToken;
            Email = user.Email;
            Username = user.UserName;
            Token = jwtToken;
            DonViId = user.DonViId;
            NhomQuyenId = user.NhomQuyenId;
            AccessFailedCount = user.AccessFailedCount;
            ChangePassWordDate = user.ChangePassWordDate;
            //ListChucNang = listChucNang;
            //chucNangDefault = chucNangDefaultParam;
        }
    }
}