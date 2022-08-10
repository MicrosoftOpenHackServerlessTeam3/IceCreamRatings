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
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = string.Empty;
        using (StreamReader streamReader = new StreamReader(req.Body))
        {
            requestBody = await streamReader.ReadToEndAsync();
        }

        var data = JsonConvert.DeserializeObject<Rate>(requestBody);

        var products = await _bfyocClient.GetProductsAsync();

        if (products.All(x => x.ProductId != data.ProductId))
            return new BadRequestObjectResult("Produto não existe");

        var users = await _bfyocClient.GetUsersAsync();

        if (users.All(x => x.UserId != data.UserId))
            return new BadRequestObjectResult("Usuário não existe");

        if (data.Rating is < 0 and < 5)
            return new BadRequestObjectResult("Nota deve ser de zero a cinco.");

        await _rateRepository.AddAsync(data, default);

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
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");


        return new OkObjectResult("o");
    }
}