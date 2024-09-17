using Moq;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceLayer.Service;
using POC.CommonModel.Models;
using Poc.CommonModel.Models;
using POC.Api.Controllers;

namespace POC.UnitTesting.OrderTesting
{
    [TestClass]
    public class OrderControllerApiTests
    {
        private Mock<IOrder> _orderServiceMock;
        private OrderController _orderController;

        public OrderControllerApiTests()
        {
            _orderServiceMock = new Mock<IOrder>();
            _orderController = new OrderController(_orderServiceMock.Object);
        }


        [TestMethod]
        public async Task CreateOrder_ReturnsOkResult_WithSuccessMessage()
        {
            // Arrange
            var order = new CommonOrderModel
            {
                OrderDate = DateTime.Now,
                OrderPrice = 10000,
                ProductId = 30,
                UserId = 1
            };
            var orderedProduct = 5;
            var validationResult = new UserValidationResult { IsValid = true, Message = "Order created successfully" };

            _orderServiceMock.Setup(service => service.CreateOrderAsync(order, orderedProduct))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _orderController.CreateOrder(order, orderedProduct);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Order created successfully", okResult.Value);
        }



        [TestMethod]
        public async Task CreateOrder_ReturnsBadRequest_WithErrorMessage()
        {
            // Arrange
            var order = new CommonOrderModel { };
            var orderedProduct = 5;
            var validationResult = new UserValidationResult { IsValid = false, Message = "Order creation failed" };

            _orderServiceMock.Setup(service => service.CreateOrderAsync(order, orderedProduct))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _orderController.CreateOrder(order, orderedProduct);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Order creation failed", badRequestResult.Value);
        }



        [TestMethod]
        public async Task SaveAsDraft_ReturnsOkResult_WithSuccessMessage()
        {
            // Arrange
            var order = new CommonTemporderTable
            {

                OrderPrice = 10000,
                ProductId = 30,
                UserId = 1
            };
            var orderedProduct = 5;
            var pincode = "123456";
            var validationResult = new UserValidationResult { IsValid = true, Message = "Order saved as draft successfully" };

            _orderServiceMock.Setup(service => service.CreateOrderAsync(order, orderedProduct, pincode))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _orderController.SaveAsDraft(order, orderedProduct, pincode);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Order saved as draft successfully", okResult.Value);
        }

        [TestMethod]
        public async Task SaveAsDraft_ReturnsBadRequest_WithErrorMessage()
        {
            // Arrange
            var order = new CommonTemporderTable { };
            var orderedProduct = 5;
            var pincode = "123456";
            var validationResult = new UserValidationResult { IsValid = false, Message = "Saving draft failed" };

            _orderServiceMock.Setup(service => service.CreateOrderAsync(order, orderedProduct, pincode))
                             .ReturnsAsync(validationResult);

            // Act
            var result = await _orderController.SaveAsDraft(order, orderedProduct, pincode);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Saving draft failed", badRequestResult.Value);
        }


    }
}
