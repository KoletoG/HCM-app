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
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles ="HrAdmin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }
        [HttpGet("users/department-{department}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> GetAllUsers(string department)
        {
            var users = await _context.Users.Where(x => x.Department == department).ToListAsync();
            return Ok(users);
        }

        [HttpGet("users/id-{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpGet("users/email-{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
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

        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] UserDataModel user)
        {
            try
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return CreatedAtAction("AddUser",user);
            }
            catch (DbException)
            {
                return Problem("Problem occured with saving data to database");
            }
        }
        [HttpPatch("updateUsers/{department}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,Roles ="Manager")]
        public async Task<IActionResult> UpdateUsers([FromBody] List<DepartmentUpdateViewModel> users,string department)
        {
            try
            {
                var usersFromDB = await _context.Users.Where(x => x.Department == department).ToDictionaryAsync(x=>x.Id);
                foreach(var user in users)
                {
                    if(usersFromDB.TryGetValue(user.Id,out UserDataModel userFromDB))
                    {
                        if (user.Salary != default)
                        {
                            userFromDB.Salary = user.Salary ?? userFromDB.Salary;
                        }
                        if (user.Email != null)
                        {
                            userFromDB.Email = user.Email;
                        }
                        if (user.FirstName != null)
                        {
                            userFromDB.FirstName = user.FirstName;
                        }
                        if (user.LastName != null)
                        {
                            userFromDB.LastName = user.LastName;
                        }
                        if (user.JobTitle != null)
                        {
                            userFromDB.JobTitle = user.JobTitle;
                        }
                        if (user.Department != null)
                        {
                            userFromDB.Department = user.Department;
                        }
                        if (user.Role != null)
                        {
                            userFromDB.Role = user.Role;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbException)
            {
                return Problem("Problem occured with saving data to database");
            }
        }
        [HttpPatch("updateUsersAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> UpdateUsers([FromBody] List<DepartmentUpdateViewModel> users)
        {
            try
            {
                var usersFromDB = await _context.Users.ToDictionaryAsync(x => x.Id);
                foreach (var user in users)
                {
                    if (usersFromDB.TryGetValue(user.Id, out UserDataModel userFromDB))
                    {
                        if (user.Salary != default)
                        {
                            userFromDB.Salary = user.Salary ?? userFromDB.Salary;
                        }
                        if (user.Email != null)
                        {
                            userFromDB.Email = user.Email;
                        }
                        if (user.FirstName != null)
                        {
                            userFromDB.FirstName = user.FirstName;
                        }
                        if (user.LastName != null)
                        {
                            userFromDB.LastName = user.LastName;
                        }
                        if (user.JobTitle != null)
                        {
                            userFromDB.JobTitle = user.JobTitle;
                        }
                        if (user.Department != null)
                        {
                            userFromDB.Department = user.Department;
                        }
                        if (user.Role != null)
                        {
                            userFromDB.Role = user.Role;
                        }
                    }
                }
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbException)
            {
                return Problem("Problem occured with saving data to database");
            }
        }
        [HttpDelete("deleteUsersAdmin")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "HrAdmin")]
        public async Task<IActionResult> DeleteUsersAdmin([FromBody] List<DepartmentUpdateViewModel> users)
        {
            try
            {
                foreach (var user in users)
                {
                   await _context.Users.Where(x => x.Id == user.Id).ExecuteDeleteAsync();
                }
                return NoContent();
            }
            catch (DbException)
            {
                return Problem("Problem occured with deleting data in database");
            }
        }
        [HttpDelete("deleteUsersManager")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager")]
        public async Task<IActionResult> DeleteUsersManager([FromBody] List<DepartmentUpdateViewModel> users,string department)
        {
            try
            {
                foreach (var user in users)
                {
                    await _context.Users.Where(x => x.Id == user.Id).ExecuteDeleteAsync();
                }
                return NoContent();
            }
            catch (DbException)
            {
                return Problem("Problem occured with deleting data in database");
            }
        }
    }
}
