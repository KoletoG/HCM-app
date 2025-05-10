using System;
using System.Diagnostics;
using HCM_app.Data;
using HCM_app.Migrations;
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
            var users = await _clientCRUD.GetAsync("api/CRUD");
            return View();
        }

        public IActionResult Privacy()
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
