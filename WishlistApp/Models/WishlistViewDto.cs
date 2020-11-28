using System.Collections.Generic;

namespace WishlistApp.Models
{
    public class WishlistViewDto
    {
        public string WishlistOwnerName { get; }
        public bool IsMyWishlist { get; }
        public IEnumerable<WishlistItemDto> WishlistItems { get; }

        public WishlistViewDto(string ownerName, bool isMyWishlist, IEnumerable<WishlistItemDto> wishlistItems)
        {
            WishlistOwnerName = ownerName;
            IsMyWishlist = isMyWishlist;
            WishlistItems = wishlistItems;
        }
    }
}
