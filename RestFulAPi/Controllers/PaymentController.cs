using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/Payment")]
    [ApiController]
    [AllowAnonymous]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment _paymentService;
        public PaymentController(IPayment paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPut]
        public async Task<IActionResult> updatePaymentAddress(int addressId,int paymentId)
        {
            var result = await _paymentService.UpdatePaymentAddress(addressId, paymentId);
            if (result)
                return Ok(new { Message = "updated" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Payment failed");
        }

        [HttpPost]
        public async Task<IActionResult> MakePayment([FromBody] CommonPaymentModel payment)
        {
            
            var result = await _paymentService.DoPayment(payment);
            if (result)
                return Ok(new { Message = "Payment successful" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Payment failed");
        }
    }
}
