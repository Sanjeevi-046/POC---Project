using Poc.CommonModel.Models;
using POC.CommonModel.Models;

namespace POC.DataLayer.Repository
{
    public interface ICartRepo
    {
        Task<List<CommonCartModel>> GetCartAsync(int id);
        Task<UserValidationResult> AddCartAsync(CommonCartModel cartTable);
        Task<bool> DeleteCartAsync(int id);

    }
}
