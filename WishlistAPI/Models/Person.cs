﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistAPI.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; }
        public int WhanauId { get; set; }
        public IEnumerable<WishlistItem> WishlistItems { get; set; }
    }
}
