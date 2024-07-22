using AutoMapper;
using Poc.CommonModel.Models;
using POC.DomainModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.Repository
{
    public class OrderServiceRepo : IOrderRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public OrderServiceRepo(DemoProjectContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserValidationResult> CreateOrder(CommonOrderModel order, int orderedProduct)
        {
            try
            {
                var orderData = _mapper.Map<Order>(order);
                order.OrderDate = DateTime.Now;
                _context.Orders.Add(orderData);

                var product = await _context.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.ProductAvailable -= orderedProduct;
                    if (product.ProductAvailable <= 0)
                    {
                        product.IsAvailable = false;
                    }
                }
                await _context.SaveChangesAsync();
                return new UserValidationResult { IsValid = true, Message = $"Your {product.Name} Order Has been Placed SuccessFully." };
            }
            catch (Exception ex)
            {
                return new UserValidationResult { IsValid = false, Message = ex.Message };
            }

        }
        public async Task<UserValidationResult> CreateOrder(CommonTemporderTable order, int orderedProduct, string SaveAsDraft)
        {
            try
            {
                var orderData = _mapper.Map<TemporderTable>(order);
                _context.TemporderTables.Add(orderData);
                var product = await _context.Products.FindAsync(order.ProductId);

                if (product != null)
                {
                    product.ProductAvailable -= orderedProduct;
                    if (product.ProductAvailable <= 0)
                    {
                        product.IsAvailable = false;
                    }

                   await _context.SaveChangesAsync();
                }
                return new UserValidationResult { IsValid = true, Message = $"Your {product.Name} Order Has Been saved in the Draft" };
            }
            catch (Exception ex)
            {
                return new UserValidationResult { IsValid = false, Message = ex.Message };
            }

        }
    }
}
