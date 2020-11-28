using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Helpers;
using WishlistApp.Models;
using WishlistApp.Repositories;

namespace WishlistApp.Services
{
    public interface IWishlistService
    {
        Task<WishlistViewDto> GetAllWishlistItemsAsync(string wishlistOwnerId, string currentUserId, string whanauId);
        Task<WishlistItemDto> AddWishlistItemAsync(WishlistItemDto wishlistItemDto);
        Task UpdateWishlistItemDescriptionAsync(WishlistItemDto wishlistItemDto);
        Task DeleteWishlistItemAsync(WishlistItemDto wishlistItemDto);
        Task UpdateWishlistItemClaimAsync(WishlistItemDto wishlistItemDto, string currentUserId, bool isClaim);
    }

    public class WishlistService : IWishlistService
    {
        private readonly ILogger<WishlistService> _logger;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IWhanauRepository _whanauRepository;

        public WishlistService(ILogger<WishlistService> logger, IWishlistRepository wishlistRepository, IWhanauRepository whanauRepository)
        {
            _logger = logger;
            _wishlistRepository = wishlistRepository;
            _whanauRepository = whanauRepository;
        }
        
        public async Task<WishlistViewDto> GetAllWishlistItemsAsync(string wishlistOwnerId, string currentUserId, string whanauId)
        {
            try
            {
                var wishlistOwner = await _whanauRepository.GetPersonAsync(wishlistOwnerId, whanauId);
                var wishlistItems = await _wishlistRepository.GetAllWishlistItemsAsync(wishlistOwnerId);

                var requesterIsOwner = wishlistOwnerId == currentUserId;

                var dtos = wishlistItems.Select(w =>
                    new WishlistItemDto
                    {
                        Id = w.Id,
                        UserId = w.UserId,
                        Description = w.Description,
                        IsClaimable = string.IsNullOrEmpty(w.ClaimedByUserId) && !requesterIsOwner,
                        IsClaimedByMe = w.ClaimedByUserId == currentUserId && !requesterIsOwner
                    });

                return new WishlistViewDto(wishlistOwner.Name, requesterIsOwner, dtos);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting wishlist items.");
                throw;
            }
        }

        public async Task<WishlistItemDto> AddWishlistItemAsync(WishlistItemDto wishlistItemDto)
        {
            try
            {
                var wishlistItem = new WishlistItem(wishlistItemDto.UserId, wishlistItemDto.Description);
                var createdWishlistItem = await _wishlistRepository.AddWishlistItemAsync(wishlistItem);

                return new WishlistItemDto
                {
                    Id = createdWishlistItem.Id,
                    Description = createdWishlistItem.Description,
                    UserId = createdWishlistItem.UserId,
                    IsClaimable = false,
                    IsClaimedByMe = false
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while adding wishlist item.");
                throw;
            }
        }

        public async Task UpdateWishlistItemDescriptionAsync(WishlistItemDto wishlistItemDto)
        {
            try 
            {
                var wishlistItem = new WishlistItem(wishlistItemDto);
                await _wishlistRepository.UpdateWishlistItemDescriptionAsync(wishlistItem);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while updating description on wishlist item.");
                throw;
            }
        }

        public async Task DeleteWishlistItemAsync(WishlistItemDto wishlistItemDto)
        {
            try
            {
                var wishlistItem = new WishlistItem(wishlistItemDto);
                await _wishlistRepository.DeleteWishlistItemAsync(wishlistItem);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while deleting wishlist item.");
                throw;
            }
        }

        public async Task UpdateWishlistItemClaimAsync(WishlistItemDto wishlistItemDto, string currentUserId, bool isClaim)
        {
            try
            {
                var wishlistItem = new WishlistItem() { Id = wishlistItemDto.Id, UserId = wishlistItemDto.UserId };

                await _wishlistRepository.UpdateWishlistItemClaimAsync(wishlistItem, currentUserId, isClaim);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while updating claim on wishlist item.");

                throw new WishlistDbException("An unknown error occurred.");
            }
        }
    }
}
