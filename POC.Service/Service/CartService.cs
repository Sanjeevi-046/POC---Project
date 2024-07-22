using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using POC.DomainModel.Models;
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
        public CartService(DemoProjectContext context)
        {
            _context = context;
        }

        public async Task<List<CartTable>> GetCart(int id )
        {
            var Cartdata = await _context.CartTables.Where(x => x.UserId==id).ToListAsync();
            if (Cartdata != null) 
            {
                    return Cartdata;
            }
            return null;
        }
        public  async Task<UserValidationResult> AddCart(CartTable cartTable)
        {
            try
            {
                _context.CartTables.Add(cartTable);
                _context.SaveChanges();

                return new UserValidationResult { IsValid = true, Message = "Product Added Successfully" };
            }
            catch (Exception ex)
            {
                return new UserValidationResult { IsValid = false, Message = ex.ToString() };
            }

        }
    }
}
