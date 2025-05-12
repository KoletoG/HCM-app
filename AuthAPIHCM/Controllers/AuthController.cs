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
            _clientCRUD.BaseAddress = new Uri("https://localhost:7261/");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel loginModel)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest("There was a problem with the input.");
            }
            var result = await _clientCRUD.GetAsync($"api/CRUD/users/email-{loginModel.Email}");
            var user = new UserDataModel();
            if (result.IsSuccessStatusCode)
            {
                user = await result.Content.ReadFromJsonAsync<UserDataModel>();
            }
            else
            {
                return NotFound("A user with this email doesn't exist.");
            }
            if (!BCrypt.Net.BCrypt.Verify(loginModel.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }
            var token = _authService.GenerateJwtToken(user);
            return Ok(token);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(registerModel);
            }
            string userRole;
            if (registerModel.Email == "admin@email.com")
            {
                userRole = "HrAdmin";
            }
            else if(registerModel.Email == "manager@email.com")
            {
                userRole = "Manager";
            }
            else
            {
                userRole = "Employee";
            }
                UserDataModel user = new UserDataModel()
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = registerModel.Email,
                    Salary = registerModel.Salary,
                    Department = registerModel.Department,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    JobTitle = registerModel.JobTitle,
                    Password = registerModel.Password,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerModel.Password),
                    Role = userRole
                };
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
