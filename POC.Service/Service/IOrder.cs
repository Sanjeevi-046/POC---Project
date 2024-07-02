using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public interface IOrder
    {
        Task<UserValidationResult> CreateOrderAsync(Order order, int orderedProduct);
    }
}
