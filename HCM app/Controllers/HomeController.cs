using System;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using AngleSharp.Html;
using Ganss.Xss;
using HCM_app.Interfaces;
using HCM_app.Migrations;
using HCM_app.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IUserInputService _userInputService;
        private readonly ITokenService _tokenService;
        public HomeController(ILogger<HomeController> logger, 
            IHttpClientFactory client, 
            IHtmlSanitizer htmlSanitizer,
            IUserInputService userInputService,
            ITokenService tokenService)
        {
            _userInputService=userInputService;
            _logger = logger;
            _clientAuth = client.CreateClient("AuthAPI");
            _clientCRUD = client.CreateClient("CRUDAPI");
            _htmlSanitizer = htmlSanitizer;
            _tokenService=tokenService;
            _htmlSanitizer.AllowedTags.Clear();
        }
        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Index)}");
                return View("Error", new ErrorViewModel());
            }
        }
        // Loads the users in the manager panel
        [HttpGet("managerPanelUpdate/page-{page}")]
        public async Task<IActionResult> UpdateUsersManager(int page = 1)
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                var secToken = _tokenService.GetToken(token);
                if (_tokenService.GetRole(secToken) != "Manager")
                {
                    return RedirectToAction("Login", "Home");
                }
                var department = _tokenService.GetDepartment(secToken);
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                var usersCount = await _clientCRUD.GetStringAsync($"api/CRUD/usersCount/department-{department}");
                ValidatePages(int.Parse(usersCount), ref page, out bool isLastPage, out bool isFirstPage);
                var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users/department-{department}/page-{page}");
                return View(new UsersToUpdateViewModel(users, isLastPage, isFirstPage, page));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(UpdateUsersManager)}"); 
                return View("Error",new ErrorViewModel());
            }
        }
        /// <summary>
        /// Logic for pages
        /// </summary>
        /// <param name="usersCount">Count of all the pages</param>
        /// <param name="page">Current page</param>
        /// <param name="isLastPage">If the user is at the first page</param>
        /// <param name="isFirstPage">If the user is at the last page</param>
        private void ValidatePages(int usersCount, ref int page, out bool isLastPage, out bool isFirstPage)
        {
            int pagesCount = (int)Math.Ceiling((double)usersCount / Constants.usersPerPage);
            isLastPage = false;
            isFirstPage = false;
            if (page >= pagesCount)
            {
                page = pagesCount;
                isLastPage = true;
            }
            else if (page <= 1)
            {
                page = 1;
                isFirstPage = true;
            }
        }
        // Loads the users in the admin panel
        [HttpGet("adminPanelUpdate/page-{page}")]
        public async Task<IActionResult> UpdateUsersAdmin(int page = 1)
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                var secToken = _tokenService.GetToken(token);
                if (_tokenService.GetRole(secToken) != "HrAdmin")
                {
                    return RedirectToAction("Login", "Home");
                }
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                var usersCount = await _clientCRUD.GetStringAsync($"api/CRUD/usersCount");
                ValidatePages(int.Parse(usersCount), ref page, out bool isLastPage, out bool isFirstPage);
                var users = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users/page-{page}");
                return View(new UsersToUpdateViewModel(users, isLastPage, isFirstPage, page));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(UpdateUsersAdmin)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsersManager1(List<DepartmentUpdateViewModel> users, int page, bool isLastPage, bool isFirstPage)
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                var secToken = _tokenService.GetToken(token);
                var role = _tokenService.GetRole(secToken);
                if (role != "Manager")
                {
                    return RedirectToAction("Index", "Home");
                }
                _userInputService.SanitizeInput(users); // Sanitize input against XSS
                _userInputService.ChangeRoleNaming(users); // Makes sure Role is correctly written
                if (_userInputService.HasInvalidRole(users))
                {
                    ModelState.AddModelError("roleError", "Invalid role, role should be one of the following - Employee / Manager / HrAdmin");
                }
                if (!_userInputService.IsValidSalary(users))
                {
                    ModelState.AddModelError("salaryError", "Invalid salary, salary should be higher than 0");
                }
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                if (!ModelState.IsValid)
                {
                    var usersForOutput = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users/page-{page}");
                    List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                    return View("UpdateUsersManager", new UsersToUpdateViewModel(usersForOutput, errors, isLastPage, isFirstPage, page));
                }
                var id = _tokenService.GetId(secToken);
                var department = _tokenService.GetDepartment(secToken);
                var userIdsToDeleteList = users.Where(x => x.ShouldDelete).Select(x => x.Id).ToList();
                department = HttpUtility.UrlEncode(department); // Encodes department for url query
                foreach (var userId in userIdsToDeleteList) // Deletes every user which checkbox has been checked
                {
                    if (userId != id)
                    {
                        await _clientCRUD.DeleteAsync($"api/CRUD/user/{department}/{HttpUtility.UrlEncode(userId)}");
                    }
                }
                int pageToReturn = page;
                var res = await _clientCRUD.PatchAsJsonAsync<List<DepartmentUpdateViewModel>>($"api/CRUD/updateUsers/{department}", users.Where(x => !x.ShouldDelete).ToList());// Updates the users which checkbox hasn't been checked
                return RedirectToAction("UpdateUsersManager", new { page = pageToReturn });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(UpdateUsersManager)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUsersAdmin1(List<DepartmentUpdateViewModel> users, int page, bool isLastPage, bool isFirstPage)
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                var secToken = _tokenService.GetToken(token);
                var role = _tokenService.GetRole(secToken);
                if (role != "HrAdmin")
                {
                    return RedirectToAction("Index", "Home");
                }
                _userInputService.SanitizeInput(users); // Sanitize input against XSS
                _userInputService.ChangeRoleNaming(users); // Makes sure Role is correctly written
                if (_userInputService.HasInvalidRole(users))
                {
                    ModelState.AddModelError("roleError", "Invalid role, role should be one of the following - Employee / Manager / HrAdmin");
                }
                if (!_userInputService.IsValidSalary(users))
                {
                    ModelState.AddModelError("salaryError", "Invalid salary, salary should be higher than 0");
                }
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                if (!ModelState.IsValid)
                {
                    var usersForOutput = await _clientCRUD.GetFromJsonAsync<List<UserDataModel>>($"api/CRUD/users/page-{page}");
                    List<string> errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                    return View("UpdateUsersAdmin", new UsersToUpdateViewModel(usersForOutput, errors, isLastPage, isFirstPage, page));
                }
                var id = _tokenService.GetId(secToken);
                var userIdsToDeleteList = users.Where(x => x.ShouldDelete).Select(x => x.Id).ToList();
                foreach (var userId in userIdsToDeleteList) // Deletes every user which checkbox has been checked
                {
                    if (userId != id)
                    {
                        await _clientCRUD.DeleteAsync($"api/CRUD/user/{HttpUtility.UrlEncode(userId)}");
                    }
                }
                int pageToReturn = page;
                var usersToSend = users.Where(x => !x.ShouldDelete).ToList(); // Updates the users which checkbox hasn't been checked
                var result = await _clientCRUD.PatchAsJsonAsync<List<DepartmentUpdateViewModel>>($"api/CRUD/users", usersToSend);
                return RedirectToAction("UpdateUsersAdmin", "Home", new { page = pageToReturn });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(UpdateUsersAdmin)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Profile)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginModel)
        {
            try
            {
                loginModel.Email = _htmlSanitizer.Sanitize(loginModel.Email); // Sanitizes against XSS
                loginModel.Password = _htmlSanitizer.Sanitize(loginModel.Password); // Sanitizes against XSS
                if (!ModelState.IsValid)
                {
                    return View(loginModel);
                }
                var result = await _clientAuth.PostAsJsonAsync<LoginViewModel>("api/auth/login", loginModel);
                if (result.IsSuccessStatusCode)
                {
                    var token = await result.Content.ReadAsStringAsync();
                    HttpContext.Session.SetString("jwt", token);
                    return RedirectToAction("Profile", "Home");
                }
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Login)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel registerModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registerModel);
                }
                var sanitizedRegisterModel = new RegisterViewModel()
                {
                    FirstName = _htmlSanitizer.Sanitize(registerModel.FirstName),
                    LastName=_htmlSanitizer.Sanitize(registerModel.LastName),
                    Department=_htmlSanitizer.Sanitize(registerModel.Department),
                    Email=_htmlSanitizer.Sanitize(registerModel.Email),
                    JobTitle=_htmlSanitizer.Sanitize(registerModel.JobTitle),
                    Password=_htmlSanitizer.Sanitize(registerModel.Password),
                    Salary=registerModel.Salary
                };  // against XSS
                var result = await _clientAuth.PostAsJsonAsync<RegisterViewModel>("api/auth/register", sanitizedRegisterModel);
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Home");
                }
                return Problem();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Register)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                var secToken = _tokenService.GetToken(token);
                var id = _tokenService.GetId(secToken);
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                var user = await _clientCRUD.GetFromJsonAsync<UserDataModel>($"api/CRUD/users/id-{id}");
                var profileViewModel = new ProfileViewModel()
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Profile)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Profile)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            try
            {
                if (!HttpContext.Session.TryGetValue("jwt", out var token))
                {
                    return RedirectToAction("Login", "Home");
                }
                oldPassword = _htmlSanitizer.Sanitize(oldPassword); // against XSS
                newPassword=_htmlSanitizer.Sanitize(newPassword);
                var secToken = _tokenService.GetToken(token);
                var idFromCurrentUser = _tokenService.GetId(secToken);
                _clientCRUD.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Encoding.UTF8.GetString(token));
                var user = await _clientCRUD.GetFromJsonAsync<UserDataModel>($"api/CRUD/users/id-{idFromCurrentUser}");
                if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                {
                    return RedirectToAction("Index","Home");
                }
                await _clientCRUD.PatchAsJsonAsync($"api/CRUD/user/password",new ChangePassViewModel(idFromCurrentUser, newPassword));

                return RedirectToAction("Profile","Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error has happened in {nameof(Profile)}");
                return View("Error", new ErrorViewModel());
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
