using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceLayer.Service;
using POC.CommonModel.Models;
using Poc.CommonModel.Models;
using POC.Api.Controllers;
using System.Collections.Generic;

namespace POC.UnitTesting.CartTesting
{
    [TestClass]
    public class CartControllerApiTesting
    {
        private Mock<ICart> _cartServiceMock;
        private Mock<IProduct> _productServiceMock;
        private CartController _cartController;
        public CartControllerApiTesting()
        {
            _cartServiceMock = new Mock<ICart>();
            _productServiceMock = new Mock<IProduct>();
            _cartController = new CartController(_cartServiceMock.Object, _productServiceMock.Object);
        }


        [TestMethod]
        public async Task GetCart_ReturnsOkResult_WithProductList()
        {
            // Arrange
            var cartItems = new List<CommonCartModel>
            {
                new CommonCartModel { ProductId = 1 },
                new CommonCartModel { ProductId = 2 }
            };

            var product1 = new CommonProductModel { ProductId = 1, Name = "Product1" };
            var product2 = new CommonProductModel { ProductId = 2, Name = "Product2" };

            _cartServiceMock.Setup(service => service.GetCart(It.IsAny<int>()))
                            .ReturnsAsync(cartItems);

            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                               .ReturnsAsync(product1);
            _productServiceMock.Setup(service => service.GetProductByIdAsync(2))
                               .ReturnsAsync(product2);

            // Act
            var result = await _cartController.GetCart(1);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public async Task GetCart_ReturnsInternalServerError_OnProductServiceFailure()
        {
            // Arrange
            var cartItems = new List<CommonCartModel>
            {
                new CommonCartModel { ProductId = 1 },
                new CommonCartModel { ProductId = 2 }
            };

            _cartServiceMock.Setup(service => service.GetCart(It.IsAny<int>()))
                            .ReturnsAsync(cartItems);

            _productServiceMock.Setup(service => service.GetProductByIdAsync(1))
                               .ReturnsAsync(new CommonProductModel { ProductId = 1, Name = "Product1" });
            _productServiceMock.Setup(service => service.GetProductByIdAsync(2))
                               .ReturnsAsync((CommonProductModel)null);

            // Act
            var result = await _cartController.GetCart(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }


        [TestMethod]
        public async Task AddCart_ReturnsCreatedAtActionResult_WithSuccessMessage()
        {
            // Arrange
            var cartModel = new CommonCartModel { UserId = 1, ProductId = 1 };
            var validationResult = new UserValidationResult { IsValid = true, Message = "Cart added successfully" };

            _cartServiceMock.Setup(service => service.AddCart(cartModel))
                            .ReturnsAsync(validationResult);

            // Act
            var result = await _cartController.AddCart(cartModel);

            // Assert
            var createdResult = result as CreatedAtActionResult;
            Assert.AreEqual(201, createdResult.StatusCode);
            Assert.AreEqual("Cart added successfully", createdResult.Value);
        }

        [TestMethod]
        public async Task AddCart_ReturnsBadRequest_WithErrorMessage()
        {
            // Arrange
            var cartModel = new CommonCartModel { UserId = 1, ProductId = 1 };
            var validationResult = new UserValidationResult { IsValid = false, Message = "Error adding to cart" };

            _cartServiceMock.Setup(service => service.AddCart(cartModel))
                            .ReturnsAsync(validationResult);

            // Act
            var result = await _cartController.AddCart(cartModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Error adding to cart", badRequestResult.Value);
        }


    }
}
