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
    public interface IPersonService 
    {
        Task<IEnumerable<Person>> GetAllPersonsAsync();
    }
    
    public class PersonService : IPersonService
    {
        private readonly ILogger<PersonService> _logger;

        private readonly Container _container;
        private readonly string _databaseId;
        private readonly string _containerId;

        public PersonService(IConfiguration configuration, ILogger<PersonService> logger, CosmosClient cosmosClient)
        {
            _logger = logger;

            _databaseId = configuration.GetValue<string>("WishlistDbId");
            _containerId = configuration.GetValue<string>("UsersContainerId");

            _container = cosmosClient.GetContainer(_databaseId, _containerId);
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            try
            {
                var persons = new List<Person>();

                using (var setIterator = _container.GetItemLinqQueryable<Person>()
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
