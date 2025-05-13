using System.Threading.Tasks;
using Azure;
using BCrypt.Net;
using CRUDHCM_API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using Newtonsoft.Json;
using Microsoft.AspNetCore.JsonPatch;
using System.Data.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using HCM_app.ViewModels;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CRUDHCM_API.Controllers
{
    [Route("api/CRUD")]
    [ApiController]
    public class CRUDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CRUDController> _logger;
        public CRUDController(ApplicationDbContext context, ILogger<CRUDController> logger)
        {
            _context = context;
            _logger = logger;
        }
        [HttpGet("users/page-{page}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> GetAllUsers(int page)
        {
            try
            {
                int usersToSkip = (page - 1) * Constants.usersPerPage;
                var users = await _context.Users.AsNoTracking()
                    .OrderBy(x => x.FirstName)
                    .Skip(usersToSkip)
                    .Take(Constants.usersPerPage)
                    .ToListAsync();
                return Ok(users);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetAllUsers)}");
                return Problem("There was a problem with the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetAllUsers)}");
                return Problem();
            }
        }
        [HttpGet("usersCount/department-{department}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> GetUsersCountManager(string department)
        {
            try
            {
                var count = await _context.Users.Where(x => x.Department == department).CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUsersCountManager)}");
                return Problem();
            }
        }
        [HttpGet("usersCount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> GetUsersCountAdmin()
        {
            try
            {
                var count = await _context.Users.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUsersCountAdmin)}");
                return Problem();
            }
        }
        [HttpGet("users/department-{department}/page-{page}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> GetAllUsers(string department, int page)
        {
            try
            {
                int usersToSkip = (page - 1) * Constants.usersPerPage;
                var users = await _context.Users.AsNoTracking()
                    .Where(x => x.Department == department)
                    .OrderBy(x => x.FirstName)
                    .Skip(usersToSkip)
                    .Take(Constants.usersPerPage)
                    .ToListAsync();
                if (users.Count == 0)
                {
                    return NotFound("Users with the specified department are non-existent");
                }
                return Ok(users);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetAllUsers)}");
                return Problem("There was a problem with the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetAllUsers)}");
                return Problem();
            }
        }
        [HttpGet("users/id-{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
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
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUserById)}");
                return Problem("There was a problem with the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUserById)}");
                return Problem();
            }
        }
        [HttpGet("users/email-{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user != default)
                {
                    return Ok(user);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUserByEmail)}");
                return Problem("There was a problem with the database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(GetUserByEmail)}");
                return Problem();
            }
        }
        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] UserDataModel user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();


                return CreatedAtAction("AddUser", user);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(AddUser)}");
                return BadRequest("Invalid user data");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(AddUser)}");
                return Problem("Problem occured with saving data to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(AddUser)}");
                return Problem();
            }
        }
        [HttpPatch("updateUsers/{department}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> UpdateUsers([FromBody] List<DepartmentUpdateViewModel> users, string department)
        {
            try
            {
                department = HttpUtility.UrlDecode(department);
                var usersFromDB = await _context.Users.Where(x => x.Department == department).ToDictionaryAsync(x => x.Id);
                if (usersFromDB.Count < 1)
                {
                    return BadRequest("Department is invalid");
                }
                foreach (var user in users)
                {
                    if (usersFromDB.TryGetValue(user.Id, out UserDataModel userFromDB)) // Gets the same user from the DepartmentUpdateVM
                    {
                        if (user.Salary != default)
                        {
                            userFromDB.Salary = user.Salary ?? userFromDB.Salary;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            userFromDB.Email = user.Email;
                        }
                        if (!string.IsNullOrEmpty(user.FirstName))
                        {
                            userFromDB.FirstName = user.FirstName;
                        }
                        if (!string.IsNullOrEmpty(user.LastName))
                        {
                            userFromDB.LastName = user.LastName;
                        }
                        if (!string.IsNullOrEmpty(user.JobTitle))
                        {
                            userFromDB.JobTitle = user.JobTitle;
                        }
                        if (!string.IsNullOrEmpty(user.Department))
                        {
                            userFromDB.Department = user.Department;
                        }
                        if (!string.IsNullOrEmpty(user.Role))
                        {
                            userFromDB.Role = user.Role;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(UpdateUsers)}");
                return Problem("Problem occured with saving data to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(UpdateUsers)}");
                return Problem();
            }
        }
        [HttpPatch("users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> UpdateUsers([FromBody] List<DepartmentUpdateViewModel> users)
        {
            try
            {
                var usersFromDB = await _context.Users.ToDictionaryAsync(x => x.Id);
                foreach (var user in users)
                {
                    if (usersFromDB.TryGetValue(user.Id, out UserDataModel userFromDB)) // Gets the same user from the DepartmentUpdateVM
                    {
                        if (user.Salary != default)
                        {
                            userFromDB.Salary = user.Salary ?? userFromDB.Salary;
                        }
                        if (!string.IsNullOrEmpty(user.Email))
                        {
                            userFromDB.Email = user.Email;
                        }
                        if (!string.IsNullOrEmpty(user.FirstName))
                        {
                            userFromDB.FirstName = user.FirstName;
                        }
                        if (!string.IsNullOrEmpty(user.LastName))
                        {
                            userFromDB.LastName = user.LastName;
                        }
                        if (!string.IsNullOrEmpty(user.JobTitle))
                        {
                            userFromDB.JobTitle = user.JobTitle;
                        }
                        if (!string.IsNullOrEmpty(user.Department))
                        {
                            userFromDB.Department = user.Department;
                        }
                        if (!string.IsNullOrEmpty(user.Role))
                        {
                            userFromDB.Role = user.Role;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(UpdateUsers)}");
                return Problem("Problem occured with saving data to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(UpdateUsers)}");
                return Problem();
            }
        }

        [HttpDelete("user/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                id = HttpUtility.UrlDecode(id);
                var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("There is no user with such Id");
                }
                _context.Users.Remove(user);


                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(DeleteUser)}");
                return Problem("Problem occured with saving data to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(DeleteUser)}");
                return Problem();
            }
        }
        [HttpDelete("user/{department}/{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string id, string department)
        {
            try
            {
                id = HttpUtility.UrlDecode(id);
                department = HttpUtility.UrlDecode(department);
                var user = _context.Users.Where(x => x.Id == id).FirstOrDefault();
                if (user == null)
                {
                    return NotFound("There is no user with such Id");
                }
                if (user.Department == department)
                {
                    _context.Users.Remove(user);
                }
                else
                {
                    return Unauthorized("You cannot delete people from other departments.");
                }
                await _context.SaveChangesAsync();


                return NoContent();
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(DeleteUser)}");
                return Problem("Problem occured with saving data to database");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"There was an error in {nameof(DeleteUser)}");
                return Problem();
            }
        }
    }
}
