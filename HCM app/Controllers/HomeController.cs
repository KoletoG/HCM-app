using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace HCM_app.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _clientAuth;
        private readonly HttpClient _clientCRUD;
        public HomeController(ILogger<HomeController> logger, IHttpClientFactory client)
        {
            _logger = logger;
            _clientAuth = client.CreateClient("AuthAPI");
            _clientCRUD = client.CreateClient("CRUDAPI");
            /*
            if(HttpContext.Session.TryGetValue("jwt", out var token))
            {
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                _clientAuth.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
            }
            */
        }
        public async Task<IActionResult> Index()
        {
            var token = this.HttpContext.Session.GetString("jwt");
            var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>("api/CRUD/users");
            return View(users);
        }
        public async Task<IActionResult> AddUser()
        {
            return View();
        }
        public async Task<IActionResult> AddUserMain()
        {
            return View();
        }
        public IActionResult Privacy()
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
            return View();
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
                return RedirectToAction("LoginMain", "Home");
            }
            return View();
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
