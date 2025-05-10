using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthAPIHCM.Data;
using SharedModels;

namespace AuthAPIHCM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Auth
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDataModel>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Auth/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDataModel>> GetUserDataModel(string id)
        {
            var userDataModel = await _context.Users.FindAsync(id);

            if (userDataModel == null)
            {
                return NotFound();
            }

            return userDataModel;
        }

        // PUT: api/Auth/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDataModel(string id, UserDataModel userDataModel)
        {
            if (id != userDataModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDataModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDataModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Auth
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDataModel>> PostUserDataModel(UserDataModel userDataModel)
        {
            _context.Users.Add(userDataModel);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDataModelExists(userDataModel.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserDataModel", new { id = userDataModel.Id }, userDataModel);
        }

        // DELETE: api/Auth/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDataModel(string id)
        {
            var userDataModel = await _context.Users.FindAsync(id);
            if (userDataModel == null)
            {
                return NotFound();
            }

            _context.Users.Remove(userDataModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDataModelExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
