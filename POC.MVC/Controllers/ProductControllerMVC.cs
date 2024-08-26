using Azure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POC.MVC.Models;
using System.Net;
using System.Text;

namespace POC.MVC.Controllers
{
    public class ProductControllerMVC : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Uri baseAddress; 
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductControllerMVC(IConfiguration configuration , IHttpClientFactory httpClientFactory)
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


        [HttpGet]
        public async Task<IActionResult> GetProductList(int page = 1 , string searchName ="")
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = userId;
            ViewBag.searchName = searchName;
            var userName = HttpContext.Session.GetString("UserName");
            ViewBag.UserName = userName;
            var Role = HttpContext.Session.GetString("Role");
            var token = HttpContext.Session.GetString("Token");

            if (Role == "Admin")
            {
                ViewBag.Role = "Admin";
            }
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //HttpResponseMessage response = await _httpClient.GetAsync($"Products?page={page}&searchTerm={searchName}");
            try
            {
                var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"Product/Products?page={page}&searchTerm={searchName}"); //page=1&searchName=I 
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var JsonData = JObject.Parse(data);
                    ViewBag.TotalPages = JsonData["total"].ToString();
                    ViewBag.CurrentPage = JsonData["page"].ToString();
                    var productData = JsonData["product"].ToString();
                    var products = JsonConvert.DeserializeObject<IEnumerable<ProductModel>>(productData);
                    return View(products);
                }
                else
                {
                    return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
                }
            }
            catch (Exception ex) 
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = "404"});
            }

        }

        public async Task<IActionResult> GetProductDetail(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            ViewBag.UserId = userId;
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            //HttpResponseMessage response = await _httpClient.GetAsync(baseAddress + $"getProductById?id={id}");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, $"Product/Product?id={id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductModel>(data);
                return View(product);
            }
			else
			{
				return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });

			}
		}
        
        public IActionResult AddProductDetail() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddProductDetail(ProductModel product , IFormFile productImage)
        {
            var imagepath = productImage.ContentDisposition.ToString();
            byte[] imageData = null;
            if (productImage != null && productImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            // Prepare data to send to API
            var token = HttpContext.Session.GetString("Token");
            var data = new ProductModel
            {
                ProductId = product.ProductId,
                Name= product.Name,
                Price= product.Price,
                Description= product.Description,   
                ProductAvailable = product.ProductAvailable,
                IsAvailable = product.IsAvailable,
                IsQuantityAvailable = product.IsQuantityAvailable,
                ProductImage = imageData
            };
        
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            StringContent content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            //HttpResponseMessage response = await _httpClient.PostAsync(baseAddress + "AddProduct", content);
            var response = await SendAuthorizedRequestAsync(HttpMethod.Post, "Product/AddProduct", content);

            if (response.IsSuccessStatusCode)
            { 
                return RedirectToAction("GetProductList");
            }
            else
            {
                  return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString()});
            }  
        }
        [HttpGet]
        public async Task<IActionResult> ExportProductsToExcel()
        {
            //HttpResponseMessage response = await _httpClient.GetAsync( "ExportProductsToExcel");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, "Product/ExportProductsToExcel");
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
			else
			{
				return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
			}

		}
        [HttpGet]
        public async Task<IActionResult> ExportProductsToHtml()
        {
            //HttpResponseMessage response = await _httpClient.GetAsync( "ExportProductsToExcel");
            var response = await SendAuthorizedRequestAsync(HttpMethod.Get, "Product/ExportProductsToHtml");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var contentType = response.Content.Headers.ContentType.ToString();
                var contentDisposition = response.Content.Headers.ContentDisposition;
                if (contentDisposition != null)
                {
                    var filename = contentDisposition.FileName;
                    return File(stream, contentType, filename);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return RedirectToAction("UnAuthorized", "ErrorHandling", new { statusCode = response.StatusCode.ToString() });
            }

        }

    }
}
