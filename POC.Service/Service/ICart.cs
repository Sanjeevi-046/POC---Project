using Poc.CommonModel.Models;
using POC.CommonModel.Models;

namespace POC.ServiceLayer.Service
{
    public interface ICart
    {
        Task<UserValidationResult> AddCart(CommonCartModel cartTable);
        Task<List<CommonCartModel>> GetCart(int id);

    }
}
