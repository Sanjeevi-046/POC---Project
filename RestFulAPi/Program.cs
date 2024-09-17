using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using POC.AutoMapper.Mapper;
using POC.DataLayer.Context;
using POC.DataLayer.Repository;
using POC.ServiceLayer.Service;
using System.Text;
using POC.DataLayer.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAutoMapper(typeof(MappingProfiles));
        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<DemoProjectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
        builder.Services.AddDbContext<AdminDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Connection")));
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("*")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
        });

        //for calling another controller using httpclient we need to addhttpclient
        builder.Services.AddHttpClient();
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("Customer", policy => policy.RequireRole("Customer"));
            options.AddPolicy("AdminOrCustomer", policy => policy.RequireRole("Admin", "Customer"));

        });
        // Add Swagger services
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
            option.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                }
            );
            option.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
                }
            );
        });
        builder.Services.AddScoped<ILogin, LoginService>();
        builder.Services.AddScoped<IProduct, ProductService>();
        builder.Services.AddScoped<IOrder, OrderService>();
        builder.Services.AddScoped<ICart, CartService>();
        builder.Services.AddScoped<ILoginRepo, LoginRepository>();
        builder.Services.AddScoped<IProductRepo, ProductRepository>();
        builder.Services.AddScoped<IOrderRepo, OrderRepository>();
        builder.Services.AddScoped<ICartRepo, CartRepository>();
        builder.Services.AddTransient<IUser, UserService>();
        builder.Services.AddTransient<IUserRepo, UserRepository>();
        builder.Services.AddTransient<ILogout, LogoutService>();
        builder.Services.AddTransient<ILogoutRepo, LogoutRepository>();
        builder.Services.AddTransient<IAddress, AddressService>();
        builder.Services.AddTransient<IAddressRepo, AddressRepository>();
        builder.Services.AddTransient<IPayment, PaymentService>();
        builder.Services.AddTransient<IPaymentRepo, PaymentRepository>();
        builder.Services.AddTransient<IAdmin, AdminService>();
        builder.Services.AddTransient<IAdminRepo, AdminRepository>();


        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            //app.UseSwaggerUI();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Demo API v1"));
        }
        app.UseCors();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}