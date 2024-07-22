using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DataAccess.Service;
namespace POC.Api.Controllers
{
    [Route("Order")]
    [ApiController]
    [Authorize(Policy = "AdminOrCustomer")]
    public class OrderControllerApi : ControllerBase
    {
        private readonly IOrder _orderService;
        public OrderControllerApi(IOrder orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("Order")]
        public async Task<IActionResult> CreateOrder( CommonOrderModel order, int orderedProduct)
        {
            try
            {
                if (order == null)
                {
                    return BadRequest("Invalid order or quantity.");
                }
                var result = await _orderService.CreateOrderAsync(order, orderedProduct);
                if (result.IsValid)
                {
                    return Ok(result.Message);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing Order request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("Draft")]
        public async Task<IActionResult> SaveAsDraft([FromBody] CommonTemporderTable order, int orderedProduct, string pincode)
        {
            try
            {
                var result = await _orderService.CreateOrderAsync (order, orderedProduct, pincode);
                if (result.IsValid)
                {
                    return Ok(result.Message);
                }
                return BadRequest(result.Message);

            }
            catch (Exception ex) 
            {
                CustomFileLogger.LogError("An error occurred while processing Order request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
