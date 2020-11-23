using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;

namespace WishlistApp.Repositories
{
    public interface IWhanauRepository 
    {
        Task<Person> GetPersonByObjectIdAsync(string objectId);
        Task<IEnumerable<Person>> GetWhanauAsync(string whanauId);
    }
    
    public class WhanauRepository : IWhanauRepository
    {
        private readonly ILogger<WhanauRepository> _logger;

        private readonly Container _container;
        private readonly string _databaseId;
        private readonly string _containerId;

      
        public WhanauRepository(IConfiguration configuration, ILogger<WhanauRepository> logger, CosmosClient cosmosClient)
        {
            _logger = logger;

            _databaseId = configuration.GetValue<string>("WishlistDbId");
            _containerId = configuration.GetValue<string>("UsersContainerId");

            _container = cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<Person> GetPersonByObjectIdAsync(string objectId)
        {
            try
            {
                using (var setIterator = _container.GetItemLinqQueryable<Person>()
                    .Where(p => p.ObjectId == objectId)
                    .ToFeedIterator()
                )
                {
                    if (setIterator.HasMoreResults)
                    {
                        var response = await setIterator.ReadNextAsync();

                        _logger.LogInformation("Request charge of get operation: {0}", response.RequestCharge);
                        _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);

                        return response.Resource.FirstOrDefault();
                    }
                }

                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting person by objectId.");
                throw;
            }
        }

        public async Task<IEnumerable<Person>> GetWhanauAsync(string whanauId)
        {
            try
            {
                var persons = new List<Person>();

                var queryRequestOptions = new QueryRequestOptions() { PartitionKey = new PartitionKey(whanauId) };
                using (var setIterator = _container.GetItemLinqQueryable<Person>(requestOptions: queryRequestOptions)
                    .ToFeedIterator())
                {
                    while (setIterator.HasMoreResults)
                    {
                        var response = await setIterator.ReadNextAsync();

                        _logger.LogInformation("Request charge of get operation: {0}", response.RequestCharge);
                        _logger.LogInformation("StatusCode of operation: {0}", response.StatusCode);

                        persons.AddRange(response);
                    }
                }

                return persons;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while getting persons.");
                throw;
            }
        }
    }
}
