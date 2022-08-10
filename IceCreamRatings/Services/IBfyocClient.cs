using IceCreamRatings.Services.Responses;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IceCreamRatings.Services
{
    public interface IBfyocClient
    {
        [Get("/api/GetProducts")]
        public Task<IEnumerable<ProductResponse>> GetProductsAsync();

        [Get("/api/GetProduct")]
        public Task<ProductResponse> GetProductAsync([Query] string productId);

        [Get("/api/GetUsers")]
        public Task<IEnumerable<UserResponse>> GetUsersAsync();

        [Get("/api/GetUser")]
        public Task<UserResponse> GetUserAsync([Query] string userId);
    }
}