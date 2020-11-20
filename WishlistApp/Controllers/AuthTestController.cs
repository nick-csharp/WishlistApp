using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WishlistApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthTestController : ControllerBase
    {
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetThing(string id)
        {
            return "Auth worked! " + id;
        }
    }
}
