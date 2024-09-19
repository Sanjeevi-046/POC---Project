using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.CommonModel.Models;
using POC.MVC.Authorization;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly AuthorizedRequest _authorizedRequest;

        public OrderController(AuthorizedRequest authorizedRequest)
        {
            _authorizedRequest = authorizedRequest;
        }

        [HttpPost]
        public async Task<IActionResult> Orders()
        {
            var userId = HttpContext.Session.GetString("UserId");

            var httpResponse = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"");

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderModel order, int orderedProduct)
        {
            order.OrderDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = userName;
                StringContent content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                var response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Post, $"Order/Order?orderedProduct={orderedProduct}", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = await response.Content.ReadAsStringAsync();
                    var Message = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = null;

                    return RedirectToAction("SelectAddress", "Address", new { Id = Message });
                }
                else
                {
                    return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
                }
            }
            ViewBag.Error = "Model Invalid";
            ViewBag.Message = null;
            return RedirectToAction("UnAuthorized", "ErrorHandling", "Model Invalid error");
        }
        [HttpPost]
        public async Task<IActionResult> AddOrderList(List<CommonProductQuantityModel> productQuantityModel)
        {
            
            
                var userName = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = userName;
                StringContent content = new StringContent(JsonConvert.SerializeObject(productQuantityModel), Encoding.UTF8, "application/json");
                var response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Post, $"Order/orderList", content);

                if (response.IsSuccessStatusCode)
                {
                    var Message = await response.Content.ReadAsStringAsync();

                    return RedirectToAction("SelectAddress", "Address", new { Id = Message });
                }
                else
                {
                    return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
                }
            
        }
        [HttpPost]
        public async Task<IActionResult> SaveAsDraft(OrderModel order, int orderedProduct, string pincode = "draft")
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
            var token = HttpContext.Session.GetString("Token");
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync( HttpMethod.Post,$"Order/Draft?orderedProduct={orderedProduct}&pincode={pincode}", content);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Message = await response.Content.ReadAsStringAsync();
                ViewBag.Error = null;
                return View();
            }
            else
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
            }
        }

    }
}
