using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistAPI.Models
{
    public class WishlistItem
    {
        public int WishlistItemId { get; set; }
        public int PersonId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
        public int Year { get; set; } = 2020;
        public bool? IsClaimed { get; set; }
    }
}
