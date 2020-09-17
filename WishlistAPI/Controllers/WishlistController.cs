using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WishlistAPI.Models;

namespace WishlistAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private static readonly Whanau _whanau = new Whanau
        {
            WhanauId = 1,
            Name = "Smith",
            People = new List<Person> 
            { 
                new Person 
                { 
                    PersonId = 29, 
                    Name = "Jimmy", 
                    WishlistItems = new List<WishlistItem>
                    {
                        new WishlistItem
                        {
                            Description = "Books"
                        },
                        new WishlistItem
                        {
                            Description = "Socks"
                        },
                    }
                },
                new Person
                {
                    PersonId = 32,
                    Name = "James",
                    WishlistItems = new List<WishlistItem>
                    {
                        new WishlistItem
                        {
                            Description = "Treetz"
                        },
                        new WishlistItem
                        {
                            Description = "Undies"
                        },
                    }
                },
                new Person
                {
                    PersonId = 69,
                    Name = "Cameron",
                    WishlistItems = new List<WishlistItem>
                    {
                        new WishlistItem
                        {
                            Description = "Vidya james"
                        },
                        new WishlistItem
                        {
                            Description = "Tunes"
                        },
                    }
                }
            }
        };

        private readonly ILogger<WishlistController> _logger;

        public WishlistController(ILogger<WishlistController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public Whanau Get(int whanauId)
        {
            return _whanau;
        }
    }
}
