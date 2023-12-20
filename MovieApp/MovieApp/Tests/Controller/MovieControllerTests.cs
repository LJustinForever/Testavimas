using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MovieApp.Controllers;
using MovieApp.Models;
using Moq.EntityFrameworkCore;
using MovieApp.DataAccess;
using System.Net;

namespace MovieApp.Tests.Controller
{
    [TestClass]
    public class MovieControllerTests
    {
        private Mock<MovieDBContext> _mockDatabase;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IWebHostEnvironment> _webHostEnvironment;
        private MovieDataAccessLayer _movieService;

        public MovieControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(e => e["DefaultPoster"]).Returns("DefaultPoster.jpg");

            _webHostEnvironment = new Mock<IWebHostEnvironment>();
            _webHostEnvironment.Setup(e => e.WebRootPath).Returns("/");

            _mockDatabase = new Mock<MovieDBContext>(); 
            _movieService = new MovieDataAccessLayer(_mockDatabase.Object);
        }

        [TestMethod]
        public async Task Get_ReturnsAllMovies()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockMoviesData());

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var data = await movieController.Get();

            //Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(3, data.Count);
            AssertHelper.AssertMovies(TestDataHelper.GetMockMoviesData(), data);
        }

        [TestMethod]
        public async Task GetGenreList_ReturnsGenreList()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Genre>>(e => e.Genres)
                .ReturnsDbSet(TestDataHelper.GetMockGenreData());

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var data = (await movieController.GenreList()).ToList();

            //Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(6, data.Count);
            AssertHelper.AssertGenre(TestDataHelper.GetMockGenreData(), data);
        }

        [TestMethod]
        public async Task GetSimilarMovies_ReturnsSimilarMovies()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockSimilarMoviesData());

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var movie = TestDataHelper.GetMockSimilarMoviesData()[0];

            var data = await movieController.SimilarMovies(movie.MovieId);

            //Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count);
            AssertHelper.AssertMovies(TestDataHelper.GetMockSimilarMoviesData()
                    .Where(e => e.Genre == movie.Genre && e.MovieId != movie.MovieId).ToList(),
                data);
        }

        [TestMethod]
        public async Task GetSimilarMovies_ReturnsNoSimilarMovies()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockSimilarMoviesData());

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var movie = TestDataHelper.GetMockSimilarMoviesData()[2];

            var data = await movieController.SimilarMovies(movie.MovieId);

            //Assert
            Assert.IsNotNull(data);
            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public async Task PostMovie_ReturnsAppendedDataToDB()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies);

            var movie = new Movie
            {
                MovieId = 2027,
                Title = "Avatar: The Way of Water",
                Overview =
                    "Set more than a decade after the events of the first film, learn the story of the Sully family (Jake, Neytiri, and their kids), the trouble that follows them, the lengths they go to keep each other safe, the battles they fight to stay alive, and the tragedies they endure.",
                Genre = "Science Fiction",
                Language = "English",
                Duration = 192,
                Rating = 8.0m,
                PosterPath = "9030899c-e80d-4204-a62b-fdd5a334263bAvatar.jpg"
            };

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var result = await movieController.Post(movie);

            var appendedData = await _movieService.GetAllMovies();

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result
                .GetType()
                .GetProperty("StatusCode")
                .GetValue(result, null));

            Assert.IsNotNull(appendedData);
            Assert.AreEqual(new List<Movie>{movie}, appendedData);
        }

        [TestMethod]
        public async Task PutMovie_ReturnsEditedDataToDB()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockSimilarMoviesData());

            var movie = new Movie
            {
                MovieId = 2027,
                Title = "EDITED",
                Overview =
                    "EDITED",
                Genre = "Science Fiction",
                Language = "EDITED",
                Duration = 192,
                Rating = 8.0m,
                PosterPath = "9030899c-e80d-4204-a62b-fdd5a334263bAvatar.jpg"
            };

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var result = await movieController.Put(movie);

            var appendedMovie = (await _movieService.GetAllMovies()).FirstOrDefault(e => e.MovieId == movie.MovieId);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result
                .GetType()
                .GetProperty("StatusCode")
                .GetValue(result, null));

            Assert.IsNotNull(appendedMovie);
            Assert.AreEqual("EDITED", appendedMovie.Language);
            Assert.AreEqual("EDITED", appendedMovie.Overview);
            Assert.AreEqual("EDITED", appendedMovie.Title);
        }

        [TestMethod]
        public async Task DeleteMovie_ReturnsLessDataFromDB()
        {
            //Arrange
            _mockDatabase.Setup<DbSet<Movie>>(e => e.Movies)
                .ReturnsDbSet(TestDataHelper.GetMockSimilarMoviesData());

            var movieToDelete = TestDataHelper.GetMockSimilarMoviesData()[0];

            //Act
            var movieController = new MovieController(_mockConfiguration.Object, _webHostEnvironment.Object,
                _movieService);

            var result = await movieController.Delete(movieToDelete.MovieId);

            var movieData = (await _movieService.GetAllMovies());

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(HttpStatusCode.OK, (HttpStatusCode)result
                .GetType()
                .GetProperty("StatusCode")
                .GetValue(result, null));

            Assert.IsNotNull(movieData);
            CollectionAssert.DoesNotContain(movieData, movieToDelete);
        }
        
    }
}
