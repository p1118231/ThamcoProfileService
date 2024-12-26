using Microsoft.AspNetCore.Mvc;
using Moq;
using ThamcoProfiles.Controllers;
using ThamcoProfiles.Models;
using ThamcoProfiles.Services.ProfileRepo;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace ThamcoProfileTests
{
    [TestClass]
    public class AccountControllerTests
    {
        public required  Mock<IProfileService> _mockProfileService;
        public  required Mock<IConfiguration> _mockConfiguration;
        public  required Mock<ILogger<AccountController>> _mockLogger;
        public required AccountController _controller;

        [TestInitialize]
        public void Setup()
        {
            _mockProfileService = new Mock<IProfileService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller = new AccountController( _mockConfiguration.Object, _mockProfileService.Object, _mockLogger.Object);
        }

        [TestMethod]
                
        public async Task Details_UserExists_ReturnsViewResult()
        {
            // Arrange
            var auth0UserId = "test-user-id";
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "John99$$",
                Auth0UserId = auth0UserId
            };

            // Mock the IProfileService to return a valid user
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(auth0UserId))
                .ReturnsAsync(user);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulate logged-in user with Auth0UserId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.Details();

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));  // Check that the result is a ViewResult
            var viewResult = result as ViewResult; 
            Assert.IsNotNull(viewResult); 
            var model = viewResult.Model as User;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.FirstName, model.FirstName);
        }


        

        [TestMethod]
        
        public async Task EditField_ValidField_ReturnsViewResult()
        {
            // Arrange
            var field = "FirstName";
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Password = "Password99$",
                Email = "john@gmail.com"
            };

            // Mock the IProfileService to return a valid user
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.EditField(field);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ViewResult));  
            var viewResult = result as ViewResult; 
            Assert.IsNotNull(viewResult); 
            var model = viewResult.Model as User;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.FirstName, model.FirstName);
        }


        [TestMethod]
        public async Task EditField_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var field = "FirstName";
            var auth0UserId = "test-user-id";

            // Mock the IProfileService to return null (user not found)
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(auth0UserId))
                .ReturnsAsync((User?)null);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.EditField(field);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }


       [TestMethod]
        public async Task EditField_Post_ValidData_ReturnsRedirectToActionResult()
        {
            // Arrange
            var field = "FirstName";
            var newValue = "Jane";
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                Email = "john@gmail.com",
                Password = "John99$$"
            };

            // Mock the IProfileService to return a valid user
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.EditField(field, newValue);

            // Assert
            var redirectResult = result as RedirectToActionResult; // Cast result to RedirectToActionResult
            Assert.IsNotNull(redirectResult); 
            Assert.AreEqual("Details", redirectResult.ActionName); 
        }


        [TestMethod]
        public async Task Delete_UserExists_ReturnsViewResult()
        {
            // Arrange
            var auth0UserId = "test-user-id";
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password99$$",
                Auth0UserId = auth0UserId
            };

            // Mock the IProfileService to return a valid user
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(auth0UserId))
                .ReturnsAsync(user);

            // Mock the HttpContext.User to simulate the authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulating the logged-in user with Auth0UserId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.Delete();

            // Assert
            // Assert
             Assert.IsInstanceOfType(result, typeof(ViewResult));  // Assert that the result is of type ViewResult

            // After the assertion passes, you can safely cast and check model properties
            var viewResult = result as ViewResult;  // Now it's safe to cast
            Assert.IsNotNull(viewResult); 
            var model = viewResult.Model as User;
            Assert.IsNotNull(model);
            Assert.AreEqual(user.FirstName, model.FirstName);
        }


        [TestMethod]
        public async Task Delete_UserNotFound_ReturnsNotFoundResult()
        {
            // Arrange
            var auth0UserId = "test-user-id";

            // Mock the IProfileService to return null (user not found)
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(auth0UserId))
                .ReturnsAsync((User?)null);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulate logged-in user with Auth0UserId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.Delete();

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));  // Assert that the result is NotFoundResult
        }


        [TestMethod]
        public async Task DeleteConfirmed_Post_DeletesUserAndRedirectsToLogout()
        {
            // Arrange
            var auth0UserId = "test-user-id";
            var user = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "Password99$",
                Auth0UserId = auth0UserId
            };

            // Mock the IProfileService to return a valid user
            _mockProfileService.Setup(service => service.GetUserByAuth0IdAsync(auth0UserId))
                .ReturnsAsync(user);

            // Simulate an authenticated user
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth0UserId)  // Simulate logged-in user with Auth0UserId
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userPrincipal }
            };

            // Act
            var result = await _controller.DeleteConfirmed(user.Id);

            // Assert
            var redirectResult = result as RedirectToActionResult;  // Cast result to RedirectToActionResult
            Assert.IsNotNull(redirectResult); // Assert that the result is not null
            Assert.AreEqual("Logout", redirectResult.ActionName);  // Assert that it redirects to the "Logout" action
        }

    }
}
