using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WishlistApp.Models;
using WishlistApp.Services;

namespace WishlistApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhanauController : ControllerBase
    {
        private readonly ILogger<WhanauController> _logger;
        private readonly IWhanauService _whanauService;

        public WhanauController(ILogger<WhanauController> logger, IWhanauService whanauService)
        {
            _logger = logger;
            _whanauService = whanauService;
        }

        [HttpGet("{whanauId}")]
        public async Task<ActionResult<List<Person>>> GetWhanau(string whanauId)
        {
            var result = await _whanauService.GetWhanauAsync(whanauId);

            return result.ToList();
        }
    }
}
