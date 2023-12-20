using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieApp.Tests.DataAccess
{
    [TestClass]
    public class WatchlistDataAccessLayerTests
    {
        private Mock<MovieDBContext> _mockDbContext;
        private WatchlistDataAccessLayer _watchlistDataAccessLayer;

        [TestInitialize]
        public void Initialize()
        {
            _mockDbContext = new Mock<MovieDBContext>();
            _watchlistDataAccessLayer = new WatchlistDataAccessLayer(_mockDbContext.Object);
        }

        [TestMethod]
        public async Task GetWatchlistId_ExistingWatchlist_ReturnsWatchlistId()
        {
            // Arrange
            int userId = 1;
            string expectedWatchlistId = "watchlistId";
            var watchlist = new Watchlist { UserId = userId, WatchlistId = expectedWatchlistId };
            var watchlists = new List<Watchlist> { watchlist }.AsQueryable();

            _mockDbContext.Setup(x => x.Watchlists).Returns(MockDbSet(watchlists));

            // Act
            string actualWatchlistId = await _watchlistDataAccessLayer.GetWatchlistId(userId);

            // Assert
            Assert.AreEqual(expectedWatchlistId, actualWatchlistId);
        }

        [TestMethod]
        public async Task GetWatchlistId_NonExistingWatchlist_CreatesWatchlistAndReturnsWatchlistId()
        {
            // Arrange
            int userId = 1;
            string expectedWatchlistId = "watchlistId";
            var watchlists = new List<Watchlist>().AsQueryable();

            _mockDbContext.Setup(x => x.Watchlists).Returns(MockDbSet(watchlists));
            _mockDbContext.Setup(x => x.Watchlists.AddAsync(It.IsAny<Watchlist>())).Callback<Watchlist>(watchlist => watchlists.Add(watchlist)).Returns(Task.CompletedTask);
            _mockDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            string actualWatchlistId = await _watchlistDataAccessLayer.GetWatchlistId(userId);

            // Assert
            Assert.AreEqual(expectedWatchlistId, actualWatchlistId);
            Assert.AreEqual(1, watchlists.Count);
            Assert.AreEqual(userId, watchlists.First().UserId);
            Assert.AreEqual(expectedWatchlistId, watchlists.First().WatchlistId);
        }

        [TestMethod]
        public async Task ToggleWatchlistItem_ExistingWatchlistItem_RemovesWatchlistItem()
        {
            // Arrange
            int userId = 1;
            int movieId = 1;
            string watchlistId = "watchlistId";
            var existingWatchlistItem = new WatchlistItem { WatchlistId = watchlistId, MovieId = movieId };
            var watchlistItems = new List<WatchlistItem> { existingWatchlistItem }.AsQueryable();

            _mockDbContext.Setup(x => x.WatchlistItems).Returns(MockDbSet(watchlistItems));
            _mockDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _watchlistDataAccessLayer.ToggleWatchlistItem(userId, movieId);

            // Assert
            Assert.IsFalse(watchlistItems.Contains(existingWatchlistItem));
        }

        [TestMethod]
        public async Task ToggleWatchlistItem_NonExistingWatchlistItem_AddsWatchlistItem()
        {
            // Arrange
            int userId = 1;
            int movieId = 1;
            string watchlistId = "watchlistId";
            var watchlistItems = new List<WatchlistItem>().AsQueryable();

            _mockDbContext.Setup(x => x.WatchlistItems).Returns(MockDbSet(watchlistItems));
            _mockDbContext.Setup(x => x.WatchlistItems.Add(It.IsAny<WatchlistItem>())).Callback<WatchlistItem>(watchlistItem => watchlistItems.Add(watchlistItem));
            _mockDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _watchlistDataAccessLayer.ToggleWatchlistItem(userId, movieId);

            // Assert
            Assert.IsTrue(watchlistItems.Any(x => x.WatchlistId == watchlistId && x.MovieId == movieId));
        }

        [TestMethod]
        public async Task ClearWatchlist_ExistingWatchlist_RemovesAllWatchlistItems()
        {
            // Arrange
            int userId = 1;
            string watchlistId = "watchlistId";
            var watchlistItems = new List<WatchlistItem>
            {
                new WatchlistItem { WatchlistId = watchlistId },
                new WatchlistItem { WatchlistId = watchlistId },
                new WatchlistItem { WatchlistId = watchlistId }
            };

            _mockDbContext.Setup(x => x.WatchlistItems).Returns(MockDbSet(watchlistItems));
            _mockDbContext.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _watchlistDataAccessLayer.ClearWatchlist(userId);

            // Assert
            Assert.AreEqual(0, watchlistItems.Count);
        }

        private DbSet<T> MockDbSet<T>(IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet.Object;
        }
    }
}