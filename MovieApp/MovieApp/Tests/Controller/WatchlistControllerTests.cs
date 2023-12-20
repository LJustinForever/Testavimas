using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.EntityFrameworkCore;
using MovieApp.Controllers;
using MovieApp.DataAccess;
using MovieApp.Dto;
using MovieApp.Interfaces;
using MovieApp.Models;
using System.Net;

namespace MovieApp.Tests.Controller
{
    [TestClass]
    public class WatchlistControllerTests
    {
        private readonly WatchlistDataAccessLayer _watchlistService;
        private readonly MovieDataAccessLayer _movieService;
        private readonly UserDataAccessLayer _userService;

        private Mock<MovieDBContext> _mockDatabase;

        public WatchlistControllerTests()
        {
            _mockDatabase = new Mock<MovieDBContext>();

            _userService = new UserDataAccessLayer(_mockDatabase.Object);
            _movieService = new MovieDataAccessLayer(_mockDatabase.Object);
            _watchlistService = new WatchlistDataAccessLayer(_mockDatabase.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsUserWatchlist()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockMoviesData());
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());
            _mockDatabase.Setup<DbSet<Watchlist>>(e => e.Watchlists)
                .ReturnsDbSet(TestDataHelper.GetMockWatchlistData());
            _mockDatabase.Setup<DbSet<WatchlistItem>>(e => e.WatchlistItems)
                .ReturnsDbSet(TestDataHelper.GetMockWatchlistItemsData());

            //Act
            var watchlistController = new WatchlistController(_watchlistService, _movieService, _userService);

            var result = await watchlistController.Get(TestDataHelper.GetMockUserData()[0].UserId);

            //Assert
            var expected = TestDataHelper.GetMockMoviesData();
            expected.RemoveAt(2);
            AssertHelper.AssertMovies(expected, result);
        }

        [TestMethod]
        public async Task ToggleWishlistItem_ReturnsRemoveWatchlist()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockMoviesData());
            _mockDatabase.Setup<DbSet<UserMaster>>(e => e.UserMasters)
                .ReturnsDbSet(TestDataHelper.GetMockUserData());
            _mockDatabase.Setup<DbSet<Watchlist>>(e => e.Watchlists)
                .ReturnsDbSet(TestDataHelper.GetMockWatchlistData());
            _mockDatabase.Setup<DbSet<WatchlistItem>>(e => e.WatchlistItems)
                .ReturnsDbSet(TestDataHelper.GetMockWatchlistItemsData());

            var movieToRemove = TestDataHelper.GetMockMoviesData()[1];

            //Act
            var watchlistController = new WatchlistController(_watchlistService, _movieService, _userService);
            var result = await watchlistController.Get(TestDataHelper.GetMockUserData()[0].UserId,
                movieToRemove.MovieId);

            //Assert
            var expected = TestDataHelper.GetMockMoviesData();
            expected.RemoveRange(1, 2);
            AssertHelper.AssertMovies(expected, result);
        }
        [TestMethod]
        public async Task ToggleWishlistItem_ReturnsAddWatchlist()
        {
            //Arrange
            var watchlistItem = TestDataHelper.GetMockWatchlistItemsData();
            watchlistItem.RemoveAt(1);

            var movieToAdd = TestDataHelper.GetMockMoviesData()[1];

            var options = new DbContextOptionsBuilder<MovieDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            //Use real in memory database
            using (var dbContext = new MovieDBContext(options))
            {
                dbContext.Movies.AddRange(TestDataHelper.GetMockMoviesData());
                dbContext.UserMasters.AddRange(TestDataHelper.GetMockUserData());
                dbContext.Watchlists.AddRange(TestDataHelper.GetMockWatchlistData());
                dbContext.WatchlistItems.AddRange(watchlistItem);
                dbContext.SaveChanges();

                //Act
                var userService = new UserDataAccessLayer(dbContext);
                var movieService = new MovieDataAccessLayer(dbContext);
                var watchlistService = new WatchlistDataAccessLayer(dbContext);

                var watchlistController = new WatchlistController(watchlistService, movieService, userService);
                var result = await watchlistController.Get(TestDataHelper.GetMockUserData()[0].UserId,
                    movieToAdd.MovieId);

                //Assert
                var expected = TestDataHelper.GetMockMoviesData();
                expected.RemoveRange(2, 1);
                AssertHelper.AssertMovies(expected, result);
            }
        }

        [TestMethod]
        public async Task Delete_ReturnsOkAndEmptyList()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<MovieDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            //Use real in memory database
            using (var dbContext = new MovieDBContext(options))
            {
                dbContext.Movies.AddRange(TestDataHelper.GetMockMoviesData());
                dbContext.UserMasters.AddRange(TestDataHelper.GetMockUserData());
                dbContext.Watchlists.AddRange(TestDataHelper.GetMockWatchlistData());
                dbContext.WatchlistItems.AddRange(TestDataHelper.GetMockWatchlistItemsData());
                dbContext.SaveChanges();

                //Act
                var userService = new UserDataAccessLayer(dbContext);
                var movieService = new MovieDataAccessLayer(dbContext);
                var watchlistService = new WatchlistDataAccessLayer(dbContext);

                var watchlistController = new WatchlistController(watchlistService, movieService, userService);
                var result = await watchlistController.Delete(TestDataHelper.GetMockUserData()[0].UserId);

                //Assert
                Assert.IsNotNull(result);
                Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result
                    .GetType()
                    .GetProperty("StatusCode")
                    .GetValue(result, null));
                Assert.AreEqual(0, dbContext.WatchlistItems.Count(e => e.WatchlistId == "1"));
            }
        }
    }
}
