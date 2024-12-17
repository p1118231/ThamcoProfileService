using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using ThamcoProfiles.Services.ProductRepo;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Moq.Protected;

namespace ThamcoProfileTests
{
    [TestClass]
    public class ProductServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private ProductService _productService;

        [TestInitialize]
        public void Setup()
        {
            // Mock the IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration mocks
            _mockConfiguration.Setup(config => config["WebServices:ProductService:ProductURL"]).Returns("http://fakeapi.com");
            _mockConfiguration.Setup(config => config["WebServices:Auth0:TokenUrl"]).Returns("http://auth0.com/token");
            _mockConfiguration.Setup(config => config["WebServices:Auth0:ClientId"]).Returns("client_id");
            _mockConfiguration.Setup(config => config["WebServices:Auth0:ClientSecret"]).Returns("client_secret");
            _mockConfiguration.Setup(config => config["WebServices:Auth0:Audience"]).Returns("audience");

            // Instantiate ProductService
            var httpClient = new HttpClient();
            _productService = new ProductService(httpClient, _mockConfiguration.Object);
        }

        [TestMethod]
        public async Task GetProductsAsync_ReturnsProductList()
        {
            // Arrange
            var productResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new List<ProductDto>
                {
                    new ProductDto { Id = 1, Name = "Product 1", Price = 100 },
                    new ProductDto { Id = 2, Name = "Product 2", Price = 200 }
                }))
            };

            // Set the content type of the response to application/json
            productResponse.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            // Mock HttpMessageHandler to simulate HttpClient behavior
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(productResponse);

            var httpClient = new HttpClient(mockHandler.Object);

            // Mock GetAccessTokenAsync method
            var mockProductService = new Mock<ProductService>(httpClient, _mockConfiguration.Object) { CallBase = true };
            mockProductService.Setup(service => service.GetAccessTokenAsync()).ReturnsAsync("fakeAccessToken");

            // Act
            var products = await mockProductService.Object.GetProductsAsync();

            // Assert
            Assert.IsNotNull(products);
            Assert.AreEqual(2, products.Count());
            Assert.AreEqual("Product 1", products.First().Name);
            Assert.AreEqual(100, products.First().Price);
        }
    }
}
