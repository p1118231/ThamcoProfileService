using System;
using System.Net;

namespace ThamcoProfiles.Services.ProductRepo;

public class ProductService : IProductService
{

    private readonly HttpClient _client;

        public ProductService(HttpClient client, IConfiguration configuration)
        {
            
            var baseUrl = configuration["WebServices:ProductService:ProductURL"] ?? "";
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var uri = "api/product/Undercutters";
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDto>>();
            return products;
        }
}