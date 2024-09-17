using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;
using Azure.Messaging;

namespace POC.Api.Controllers
{
    [Route("api/Cart")]
    [ApiController]
    
    public class CartController : ControllerBase
    {
        private readonly ICart _cartService;
        private readonly IProduct _productService;
       
        public CartController(ICart cart , IProduct productService)

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
                var productList = new List<CommonProductQuantityModel>();
                if (result!=null)
                {
                    foreach (var item in result)
                    {
                        if (item.ProductId != null)
                        {
                            var productResponse = await _productService.GetProductByIdAsync(item.ProductId);
                            if (productResponse != null)
                            {
                                var quantityItem = 1;
                                if (item.Quantity == 0 || item.Quantity == null)
                                {
                                    quantityItem = quantityItem;
                                }
                                else
                                {
                                    quantityItem = item.Quantity;
                                }
                                var productWithQuantity = new CommonProductQuantityModel
                                {
                                    ProductList = productResponse,
                                    Quantity = quantityItem,
                                    UserID = id

                                };
                                productList.Add(productWithQuantity);
                            }
                            else
                            {
                                return StatusCode(500, "Not Found");
                            }
                        }
                    }
                    return Ok(productList);
                }
                else
                {
                    return Ok(productList=null);
                }
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
        [HttpDelete]
        [Authorize(Policy = "AdminOrCustomer")]
        public async Task<IActionResult> RemoveCart(int ItemId)
        {
            try
            {
                var result = await _cartService.DeleteItem(ItemId);
                if (result)
                {
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing your request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
