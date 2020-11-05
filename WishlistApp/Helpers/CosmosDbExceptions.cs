
namespace WishlistApp.Helpers
{
    public static class CosmosDbExceptions
    {
        public const string ClaimUnsuccessfulAlreadyClaimed = "Cannot claim because wishlist item is already claimed by someone else.";
        public const string UnclaimUnsuccessfulAlreadyClaimed = "Cannot unclaim because wishlist item is already claimed by someone else.";
    }
}
