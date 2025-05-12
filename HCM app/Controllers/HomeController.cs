using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Ganss.Xss;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SharedModels;

namespace HCM_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _clientAuth;
        private readonly HttpClient _clientCRUD;
        private readonly IHtmlSanitizer _htmlSanitizer;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory client, IHtmlSanitizer htmlSanitizer)
        {
            _logger = logger;
            _clientAuth = client.CreateClient("AuthAPI");
            _clientCRUD = client.CreateClient("CRUDAPI");
            _htmlSanitizer = htmlSanitizer;
            _htmlSanitizer.AllowedTags.Clear();
        }
        public async Task<IActionResult> Index()
        {
            var token = this.HttpContext.Session.GetString("jwt");
            if (token != null)
            {
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>("api/CRUD/users");
                return View(users);
            }
            return RedirectToAction("Login");
        }
        public IActionResult Department()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            loginModel.Email = _htmlSanitizer.Sanitize(loginModel.Email);
            loginModel.Password = _htmlSanitizer.Sanitize(loginModel.Password);
            if (!ModelState.IsValid)
            {
                return View(loginModel);
            }
            var result = await _clientAuth.PostAsJsonAsync<LoginViewModel>("api/auth/login",loginModel);
            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("jwt", token);
                return RedirectToAction("Index", "Home");
            }
            return View(loginModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerModel);
            }
            var result = await _clientAuth.PostAsJsonAsync<RegisterViewModel>("api/auth/register", registerModel);
            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction("Login", "Home");
            }
            return Problem();
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            if(!HttpContext.Session.TryGetValue("jwt",out var token))
            {
                return RedirectToAction("Login","Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            var id = secToken.Claims.First(x=>x.Type == "sub").Value;
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            var user = await _clientCRUD.GetFromJsonAsync<UserDataModel>($"api/CRUD/users/id-{id}");
            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                Email = user.Email,
                FirstName=user.FirstName,
                LastName=user.LastName,
                Department=user.Department,
                JobTitle=user.JobTitle,
                Role = user.Role,
                Salary=user.Salary
            };
            return View(profileViewModel);
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
