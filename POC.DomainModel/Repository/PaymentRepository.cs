using AutoMapper;
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

        public async Task<bool> DoPayment(CommonPaymentModel payment)
        {

            try
            {
                var paymentEntity = _mapper.Map<Payment>(payment);
                await _context.Payments.AddAsync(paymentEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
