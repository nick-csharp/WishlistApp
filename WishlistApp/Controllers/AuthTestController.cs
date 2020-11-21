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

            var objectId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            
            return "Auth worked! objectId:" + objectId;
        }
    }
}
