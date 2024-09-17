using Moq;
using Microsoft.AspNetCore.Mvc;
using POC.ServiceLayer.Service;
using POC.CommonModel.Models;
using Poc.CommonModel.Models;
using POC.Api.Controllers;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace POC.UnitTesting.ProductTesting
{
    [TestClass]
    public class ProductControllerTesting
    {
        private Mock<IProduct> _productServiceMock;
        private ProcuctController _productController;
        private ILogger<ProcuctController> _loggerMock;

        public ProductControllerTesting(ILogger<ProcuctController>logger)
        {
            _productServiceMock = new Mock<IProduct>();
            _loggerMock = logger;
            _productController = new ProcuctController(_productServiceMock.Object,_loggerMock);
        }

        [TestMethod]
        public async Task GetProductList_ReturnsProducts()
        {
            // Arrange
            var products = new CommonPaginationModel
            {
                Product = new List<CommonProductModel>
                {
                    new CommonProductModel { ProductId = 1, Name = "Product1" },
                    new CommonProductModel { ProductId = 2, Name = "Product2" }
                },
                Total = 2
            };

            _productServiceMock.Setup(x => x.GetAllProductsAsync(1, ""))
                               .ReturnsAsync(products);

            // Act
            var result = await _productController.GetProductList(1, "");
            Console.WriteLine(result);
            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(products, okResult.Value);
        }


        [TestMethod]
        public async Task GetProductById_ReturnsProduct()
        {
            // Arrange
            var product = new CommonProductModel { ProductId = 1, Name = "Product1" };

            _productServiceMock.Setup(x => x.GetProductByIdAsync(1))
                               .ReturnsAsync(product);

            // Act
            var result = await _productController.GetProductById(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(product, okResult.Value);
        }

        [TestMethod]
        public async Task AddProduct_ReturnsProductIsValid()
        {
            // Arrange
            var product = new CommonProductModel { ProductId = 1, Name = "Product1" };
            var validationResult = new UserValidationResult { IsValid = true, Message = "Product Added Successfully" };

            _productServiceMock.Setup(x => x.AddProductAsync(product))
                               .ReturnsAsync(validationResult);

            // Act
            var result = await _productController.AddProduct(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(validationResult, okResult.Value);
        }

        [TestMethod]
        public async Task AddProduct_ReturnsBadRequest_WhenProductIsInvalid()
        {
            // Arrange
            var product = new CommonProductModel { ProductId = 1, Name = "Product1" };
            var validationResult = new UserValidationResult { IsValid = false, Message = "Invalid product data" };

            _productServiceMock.Setup(x => x.AddProductAsync(product))
                               .ReturnsAsync(validationResult);

            // Act
            var result = await _productController.AddProduct(product);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual(400, badRequestResult.StatusCode);
            Assert.AreEqual("Invalid product data", badRequestResult.Value);
        }
    }
}
