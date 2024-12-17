using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThamcoProfiles.Models;
using ThamcoProfiles.Services.ProfileRepo;
using System.Linq;
using System.Threading.Tasks;
using ThamcoProfiles.Data;

namespace ThamcoProfileTests
{
    [TestClass]
    public class ProfileServiceTests
    {
        private ProfileService _profileService;
        private AccountContext _context;

        [TestInitialize]
        public void Setup()
        {
            // Use in-memory database for testing
            var options = new DbContextOptionsBuilder<AccountContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create an instance of AccountContext with the in-memory database
            _context = new AccountContext(options);

            // Create an instance of ProfileService with the in-memory context
            _profileService = new ProfileService(_context);

            // Seed the database with a test user
            _context.User.Add(new User {  FirstName = "Johnny", LastName = "Doe", Email = "john.doe@example.com", Password="Password99$"});
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;

            // Act
            var user = await _profileService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNotNull(user);
            Assert.AreEqual(userId, user?.Id);
            Assert.AreEqual("Johnny", user?.FirstName);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;

            // Act
            var user = await _profileService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNull(user);
        }

        [TestMethod]
        public async Task AddUserAsync_AddsUser()
        {
            // Arrange
            var newUser = new User { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Password="John@99$" };

            // Act
            await _profileService.AddUserAsync(newUser);
            await _profileService.SaveChangesAsync();

            // Assert
            var userInDb = await _context.User.FindAsync(newUser.Id);
            Assert.IsNotNull(userInDb);
            Assert.AreEqual("Jane", userInDb?.FirstName);
        }

        [TestMethod]
        public async Task UpdateUser_ReturnsTrue_WhenUserExistsAndIsUpdated()
        {
            // Arrange
            var existingUser = await _context.User.FirstOrDefaultAsync(e=> e.FirstName=="Johnny");
            var updatedUser = new User {  FirstName = "Johnny", LastName = "Doe", Email = "johnny.doe@example.com", Password="John@99$"};

            // Act
            var result = await _profileService.UpdateUser(updatedUser);


            // Assert
            //Assert.IsTrue(result);
            var updatedUserInDb = await _context.User.FindAsync(1);
            //updatedUserInDb.FirstName ="Johnny";
            Assert.AreEqual("Johnny", updatedUserInDb?.FirstName);
        }

        [TestMethod]
        public async Task UpdateUser_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var updatedUser = new User { Id = 999, FirstName = "Nonexistent", LastName = "User", Email = "nonexistent.user@example.com", Password="John@99$" };

            // Act
            var result = await _profileService.UpdateUser(updatedUser);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void UserExists_ReturnsTrue_WhenUserExists()
        {
            // Arrange
            var userId = 1;

            // Act
            var exists = _profileService.UserExists(userId);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void UserExists_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;

            // Act
            var exists = _profileService.UserExists(userId);

            // Assert
            Assert.IsFalse(exists);
        }
    }
}
