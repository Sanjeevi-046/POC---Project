using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.MVC.Models;
using System.Net;
using System.Net.Http;
using System.Text;

namespace POC.MVC.Controllers
{
    public class ProductControllerMVC : Controller
    {
        private readonly Uri baseAddress = new Uri("https://localhost:7244/Product/");
        private readonly Uri tokenurl = new Uri("https://localhost:7244/Login/refreshToken");
        private readonly HttpClient _httpClient;
        private readonly HttpClient _httpClient2;
        public ProductControllerMVC()
        {
            _httpClient = new HttpClient ();
            _httpClient2 = new HttpClient ();
            _httpClient.BaseAddress= baseAddress;
            _httpClient2.BaseAddress= tokenurl;
           
        }
        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null; 
            }
            
            var refreshContent = new StringContent( refreshToken ,Encoding.UTF8,"application/json");
            
            var refreshResponse = await _httpClient2.PostAsync($"?refreshToken={refreshToken}", refreshContent);
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
        public async Task<IActionResult> GetProductList()
        {
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            var Role = HttpContext.Session.GetString("Role");
          
            if (Role == "Admin")
            {
                ViewBag.Role = "Admin";
            }
           
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //HttpResponseMessage response = await _httpClient.GetAsync("getProductList");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, "getProductList");
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
        public async Task<IActionResult> GetProductDetail(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = userId;
            
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //HttpResponseMessage response = await _httpClient.GetAsync(baseAddress + $"getProductById?id={id}");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"getProductById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductModel>(data);
                return View(product);
            }
            return NotFound();
        }
        
        public IActionResult AddProductDetail() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProductDetail(ProductModel product)
        {
           
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "AddProduct", content);
            var response = await SendAuthorizedRequestAsync(HttpMethod.Post, "AddProduct", content);

            if (response.IsSuccessStatusCode)
            {
                //return method should be 201 created 
                return RedirectToAction("GetProductList");
                //return CreatedAtAction("GetProductList",null);
            }
            return NotFound();
           
        }
        [HttpGet]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            //HttpResponseMessage response = await _httpClient.GetAsync( "ExportProductsToExcel");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, "ExportProductsToExcel");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.ToString();

                var contentDisposition = response.Content.Headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    var filename = contentDisposition.FileName;
                    return File(stream,contentType,filename);

                }
                else
                {
                    return NotFound();
                }
                
            }
            return View();
            
        }



    }
}
