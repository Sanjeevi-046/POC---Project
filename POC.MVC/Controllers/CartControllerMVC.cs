using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class CartControllerMVC : Controller
    {
        private readonly Uri baseAddress = new Uri("https://localhost:7244/Cart/");
        private readonly Uri tokenurl = new Uri("https://localhost:7244/Login/refreshToken");
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClient2;
        public CartControllerMVC()
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

            var refreshResponse = await _httpClient2.PostAsync($"?refreshToken={refreshToken}", refreshContent);
            //refreshResponse.EnsureSuccessStatusCode();
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
        public async Task<IActionResult> GetCart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            //HttpResponseMessage response = await _httpClient.GetAsync(baseAddress + $"Cart?id={userId}");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"Cart?id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<IEnumerable<ProductModel>>(data);
                return View(products);
            }
            else
            {
                return BadRequest();
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddCart(CartModel cartModel)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(cartModel), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "Carts", content);
            var response = await SendAuthorizedRequestAsync(HttpMethod.Post, "Carts", content);
            if (response.IsSuccessStatusCode)
            {
                ViewBag.IsCartAdded = "True";
                return RedirectToAction("GetProductList", "ProductControllerMVC");
            }
            ViewBag.IsCartAdded = "false";
            return RedirectToAction("GetProductList", "ProductControllerMVC");
        }
    }
}
