using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POC.DataAccess.Service;
using POC.DomainModel.Models;
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
        [HttpPost("createOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order, int orderedProduct)
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
    }
}
