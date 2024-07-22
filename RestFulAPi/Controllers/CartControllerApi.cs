using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.DataAccess.Service;
using POC.DomainModel.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authorization;
using POC.CommonModel.Models;

namespace POC.Api.Controllers
{
    [Route("Cart")]
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
                            return StatusCode(500, "Error retrieving product details.");
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
