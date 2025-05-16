
using System.Security.Claims;
using System.Text;
using CRUDHCM_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CRUDHCM_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
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
            builder.Services.AddAuthorization(options => 
            {
                options.AddPolicy("HrAdminPolicy", x => x.RequireClaim("HrAdmin")); 
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
