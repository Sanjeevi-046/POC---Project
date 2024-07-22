using Poc.CommonModel.Models;
using POC.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.Repository
{
    public interface ILoginRepo
    {
        Task<UserValidationResult> ValidateUserAsync(string name, string password);
        Task<UserValidationResult> RegisterUserAsync(CommonLoginModel login, string rePassword);
        Task<UserValidationResult> GetUserIdAsync(string userId);
        Task<CommonLoginModel> GetLoginUserAsync(string userId);
        Task<CommonRefereshToken> GetRefreshTokenAsync(int userId);
        Task<UserValidationResult> InvalidateRefreshTokenAsync(string token);
        Task<CommonRefereshToken> ValidateRefreshTokenAsync(string token);
    }
}
