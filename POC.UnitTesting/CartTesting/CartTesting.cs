using Moq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using POC.ServiceLayer.Service;

namespace POC.UnitTesting.CartTesting
{
    [TestClass]
    public class CartTesting
    {
        private readonly CartService _cartService;
        private readonly Mock<ICartRepo> _cartMock;

        public CartTesting()
        {
            _cartMock = new Mock<ICartRepo>();
            _cartService = new CartService(_cartMock.Object);
        }

        [TestMethod]
        public async Task GetCart_ShouldReturnList()
        {
            // Arrange
            var userId = 1;
            var cartModel = new List<CommonCartModel>
            {
                new CommonCartModel { CartId = 1, UserId = userId, ProductId = 10 },
                new CommonCartModel { CartId = 2, UserId = userId, ProductId = 11 }
            };
            _cartMock.Setup(x => x.GetCartAsync(userId)).ReturnsAsync(cartModel);

            // Act
            var result = await _cartService.GetCart(userId);
            Console.WriteLine(result);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count,cartModel.Count);
        }

        [TestMethod]
        public async Task GetCart_ShouldReturnNull()
        {
            // Arrange
            var userId = 5;
            var cartModel = new List<CommonCartModel>
            {
               
            };
            _cartMock.Setup(x => x.GetCartAsync(userId)).ReturnsAsync(cartModel);
          
            // Act
            var result = await _cartService.GetCart(userId);
            Console.WriteLine(result);
            // Assert
            Assert.AreEqual(result.Count,0);
        }

        [TestMethod]
        public async Task AddCart_ShouldReturnSuccess()
        {
            // Arrange
            var cartModel = new CommonCartModel
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                
            };
            _cartMock.Setup(x => x.AddCartAsync(cartModel)).ReturnsAsync(new UserValidationResult { IsValid = true, Message = "Product Added Successfully" });

            // Act
            var result = await _cartService.AddCart(cartModel);
            Console.WriteLine(result);
            // Assert
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("Product Added Successfully", result.Message);
        }

        [TestMethod]
        public async Task AddCart_ShouldReturnFail()
        {
            // Arrange
            var cartModel = new CommonCartModel
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                
            };
            _cartMock.Setup(x => x.AddCartAsync(cartModel)).ReturnsAsync(new UserValidationResult { IsValid = false, Message = "Error occurred" });

            // Act
            var result = await _cartService.AddCart(cartModel);
            Console.WriteLine(result);

            // Assert
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("Error occurred", result.Message);
        }
    }
}
