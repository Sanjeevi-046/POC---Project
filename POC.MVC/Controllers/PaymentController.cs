using Microsoft.AspNetCore.Mvc;
using POC.CommonModel.Models;
using POC.MVC.Authorization;

namespace POC.MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly AuthorizedRequest _authorizedRequest;
        public PaymentController(AuthorizedRequest authorizedRequest) 
        {
            _authorizedRequest = authorizedRequest;
        }
        public async Task<IActionResult> DoPayment(int AddressId,int PaymentID)
        {
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DoPayment(CommonPaymentModel paymentModel)
        {
            if (ModelState.IsValid) 
            { 
                
            }
            TempData["PaymentID"] = paymentModel.Id;
            return View();
        }
    }
}
