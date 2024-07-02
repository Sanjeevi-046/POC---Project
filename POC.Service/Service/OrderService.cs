using POC.DomainModel.Models;
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
        private readonly DemoProjectContext _context;

        public OrderService(DemoProjectContext context)
        {
            _context = context;
        }

        public async Task<UserValidationResult> CreateOrderAsync(Order order, int orderedProduct)
        {
            try
            {
                order.OrderDate = DateTime.Now;
                _context.Orders.Add(order);

                var product = await _context.Products.FindAsync(order.ProductId);
                if (product != null)
                {
                    product.ProductAvailable -= orderedProduct;
                    if (product.ProductAvailable <= 0)
                    {
                        product.IsAvailable = false;
                    }
                }
                _context.SaveChangesAsync();
                return new UserValidationResult { IsValid = true, Message = $"Your {product.Name} Order Has been Placed SuccessFully." };
            }
            catch (Exception ex)
            {
                return new UserValidationResult { IsValid = false, Message = ex.Message };
            }

        }
    }
}
