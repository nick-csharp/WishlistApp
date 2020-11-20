using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;

namespace WishlistApp.Controllers
{
    [Route("api/person/{personId}/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly IWishlistService _wishlistService;

        private readonly string _defaultRequestingId;

        public WishlistController(IConfiguration configuration, ILogger<WishlistController> logger, IWishlistService wishlistService)
        {
            _defaultRequestingId = configuration.GetValue<string>("DefaultRequestingId");
            if (string.IsNullOrEmpty(_defaultRequestingId))
            {
                throw new Exception("Default user not configured.");
            }


            _logger = logger;
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WishlistItemDto>>> GetWishlist(string personId)
        {
            var result = await _wishlistService.GetAllWishlistItemsAsync(personId, _defaultRequestingId);

            return result.ToList();
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            // validate person is manipulating their own wishlist
            var result = await _wishlistService.AddWishlistItemAsync(wishlistItem);

            return CreatedAtAction(nameof(GetWishlist), new { personId, result.Id }, result); ;
        }

        [HttpPut("{wishlistItemId}")]
        public async Task<IActionResult> EditWishlistItem(string personId, WishlistItemDto wishlistItem)
        {
            // validate person is manipulating their own wishlist
            await _wishlistService.EditWishlistItemAsync(personId, wishlistItem);

            return Ok();
        }

        [HttpDelete("{wishlistItemId}")]
        public async Task<IActionResult> DeleteWishlistItem(string personId, string wishlistItemId)
        {
            // validate person is manipulating their own wishlist
            await _wishlistService.DeleteWishlistItemAsync(personId, wishlistItemId);

            return NoContent();
        }
        
        [HttpPatch("{wishlistItemId}/claim")]
        public async Task<IActionResult> ClaimWishlistItem(string personId, string wishlistItemId)
        {
            // validate person is manipulating someone else's wishlist
            await _wishlistService.ClaimWishlistItemAsync(personId, wishlistItemId, _defaultRequestingId, true);

            return Ok();
        }

        [HttpPatch("{wishlistItemId}/unclaim")]
        public async Task<IActionResult> UnclaimWishlistItem(string personId, string wishlistItemId)
        {
            // validate person is manipulating someone else's wishlist
            await _wishlistService.ClaimWishlistItemAsync(personId, wishlistItemId, _defaultRequestingId, false);

            return Ok();
        }
    }
}
