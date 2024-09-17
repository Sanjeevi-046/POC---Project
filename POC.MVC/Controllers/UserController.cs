using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress;
        private readonly IHttpClientFactory _httpClientFactory;
        public UserController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            using var _httpClient = _httpClientFactory.CreateClient();
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
            using var _httpClient = _httpClientFactory.CreateClient();
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

        public async Task<IActionResult> profile()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"User?Id={userId}");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var profile = JsonConvert.DeserializeObject<UserVm>(data);
                return View(profile);
            }
            if(response.StatusCode == HttpStatusCode.NotFound)
            {
                TempData["Error"] = "Data Not Found";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> profile(UserVm userVm)
        {
            if (ModelState.IsValid) 
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(userVm), Encoding.UTF8, "application/json");

                var response = await SendAuthorizedRequestAsync(HttpMethod.Put, "User", content);

                if (response.IsSuccessStatusCode)
                {
                    // Handle success
                    var result = await response.Content.ReadAsStringAsync();
                    TempData["Success"] = "Updated Successfully!";
                    // Process the result if needed
                    return RedirectToAction("Products","ProductControllerMVC");
                }
                else
                {

                    ModelState.AddModelError(string.Empty, "An error occurred while updating the user.");
                    return View(userVm);
                }
            }
            return View();
        }
    }
}
