using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.ServiceLayer.Service
{
    public interface IPayment
    {
        Task<bool> DoPayment(CommonPaymentModel payment);
        Task<bool> UpdatePaymentAddress(int AddressId, int PaymentID);
    }
}
