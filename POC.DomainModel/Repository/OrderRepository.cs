using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Models;
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
                var defaultAddress = await (from address in _context.AddressDetails
                                            where address.UserId == cartOrder[0].UserID && address.IsDefault == true
                                            select address).FirstOrDefaultAsync();

                var newOrder = new Order();
                var TotalAmount = 0;
               
                newOrder.ProductList = "";
                newOrder.ProductQuantityList = "";
                foreach (var order in cartOrder) 
                {
                    var orderPrice = order.Quantity * order.ProductList.Price;
                    TotalAmount += (int)orderPrice;
                    newOrder.ProductList += order.ProductList.ProductId + ",";
                    newOrder.ProductQuantityList += order.Quantity + ",";

                }
                newOrder.UserId = cartOrder[0].UserID;
                newOrder.ProductList = newOrder.ProductList.TrimEnd(',');
                newOrder.ProductQuantityList = newOrder.ProductQuantityList.TrimEnd(',');
                newOrder.OrderDate = DateTime.Now;
                newOrder.OrderPrice = TotalAmount;
                newOrder.AddressId = defaultAddress.Id;
                await _context.Orders.AddAsync(newOrder);   
                await _context.SaveChangesAsync();

                var payment = new Payment
                {
                    CardNumber = "",
                    Cvv = 000,
                    ExpiryMonth = 0,
                    ExpiryYear = 0,
                    PaymentReceived = false,
                    OrderId =newOrder.OrderId,
                    Amount = TotalAmount,
                    UserId = cartOrder[0].UserID,
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return new UserValidationResult{IsValid = true, PaymentId = payment.Id};
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

                var payment = new Payment
                {
                    CardNumber = "",
                    Cvv = 000,
                    ExpiryMonth = 0,
                    ExpiryYear = 0,
                    PaymentReceived = false,
                    OrderId = orderData.OrderId,
                    Amount = orderData.OrderPrice * orderedProduct,
                    UserId = orderData.UserId,
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return new UserValidationResult { IsValid = true, PaymentId = payment.Id };
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
