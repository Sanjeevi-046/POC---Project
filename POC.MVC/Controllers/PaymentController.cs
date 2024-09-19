using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using POC.CommonModel.Models;
using POC.MVC.Authorization;
using POC.MVC.Models;
using System.Text;

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
            TempData["PaymentID"] = PaymentID;
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Put, $"Payment?addressId={AddressId}&paymentId={PaymentID}");
            if (response.IsSuccessStatusCode)
            {
                return View();
            }
            return RedirectToAction("SelectAddress", "Address", new {Id = PaymentID});
        }
        [HttpPost]
        public async Task<IActionResult> DoPayment(CommonPaymentModel paymentModel)
        {
            if (paymentModel.Id!=null || paymentModel.CardNumber !=null||paymentModel.Cvv!=null) 
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(paymentModel), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Post, $"Payment",content);
                if (response.IsSuccessStatusCode)
                {

                    return RedirectToAction("","");
                }
            }
            TempData["PaymentID"] = paymentModel.Id;
            return View();
        }
    }
}
