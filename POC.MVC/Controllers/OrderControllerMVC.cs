using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.MVC.Models;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class OrderControllerMVC : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7244/Order/");
        private readonly Uri tokenurl = new Uri("https://localhost:7244/Login/refreshToken");
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClient2;
        public OrderControllerMVC()
        {
            _httpClient = new HttpClient();
            _httpClient2 = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
            _httpClient2.BaseAddress = tokenurl;

        }
        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var refreshContent = new StringContent(refreshToken, Encoding.UTF8, "application/json");

            var refreshResponse = await _httpClient2.PostAsync(tokenurl + $"?refreshToken={refreshToken}", refreshContent);
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



        [HttpGet]
        public IActionResult AddOrder()
        {
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
                //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + $"createOrder?orderedProduct={orderedProduct}", content);
                var response = await SendAuthorizedRequestAsync(HttpMethod.Post, baseAddress + $"createOrder?orderedProduct={orderedProduct}", content);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = await response.Content.ReadAsStringAsync();
                    ViewBag.Error = null;
                    return View();
                }
                else
                {
                    ViewBag.Error = await response.Content.ReadAsStringAsync();
                    ViewBag.Message = null;
                    return View();
                }
            }
            ViewBag.Error = "Model Invalid";
            ViewBag.Message = null;
            return View();
        }

    }
}
