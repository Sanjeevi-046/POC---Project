using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public interface ICart
    {
        Task<UserValidationResult> AddCart(CartTable cartTable);
        Task<List<CartTable>> GetCart(int id);

    }
}
