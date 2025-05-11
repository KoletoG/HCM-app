using System.Text;
using AuthAPIHCM.Data;
using AuthAPIHCM.Interfaces;
using AuthAPIHCM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedModels;

namespace AuthAPIHCM
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "your_issuer",
                ValidAudience = "your_audience",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TheSuperSecretKeyOfMineHaha"))
            };
        }); 
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            });
        }); 
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            }); 
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HrAdminPolicy", policy => policy.RequireRole(UserRole.HrAdmin.ToString()));
            });
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllers();

            app.Run();
        }
    }
}
