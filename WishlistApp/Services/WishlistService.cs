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

namespace WishlistApp
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetAllWishlistItemsAsync(string ownerUserId, string requestingUserId);
        Task<WishlistItemDto> AddWishlistItemAsync(WishlistItemDto wishlistItemDto);
        Task EditWishlistItemAsync(string ownerUserId, WishlistItemDto wishlistItemDto);
        Task DeleteWishlistItemAsync(string ownerUserId, string wishlistItemId);
        Task ClaimWishlistItemAsync(string ownerUserId, string wishlistItemId, string claimerUserId, bool isClaim);
    }

    public class WishlistService : IWishlistService
    {
        private readonly ILogger<WishlistService> _logger;

        private readonly Container _container;
        private readonly string _databaseId;
        private readonly string _containerId;

        public WishlistService(IConfiguration configuration, ILogger<WishlistService> logger, CosmosClient cosmosClient)
        {
            _logger = logger;

            _databaseId = configuration.GetValue<string>("WishlistDbId");
            _containerId = configuration.GetValue<string>("WishlistsContainerId");

            _container = cosmosClient.GetContainer(_databaseId, _containerId);
        }
        
        public async Task<IEnumerable<WishlistItemDto>> GetAllWishlistItemsAsync(string ownerId, string requestingId)
        {
            try
            {
                var wishlistItems = new List<WishlistItem>();

                var queryRequestOptions = new QueryRequestOptions() { PartitionKey = new PartitionKey(ownerId) };
                using (var setIterator = _container.GetItemLinqQueryable<WishlistItem>(requestOptions: queryRequestOptions)
                    .ToFeedIterator())
                {
                    while (setIterator.HasMoreResults)
                    {
                        var response = await setIterator.ReadNextAsync();

                        _logger.LogInformation("Request charge of get operation: {0}", response.RequestCharge);
                        _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);

                        wishlistItems.AddRange(response);
                    }
                }

                var requesterIsOwner = ownerId == requestingId;

                var dtos = wishlistItems.Select(w =>
                    new WishlistItemDto
                    {
                        Id = w.Id,
                        UserId = w.UserId,
                        Description = w.Description,
                        IsClaimable = string.IsNullOrEmpty(w.ClaimedByUserId) && !requesterIsOwner,
                        IsClaimedByMe = w.ClaimedByUserId == requestingId && !requesterIsOwner
                    });

                return dtos;
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

                ItemResponse<WishlistItem> response = await _container.CreateItemAsync(wishlistItem, new PartitionKey(wishlistItem.UserId));

                _logger.LogInformation("Request charge of create operation: {0}", response.RequestCharge);
                _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);

                return new WishlistItemDto
                {
                    Id = response.Resource.Id,
                    Description = response.Resource.Description,
                    UserId = response.Resource.UserId,
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

        public async Task EditWishlistItemAsync(string ownerId, WishlistItemDto wishlistItemDto)
        {
            try 
            {
                StoredProcedureExecuteResponse<string> response = 
                    await _container.Scripts.ExecuteStoredProcedureAsync<string>(
                        "updateWishlistItem",
                        new PartitionKey(ownerId),
                        new dynamic[] { wishlistItemDto.Id, wishlistItemDto.Description });

                _logger.LogInformation("Request charge of edit operation: {0}", response.RequestCharge);
                _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while editing wishlist item.");
                throw;
            }
        }

        public async Task DeleteWishlistItemAsync(string ownerUserId, string wishlistItemId)
        {
            try
            {
                ItemResponse<WishlistItem> response = await _container.DeleteItemAsync<WishlistItem>(
                    partitionKey: new PartitionKey(ownerUserId),
                    id: wishlistItemId
                );

                _logger.LogInformation("Request charge of delete operation: {0}", response.RequestCharge);
                _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while deleting wishlist item.");
                throw;
            }
        }

        public async Task ClaimWishlistItemAsync(string ownerUserId, string wishlistItemId, string claimerUserId, bool isClaim)
        {
            try
            {
                StoredProcedureExecuteResponse<string> response =
                    await _container.Scripts.ExecuteStoredProcedureAsync<string>(
                        "claimWishlistItem",
                        new PartitionKey(ownerUserId),
                        new dynamic[] { wishlistItemId, claimerUserId, isClaim });

                _logger.LogInformation("Request charge of claim operation: {0}", response.RequestCharge);
                _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);
            }
            catch (CosmosException ce) when (ce.Message.Contains(CosmosDbExceptions.ClaimUnsuccessfulAlreadyClaimed))
            {
                throw new InvalidOperationException(CosmosDbExceptions.ClaimUnsuccessfulAlreadyClaimed);
            }
            catch (CosmosException ce) when (ce.Message.Contains(CosmosDbExceptions.UnclaimUnsuccessfulAlreadyClaimed))
            {
                throw new InvalidOperationException(CosmosDbExceptions.UnclaimUnsuccessfulAlreadyClaimed);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while claiming wishlist item.");

                throw new WishlistDbException("An unknown error occurred.");
            }
        }
    }
}
