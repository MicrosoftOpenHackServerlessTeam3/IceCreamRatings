using IceCreamRatings.Entities;
using IceCreamRatings.Repositories;
using IceCreamRatings.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace IceCreamRatings;

public class IceCreamRatingsIceCreamRatingsFunction
{
    private readonly RateRepository _rateRepository;
    private readonly IBfyocClient _bfyocClient;
    private readonly ILogger<IceCreamRatingsIceCreamRatingsFunction> _logger;

    public IceCreamRatingsIceCreamRatingsFunction(
        RateRepository rateRepository,
        IBfyocClient bfyocClient,
        ILogger<IceCreamRatingsIceCreamRatingsFunction> log)
    {
        _rateRepository = rateRepository;
        _bfyocClient = bfyocClient;
        _logger = log;
    }

    [FunctionName("CreateRating")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiRequestBody(contentType: "application/json", typeof(Rate))]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> CreateRating(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "api/v1/ratings")] HttpRequest req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        using var streamReader = new StreamReader(req.Body);
        var requestBody = await streamReader.ReadToEndAsync();

        var data = JsonConvert.DeserializeObject<Rate>(requestBody);

        var products = await _bfyocClient.GetProductsAsync();

        if (products.All(x => x.ProductId != data.ProductId))
            return new BadRequestObjectResult("Produto não existe");

        var users = await _bfyocClient.GetUsersAsync();

        if (users.All(x => x.UserId != data.UserId))
            return new BadRequestObjectResult("Usuário não existe");

        if (data.Rating is < 0 and < 5)
            return new BadRequestObjectResult("Nota deve ser de zero a cinco.");

        await _rateRepository.AddAsync(data, cancellationToken);

        return new CreatedResult("rate", data);
    }

    [FunctionName("GetRating")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> GetRating(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");


        return new OkObjectResult("o");
    }


    [FunctionName("GetRatings")]
    [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
    public async Task<IActionResult> GetRatings(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "api/v1/ratings")] HttpRequest req, CancellationToken cancellationToken)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var userId = req.Query["userId"];

        var ratings = await _rateRepository.GetAllAsync(cancellationToken);

        if (!string.IsNullOrEmpty(userId))
        {
            ratings = ratings.Where(x => x.UserId.Equals(userId)).ToList();
        }

        return new OkObjectResult(ratings);
    }
}