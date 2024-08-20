using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Repository;
using POC.DomainModel.Models;

namespace POC.ServiceLayer.Service
{
    public class CartService : ICart
    {
        private readonly DemoProjectContext _context;
        private readonly ICartRepo _cartRepoService;
        public CartService(ICartRepo cartRepo)
        {
            _cartRepoService = cartRepo;
        }

        public async Task<List<CommonCartModel>> GetCart(int id)
        {
            var Cartdata = await _cartRepoService.GetCartAsync(id);
            return Cartdata;
        }
        public async Task<UserValidationResult> AddCart(CommonCartModel cartTable)
        {
            var CartData = await _cartRepoService.AddCartAsync(cartTable);
            return CartData;

        }
    }
}
