using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using System.Collections.Generic;

namespace POC.ServiceLayer.Service
{
    public class OrderService : IOrder
    {
        private readonly IOrderRepo _orderRepoService;

        public OrderService(IOrderRepo orderRepo)
        {
            _orderRepoService = orderRepo;
        }

        public async Task<UserValidationResult> CreateOrderAsync(CommonOrderModel order, int orderedProduct)
        {
            var OrderData = await _orderRepoService.CreateOrder(order, orderedProduct);
            return OrderData;

        }

        public async Task<UserValidationResult> CreateOrderAsync(List<CommonProductQuantityModel> commonProductQuantity)
        {

            var OrderData = await _orderRepoService.CreateOrder(commonProductQuantity);
            return OrderData;

        }
        public async Task<UserValidationResult> CreateOrderAsync(CommonTemporderTable order, int orderedProduct, string SaveAsDraft)
        {
            var OrderData = await _orderRepoService.CreateOrder(order, orderedProduct, SaveAsDraft);
            return OrderData;

        }
    }
}
