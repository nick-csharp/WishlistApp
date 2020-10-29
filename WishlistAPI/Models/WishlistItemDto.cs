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
}
