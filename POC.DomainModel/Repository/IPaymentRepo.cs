using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataLayer.Repository
{
    public interface IPaymentRepo
    {
        Task<bool> DoPayment(CommonPaymentModel payment);
    }
}
