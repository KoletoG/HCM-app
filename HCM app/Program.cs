using System.Security.Claims;
using System.Text;
using Ganss.Xss;
using HCM_app.Data;
using HCM_app.Interfaces;
using HCM_app.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HCM_app
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            // Add services to the container.
            builder.Services.AddControllersWithViews();
            var secretkey = Encoding.UTF8.GetBytes("TheSuperSecretKeyOfMineHaha123123123123123123123123123123123123");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddHttpClient("AuthAPI", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7029/");
                });

            builder.Services.AddHttpClient("CRUDAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7261/");
            });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IHtmlSanitizer, HtmlSanitizer>();
            builder.Services.AddTransient<IUserInputService, UserInputService>();
            builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters.ValidateIssuer = true;
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidIssuer = "your_issuer";
            options.TokenValidationParameters.ValidAudience = "your_audience";
            options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(secretkey);
            options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        }); builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("HrAdminPolicy", x => x.RequireClaim("HrAdmin"));
        });
            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                await next();
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
