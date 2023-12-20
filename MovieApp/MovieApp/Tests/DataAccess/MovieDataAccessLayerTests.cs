using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.DataAccess;
using MovieApp.Models;

namespace MovieApp.Tests.DataAccess
{
    [TestClass]
    public class MovieDataAccessLayerTests
    {
        private readonly MovieDBContext _dbContext;

        public MovieDataAccessLayerTests()
        {
            var options = new DbContextOptionsBuilder<MovieDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new MovieDBContext(options);
        }

        [TestMethod]
        public async Task AddMovie_ReturnsAddedMovie()
        {
            //Arrange
            var moviesToAdd = TestDataHelper.GetMockMoviesData();

            //Act
            var movieService = new MovieDataAccessLayer(_dbContext);
            await movieService.AddMovie(moviesToAdd[0]);
            await movieService.AddMovie(moviesToAdd[1]);
            await movieService.AddMovie(moviesToAdd[2]);

            //Assert
            AssertHelper.AssertMovies(TestDataHelper.GetMockMoviesData(), _dbContext.Movies.ToList());
        }
        [TestMethod]
        public async Task DeleteMovie_RemovesMovie()
        {
            // Arrange
            var movie = TestDataHelper.GetMockMoviesData()[0];
            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Act
            var deletedPosterPath = await movieService.DeleteMovie(movie.MovieId);
            var deletedMovie = await _dbContext.Movies.FirstOrDefaultAsync(e => e.MovieId == movie.MovieId);

            // Assert
            Assert.IsNull(deletedMovie);
            Assert.AreEqual(0, _dbContext.Movies.Count());
        }
        [TestMethod]
        public async Task GetAllMovies_ReturnsAllMovies()
        {
            // Arrange
            var moviesToAdd = TestDataHelper.GetMockMoviesData();
            await _dbContext.Movies.AddRangeAsync(moviesToAdd);
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Act
            var result = await movieService.GetAllMovies();

            // Assert
            Assert.AreEqual(moviesToAdd.Count, result.Count);
            AssertHelper.AssertMovies(moviesToAdd, result);
        }
        [TestMethod]
        public async Task GetGenre_ReturnsGenres()
        {
            // Arrange
            var genresToAdd = TestDataHelper.GetMockGenreData();
            await _dbContext.Genres.AddRangeAsync(genresToAdd);
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Act
            var result = await movieService.GetGenre();

            // Assert
            Assert.AreEqual(genresToAdd.Count, result.Count);
            AssertHelper.AssertGenre(genresToAdd, result);
        }
        [TestMethod]
        public async Task GetSimilarMovies_ReturnsSimilarMovies()
        {
            // Arrange
            var expectedSimilarMovies = TestDataHelper.GetMockSimilarMoviesData().GetRange(1, 1);
            var movie = TestDataHelper.GetMockSimilarMoviesData()[0];

            await _dbContext.Movies.AddRangeAsync(TestDataHelper.GetMockSimilarMoviesData());
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Act
            var result = await movieService.GetSimilarMovies(movie.MovieId);

            // Assert
            Assert.AreEqual(expectedSimilarMovies.Count, result.Count);
            AssertHelper.AssertMovies(expectedSimilarMovies, result);
        }
        [TestMethod]
        public async Task UpdateMovie_UpdatesMovie()
        {
            // Arrange
            var movie = TestDataHelper.GetMockMoviesData()[0];
            await _dbContext.Movies.AddAsync(movie);
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Modify the movie object
            movie.Rating = 9.0m;
            movie.Duration = 200;

            // Act
            await movieService.UpdateMovie(movie);
            var updatedMovie = await _dbContext.Movies.FirstOrDefaultAsync(e => e.MovieId == movie.MovieId);

            // Assert
            Assert.IsNotNull(updatedMovie);
            Assert.AreEqual(movie.Rating, updatedMovie.Rating);
            Assert.AreEqual(movie.Duration, updatedMovie.Duration);
        }
        [TestMethod]
        public async Task GetMoviesAvailableInWatchlist_ReturnsMovies()
        {
            // Arrange
            var watchlistId = "1";
            var expectedMovies = TestDataHelper.GetMockWatchlistItemsData()
                .Select(item => TestDataHelper.GetMockMoviesData().FirstOrDefault(m => m.MovieId == item.MovieId))
                .ToList();

            await _dbContext.WatchlistItems.AddRangeAsync(TestDataHelper.GetMockWatchlistItemsData());
            await _dbContext.Movies.AddRangeAsync(expectedMovies);
            await _dbContext.SaveChangesAsync();

            var movieService = new MovieDataAccessLayer(_dbContext);

            // Act
            var result = await movieService.GetMoviesAvailableInWatchlist(watchlistId);

            // Assert
            Assert.AreEqual(expectedMovies.Count, result.Count);
            AssertHelper.AssertMovies(expectedMovies, result);
        }
    }
}
