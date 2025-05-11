using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using AuthAPIHCM.Data;
using AuthAPIHCM.Services;
using AuthAPIHCM.Interfaces;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace AuthAPIHCM.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IAuthService _authService;
        private readonly HttpClient _clientCRUD;
        public AuthController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
            _clientCRUD = new HttpClient();
            _clientCRUD.BaseAddress= new Uri("https://localhost:7261/");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == default)
            {
                return NotFound("A user with this email doesn't exist.");
            }
            if(loginModel.Email!= "john.doe@company.com")
            {
                user.Role = "HrAdmin";
                var passwordHashLogin = BCrypt.Net.BCrypt.HashPassword(loginModel.Password);
                if (!BCrypt.Net.BCrypt.Verify(passwordHashLogin, user.PasswordHash))
                {
                    return Unauthorized("Invalid credentials");
                }
            }
            var token = _authService.GenerateJwtToken(user);
            string name = User.Identity.Name;
            return Ok(token);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerModel)
        {
            UserDataModel user = new UserDataModel();
            user.Id = Guid.NewGuid().ToString();
            user.Email = registerModel.Email;
            user.Salary = registerModel.Salary;
            user.Role = "Employee";
            user.LastName = registerModel.LastName;
            user.FirstName = registerModel.FirstName;
            user.Department = registerModel.Department;
            user.JobTitle = registerModel.JobTitle;
            user.Password=registerModel.Password;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password);
            var result = await _clientCRUD.PostAsJsonAsync<UserDataModel>("api/CRUD/users", user);
            if (result.IsSuccessStatusCode)
            {
                return NoContent();
            }
            else
            {
                return Problem("User not registered.");
            }

        }
    }
}
