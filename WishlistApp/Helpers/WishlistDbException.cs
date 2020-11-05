using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WishlistApp.Helpers
{
    public class WishlistDbException : Exception
    {
        public WishlistDbException()
        {
        }

        public WishlistDbException(string message)
            : base(message)
        {
        }

        public WishlistDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
