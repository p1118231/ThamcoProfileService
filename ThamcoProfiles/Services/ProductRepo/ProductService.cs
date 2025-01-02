using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Newtonsoft.Json;

namespace ThamcoProfiles.Services.ProductRepo;

public class ProductService : IProductService
{

    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

        public ProductService(HttpClient client, IConfiguration configuration)
        {
            _configuration = configuration;
            
            var baseUrl = _configuration["WebServices:ProductService:ProductURL"] ?? "";
            client.BaseAddress = new System.Uri(baseUrl);
            client.Timeout = TimeSpan.FromSeconds(20);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            _client = client;
        }
        
        // get the products from the product service 
        public virtual async Task<string> GetAccessTokenAsync()
        {
        var tokenUrl = _configuration["WebServices:Auth0:TokenUrl"];
        var clientId = _configuration["WebServices:Auth0:ClientId"];
        var clientSecret = _configuration["WebServices:Auth0:ClientSecret"];
        var audience = _configuration["WebServices:Auth0:Audience"];

        var client = new HttpClient();

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "client_id", clientId ?? ""},
            { "client_secret", clientSecret ?? "" },
            { "audience", audience ?? "" }
        };

        var request = new FormUrlEncodedContent(requestBody);

        var response = await client.PostAsync(tokenUrl, request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonResponse = System.Text.Json.JsonSerializer.Deserialize<JsonElement>(responseBody);
        return jsonResponse.GetProperty("access_token").GetString() ?? string.Empty;
       }

        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            try{
            var uri = "api/product/Undercutters";
            var accessToken = await GetAccessTokenAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadAsAsync<IEnumerable<ProductDto>>();
            return products;
            }
            catch (HttpRequestException httpEx)
            {
                // Log the error or handle specific HttpRequestException (e.g., network issues)
                Console.WriteLine($"HttpRequestException occurred: {httpEx.Message}");
                // Return an empty list or rethrow as needed
                return Enumerable.Empty<ProductDto>();
            }
            catch (TaskCanceledException taskEx)
            {
                // Handle timeout-related exceptions (if any)
                Console.WriteLine($"Task was canceled: {taskEx.Message}");
                // Return an empty list or handle timeout accordingly
                return Enumerable.Empty<ProductDto>();
            }
            catch (Exception ex)
            {
                // Handle other general exceptions
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Return an empty list or handle the exception in some other way
                return Enumerable.Empty<ProductDto>();
            }
        }
}