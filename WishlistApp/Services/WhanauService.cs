using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Repositories;

namespace WishlistApp.Services
{
    public interface IWhanauService
    {
        Task<Whanau> GetWhanauAsync(string whanauId);
    }
    
    public class WhanauService : IWhanauService
    {
        private readonly ILogger<WhanauService> _logger;
        private readonly IWhanauRepository _whanauRepository;
        private readonly string _defaultWhanauName;

        public WhanauService(
            IConfiguration configuration, 
            ILogger<WhanauService> logger, 
            IWhanauRepository whanauRepository)
        {
            _logger = logger;
            _whanauRepository = whanauRepository;

            _defaultWhanauName = configuration.GetValue<string>("DefaultWhanauName");
        }

        public async Task<Whanau> GetWhanauAsync(string whanauId)
        {
            var people = await _whanauRepository.GetWhanauAsync(whanauId);

            return new Whanau(_defaultWhanauName, people);
        }
    }
}
