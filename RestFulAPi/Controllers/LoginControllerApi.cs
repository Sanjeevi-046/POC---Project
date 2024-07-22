using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Poc.CommonModel.Models;
using POC.DataAccess.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POC.Api.Controllers
{
    [Route("Login")]
    [ApiController]
    [AllowAnonymous]
    public class LoginControllerApi : ControllerBase
    {
        private readonly ILogin _loginService;
        private readonly IConfiguration _configuration;
        public LoginControllerApi(ILogin loginService, IConfiguration configuration)
        {
            _loginService = loginService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
        [HttpGet("ID")]
        public async Task<IActionResult> GetId(string name)
        {
            var result = await _loginService.GetUserId(name);
            if (result.IsValid)
            {
                return Ok(result.Message);
            }
            return BadRequest();
        }
        [HttpPost("User")]
        public async Task<IActionResult> CheckUser(CommonLoginModel login)
        {
            var result = await _loginService.ValidateUserAsync(login.Name, login.Password);

            if (result.IsValid)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, login.Name),
                    new Claim(ClaimTypes.Email, result.Mail),
                    new Claim(ClaimTypes.Role, result.Role)
                };

                var jwtSettings = _configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"])),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = await _loginService.GenerateRefreshToken((int)result.userId);

                return Ok(new { Token = tokenString, RefreshToken = refreshToken.RefreshToken1, Message = result.Message, Role = result.Role });
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
        [HttpPost("Users")]
        public async Task<IActionResult> NewUser(UserRegistrationModel model)
        {
            var result = await _loginService.RegisterUserAsync(model.Login, model.rePassword);
            if (result.IsValid)
            {
                return Ok(result.Message);
            }
            else
            {
                return NotFound(result.Message);
            }
        }
        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            var validRefreshToken = await _loginService.ValidateRefreshToken(refreshToken);

            if (validRefreshToken == null)
            {
                return Unauthorized("Invalid refresh token");
            }
            var user = await _loginService.GetLoginAsync(validRefreshToken.UserId.ToString());
            if (user == null)
            {
                return Unauthorized("User not found");
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"])),
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var newRefreshToken = await _loginService.GenerateRefreshToken(validRefreshToken.UserId);
           
            await _loginService.InvalidateRefreshToken(refreshToken);

            return Ok(new { Token = tokenString, RefreshToken = newRefreshToken.RefreshToken1 });   
        }
    }
}
