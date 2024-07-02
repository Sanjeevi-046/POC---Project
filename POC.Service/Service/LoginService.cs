using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public class LoginService : ILogin
    {
        private readonly DemoProjectContext context;
        private readonly ILogger<LoginService> _logger;

        public LoginService(DemoProjectContext context, ILogger<LoginService> logger)
        {
            this.context = context;
            _logger = logger;
        }
        public async Task<UserValidationResult> getUserId(string userName)
        {
            var user = await context.Logins.FirstOrDefaultAsync(u => u.Name == userName);
            if (user != null)
            {
                return new UserValidationResult { IsValid = true, Message = user.Id.ToString() };
            }
            return new UserValidationResult { IsValid = false, Message = "Name not found" };
        }
        public async Task<Login> GetLoginAsync(string userId)
        {
            var user = await context.Logins.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user != null)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<UserValidationResult> ValidateUserAsync(string name, string password)
        {
            var user = await context.Logins.FirstOrDefaultAsync(c => c.Name == name);
            if (user == null)
            {
                return new UserValidationResult { IsValid = false, Message = "User not found." };
            }
            if (user.Password == password)
            {

                return new UserValidationResult { IsValid = true, Message = "Login successful.", Mail = user.Email ,Role=user.Role ,userId=user.Id};
            }
            else
            {
                return new UserValidationResult { IsValid = false, Message = "The password doesn't match." };
            }

        }

        public async Task<UserValidationResult> RegisterUserAsync(Login login, string rePassword)
        {
            var userdetail = new User();
            var user = context.Logins.FirstOrDefault(u => u.Name == login.Name);
            login.Role = "Customer";
            if (user == null)
            {
                if (rePassword == login.Password)
                {
                    userdetail.Email = login.Email;
                    userdetail.Name = login.Name;
                    context.Users.Add(userdetail);
                    context.Logins.Add(login);
                    await context.SaveChangesAsync();
                    return new UserValidationResult { IsValid = true, Message = "Registered successfully." };
                }
                else
                {
                    return new UserValidationResult { IsValid = false, Message = "Password miss match." };
                }
            }
            else
            {
                return new UserValidationResult { IsValid = false, Message = "The User Name is already registered." };
            }
        }

        public async Task<Refreshtoken> GenerateRefreshToken(int userId)
        {
            var id = context.Refreshtokens.FirstOrDefaultAsync(u => u.Id == userId);
            if (id != null)
            {
                var refreshToken = new Refreshtoken
                {
                    RefreshToken1 = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                    ExpirationTime = DateTime.UtcNow.AddDays(1),
                    UserId = userId
                };

                context.Refreshtokens.Add(refreshToken);
                await context.SaveChangesAsync();

                return refreshToken;
            }
            else
            {
                return null;
            }
        }

        public async Task<Refreshtoken> ValidateRefreshToken(string token)
        {
            var refreshToken = await context.Refreshtokens
                .FirstOrDefaultAsync(t => t.RefreshToken1 == token && t.ExpirationTime > DateTime.UtcNow);

            return refreshToken;
        }

        public async Task InvalidateRefreshToken(string token)
        {
            var refreshToken = await context.Refreshtokens.FirstOrDefaultAsync(t => t.RefreshToken1 == token);
            if (refreshToken != null)
            {
                context.Refreshtokens.Remove(refreshToken);
                await context.SaveChangesAsync();
            }
        }

    }
}
