using System.Threading.Tasks;
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
        // GET: api/<CRUDController>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        // GET api/<CRUDController>/5
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

        // POST api/<CRUDController>
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserDataModel user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok();
        }
        // PUT api/<CRUDController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CRUDController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
