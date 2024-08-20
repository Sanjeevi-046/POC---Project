using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol.Plugins;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class OrderControllerMVC : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress;
        private readonly HttpClient _httpClient;
        public OrderControllerMVC(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            baseAddress = new Uri(_configuration["BaseUrl:Url"]);
            _httpClient.BaseAddress = baseAddress;
        }
        
        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var refreshContent = new StringContent(refreshToken, Encoding.UTF8, "application/json");

            var refreshResponse = await _httpClient.PostAsync( baseAddress + $"Login/refreshToken?refreshToken={refreshToken}", refreshContent);
            refreshResponse.EnsureSuccessStatusCode();
            if (refreshResponse.IsSuccessStatusCode)
            {
                var refreshData = await refreshResponse.Content.ReadAsStringAsync();
                var refreshJson = JObject.Parse(refreshData);
                var newToken = refreshJson["token"].ToString();
                var newRefreshToken = refreshJson["refreshToken"].ToString();

                HttpContext.Session.SetString("Token", newToken);
                HttpContext.Session.SetString("RefreshToken", newRefreshToken);

                return newToken;
            }
            else
            {
                return null;
            }
        }
        private async Task<HttpResponseMessage> SendAuthorizedRequestAsync(HttpMethod method, string requestUri, HttpContent content = null)
        {
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
            {
                Content = content
            });

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                token = await RefreshTokenAsync();

                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                    response = await _httpClient.SendAsync(new HttpRequestMessage(method, requestUri)
                    {
                        Content = content
                    });
                }
            }

            return response;
        }



        //[HttpGet]
        //public IActionResult AddOrder()
        //{
        //    return View();
        //}

        [HttpPost]
        public async Task<IActionResult> AddOrder(OrderModel order, int orderedProduct)
        {
            order.OrderDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                ViewBag.UserName = userName;
                StringContent content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + $"createOrder?orderedProduct={orderedProduct}", content);
                var response = await SendAuthorizedRequestAsync(HttpMethod.Post, baseAddress + $"Order/Order?orderedProduct={orderedProduct}", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = await response.Content.ReadAsStringAsync();
                    var Message = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = null;
                    //return RedirectToAction("Test", "Views", new { Message = Message, UserName = userName });

                    return RedirectToPage("/Test",new{Message=Message, UserName = userName });
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
    public async Task<IActionResult> SaveAsDraft(OrderModel order, int orderedProduct, string pincode = "draft")
    {
        StringContent content = new StringContent(JsonConvert.SerializeObject(order), Encoding.UTF8, "application/json");
        //var response = await SendAuthorizedRequestAsync(HttpMethod.Post, baseAddress + $"Order?orderedProduct={orderedProduct}&pincode={pincode}", content); //Order?orderedProduct=3&pincode=sanj
        var token = HttpContext.Session.GetString("Token");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + $"Order/Draft?orderedProduct={orderedProduct}&pincode={pincode}", content);
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
