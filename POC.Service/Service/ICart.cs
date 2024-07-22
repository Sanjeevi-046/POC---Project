using Poc.CommonModel.Models;
using POC.CommonModel.Models;
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
        Task<UserValidationResult> AddCart(CommonCartModel cartTable);
        Task<List<CommonCartModel>> GetCart(int id);

    }
}
