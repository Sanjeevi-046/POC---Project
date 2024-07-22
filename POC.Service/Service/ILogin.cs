
using Poc.CommonModel.Models;
using POC.DomainModel.Models;

namespace POC.DataAccess.Service

{
    public interface ILogin
    {
        Task<UserValidationResult> ValidateUserAsync(string name, string password);
        Task<UserValidationResult> RegisterUserAsync(CommonLoginModel login, string rePassword);
        Task<UserValidationResult> GetUserId(string userId);
        Task<CommonLoginModel> GetLoginAsync(string userId);
        Task<CommonRefereshToken> GenerateRefreshToken(int userId);
        Task<UserValidationResult> InvalidateRefreshToken(string token);
        Task<CommonRefereshToken> ValidateRefreshToken(string token);
        
    }
}
