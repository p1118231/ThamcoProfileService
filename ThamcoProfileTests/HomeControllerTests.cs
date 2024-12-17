using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ThamcoProfiles.Controllers;
using ThamcoProfiles.Models;
using ThamcoProfiles.Services.ProductRepo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

[TestClass]
public class HomeControllerTests
{
    private Mock<ILogger<HomeController>> _mockLogger;
    private Mock<IProductService> _mockProductService;
    private HomeController _controller;

    [TestInitialize]
    public void Setup()
    {
        // Mock dependencies
        _mockLogger = new Mock<ILogger<HomeController>>();
        _mockProductService = new Mock<IProductService>();

        // Initialize the controller with mocked dependencies
        _controller = new HomeController(_mockLogger.Object, _mockProductService.Object);
    }

    [TestMethod]
    public async Task Index_WhenServiceReturnsProducts_ReturnsViewWithProducts()
    {
        // Arrange
        var mockProducts = new List<ProductDto>
        {
            new ProductDto { Name = "Product 1" },
            new ProductDto { Name = "Product 2" }
        };

        _mockProductService.Setup(service => service.GetProductsAsync())
            .ReturnsAsync(mockProducts);

        // Simulate an authenticated user
        var auth0UserId = "test-user-id";
        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulate logged-in user with Auth0UserId
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = userPrincipal }
        };

        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));  // Assert that the result is of type ViewResult

            // After the assertion passes, you can safely cast and check model properties
        var viewResult = result as ViewResult;  // Now it's safe to cast
        Assert.IsNotNull(viewResult); 
        var model = viewResult.Model as IEnumerable<ProductDto>;
        Assert.IsNotNull(model);
        Assert.AreEqual(2, model.Count());
    }

    [TestMethod]
    public async Task Index_WhenServiceThrowsException_ReturnsViewWithEmptyProducts()
    {
        // Arrange
        _mockProductService.Setup(service => service.GetProductsAsync())
            .ThrowsAsync(new Exception("Service error"));

        // Simulate an authenticated user
        var auth0UserId = "test-user-id";
        var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, auth0UserId)
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = userPrincipal }
        };

        // Act
        var result = await _controller.Index();

        // Assert
        Assert.IsInstanceOfType(result, typeof(ViewResult));  // Assert that the result is of type ViewResult

            // After the assertion passes, you can safely cast and check model properties
            var viewResult = result as ViewResult;  // Now it's safe to cast
            Assert.IsNotNull(viewResult); 
        var model = viewResult.Model as IEnumerable<ProductDto>;
        Assert.IsNotNull(model);
        Assert.AreEqual(0, model.Count());
    }

    [TestMethod]
        public async Task Products_WhenServiceReturnsProducts_ReturnsViewWithProducts()
        {
            // Arrange
            var mockProducts = new List<ProductDto>
            {
                new ProductDto { Name = "Product 1" },
                new ProductDto { Name = "Product 2" }
            };

            _mockProductService.Setup(service => service.GetProductsAsync())
                .ReturnsAsync(mockProducts);

            // Simulate an authenticated user
            var auth0UserId = "test-user-id";
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulate logged-in user with Auth0UserId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.Products();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));  // Assert that the result is of type ViewResult

            // After the assertion passes, you can safely cast and check model properties
            var viewResult = result as ViewResult;  // Now it's safe to cast
            Assert.IsNotNull(viewResult); 
            var model = viewResult.Model as IEnumerable<ProductDto>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count());
        }

        [TestMethod]
        public async Task Search_WhenQueryIsGiven_ReturnsFilteredProducts()
        {
            // Arrange
            var query = "Product 1";
            var allProducts = new List<ProductDto>
            {
                new ProductDto { Name = "Product 1" },
                new ProductDto { Name = "Product 2" }
            };

            _mockProductService.Setup(service => service.GetProductsAsync())
                .ReturnsAsync(allProducts);

            // Simulate an authenticated user
            var auth0UserId = "test-user-id";
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.Search(query);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));  // Assert that the result is of type ViewResult

            // After the assertion passes, you can safely cast and check model properties
            var viewResult = result as ViewResult;  // Now it's safe to cast
            Assert.IsNotNull(viewResult); 
            var model = viewResult.Model as List<ProductDto>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count); // Only 1 product should be filtered by the query
            Assert.AreEqual("Product 1", model[0].Name);
        }


}
