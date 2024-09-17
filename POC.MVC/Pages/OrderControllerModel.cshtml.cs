using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using POC.MVC.Models;

namespace POC.MVC.Pages
{
    public class OrderControllerModelModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress;
        private readonly HttpClient _httpClient;

        public OrderControllerModelModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            baseAddress = new Uri(_configuration["BaseUrl:Url"]);
            _httpClient.BaseAddress = baseAddress;
        }

        [BindProperty]
        public OrderModel Order { get; set; }

        public string Message { get; set; }
        public string Error { get; set; }

        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            var refreshContent = new StringContent(refreshToken, Encoding.UTF8, "application/json");

            var refreshResponse = await _httpClient.PostAsync(baseAddress + $"Login/refreshToken?refreshToken={refreshToken}", refreshContent);
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

       

        public async Task<IActionResult> OnPostAddOrderAsync(OrderModel order, int orderedProduct)
        {
            order.OrderDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                var userName = HttpContext.Session.GetString("UserName");
                ViewData["UserName"] = userName;
                StringContent content = new StringContent(JsonConvert.SerializeObject(Order), Encoding.UTF8, "application/json");

                var response = await SendAuthorizedRequestAsync(HttpMethod.Post, baseAddress + $"Order/Order?orderedProduct={orderedProduct}", content);

                if (response.IsSuccessStatusCode)
                {
                    Message = await response.Content.ReadAsStringAsync();
                    Error = null;

                    return RedirectToPage("/Test", new { message = Message, userName = userName });
                }
                else
                {
                    return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
                }
            }
            Error = "Model Invalid";
            Message = null;
            return RedirectToAction("UnAuthorized", "ErrorHandling","Model Invalid error" );
        }

    }
}
