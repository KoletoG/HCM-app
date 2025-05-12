using System;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Web;
using Ganss.Xss;
using HCM_app.Migrations;
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
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUsersManager()
        {
            if (!HttpContext.Session.TryGetValue("jwt", out var token))
            {
                return RedirectToAction("Login", "Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            if (secToken.Claims.First(x => x.Type == ClaimTypes.Role).Value != "Manager")
            {
                return RedirectToAction("Login", "Home");
            }
            var department = secToken.Claims.First(x => x.Type == "Department").Value;
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users/department-{department}");
            return View(new UsersToUpdateViewModel(users));
        }
        [HttpGet]
        public async Task<IActionResult> UpdateUsersAdmin()
        {
            if (!HttpContext.Session.TryGetValue("jwt", out var token))
            {
                return RedirectToAction("Login", "Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            if (secToken.Claims.First(x => x.Type == ClaimTypes.Role).Value != "HrAdmin")
            {
                return RedirectToAction("Login", "Home");
            }
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users");
            return View(new UsersToUpdateViewModel(users));
        }
        private void SanitizeInput(List<DepartmentUpdateViewModel> viewModel)
        {
            foreach (var user in viewModel)
            {
                user.Id = _htmlSanitizer.Sanitize(user.Id);
                user.Department = _htmlSanitizer.Sanitize(user.Department);
                user.JobTitle = _htmlSanitizer.Sanitize(user.JobTitle);
                user.Email = _htmlSanitizer.Sanitize(user.Email);
                user.FirstName = _htmlSanitizer.Sanitize(user.FirstName);
                user.LastName = _htmlSanitizer.Sanitize(user.LastName);
                user.Role = _htmlSanitizer.Sanitize(user.Role);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsersManager(List<DepartmentUpdateViewModel> users)
        {
            if (!HttpContext.Session.TryGetValue("jwt", out var token))
            {
                return RedirectToAction("Login", "Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            var role = secToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            if (role != "Manager")
            {
                return RedirectToAction("Index", "Home");
            }
            SanitizeInput(users);
            ChangeRoleNaming(users);
            if (HasInvalidRole(users))
            {
                ModelState.AddModelError("roleError", "Invalid role, role should be one of the following - Employee / Manager / HrAdmin");
            }
            if (!IsValidSalary(users))
            {
                ModelState.AddModelError("salaryError", "Invalid salary, salary should be higher than 0");
            }
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            if (!ModelState.IsValid)
            {
                var usersForOutput = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users");
                List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return View(new UsersToUpdateViewModel(usersForOutput, errors));
            }
            var id = secToken.Claims.First(x => x.Type == "sub").Value;
            var department = secToken.Claims.First(x => x.Type == "Department").Value;
            var userIdsToDeleteList = users.Where(x => x.ShouldDelete).Select(x => x.Id).ToList();
            department=HttpUtility.UrlEncode(department);
            foreach (var userId in userIdsToDeleteList)
            {
                if (userId != id)
                {
                    await _clientCRUD.DeleteAsync($"api/CRUD/user/{department}/{HttpUtility.UrlEncode(userId)}");
                }
            }
            await _clientCRUD.PatchAsJsonAsync<List<DepartmentUpdateViewModel>>($"api/CRUD/updateUsers/{department}", users.Where(x => !x.ShouldDelete).ToList());
            return RedirectToAction("Department");
        }
        private void ChangeRoleNaming(List<DepartmentUpdateViewModel> users)
        {
            foreach (var user in users)
            {
                string userRoleLowered = user.Role.ToLower();
                if (userRoleLowered == "manager")
                {
                    user.Role = "Manager";
                }
                else if (userRoleLowered == "hradmin")
                {
                    user.Role = "HrAdmin";
                }
                else if (userRoleLowered == "employee")
                {
                    user.Role = "Employee";
                }
            }
        }
        private bool HasInvalidRole(List<DepartmentUpdateViewModel> users)
        {
            foreach(var user in users)
            {
                if (user.Role != "Manager" && user.Role != "HrAdmin" && user.Role != "Employee" && !string.IsNullOrEmpty(user.Role))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsValidSalary(List<DepartmentUpdateViewModel> users)
        {
            foreach(var user in users)
            {
                if (user.Salary!=default)
                {
                    if (user.Salary < 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        // ADD PAGING, CACHING, LIST OF ROLES WHEN UPDATING
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsersAdmin(List<DepartmentUpdateViewModel> users)
        {
            if (!HttpContext.Session.TryGetValue("jwt", out var token))
            {
                return RedirectToAction("Login", "Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            var role = secToken.Claims.First(x => x.Type == ClaimTypes.Role).Value;
            if (role != "HrAdmin")
            {
                return RedirectToAction("Index", "Home");
            }
            SanitizeInput(users);
            ChangeRoleNaming(users);
            if (HasInvalidRole(users))
            {
                ModelState.AddModelError("roleError", "Invalid role, role should be one of the following - Employee / Manager / HrAdmin");
            }
            if (!IsValidSalary(users))
            {
                ModelState.AddModelError("salaryError", "Invalid salary, salary should be higher than 0");
            }
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            if (!ModelState.IsValid)
            {
                var usersForOutput = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users");
                List<string> errors = ModelState.Values.SelectMany(x=>x.Errors).Select(x=>x.ErrorMessage).ToList();
                return View(new UsersToUpdateViewModel(usersForOutput,errors));
            }
            var id = secToken.Claims.First(x => x.Type == "sub").Value;
            var userIdsToDeleteList = users.Where(x => x.ShouldDelete).Select(x => x.Id).ToList();
            foreach (var userId in userIdsToDeleteList)
            {
                if (userId != id)
                {
                    await _clientCRUD.DeleteAsync($"api/CRUD/user/{HttpUtility.UrlEncode(userId)}");
                }
            }
            await _clientCRUD.PatchAsJsonAsync<List<DepartmentUpdateViewModel>>($"api/CRUD/updateUsersAdmin", users.Where(x => !x.ShouldDelete).ToList());
            return RedirectToAction("AdminPanel");
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
            var result = await _clientAuth.PostAsJsonAsync<LoginViewModel>("api/auth/login", loginModel);
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
            if (!HttpContext.Session.TryGetValue("jwt", out var token))
            {
                return RedirectToAction("Login", "Home");
            }
            var handler = new JwtSecurityTokenHandler();
            var tokenString = Encoding.UTF8.GetString(token);
            var secToken = handler.ReadJwtToken(tokenString);
            var id = secToken.Claims.First(x => x.Type == "sub").Value;
            _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenString);
            var user = await _clientCRUD.GetFromJsonAsync<UserDataModel>($"api/CRUD/users/id-{id}");
            ProfileViewModel profileViewModel = new ProfileViewModel()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Department = user.Department,
                JobTitle = user.JobTitle,
                Role = user.Role,
                Salary = user.Salary
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
