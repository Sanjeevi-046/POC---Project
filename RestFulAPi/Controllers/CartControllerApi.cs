using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    
    public class CartControllerApi : ControllerBase
    {
        private readonly ICart _cartService;
        private readonly IProduct _productService;
       
        public CartControllerApi(ICart cart , IProduct productService)

        {
            _cartService = cart;
            _productService = productService;
        }

        [HttpGet("cart")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> GetCart(int id)
        {
            try
            {
                var result = await _cartService.GetCart(id);
                var productList = new List<CommonProductModel>();

                foreach (var item in result)
                {
                    if (item.ProductId.HasValue)
                    {
                        var productResponse = await _productService.GetProductByIdAsync(item.ProductId.Value);
                        if (productResponse!=null)
                        { 
                            productList.Add(productResponse);                            
                        }
                        else
                        {
                            return StatusCode(500, "Not Found");
                        }
                    }
                }
                return Ok(productList);
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500 , "Internal server error");
            }
        }

        [HttpPost("Carts")]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> AddCart(CommonCartModel cartTable)
        {
            try
            {
                var result = await _cartService.AddCart(cartTable);
                if (result.IsValid)
                {
                    return CreatedAtAction(null, result.Message);
                }
                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
