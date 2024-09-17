using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataLayer.Models;



namespace POC.DataLayer.Repository
{
    public class CartRepository : ICartRepo
    {
        private readonly DemoProjectContext _context;
        private readonly IProductRepo _productRepo;
        private readonly IMapper _mapper;

        public CartRepository(DemoProjectContext context, IMapper mapper, IProductRepo productRepo)
        {
            _context = context;
            _mapper = mapper;
            _productRepo = productRepo;

        }
        public async Task<List<CommonCartModel>> GetCartAsync(int id)
        {
            var Cartdata = await _context.CartTables.Where(x => x.UserId == id).ToListAsync();
            if (Cartdata.Count != 0)
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
                var data = await _context.CartTables.FirstOrDefaultAsync(x => x.UserId == CartData.UserId && x.ProductId == CartData.ProductId);

                if (data == null)
                {
                    var productData = await _productRepo.GetProductById(cartTable.ProductId);
                    if (productData != null)
                    {
                        if (productData.IsAvailable && productData.ProductAvailable > 0)
                        {
                            _context.CartTables.Add(CartData);
                            _context.SaveChanges();
                            return new UserValidationResult { IsValid = true, Message = "Product Added To Cart" };
                        }
                        else
                        {
                            return new UserValidationResult { IsValid = false, Message = "Product Not Available!" };
                        }
                    }
                    return new UserValidationResult { IsValid = false, Message = "Product Not Available!" };
                }
                else
                {
                    var cartDetail = _context.CartTables.Find(data.CartId);
                    if (cartDetail != null)
                    {
                        if (cartDetail.Quantity == null)
                        {
                            cartDetail.Quantity = 1;
                        }
                        cartDetail.Quantity = cartDetail.Quantity + 1;
                        _context.CartTables.Update(cartDetail);
                        _context.SaveChanges();
                        return new UserValidationResult { IsValid = true, Message = "Product Added to Cart" };
                    }
                    return new UserValidationResult { IsValid = false, Message = "Product Not Added!" };
                }

            }
            catch (Exception ex)
            {
                return new UserValidationResult { IsValid = false, Message = ex.ToString() };
            }

        }

        public async Task<bool> DeleteCartAsync(int id)
        {
            var deleteItem = await _context.CartTables.FirstOrDefaultAsync(x=>x.ProductId==id);
            if (deleteItem != null)
            {
                _context.CartTables.Remove(deleteItem);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
