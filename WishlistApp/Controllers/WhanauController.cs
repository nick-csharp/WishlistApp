using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Services;

namespace WishlistApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WhanauController : ControllerBase
    {
        private readonly ILogger<WhanauController> _logger;
        private readonly IWhanauService _whanauService;
        private readonly IWishlistAuthorizationService _authorizationService;

        public WhanauController(
            ILogger<WhanauController> logger,
            IWhanauService whanauService,
            IWishlistAuthorizationService authorizationService)
        {
            _logger = logger;
            _whanauService = whanauService;
            _authorizationService = authorizationService;
        }

        [HttpGet("{whanauId}")]
        public async Task<ActionResult<List<Person>>> GetWhanau(string whanauId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, whanauId, Operation.GetWhanau);
            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            var result = await _whanauService.GetWhanauAsync(whanauId);

            return result.ToList();
        }
    }
}
