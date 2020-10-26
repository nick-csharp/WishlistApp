using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistAPI.Models;

namespace WishlistAPI
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistItemDto>> GetAllWishlistItemsAsync(string ownerId, string requestingId);
        Task<WishlistItemDto> AddWishlistItemAsync(WishlistItemDto wishlistItemDto);
        Task EditWishlistItemAsync(string ownerId, WishlistItemDto wishlistItemDto);
        Task DeleteWishlistItemAsync(string ownerId, string wishlistItemId);
        Task ClaimWishlistItemAsync(string ownerId, string wishlistItemId, string claimerId);
    }

    public class WishlistService : IWishlistService
    {
        private readonly IDocumentClient _documentClient;
        private readonly string _wishlistDatabase;
        private readonly string _wishlistCollection;

        public WishlistService(IConfiguration configuration, IDocumentClient documentClient)
        {
            _documentClient = documentClient;
            _wishlistDatabase = configuration.GetValue<string>("WishlistDbName");
            _wishlistCollection = configuration.GetValue<string>("WishlistCollectionName");
        }
        
        public async Task<IEnumerable<WishlistItemDto>> GetAllWishlistItemsAsync(string ownerId, string requestingId)
        {
            try
            {
                var wishlistItems = new List<WishlistItem>();

                var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_wishlistDatabase, _wishlistCollection);
                var queryable = _documentClient.CreateDocumentQuery<WishlistItem>(documentCollectionUri, new FeedOptions { EnableCrossPartitionQuery = true })
                    .Where(item => item.UserId == ownerId)
                    .AsDocumentQuery();

                while (queryable.HasMoreResults)
                {
                    var response = await queryable.ExecuteNextAsync<WishlistItem>();
                    wishlistItems.AddRange(response);
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
                throw;
            }
        }

        public async Task<WishlistItemDto> AddWishlistItemAsync(WishlistItemDto wishlistItemDto)
        {
            var wishlistItem = new WishlistItem
            {
                UserId = wishlistItemDto.UserId,
                Description = wishlistItemDto.Description,
                ClaimedByUserId = null,
                CreatedDate = DateTime.Now.ToString()
            };

            var documentCollectionUri = UriFactory.CreateDocumentCollectionUri(_wishlistDatabase, _wishlistCollection);
            var result = await _documentClient.CreateDocumentAsync(documentCollectionUri, wishlistItem);

            return new WishlistItemDto
            {
                Id = result.Resource.Id,
                Description = wishlistItem.Description,
                UserId = wishlistItem.UserId,
                IsClaimable = false,
                IsClaimedByMe = false
            };
        }

        public async Task EditWishlistItemAsync(string ownerId, WishlistItemDto wishlistItemDto)
        {
            try 
            { 
               //dynamic[] parameters = new dynamic[]
               //{
               //    new { wishlistItemId = wishlistItemDto.Id },
               //    new { description = wishlistItemDto.Description }
               //};

                await _documentClient.ExecuteStoredProcedureAsync<string>(
                    UriFactory.CreateStoredProcedureUri(_wishlistDatabase, _wishlistCollection, "updateWishlistItem"), 
                    new RequestOptions { PartitionKey = new PartitionKey(ownerId) },
                    wishlistItemDto.Id,
                    wishlistItemDto.Description
                );
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task DeleteWishlistItemAsync(string ownerId, string wishlistItemId)
        {
            await _documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_wishlistDatabase, _wishlistCollection, wishlistItemId.ToString()));
        }

        public async Task ClaimWishlistItemAsync(string ownerId, string wishlistItemId, string claimerId)
        {
            try
            {
                await _documentClient.ExecuteStoredProcedureAsync<string>(
                    UriFactory.CreateStoredProcedureUri(_wishlistDatabase, _wishlistCollection, "claimWishlistItem"),
                    new RequestOptions { PartitionKey = new PartitionKey(ownerId) },
                    wishlistItemId,
                    claimerId
                );
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
