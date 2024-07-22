using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DomainModel.Repository
{
    public interface IOrderRepo
    {
        Task<UserValidationResult> CreateOrder(CommonOrderModel order, int orderedProduct);
        Task<UserValidationResult> CreateOrder(CommonTemporderTable order, int orderedProduct, string SaveAsaDraft);
    }
}
