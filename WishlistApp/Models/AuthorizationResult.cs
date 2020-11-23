using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistApp.Models
{
    public class AuthorizationResult
    {
        public bool IsAuthorised { get; }
        public Person Person { get; }
        public AuthorizationResult(Person person, bool isAuthorized)
        {
            IsAuthorised = isAuthorized;
            Person = person;
        }
    }
}
