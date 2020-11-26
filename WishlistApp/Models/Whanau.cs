using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistApp.Models
{
    public class Whanau
    {
        public string Name { get; set; }
        public IEnumerable<Person> People { get; set; }
        public Whanau(string name, IEnumerable<Person> people)
        {
            Name = name;
            People = people;
        }
    }
}
