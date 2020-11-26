using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistApp.Models
{
    public class WishlistItemDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }
        public bool IsClaimable { get; set; }
        public bool IsClaimedByMe { get; set; }
    }

    public class WishlistViewDto
    {
        public bool IsMyWishlist { get; }
        public IEnumerable<WishlistItemDto> WishlistItems { get; }
        public WishlistViewDto(bool isMyWishlist, IEnumerable<WishlistItemDto> wishlistItems)
        {
            IsMyWishlist = isMyWishlist;
            WishlistItems = wishlistItems;
        }
    }
}
