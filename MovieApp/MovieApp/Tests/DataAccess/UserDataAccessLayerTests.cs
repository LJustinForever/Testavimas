using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieApp.DataAccess;
using MovieApp.Models;
using System.Linq;
using System.Threading.Tasks;
using Moq.EntityFrameworkCore;
using MovieApp.Dto;

namespace MovieApp.Tests.DataAccess
{
    [TestClass]
    public class UserDataAccessLayerTests
    {
        private UserDataAccessLayer _userDataAccessLayer;
        private Mock<MovieDBContext> _mockDbContext;

        [TestInitialize]
        public void TestInitialize()
        {
            // Create a mock MovieDBContext using an in-memory database
            var options = new DbContextOptionsBuilder<MovieDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDB")
                .Options;
            _mockDbContext = new Mock<MovieDBContext>(options);

            _userDataAccessLayer = new UserDataAccessLayer(_mockDbContext.Object);
        }

        [TestMethod]
        public void AuthenticateUser_ValidCredentials_ReturnsAuthenticatedUser()
        {
            // Arrange
            var loginCredentials = new UserLogin
            {
                Username = "TestUser",
                Password = "TestPassword1"
            };

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = _userDataAccessLayer.AuthenticateUser(loginCredentials);

            // Assert
            var expected = TestDataHelper.GetMockUserData()[0];
            Assert.IsNotNull(result);
            Assert.AreEqual(expected.Username, result.Username);
            Assert.AreEqual(expected.UserId, result.UserId);
            Assert.AreEqual(expected.UserTypeName, result.UserTypeName);
        }

        [TestMethod]
        public async Task IsUserExists_ExistingUserId_ReturnsTrue()
        {
            // Arrange
            var userId = 1;
            var userMaster = TestDataHelper.GetMockUserData()[0];

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = await _userDataAccessLayer.IsUserExists(userId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task IsUserExists_NonExistingUserId_ReturnsFalse()
        {
            // Arrange
            var userId = 2;

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = await _userDataAccessLayer.IsUserExists(userId);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task RegisterUser_UserNameAvailable_ReturnsTrue()
        {
            // Arrange
            var userData = new UserMaster
            {
                Username = "newuser",
                Password = "newpassword"
            };

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = await _userDataAccessLayer.RegisterUser(userData);

            // Assert
            Assert.IsTrue(result);
            _mockDbContext.Verify(db => db.UserMasters.Add(userData), Times.Once);
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
        }

        [TestMethod]
        public async Task RegisterUser_UserNameNotAvailable_ReturnsFalse()
        {
            // Arrange
            var userData = new UserMaster
            {
                Username = "TestUser",
                Password = "newpassword1"
            };

            var existingUser = TestDataHelper.GetMockUserData()[0];

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = await _userDataAccessLayer.RegisterUser(userData);

            // Assert
            Assert.IsFalse(result);
            _mockDbContext.Verify(db => db.UserMasters.Add(userData), Times.Never);
            _mockDbContext.Verify(db => db.SaveChanges(), Times.Never);
        }

        [TestMethod]
        public void CheckUserNameAvailability_UserNameAvailable_ReturnsTrue()
        {
            // Arrange
            var userName = "newuser";

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = _userDataAccessLayer.CheckUserNameAvailability(userName);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CheckUserNameAvailability_UserNameNotAvailable_ReturnsFalse()
        {
            // Arrange
            var userName = "TestUser";
            var existingUser = TestDataHelper.GetMockUserData()[0];

            _mockDbContext.Setup<DbSet<UserMaster>>(db => db.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            // Act
            var result = _userDataAccessLayer.CheckUserNameAvailability(userName);

            // Assert
            Assert.IsFalse(result);
        }
    }
}