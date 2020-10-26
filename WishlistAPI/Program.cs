using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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
                        .AddSingleton<IDocumentClient>(x =>
                            new DocumentClient(new Uri(context.Configuration["CosmosDBEndpoint"]), context.Configuration["CosmosDBKey"]))
                        .AddScoped<IWishlistService, WishlistService>()
                );
    }
}
