using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using MovieApp.Controllers;
using MovieApp.DataAccess;
using MovieApp.Interfaces;
using MovieApp.Models;

namespace MovieApp.Tests.Controller
{
    [TestClass]
    public class UserControllerTests
    {
        private UserDataAccessLayer _userService;
        private Mock<MovieDBContext> _mockDatabase;

        public UserControllerTests()
        {
            _mockDatabase = new Mock<MovieDBContext>();
            _userService = new UserDataAccessLayer(_mockDatabase.Object);
        }

        [TestMethod]
        public async Task Post_ReturnsRegisteredUser()
        {
            //Arrange

            var options = new DbContextOptionsBuilder<MovieDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            //Use real in memory database
            using (var dbContext = new MovieDBContext(options))
            {
                var userController = new UserController(new UserDataAccessLayer(dbContext));

                var user = new UserMaster()
                {
                    FirstName = "New",
                    LastName = "NewLast",
                    Gender = "Male",
                    Password = "Password1",
                    Username = "TestingUser",
                    UserTypeName = "User",
                    UserId = 2
                };

                // Act
                await userController.Post(user);

                var newUser = dbContext.UserMasters.FirstOrDefault(e => e.Username == user.Username);

                //Assert
                Assert.IsNotNull(newUser);
                Assert.AreEqual(user.UserId, newUser.UserId);
                Assert.AreEqual(user.UserTypeName, newUser.UserTypeName);
                Assert.AreEqual(user.Password, newUser.Password);
                Assert.AreEqual(user.Username, newUser.Username);
                Assert.AreEqual(user.FirstName, newUser.FirstName);
                Assert.AreEqual(user.Gender, newUser.Gender);
                Assert.AreEqual(user.LastName, newUser.LastName);
            }
        }
        [TestMethod]
        public async Task Post_UserNameExists_ReturnsBadRequest()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var user = new UserMaster()
            {
                FirstName = "New",
                LastName = "NewLast",
                Gender = "Male",
                Password = "Password1",
                Username = "TestUser",
                UserTypeName = "User",
                UserId = 2
            };

            var userController = new UserController(_userService);
            // Act
            await userController.Post(user);

            int userCount = _mockDatabase.Object.UserMasters.Count();

            //Assert
            Assert.AreEqual(1, userCount);
        }

        [TestMethod]
        public async Task CheckUserNameAvailability_ReturnsTrue()
        {
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var user = new UserMaster()
            {
                FirstName = "New",
                LastName = "NewLast",
                Gender = "Male",
                Password = "Password1",
                Username = "TestUserAvail",
                UserTypeName = "User",
                UserId = 2
            };

            var userController = new UserController(_userService);
            // Act
            bool isAvailable = userController.ValidateUserName(user.Username);

            //Assert
            Assert.IsTrue(isAvailable);
        }
        [TestMethod]
        public async Task CheckUserNameAvailability_ReturnsFalse()
        {
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var user = new UserMaster()
            {
                FirstName = "New",
                LastName = "NewLast",
                Gender = "Male",
                Password = "Password1",
                Username = "TestUser",
                UserTypeName = "User",
                UserId = 2
            };

            var userController = new UserController(_userService);
            // Act
            bool isAvailable = userController.ValidateUserName(user.Username);

            //Assert
            Assert.IsFalse(isAvailable);
        }
    }
}
