using Moq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using POC.ServiceLayer.Service;

namespace POC.UnitTesting.LoginTesting
{
    [TestClass]
    public class LoginTesting
    {
        private readonly LoginService _loginService;
        private readonly Mock<ILoginRepo> _loginMock;

        public LoginTesting()
        {
            _loginMock = new Mock<ILoginRepo>();
            _loginService = new LoginService(_loginMock.Object);
        }


        [TestMethod]
        public async Task ValidateUserAsync_ValidCredentials_ShouldReturnTrue()
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

            _loginMock.Setup(x => x.ValidateUserAsync(name, password))
                          .ReturnsAsync(expectedResult);

            // Act
            var result = await _loginService.ValidateUserAsync(name, password);

            // Assert
            Assert.IsTrue(result.IsValid);
        }
        [TestMethod]
        public async Task ValidateUserAsync_ValidCredentials_ShouldReturnFalse()
        {
            // Arrange
            var name = "Rama";
            var password = "test";
            var expectedResult = new UserValidationResult { IsValid = false, Message = "The user name or password provided is incorrect." };

            _loginMock.Setup(x => x.ValidateUserAsync(name, password))
                          .ReturnsAsync(expectedResult);

            // Act
            var result = await _loginService.ValidateUserAsync(name, password);

            // Assert
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public async Task GetLoginAsync_ShouldReturnLoginModel()
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
            Assert.AreEqual(expectedLoginModel, actual);
        }
        [TestMethod]
        public async Task GetLoginAsync_ShouldReturnNull()
        {
            // Arrange
            var userId = "30";
            var expectedLoginModel = new CommonLoginModel
            {
            };

            _loginMock.Setup(x => x.GetLoginUserAsync(userId)).ReturnsAsync(expectedLoginModel);

            // Act
            var actual = await _loginService.GetLoginAsync(userId);

            // Assert
            Assert.IsNull(actual.Name);
        }

        [TestMethod]
        public async Task ValidateUserAsync_ShouldReturnTrue()
        {
            // Arrange
            var name = "test";
            var password = "test";
            var expectedValidationResult = new UserValidationResult
            {
                IsValid = true,
                Message = "Login successful.",
                Mail = "test@test.com",
                Role = "Customer",
                userId = 1
            };

            _loginMock.Setup(x => x.ValidateUserAsync(name, password)).ReturnsAsync(expectedValidationResult);

            // Act
            var actual = await _loginService.ValidateUserAsync(name, password);

            // Assert
            Assert.IsTrue(actual.IsValid);
            Assert.AreEqual(expectedValidationResult, actual);

        }

        [TestMethod]
        public async Task GetUserIdAsync_ShouldReturnNulll()
        {
            // Arrange
            var userId = "30";
            var expectedLoginModel = new UserValidationResult { IsValid = false };

            _loginMock.Setup(x => x.GetUserIdAsync(userId)).ReturnsAsync(expectedLoginModel);

            // Act
            var Actual = await _loginService.GetUserId(userId);

            // Assert
            Assert.IsFalse(Actual.IsValid);
            Assert.AreEqual(Actual.IsValid, expectedLoginModel.IsValid);
        }
        [TestMethod]
        public async Task GetUserIdAsync_ShouldReturn()
        {
            // Arrange
            var userId = "1";
            var expectedLoginModel = new UserValidationResult { IsValid = true };

            _loginMock.Setup(x => x.GetUserIdAsync(userId)).ReturnsAsync(expectedLoginModel);

            // Act
            var Actual = await _loginService.GetUserId(userId);

            // Assert
            Assert.IsTrue(Actual.IsValid);
            Assert.AreEqual(Actual.IsValid, expectedLoginModel.IsValid);
        }

    }
}
