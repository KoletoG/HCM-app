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

namespace AuthAPIHCM.Controllers
{
    [Route("api/Auth")]
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
        public async Task<IActionResult> Login([FromBody] UserDataModel login)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == login.Email);

            if (user == default || !BCrypt.Net.BCrypt.Verify(login.PasswordHash, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials");
            }

            var token = _authService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}
