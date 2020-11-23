using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Repositories;

namespace WishlistApp.Services
{
    public interface IWhanauService
    {
        Task<IEnumerable<Person>> GetWhanauAsync(string whanauId);
    }
    
    public class WhanauService : IWhanauService
    {
        private readonly ILogger<WhanauService> _logger;
        private readonly IWhanauRepository _whanauRepository;

        public WhanauService(ILogger<WhanauService> logger, IWhanauRepository whanauRepository)
        {
            _logger = logger;
            _whanauRepository = whanauRepository;
        }

        public async Task<IEnumerable<Person>> GetWhanauAsync(string whanauId)
        {
            return await _whanauRepository.GetWhanauAsync(whanauId);
        }
    }
}
