using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieApp.Models;

namespace MovieApp.Tests
{
    public static class AssertHelper
    {
        public static void AssertMovies(IReadOnlyList<Movie> expected, IReadOnlyList<Movie> actual)
        {

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Title, actual[i].Title);
                Assert.AreEqual(expected[i].Duration, actual[i].Duration);
                Assert.AreEqual(expected[i].Genre, actual[i].Genre);
                Assert.AreEqual(expected[i].Language, actual[i].Language);
                Assert.AreEqual(expected[i].MovieId, actual[i].MovieId);
                Assert.AreEqual(expected[i].Overview, actual[i].Overview);
                Assert.AreEqual(expected[i].Rating, actual[i].Rating);
            }
        }
        public static void AssertGenre(IReadOnlyList<Genre> expected, IReadOnlyList<Genre> actual)
        {

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].GenreId, actual[i].GenreId);
                Assert.AreEqual(expected[i].GenreName, actual[i].GenreName);
            }
        }
    }
}
