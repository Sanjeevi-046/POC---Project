using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Repository;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
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
            
                var OrderData = await _orderRepoService.CreateOrder(order,orderedProduct);
            return OrderData;

        }
        public async Task<UserValidationResult> CreateOrderAsync(CommonTemporderTable order, int orderedProduct,string SaveAsDraft)
        {
            var OrderData = await _orderRepoService.CreateOrder(order, orderedProduct,SaveAsDraft);
            return OrderData;

        }
    }
}
