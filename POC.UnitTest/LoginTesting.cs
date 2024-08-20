using Moq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataAccess.Service;
using POC.DomainModel.Repository;
using Xunit;

namespace POC.UnitTest
{
    public class LoginTesting
    {
        private readonly LoginService _loginService;
        private readonly Mock<ILoginRepo> _loginMock;

        public LoginTesting()
        {
            _loginMock = new Mock<ILoginRepo>();
            _loginService = new LoginService(_loginMock.Object);
        }


        [Fact]
        public async Task ValidateUserAsync_ValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var name = "test";
            var password = "test";
            var expectedResult = new UserValidationResult
            {
                IsValid = true,
                Message = "Login successful.",
                Mail = "test@test.com",
                Role = "Customer",
                userId = 1
            };

            _loginMock.Setup(repo => repo.ValidateUserAsync(name, password))
                          .ReturnsAsync(expectedResult);

            // Act
            var result = await _loginService.ValidateUserAsync(name, password);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task GetLoginAsync_ShouldReturn()
        {
            // Arrange
            var userId = "1";
            var expectedLoginModel = new CommonLoginModel
            {
                Id = int.Parse(userId),
                Name = "test",
                Password = "test",
                Role = "Customer",
                Email = "test@test.com"
            };

            _loginMock.Setup(x => x.GetLoginUserAsync(userId)).ReturnsAsync(expectedLoginModel);

            // Act
            var actual = await _loginService.GetLoginAsync(userId);

            // Assert
            Assert.Equal(expectedLoginModel, actual);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnTrue()
        {
            // Arrange
            var name = "test";
            var password = "test";
            var expectedValidationResult = new UserValidationResult { IsValid = true, Message = "Login successful.", Mail = "test@test.com", Role = "Customer", userId = 1 };

            _loginMock.Setup(x => x.ValidateUserAsync(name, password)).ReturnsAsync(expectedValidationResult);

            // Act
            var actual = await _loginService.ValidateUserAsync(name, password);

            // Assert
            Assert.IsType<UserRegistrationModel>(actual);
            Assert.True(actual.IsValid);
            Assert.Equal(expectedValidationResult.Message, actual.Message);
        }
    }
}
