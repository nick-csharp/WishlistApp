using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;

namespace WishlistApp.Services
{
    public interface IWhanauService 
    {
        DefaultWhanauDto GetDefaultWhanau();
        Task<IEnumerable<Person>> GetWhanauAsync(string whanauId);
    }
    
    public class WhanauService : IWhanauService
    {
        private readonly ILogger<WhanauService> _logger;

        private readonly Container _container;
        private readonly string _databaseId;
        private readonly string _containerId;

        private readonly string _defaultWhanauId;
        private readonly string _defaultWhanauName;
        private readonly string _defaultPersonId;

        public WhanauService(IConfiguration configuration, ILogger<WhanauService> logger, CosmosClient cosmosClient)
        {
            _logger = logger;

            _databaseId = configuration.GetValue<string>("WishlistDbId");
            _containerId = configuration.GetValue<string>("UsersContainerId");

            _container = cosmosClient.GetContainer(_databaseId, _containerId);

            _defaultWhanauId = configuration.GetValue<string>("DefaultWhanauId");
            _defaultWhanauName = configuration.GetValue<string>("DefaultWhanauName");
            _defaultPersonId = configuration.GetValue<string>("DefaultUserId");
        }

        public DefaultWhanauDto GetDefaultWhanau()
        {
            return new DefaultWhanauDto
            {
                DefaultWhanauId = _defaultWhanauId,
                DefaultWhanauName = _defaultWhanauName,
                DefaultUserId = _defaultPersonId
            };
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
