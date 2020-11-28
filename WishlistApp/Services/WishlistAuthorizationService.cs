using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System.Security.Claims;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Repositories;

namespace WishlistApp.Services
{
    public interface IWishlistAuthorizationService
    {
        Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, string resourceId, Operation operation);
    }
    
    public class WishlistAuthorizationService : IWishlistAuthorizationService
    {
        private readonly ILogger<WishlistAuthorizationService> _logger;
        private readonly IWhanauRepository _whanauService;

        public WishlistAuthorizationService(ILogger<WishlistAuthorizationService> logger, IWhanauRepository whanauService)
        {
            _logger = logger;
            _whanauService = whanauService;
        }

        public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, string resourceId, Operation operation)
        {
            if (user == null)
            {
                return new AuthorizationResult(null, false);
            }
            
            var currentUserObjectId = user.GetObjectId();
            if (string.IsNullOrEmpty(currentUserObjectId))
            {
                return new AuthorizationResult(null, false);
            }

            var currentPerson = await _whanauService.GetPersonByObjectIdAsync(currentUserObjectId);
            if (currentPerson == null
                || string.IsNullOrEmpty(currentPerson.Id)
                || string.IsNullOrEmpty(currentPerson.WhanauId))
            {
                return new AuthorizationResult(null, false);
            }

            bool isAuthorized;
            switch (operation)
            {
                case Operation.GetWhanau:
                    isAuthorized = true;
                    break;
                case Operation.GetWishlistItems:
                    isAuthorized = true; // todo: auth by whanau
                    break;
                case Operation.AddEditDeleteWishlistItem:
                    isAuthorized = currentPerson.Id == resourceId;
                    break;
                case Operation.ClaimOrUnclaimWishlistItem:
                    isAuthorized =  currentPerson.Id != resourceId;
                    break;
                default:
                    isAuthorized = false;
                    break;
            }

            return new AuthorizationResult(currentPerson, isAuthorized);
        }
    }
}
