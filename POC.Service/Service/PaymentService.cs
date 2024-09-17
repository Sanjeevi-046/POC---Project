using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public class PaymentService : IPayment
    {
        private readonly IPaymentRepo _paymentRepository;
        public PaymentService(IPaymentRepo paymentRepository) { _paymentRepository = paymentRepository; }
        public async Task<bool> DoPayment(CommonPaymentModel payment)
        {
            return await _paymentRepository.DoPayment(payment);
        }
    }
}
