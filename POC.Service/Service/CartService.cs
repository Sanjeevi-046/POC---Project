using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Models;
using POC.DomainModel.Repository;
using POC.DomainModel.TempModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POC.DataAccess.Service
{
    public class CartService : ICart
    {
        private readonly DemoProjectContext _context;
        private readonly ICartRepo _cartRepoService;
        public CartService(ICartRepo cartRepo)
        {
           _cartRepoService = cartRepo;
        }

        public async Task<List<CommonCartModel>> GetCart(int id )
        {
            var Cartdata = await _cartRepoService.GetCartAsync(id);
            return Cartdata;
        }
        public  async Task<UserValidationResult> AddCart(CommonCartModel cartTable)
        {
            var CartData = await _cartRepoService.AddCartAsync(cartTable);
            return CartData;

        }
    }
}
