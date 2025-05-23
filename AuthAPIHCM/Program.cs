﻿using System.Security.Claims;
using System.Text;
using AuthAPIHCM.Data;
using AuthAPIHCM.Interfaces;
using AuthAPIHCM.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
            var secretkey = Encoding.UTF8.GetBytes("TheSuperSecretKeyOfMineHaha123123123123123123123123123123123123");
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
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
        });
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            }); 
            if (builder.Environment.IsEnvironment("Testing"))
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("TestDb")); 
                builder.Services.AddHttpClient("CRUDAPI", client =>
                    {
                        client.BaseAddress = new Uri("http://localhost");
                    });
            }
            else
            {
                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

                builder.Services.AddHttpClient("CRUDAPI", client =>
                {
                    client.BaseAddress = new Uri("https://localhost:7261/");
                });
            }
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("HrAdminPolicy", x => x.RequireClaim("HrAdmin"));
            });
            builder.Services.AddSingleton<IAuthService, AuthService>();
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
