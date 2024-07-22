using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Models;



namespace POC.DomainModel.Repository
{
    public class CartServiceRepo : ICartRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IMapper _mapper;

        public CartServiceRepo(DemoProjectContext context , IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CommonCartModel>> GetCartAsync(int id)
        {
            var cartId = _mapper.Map<CartTable>(id);
            var Cartdata = await _context.CartTables.Where(x => x.UserId == cartId.UserId).ToListAsync();
            if (Cartdata != null)
            {
                return _mapper.Map<List<CommonCartModel>>(Cartdata);
            }
            return null;
        }
        public async Task<UserValidationResult> AddCartAsync(CommonCartModel cartTable)
        {
            try
            {
                var CartData = _mapper.Map<CartTable>(cartTable);
                _context.CartTables.Add(CartData);
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
