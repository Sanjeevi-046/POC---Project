using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using POC.MVC.Authorization;
using Newtonsoft.Json;
using POC.CommonModel.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Text;
using System.ComponentModel;

namespace POC.MVC.Controllers
{
    public class AddressController : Controller
    {
        private readonly AuthorizedRequest _authorizedRequest;

        public AddressController(AuthorizedRequest authorizedRequest)
        {
            _authorizedRequest = authorizedRequest;
        }

        public async Task<IActionResult> Address()
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account"); 
            }
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Address?Id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var Addresses = JsonConvert.DeserializeObject<List<CommonAddressModel>>(responseData);
                return View(Addresses);
            }
            else
            {
                return View("Error"); 
            }
        }
        //Add Address Method
        public async Task<IActionResult> Add()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(CommonAddressModel commonAddressModel)
        {
            var userId = HttpContext.Session.GetString("UserId");
            commonAddressModel.UserId = int.Parse(userId);
            StringContent content = new StringContent(JsonConvert.SerializeObject(commonAddressModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Post, $"Address",content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Added the Address!";
                return RedirectToAction("Address");
            }
            else
            {
                return View("Error");
            }
        }
        // GET: Address/Edit/
        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Address/Address?Id={id}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var address = JsonConvert.DeserializeObject<CommonAddressModel>(responseData);
                return View(address);
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CommonAddressModel commonAddressModel)
        {
            var userId = HttpContext.Session.GetString("UserId");
            commonAddressModel.UserId = int.Parse(userId);
            StringContent content = new StringContent(JsonConvert.SerializeObject(commonAddressModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Put, $"Address", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address updated successfully!";
                return RedirectToAction("Address");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Delete, $"Address/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address deleted successfully!";
                return RedirectToAction("Address");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SetAsDefault(int AddressId)
        {
            var userId = HttpContext.Session.GetString("UserId");
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Put, $"Address/Set-Default?Id={AddressId}");
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = " Updated default Address!";
                return RedirectToAction("Address");
            }
            else
            {
                return View("Error");
            }
        }
        [HttpGet]
        public async Task<IActionResult> SelectAddress(string Id)
        {
            TempData["PaymentID"] = Id;
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }
            HttpResponseMessage response = await _authorizedRequest.SendAuthorizedRequestAsync(HttpMethod.Get, $"Address?Id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var Addresses = JsonConvert.DeserializeObject<List<CommonAddressModel>>(responseData);
                return View(Addresses);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
