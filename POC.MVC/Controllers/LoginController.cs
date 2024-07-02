using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.DomainModel.Models;
using POC.DomainModel.TempModel;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class LoginController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:7244/Login");
        private readonly HttpClient _httpClient;
        public LoginController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        
        [HttpGet]
        public IActionResult LoginPage()
        {
            HttpResponseMessage response = _httpClient.GetAsync(baseAddress + "/Get").Result;
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.Content);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginPage(Login login)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "/checkUser", content);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(data);
                var token = json["token"].ToString();
                var Role = json["role"].ToString();
                var RefershToken = json["refreshToken"].ToString();
                HttpContext.Session.SetString("Role", Role);
                HttpContext.Session.SetString("Token", token);
                HttpContext.Session.SetString("RefreshToken", RefershToken);
                HttpContext.Session.SetString("UserName", login.Name);
                HttpResponseMessage userIdResponse = await _httpClient.GetAsync(baseAddress + "/getID?name=" + login.Name);
                if (userIdResponse.IsSuccessStatusCode)
                {
                    var userId = await userIdResponse.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("UserId", userId);
                    ViewBag.Error = null;
                    return RedirectToAction("GetProductList", "ProductControllerMVC");
                }
                else
                {
                    var errorMessage = await userIdResponse.Content.ReadAsStringAsync();
                    ViewBag.Error = errorMessage;
                    return View();
                }

            }
            else
            {
                var errorMessage = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                ViewBag.Error = errorMessage;
                return View();
            }
        }
        [HttpGet]
        public IActionResult NewUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> NewUser(string RePassword, Login login)
        {
            var newUserModel = new UserRegistrationModel { rePassword = RePassword, Login = login };
            StringContent content = new StringContent(JsonConvert.SerializeObject(newUserModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "/newUser", content);
            if (response.IsSuccessStatusCode)
            {

                ViewBag.Error = "Registered Successfully";
                return RedirectToAction("LoginPage");
            }
            else
            {
                var errorMessage = response.Content.ReadAsStringAsync();
                ViewBag.Error = errorMessage;
                return View();
            }
        }
    }
}