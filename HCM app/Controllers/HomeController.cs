using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        }
        public async Task<IActionResult> Index()
        {
            var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>("api/CRUD/users");
            return View(users);
        }
        public async Task<IActionResult> AddUser()
        {
            var currentEmail = this.HttpContext.Session.Get("email");
            var currentRole = this.HttpContext.Session.Get("role");
            var user = await _clientCRUD.GetAsync($"api/CRUD/users/{currentEmail}");
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
        public IActionResult LoginMain()
        {
            return View("Login");
        }
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            var result = await _clientAuth.PostAsJsonAsync<LoginViewModel>("api/auth/login",loginModel);
            if (result.IsSuccessStatusCode)
            {
                var token = await result.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("jwt", token);
                HttpContext.Session.SetString("email",loginModel.Email);
                var user = await _clientCRUD.GetFromJsonAsync<UserDataModel>($"api/CRUD/users/email-{loginModel.Email}");
                HttpContext.Session.SetString("role", user.Role.ToString());
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
