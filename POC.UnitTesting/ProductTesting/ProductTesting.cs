using Moq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using POC.DomainModel.Models;
using POC.ServiceLayer.Service;

namespace POC.UnitTesting.ProductTesting
{
    [TestClass]
    public class ProductTesting
    {
        private readonly ProductService productService;
        private readonly Mock<IProductRepo> _productMock;

        public ProductTesting()
        {
            _productMock = new Mock<IProductRepo>();
            productService = new ProductService(_productMock.Object);
        }

        [TestMethod]
        public async Task GetProductById_ShouldReturnTrue()
        {
            //Arrange 
            var UserId = 30;

            var ProductModel = new CommonProductModel { ProductId = UserId, Name = "Iphone 14", Price = 75000, Description = "Best Camera and Performance" };
            _productMock.Setup(x => x.GetProductById(UserId)).ReturnsAsync(ProductModel);

            //Act
            var Result = productService.GetProductByIdAsync(UserId).Result;
            Console.WriteLine(Result);

            //Assert
            Assert.AreEqual(ProductModel.Name, Result.Name);
            Assert.AreEqual(ProductModel.Price, Result.Price);
            Assert.AreEqual(ProductModel.Description, Result.Description);

        }
        [TestMethod]
        public async Task GetProductById_ShouldReturnsNull()
        {
            //Arrange 
            var UserId = 1;

            var ProductModel = new CommonProductModel();
            _productMock.Setup(x => x.GetProductById(UserId)).ReturnsAsync(ProductModel);

            //Act
            var Result = productService.GetProductByIdAsync(UserId).Result;
            Console.WriteLine(Result);

            //Assert
            Assert.IsNull(Result.Name);
        }
        [TestMethod]
        public async Task GetAllProductsAsync_ShouldReturnProductsList()
        {
            //Arrange 
            var paginationModel = new CommonPaginationModel();

            _productMock.Setup(x => x.GetAllProducts(1, "")).ReturnsAsync(new CommonPaginationModel { });

            //Act
            var Actual = productService.GetAllProductsAsync(1, "");
            Console.WriteLine(Actual);

            //Assert
            Assert.IsNotNull(Actual);

        }
        [TestMethod]
        public async Task AddProductAsync_ShouldReturnSuccess()
        {
            // Arrange
            var product = new CommonProductModel
            {
                ProductId = 1,
                Name = "Product A",
                Price = 200,
                Description = "Description A",
                ProductAvailable = 5,
                ProductImage = new byte[] { 1, 2, 3 }
            };
            var userValidation = new UserValidationResult { IsValid = true, Message = "Product Added Successfully" };
            _productMock.Setup(x => x.AddProduct(product)).ReturnsAsync(userValidation);

            // Act
            var result = await productService.AddProductAsync(product);

            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(userValidation.Message, result.Message);
        }

        [TestMethod]
        public async Task AddProduct_ShouldReturnFalse_WhenProductExists()
        {
            // Arrange
            var product = new CommonProductModel
            {
                Name = "Iphone 15",
                Price = 200,
                ProductAvailable = 5
            };
            var userValidation = new UserValidationResult { IsValid=false ,Message = "Product has Already existed" };
            _productMock.Setup(x => x.AddProduct(product)).ReturnsAsync(userValidation);

            // Act
            var result = await productService.AddProductAsync(product);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(userValidation.Message, result.Message);
        }

    }
}
