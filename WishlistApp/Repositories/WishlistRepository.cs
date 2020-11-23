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

namespace WishlistApp.Repositories
{
    public interface IWishlistRepository
    {
        Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(string wishlistOwnerUserId);
        Task<WishlistItem> AddWishlistItemAsync(WishlistItem wishlistItem);
        Task UpdateWishlistItemDescriptionAsync(WishlistItem wishlistItem);
        Task DeleteWishlistItemAsync(WishlistItem wishlistItem);
        Task UpdateWishlistItemClaimAsync(WishlistItem wishlistItem, string currentUserId, bool isClaim);
    }

    public class WishlistRepository : IWishlistRepository
    {
        private readonly ILogger<WishlistRepository> _logger;

        private readonly Container _container;
        private readonly string _databaseId;
        private readonly string _containerId;

        public WishlistRepository(IConfiguration configuration, ILogger<WishlistRepository> logger, CosmosClient cosmosClient)
        {
            _logger = logger;

            _databaseId = configuration.GetValue<string>("WishlistDbId");
            _containerId = configuration.GetValue<string>("WishlistsContainerId");

            _container = cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<IEnumerable<WishlistItem>> GetAllWishlistItemsAsync(string wishlistOwnerId)
        {
            var wishlistItems = new List<WishlistItem>();

            var queryRequestOptions = new QueryRequestOptions() { PartitionKey = new PartitionKey(wishlistOwnerId) };
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

            return wishlistItems;
        }

        public async Task<WishlistItem> AddWishlistItemAsync(WishlistItem wishlistItem)
        {
            ItemResponse<WishlistItem> response = await _container.CreateItemAsync(wishlistItem, new PartitionKey(wishlistItem.UserId));

            _logger.LogInformation("Request charge of create operation: {0}", response.RequestCharge);
            _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);

            return response;
        }

        public async Task UpdateWishlistItemDescriptionAsync(WishlistItem wishlistItem)
        {
            StoredProcedureExecuteResponse<string> response =
                await _container.Scripts.ExecuteStoredProcedureAsync<string>(
                    "updateWishlistItem",
                    new PartitionKey(wishlistItem.UserId),
                    new dynamic[] { wishlistItem.Id, wishlistItem.Description });

            _logger.LogInformation("Request charge of edit operation: {0}", response.RequestCharge);
            _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);
        }

        public async Task DeleteWishlistItemAsync(WishlistItem wishlistItem)
        {
            ItemResponse<WishlistItem> response = await _container.DeleteItemAsync<WishlistItem>(
                partitionKey: new PartitionKey(wishlistItem.UserId),
                id: wishlistItem.Id
            );

            _logger.LogInformation("Request charge of delete operation: {0}", response.RequestCharge);
            _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);
        }

        public async Task UpdateWishlistItemClaimAsync(WishlistItem wishlistItem, string currentUserId, bool isClaim)
        {
            try
            {
                
                StoredProcedureExecuteResponse<string> response =
                    await _container.Scripts.ExecuteStoredProcedureAsync<string>(
                        "claimWishlistItem",
                        new PartitionKey(wishlistItem.UserId),
                        new dynamic[] { wishlistItem.Id, currentUserId, isClaim });

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
        }
    }
}
