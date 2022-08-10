using System;
using IceCreamRatings.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Refit;

[assembly: FunctionsStartup(typeof(IceCreamRatings.Startup))]
namespace IceCreamRatings;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {

        builder.Services
            .AddRefitClient<IBfyocClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://serverlessohapi.azurewebsites.net/"));
    }
}