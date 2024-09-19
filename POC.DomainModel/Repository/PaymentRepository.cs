using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using POC.CommonModel.Models;
using POC.DataLayer.Models;

namespace POC.DataLayer.Repository
{
    public class PaymentRepository :IPaymentRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;
        public PaymentRepository(DemoProjectContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> UpdatePaymentAddress(int AddressId, int PaymentID)
        {
            try
            {
                var paymentDetails = await _context.Payments.FindAsync(PaymentID);
                if (paymentDetails != null)
                {
                    var orderDetail = await _context.Orders.FindAsync(paymentDetails.OrderId);

                    if (orderDetail != null)
                    {
                        orderDetail.AddressId = AddressId;

                        await _context.SaveChangesAsync(); 
                        return true;
                    }
                }
                return false; 
            }
            catch (Exception ex)
            {
                throw; 
            }
        }


        public async Task<bool> DoPayment(CommonPaymentModel payment)
        {

            try
            {
                var paymentDetails = await _context.Payments.FindAsync(payment.Id);
                if (paymentDetails != null) 
                { 
                    paymentDetails.CardNumber = payment.CardNumber;
                    paymentDetails.ExpiryMonth = payment.ExpiryMonth;
                    paymentDetails.Cvv = payment.Cvv;
                    payment.PaymentReceived = true;
                    payment.ExpiryYear = payment.ExpiryYear;

                    var orderdetails = await _context.Orders.FindAsync(paymentDetails.OrderId);
                    payment.UserId = orderdetails.UserId;
                    if (orderdetails != null)
                    {
                        var productList = orderdetails.ProductList.Split(',').Select(int.Parse).ToList();
                        var productQuantityList = orderdetails.ProductQuantityList.Split(',').Select(int.Parse).ToList();

                        for (int i = 0; i < productList.Count; i++)
                        {
                            int productId = productList[i];
                            int orderedQuantity = productQuantityList[i];

                            var product = await _context.Products.FindAsync(productId);
                            if (product != null)
                            {
                                product.ProductAvailable -= orderedQuantity;

                                if (product.ProductAvailable <= 0)
                                {
                                    product.IsAvailable = false;
                                }

                                product.IsQuantityAvailable = product.ProductAvailable > 0;
                            }
                        }
                    }
                    var cartitem = await _context.CartTables.Where(x => x.UserId == orderdetails.UserId).ToListAsync();
                    if (cartitem != null)
                    {
                         _context.CartTables.RemoveRange(cartitem);
                    }
                   
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
                
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
