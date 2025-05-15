using Microsoft.AspNetCore.Mvc;
using SharedModels;

namespace CRUDHCM_API.Controllers
{
    [ApiController]
    [Route("api/CRUD/users")]
    public class FakeCrudController : ControllerBase
    {
        [HttpPost]
        public IActionResult CreateUser([FromBody] UserDataModel user)
        {
            return NoContent();
        }
    }
}
