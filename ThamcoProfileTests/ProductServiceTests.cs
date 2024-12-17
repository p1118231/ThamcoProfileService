using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThamcoProfiles.Models;
using ThamcoProfiles.Services.ProductRepo;
using Microsoft.Extensions.Configuration;

[TestClass]
public class ProductServiceTests
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _mockHttpClient;
    private ProductService _productService;

    [TestInitialize]
    public void Setup()
    {
        // Mock IConfiguration for setting base URLs and other configuration values
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["WebServices:ProductService:ProductURL"]).Returns("https://mockapi.com/");
        _mockConfiguration.Setup(c => c["WebServices:Auth0:TokenUrl"]).Returns("https://mockauth.com/token");
        _mockConfiguration.Setup(c => c["WebServices:Auth0:ClientId"]).Returns("clientId");
        _mockConfiguration.Setup(c => c["WebServices:Auth0:ClientSecret"]).Returns("clientSecret");
        _mockConfiguration.Setup(c => c["WebServices:Auth0:Audience"]).Returns("audience");

        // Set up a MockHttpMessageHandler to simulate HTTP requests and responses
        var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonConvert.SerializeObject(new List<ProductDto>
            {
                new ProductDto { Name = "Product 1" },
                new ProductDto { Name = "Product 2" }
            }))
        };

        // Mock the HttpMessageHandler to simulate HTTP requests
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsNull<HttpRequestMessage>(), ItExpr.IsNull<CancellationToken>())
            .ReturnsAsync(mockResponse);

        // Use the MockHttpMessageHandler to create an HttpClient
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
        _productService = new ProductService(_mockHttpClient, _mockConfiguration.Object);
    }

  [TestMethod]
    public async Task GetProductsAsync_WhenTokenAndProductsAreValid_ReturnsProductList()
    {
        // Arrange
        var mockAccessToken = "mock-access-token"; // Simulated token
        var mockProducts = new List<ProductDto>
        {
            new ProductDto { Name = "Product 1" },
            new ProductDto { Name = "Product 2" }
        };

        // Mock the HttpMessageHandler to simulate a successful HTTP response
        _mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.IsNull<HttpRequestMessage>(), 
                ItExpr.IsNull<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(mockProducts))
            });

        // Act
        var result = await _productService.GetProductsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count()); // Ensure 2 products are returned
        Assert.AreEqual("Product 1", result.First().Name); // Ensure the first product name is correct
    }
/*
    [TestMethod]
    public async Task GetProductsAsync_WhenTokenFails_ReturnsEmptyList()
    {
        // Arrange: Simulate a failure to retrieve the token
        var mockFailureResponse = new HttpResponseMessage(HttpStatusCode.Unauthorized);

        // Mock the HttpMessageHandler to simulate a failed token retrieval
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())
            .ReturnsAsync(mockFailureResponse);

        // Create a new HttpClient using the MockHttpMessageHandler
        _mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
        _productService = new ProductService(_mockHttpClient, _mockConfiguration.Object);

        // Act: Attempt to get products when token retrieval fails
        var result = await _productService.GetProductsAsync();

        // Assert: Ensure that an empty list is returned when the token fails
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count()); // Ensure the result is empty
    }

    [TestMethod]
    public async Task GetProductsAsync_WhenApiCallFails_ReturnsEmptyList()
    {
        // Arrange: Simulate a failed API call (e.g., 500 Internal Server Error)
        var mockApiFailureResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

        // Mock the HttpMessageHandler to simulate a failed API call
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>())
            .ReturnsAsync(mockApiFailureResponse);

        // Create a new HttpClient using the MockHttpMessageHandler
        _mockHttpClient = new HttpClient(mockHttpMessageHandler.Object);
        _productService = new ProductService(_mockHttpClient, _mockConfiguration.Object);

        // Act: Call the method to test it
        var result = await _productService.GetProductsAsync();

        // Assert: Verify that an empty list is returned when the API call fails
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count()); // Ensure the result is empty
    }*/
}
