﻿using IceCreamRatings.Repositories;
using IceCreamRatings.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Refit;
using System;

[assembly: FunctionsStartup(typeof(IceCreamRatings.Startup))]
namespace IceCreamRatings;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = builder.GetContext().Configuration;

        builder.Services
            .AddRefitClient<IBfyocClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(configuration["BfyocClient_Address"]));
        
        var clientSettings = MongoClientSettings.FromConnectionString(configuration["Database_ConnectionString"]);
        builder.Services.AddSingleton<IMongoClient>(new MongoClient(clientSettings));
        builder.Services.AddSingleton<RateRepository>();
    }
}