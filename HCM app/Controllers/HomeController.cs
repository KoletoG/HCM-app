using System;
using System.Diagnostics;
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
        [Route("register")]
        public IActionResult RegisterMain()
        {
            return View("Register");
        }
        [Route("register/errors")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            UserDataModel user = new UserDataModel();
            user.JobTitle=registerModel.JobTitle;
            user.Salary=registerModel.Salary;
            user.Email=registerModel.Email;
            user.FirstName=registerModel.FirstName;
            user.LastName=registerModel.LastName;
            user.Role = UserRole.Employee;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
