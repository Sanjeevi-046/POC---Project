using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.CommonModel.Models;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class CartController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress;
        private readonly HttpClient _httpClient;
       
        public CartController(IConfiguration configuration)
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

            var refreshResponse = await _httpClient.PostAsync($"Login/refreshToken?refreshToken={refreshToken}", refreshContent);
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
        public async Task<IActionResult> Cart()
        {
            var userId = HttpContext.Session.GetString("UserId");
            //HttpResponseMessage response = await _httpClient.GetAsync(baseAddress + $"Cart?id={userId}");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"Cart/Cart?id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                int productCount = 0;
                if (!string.IsNullOrEmpty(data))
                {
                    JArray jsonArray = JArray.Parse(data);
                    productCount = jsonArray.Count;
                }
                
                TempData["productCount"] = productCount;
                var products = JsonConvert.DeserializeObject<List<CommonProductQuantityModel>>(data);
                return View(products);
            }
            else
            {
                var message = response.Content.ReadAsStringAsync();
                TempData["Error"] = message;
                return RedirectToAction("Products", "Product");
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> AddCart(CartModel cartModel)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(cartModel), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "Carts", content);
            var response = await SendAuthorizedRequestAsync(HttpMethod.Post, "Cart/Carts", content);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                TempData["Message"] = data;
                return RedirectToAction("Products", "Product");
            }
            var Error = await response.Content.ReadAsStringAsync();
            TempData["Error"] = Error;
            return RedirectToAction("Products", "Product");
        }

        public async Task<IActionResult> Delete(int ItemId)
        {
            var response = await SendAuthorizedRequestAsync(HttpMethod.Delete, $"Cart?ItemId={ItemId}");
            if (response.IsSuccessStatusCode) 
            {
                return RedirectToAction("Cart", "Cart");
            }
            return RedirectToAction("Cart", "Cart");
        }

    }
}
