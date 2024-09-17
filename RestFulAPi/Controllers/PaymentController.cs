using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.ServiceLayer.Service;

namespace POC.Api.Controllers
{
    [Route("api/Payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayment _paymentService;
        public PaymentController(IPayment paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost]
        public async Task<IActionResult> MakePayment([FromBody] CommonPaymentModel payment)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _paymentService.DoPayment(payment);
            if (result)
                return Ok(new { Message = "Payment successful" });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, "Payment failed");
        }
    }
}
