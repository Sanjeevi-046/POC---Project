using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public interface ILogin
    {
        Task<UserValidationResult> ValidateUserAsync(string name, string password);
        Task<UserValidationResult> RegisterUserAsync(Login login, string rePassword);
        Task<UserValidationResult> getUserId(string userId);
        Task<Login> GetLoginAsync(string userId);
        Task<Refreshtoken> GenerateRefreshToken(int userId);
        Task InvalidateRefreshToken(string token);
        Task<Refreshtoken> ValidateRefreshToken(string token);
        
    }
}
