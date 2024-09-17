using Newtonsoft.Json.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace POC.MVC.Authorization
{
    public class AuthorizedRequest
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public AuthorizedRequest(IConfiguration configuration, IHttpClientFactory httpClientFactory, IHttpContextAccessor httpcontextAccessor)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _httpcontextAccessor = httpcontextAccessor;

        }

        private async Task<string> RefreshTokenAsync()
        {
            var refreshToken = _httpcontextAccessor.HttpContext.Session.GetString("RefreshToken");

            if (string.IsNullOrEmpty(refreshToken))
            {
                return null;
            }

            using var _httpClient = _httpClientFactory.CreateClient();
           
            var refreshContent = new StringContent(refreshToken, Encoding.UTF8, "application/json");

            var refreshResponse = await _httpClient.PostAsync($"Login/refreshToken?refreshToken={refreshToken}", refreshContent);
            if (refreshResponse.IsSuccessStatusCode)
            {
                var refreshData = await refreshResponse.Content.ReadAsStringAsync();
                var refreshJson = JObject.Parse(refreshData);
                var newToken = refreshJson["token"].ToString();
                var newRefreshToken = refreshJson["refreshToken"].ToString();

                _httpcontextAccessor.HttpContext.Session.SetString("Token", newToken);
                _httpcontextAccessor.HttpContext.Session.SetString("RefreshToken", newRefreshToken);

                return newToken;
            }
            else
            {
                return null;
            }
        }

        public async Task<HttpResponseMessage> SendAuthorizedRequestAsync(HttpMethod method, string requestUri, HttpContent content = null)
        {
            using var _httpClient = _httpClientFactory.CreateClient();
            var token = _httpcontextAccessor.HttpContext.Session.GetString("Token");
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
    }
}
