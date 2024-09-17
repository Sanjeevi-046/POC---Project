using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Poc.CommonModel.Models;
using POC.Api.Controllers;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.UnitTesting.LoginTesting
{
    [TestClass] 
    public class LoginControllerTesting
    {
        private Mock<ILogin> _loginServiceMock;
        private LoginController _loginController;
        private readonly IConfiguration _configuration;
        public LoginControllerTesting()
        {
            _loginServiceMock = new Mock<ILogin>();
            _loginController = new LoginController(_loginServiceMock.Object,_configuration);
            

        }

        [TestMethod]
        public async Task GetId_ReturnsOkResult_WithUserId()
        {
            // Arrange
            var userName = "testUser";
            var validationResult = new UserValidationResult { IsValid = true, Message = "1" };

            _loginServiceMock.Setup( x => x.GetUserId(userName))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _loginController.GetId(userName);

            // Assert
           
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }
        [TestMethod]
        public async Task GetId_ReturnsFalse()
        {
            // Arrange
            var userName = "invalidUser";
            var validationResult = new UserValidationResult { IsValid = false };

            _loginServiceMock.Setup( x => x.GetUserId(userName))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _loginController.GetId(userName);
            Console.WriteLine(result);
            // Assert
            var okResult = result as OkObjectResult;

            var status = result as BadRequestObjectResult;
            Assert.IsNull(status);
           
        }



        [TestMethod]
        public async Task NewUser_ReturnsTrue()
        {
            // Arrange
            var userRegistrationModel = new UserRegistrationModel
            {
                Login = new CommonLoginModel { Name = "newUser", Password = "password" },
                rePassword = "password"
            };
            var validationResult = new UserValidationResult { IsValid = true, Message = "User registered successfully" };

            _loginServiceMock.Setup( x => x.RegisterUserAsync(userRegistrationModel.Login, userRegistrationModel.rePassword))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _loginController.NewUser(userRegistrationModel);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

    }
}
