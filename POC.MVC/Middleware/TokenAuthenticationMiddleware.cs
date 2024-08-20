using Newtonsoft.Json.Linq;
using System.Text;

namespace POC.MVC.Middleware
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public TokenAuthenticationMiddleware(RequestDelegate next, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _next = next;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Session.GetString("Token");
            var refreshToken = context.Session.GetString("RefreshToken");

            if (context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer "))
                {
                    token = authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                token = await RefreshTokenAsync(context, refreshToken);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Session.SetString("Token", token);
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                }
            }
            else if (!string.IsNullOrEmpty(token))
            {
                context.Request.Headers["Authorization"] = $"Bearer {token}";
            }

            await _next(context);
        }

        private async Task<string> RefreshTokenAsync(HttpContext context, string refreshToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            var refreshContent = new StringContent($"{{\"refreshToken\":\"{refreshToken}\"}}", Encoding.UTF8, "application/json");

            var refreshResponse = await httpClient.PostAsync($"{_configuration["ApiBaseUrl"]}/Login/refreshToken", refreshContent);
            if (refreshResponse.IsSuccessStatusCode)
            {
                var refreshData = await refreshResponse.Content.ReadAsStringAsync();
                var refreshJson = JObject.Parse(refreshData);
                return refreshJson["token"].ToString();
            }
            return null;
        }
    }

    public static class TokenAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenAuthenticationMiddleware>();
        }
    }
}
