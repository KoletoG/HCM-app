using System;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
