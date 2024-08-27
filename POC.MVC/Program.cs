using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Session;
using POC.MVC.Middleware;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var BaseUrl = builder.Configuration.GetSection("BaseUrl");
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("",_httpClient=> { _httpClient.BaseAddress = new Uri(BaseUrl["Url"]);
});
builder.Services.AddMvc().AddViewOptions(options =>
    options.HtmlHelperOptions.ClientValidationEnabled = true);
builder.Services.AddControllers(options =>
{
    options.ModelMetadataDetailsProviders.Add(new SystemTextJsonValidationMetadataProvider());
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout period
    options.Cookie.HttpOnly = true; // Cookie settings
    options.Cookie.IsEssential = true;
});

builder.Services.AddRazorPages();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
//app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseRouting();
app.MapRazorPages();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
