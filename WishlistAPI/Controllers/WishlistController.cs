using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistAPI.Models;

namespace WishlistAPI.Controllers
{
    [Route("person/{personId}/[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly IWishlistService _wishlistService;

        public WishlistController(ILogger<WishlistController> logger, IWishlistService wishlistService)
        {
            _logger = logger;
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<ActionResult<List<WishlistItemDto>>> GetWishlist(string personId)
        {
            string requestingId = "1";
            var result = await _wishlistService.GetAllWishlistItemsAsync(personId, requestingId);

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
            await _wishlistService.ClaimWishlistItemAsync(personId, wishlistItemId, "5fe116ac-19e9-401b-8f3a-6674996865b5");

            return Ok();
        }
    }
}
