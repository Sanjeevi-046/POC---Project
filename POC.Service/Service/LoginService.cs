using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;

namespace POC.ServiceLayer.Service
{
    public class LoginService : ILogin
    {
        private readonly ILoginRepo _loginRepoService;

        public LoginService(ILoginRepo loginRepo)
        {
            _loginRepoService = loginRepo;
        }
        public async Task<UserValidationResult> GetUserId(string userName)
        {
            var userData = await _loginRepoService.GetUserIdAsync(userName);
            return userData;
        }
        public async Task<CommonLoginModel> GetLoginAsync(string userId)
        {
            var userData = await _loginRepoService.GetLoginUserAsync(userId);
            return userData;
        }

        public async Task<UserValidationResult> ValidateUserAsync(string name, string password)
        {
            var userData = await _loginRepoService.ValidateUserAsync(name, password);
            return userData;

        }

        public async Task<UserValidationResult> RegisterUserAsync(CommonLoginModel login, string rePassword)
        {
            var userData = await _loginRepoService.RegisterUserAsync(login, rePassword);
            return userData;
        }

        public async Task<CommonRefereshToken> GenerateRefreshToken(int userId)
        {
            var TokenId = await _loginRepoService.GetRefreshTokenAsync(userId);
            return TokenId;
        }
        public async Task<CommonRefereshToken> ValidateRefreshToken(string token)
        {
            var refreshToken = await _loginRepoService.ValidateRefreshTokenAsync(token);
            return refreshToken;
        }
        public async Task<UserValidationResult> InvalidateRefreshToken(string token)
        {
            var refreshToken = await _loginRepoService.InvalidateRefreshTokenAsync(token);
            return refreshToken;
        }

    }
}
