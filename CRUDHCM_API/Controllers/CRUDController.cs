using System.Threading.Tasks;
using BCrypt.Net;
using CRUDHCM_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CRUDHCM_API.Controllers
{
    [Route("api/CRUD")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CRUDController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user != default)
            {
                return Ok(user);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDataModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("updateUser/{id}")]
        public async Task<IActionResult> UpdateUser(string id,[FromBody] UserDataModel user)
        {
            if (user.Id != id)
            {
                return BadRequest("User Id doesn't match the id from the URL.");
            }
            var userOld = await _context.Users.FirstAsync(x => x.Id == id);
            userOld.FirstName = user.FirstName;
            userOld.LastName = user.LastName;
            userOld.Email = user.Email;
            userOld.Role = user.Role;
            userOld.Department = user.Department;
            userOld.JobTitle = user.JobTitle;
            userOld.Salary = user.Salary;
            if (userOld.Password != user.Password)
            {
                userOld.Password = user.Password;
                userOld.PasswordHash=BCrypt.Net.BCrypt.HashPassword(user.Password);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("user/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _context.Users.Where(x => x.Id == id).ExecuteDeleteAsync();
            return NoContent();
        }
    }
}
