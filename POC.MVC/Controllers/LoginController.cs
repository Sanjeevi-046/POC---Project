using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Poc.CommonModel.Models;
using POC.CommonModel.Models;
using POC.DomainModel.Models;
using System.Configuration;
using System.Text;

namespace POC.MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress;
        private readonly HttpClient _httpClient;
        
        public LoginController( IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
             baseAddress = new Uri(_configuration["BaseUrl:Url"]);
            _httpClient.BaseAddress = baseAddress;
           
        }

        [HttpGet]
        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginPage(Login login)
        {
            try
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "Login/User", content);

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
                    HttpResponseMessage userIdResponse = await _httpClient.GetAsync(baseAddress + "Login/ID?name=" + login.Name);
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
            catch (Exception ex)
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = ex.ToString()});
            }
        }

        [HttpGet]
        public IActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewUser(string RePassword, CommonLoginModel login)
        {
            var newUserModel = new UserRegistrationModel { rePassword = RePassword, Login = login };
            StringContent content = new StringContent(JsonConvert.SerializeObject(newUserModel), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "Login/Users", content);
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