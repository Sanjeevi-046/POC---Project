﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;
namespace POC.Api.Controllers
{
    [Route("api/Order")]
    [ApiController]
    [Authorize(Policy = "AdminOrCustomer")]
    
    public class OrderController : ControllerBase
    {
        private readonly IOrder _orderService;
        public OrderController(IOrder orderService)
        {
            _orderService = orderService;
        }
        /// <summary>
        ///  get the user's order
        /// </summary>
        /// <param name="Id"></param>
        /// <returns>
        ///     list of order item in CommonProductQuantityModel 
        /// </returns>
        //[HttpGet]
        //public async Task<IActionResult> GetUserOrders(int Id)
        //{
        //    try
        //    {
                
        //    }
        //    catch 
        //    { 
            
        //    }
        //}





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
                    return Ok(result.PaymentId);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                CustomFileLogger.LogError("An error occurred while processing Order request.", ex);
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("orderList")]
        public async Task<IActionResult> CreateOrder(List<CommonProductQuantityModel> commonProductQuantity)
        {
            try
            {
                if (commonProductQuantity == null)
                {
                    return BadRequest("Invalid order or quantity.");
                }
                var result = await _orderService.CreateOrderAsync(commonProductQuantity);
                if (result.IsValid)
                {
                    return Ok(result.PaymentId);
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
