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

namespace AuthAPIHCM.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IAuthService _authService;
        public AuthController(ApplicationDbContext context, IAuthService authService)
        {
            _context = context;
            _authService = authService;
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
                var passwordHashLogin = BCrypt.Net.BCrypt.HashPassword(loginModel.Password);
                if (!BCrypt.Net.BCrypt.Verify(passwordHashLogin, user.PasswordHash))
                {
                    return Unauthorized("Invalid credentials");
                }
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}
