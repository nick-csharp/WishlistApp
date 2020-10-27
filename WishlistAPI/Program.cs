using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace WishlistAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((context, serviceCollection) =>
                    serviceCollection
                        .AddSingleton(s =>
                        {
                            var endpoint = context.Configuration["CosmosDBEndpoint"];
                            var key = context.Configuration["CosmosDBKey"];

                            if (string.IsNullOrEmpty(endpoint))
                                throw new InvalidOperationException("Missing CosmosDBEndpoint in appsettings.json");

                            if (string.IsNullOrEmpty(key))
                                throw new InvalidOperationException("Missing CosmosDBKey in appsettings.json");

                            return new CosmosClientBuilder(endpoint, key).Build();
                        })
                        .AddScoped<IWishlistService, WishlistService>()
                );
    }
}
