using Moq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using POC.ServiceLayer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.UnitTesting.OrderTesting
{
    [TestClass]
    public class OrderServiceTesting
    {
        private readonly OrderService _orderService;
        private readonly Mock<IOrderRepo> _orderMock;
        public OrderServiceTesting()
        {
            _orderMock = new Mock<IOrderRepo>();
            _orderService = new OrderService(_orderMock.Object);
        }


        [TestMethod]
        public async Task CreateOrder_ShouldSave()
        {
            //Arrange
            var orderData = new CommonOrderModel
            {
                OrderDate = DateTime.Now,
                OrderPrice = 10000,
                ProductId = 30,
                UserId = 1
            };
            var expectedResult = new UserValidationResult { IsValid = true };


            _orderMock.Setup(x => x.CreateOrder(orderData, 2)).ReturnsAsync(expectedResult);

            //Act
            var Actual = _orderService.CreateOrderAsync(orderData, 2).Result;

            //Assert

            Assert.AreEqual(expectedResult.IsValid, Actual.IsValid);


        }
        [TestMethod]
        public async Task CreateOrder_ShouldNotSave()
        {
            //Arrange
            var orderData = new CommonOrderModel
            ();
            var expectedResult = new UserValidationResult { IsValid = false };


            _orderMock.Setup(x => x.CreateOrder(orderData, 2)).ReturnsAsync(expectedResult);

            //Act
            var Actual = _orderService.CreateOrderAsync(orderData, 2).Result;

            //Assert

            Assert.AreEqual(expectedResult.IsValid, Actual.IsValid);

        }
    }
}
