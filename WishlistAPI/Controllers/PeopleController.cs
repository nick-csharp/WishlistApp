using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistAPI.Models;

namespace WishlistAPI.Controllers
{

    [ApiController]
    public class PeopleController : ControllerBase
    {

        [HttpGet("people/all")]
        public ActionResult<IEnumerable<Person>> GetAll()
        {
            return new List<Person>
            {
                new Person
                {
                    Name = "George",
                    PersonId = 1
                },
                new Person
                {
                    Name = "Fountain",
                    PersonId = 2
                },
                new Person
                {
                    Name = "Thornton",
                    PersonId = 3
                },
            };
        }
    }
}
