using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistAPI.Models
{
    public class Whanau
    {
        public int WhanauId { get; set; }
        public string Name { get; set; }
        public IEnumerable<Person> People { get; set; }
    }
}
