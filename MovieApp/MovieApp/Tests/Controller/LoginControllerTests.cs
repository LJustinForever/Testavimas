using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieApp.Controllers;
using MovieApp.Models;
using Moq.EntityFrameworkCore;
using MovieApp.DataAccess;
using System.Net;
using MovieApp.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MovieApp.Tests.Controller
{
    [TestClass]
    public class LoginControllerTests
    {
        private UserDataAccessLayer _userService;
        private Mock<MovieDBContext> _mockDatabase;
        private Mock<IConfiguration> _mockConfiguration;

        public LoginControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(e => e["Jwt:Issue"]).Returns("https://localhost:7073/");
            _mockConfiguration.Setup(e => e["Jwt:SecretKey"]).Returns("E6ls8Q47g8UBzySY");
            _mockConfiguration.Setup(e => e["Jwt:Audience"]).Returns("https://localhost:7073/");

            _mockDatabase = new Mock<MovieDBContext>();
            _userService = new UserDataAccessLayer(_mockDatabase.Object);
        }

        [TestMethod]
        public async Task Login_ReturnsSuccessfulLogin()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var userLogin = new Dto.UserLogin {Username = "TestUser", Password = "TestPassword1"};

            //Act
            var loginController = new LoginController(_mockConfiguration.Object, _userService);

            var result = loginController.Login(userLogin);

            var data = (dynamic)((OkObjectResult)result).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result
                .GetType()
                .GetProperty("StatusCode")
            .GetValue(result, null));

            Assert.IsInstanceOfType<OkObjectResult>(((OkObjectResult)result));

            Assert.IsNotNull(data);
            Assert.AreEqual("TestUser", data.userDetails.Username);
            Assert.AreEqual("Admin", data.userDetails.UserTypeName);
            Assert.AreEqual(1, data.userDetails.UserId);
            Assert.IsFalse(string.IsNullOrEmpty(data.token));
        }

        [TestMethod]
        public async Task InvalidLogin_ReturnsUnsuccessfulLogin()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var userLogin = new Dto.UserLogin { Username = "TestUser", Password = "InvalidPassword" };

            //Act
            var loginController = new LoginController(_mockConfiguration.Object, _userService);

            var result = loginController.Login(userLogin);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.Unauthorized, (HttpStatusCode)result
                .GetType()
                .GetProperty("StatusCode")
                .GetValue(result, null));

            Assert.IsInstanceOfType<UnauthorizedResult>(((UnauthorizedResult)result));
        }

        [TestMethod]
        public async Task GenerateJsonWebToken_ReturnsGeneratedToken()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());

            var userLogin = new Dto.AuthenticatedUser() { Username = "TestUser", UserTypeName = "Admin" };

            //Act
            var loginController = new LoginController(_mockConfiguration.Object, _userService);

            var result = loginController.GenerateJSONWebToken(userLogin);

            //Assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
