using IceCreamRatings.Repositories;
using IceCreamRatings.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Refit;
using System;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(IceCreamRatings.Startup))]
namespace IceCreamRatings;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var context = builder.GetContext();

        var config = new ConfigurationBuilder()
            .SetBasePath(context.ApplicationRootPath)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var configuration = builder.GetContext().Configuration;

        builder.Services
            .AddRefitClient<IBfyocClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://serverlessohapi.azurewebsites.net"));

        var connection = config.GetConnectionStringOrSetting("MONGODB");
        var clientSettings = MongoClientSettings.FromConnectionString(connection);
        builder.Services.AddSingleton<IMongoClient>(new MongoClient(clientSettings));
        builder.Services.AddSingleton<RateRepository>();
    }
}