using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Services;

namespace WishlistApp.Controllers
{
    [Route("api/person/{personId}/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly IWishlistService _wishlistService;
        private readonly IWishlistAuthorizationService _authorizationService;

        public WishlistController(
            ILogger<WishlistController> logger, 
            IWishlistService wishlistService,
            IWishlistAuthorizationService authorizationService )
        {
            _logger = logger;
            _wishlistService = wishlistService;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<ActionResult<WishlistViewDto>> GetWishlist(string personId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.GetWishlistItems);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            var result = await _wishlistService.GetAllWishlistItemsAsync(personId, authResult.Person.Id, authResult.Person.WhanauId);

            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            if (personId != wishlistItem.UserId)
            {
                return BadRequest();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.AddEditDeleteWishlistItem);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }
            
            var result = await _wishlistService.AddWishlistItemAsync(wishlistItem);

            return CreatedAtAction(nameof(GetWishlist), new { personId, result.Id }, result);
        }

        [HttpPut("{wishlistItemId}")]
        public async Task<IActionResult> EditWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            if (personId != wishlistItem.UserId)
            {
                return BadRequest();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.AddEditDeleteWishlistItem);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            await _wishlistService.UpdateWishlistItemDescriptionAsync(wishlistItem);

            return Ok();
        }

        [HttpDelete("{wishlistItemId}")]
        public async Task<IActionResult> DeleteWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            if (personId != wishlistItem.UserId)
            {
                return BadRequest();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.AddEditDeleteWishlistItem);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            await _wishlistService.DeleteWishlistItemAsync(wishlistItem);

            return NoContent();
        }
        
        [HttpPatch("{wishlistItemId}/claim")]
        public async Task<IActionResult> ClaimWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            if (personId != wishlistItem.UserId)
            {
                return BadRequest();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.ClaimOrUnclaimWishlistItem);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            await _wishlistService.UpdateWishlistItemClaimAsync(wishlistItem, authResult.Person.Id, true);

            return Ok();
        }

        [HttpPatch("{wishlistItemId}/unclaim")]
        public async Task<IActionResult> UnclaimWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            if (personId != wishlistItem.UserId)
            {
                return BadRequest();
            }

            var authResult = await _authorizationService.AuthorizeAsync(User, personId, Operation.ClaimOrUnclaimWishlistItem);

            if (!authResult.IsAuthorised)
            {
                return Forbid();
            }

            await _wishlistService.UpdateWishlistItemClaimAsync(wishlistItem, authResult.Person.Id, false);

            return Ok();
        }
    }
}
