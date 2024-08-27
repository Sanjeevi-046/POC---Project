using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Models;
namespace POC.DataLayer.Repository
{
    public class OrderRepository : IOrderRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(DemoProjectContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<UserValidationResult> CreateOrder(List<CommonProductQuantityModel> cartOrder)
        {
            try
            {
                var cartItems = await _context.CartTables.Where(x => x.UserId == cartOrder[0].UserID).ToListAsync();
                var orderList = new List<Order>();

                foreach (var item in cartOrder)
                {
                    var orderModel = new Order
                    {
                        OrderDate = DateTime.Now,
                        OrderPrice = item.Quantity * item.ProductList.Price,
                        UserId = item.UserID
                    };
                    _context.Orders.Add(orderModel);
                    var product = await _context.Products.FindAsync(item.ProductList.ProductId);
                    if (product != null)
                    {
                        product.ProductAvailable -= item.Quantity;
                        if (product.ProductAvailable <= 0)
                        {
                            product.IsAvailable = false;
                        }
                    }
                }

                await _context.SaveChangesAsync();

                _context.CartTables.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return new UserValidationResult{IsValid = true, Message = "Order has been placed successfully!"};
            }
            catch (Exception ex) {
                return new UserValidationResult { IsValid = false, Message = ex.Message };
            }
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
