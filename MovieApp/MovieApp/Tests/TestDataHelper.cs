using MovieApp.Models;

namespace MovieApp.Tests
{
    public static class TestDataHelper
    {
        public static List<Movie> GetMockMoviesData()
        {
            return new List<Movie>
            {
                new Movie
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
                },
                new Movie
                {
                    MovieId = 2028,
                    Title = "Violent Night",
                    Overview =
                        "When a team of mercenaries breaks into a wealthy family compound on Christmas Eve, taking everyone inside hostage, the team isn’t prepared for a surprise combatant: Santa Claus is on the grounds, and he’s about to show why this Nick is no saint.",
                    Genre = "Action",
                    Language = "English",
                    Duration = 112,
                    Rating = 7.7m,
                    PosterPath = "193348a7-e783-4ea0-afe6-c89c75025155Violent Night.jpg"
                },
                new Movie
                {
                    MovieId = 2029,
                    Title = "Puss in Boots: The Last Wish",
                    Overview =
                        "Puss in Boots discovers that his passion for adventure has taken its toll: He has burned through eight of his nine lives, leaving him with only one life left. Puss sets out on an epic journey to find the mythical Last Wish and restore his nine lives.",
                    Genre = "Animation",
                    Language = "English",
                    Duration = 102,
                    Rating = 8.0m,
                    PosterPath = "e07ebec3-3773-4272-bf2a-2d2eea6c7407Puss in Boots.jpg"
                }
            };
        }
        public static List<Movie> GetMockSimilarMoviesData()
        {
            return new List<Movie>
            {
                new Movie
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
                },
                new Movie
                {
                    MovieId = 2028,
                    Title = "Violent Night",
                    Overview =
                        "When a team of mercenaries breaks into a wealthy family compound on Christmas Eve, taking everyone inside hostage, the team isn’t prepared for a surprise combatant: Santa Claus is on the grounds, and he’s about to show why this Nick is no saint.",
                    Genre = "Science Fiction",
                    Language = "English",
                    Duration = 112,
                    Rating = 7.7m,
                    PosterPath = "193348a7-e783-4ea0-afe6-c89c75025155Violent Night.jpg"
                },
                new Movie
                {
                    MovieId = 2029,
                    Title = "Puss in Boots: The Last Wish",
                    Overview =
                        "Puss in Boots discovers that his passion for adventure has taken its toll: He has burned through eight of his nine lives, leaving him with only one life left. Puss sets out on an epic journey to find the mythical Last Wish and restore his nine lives.",
                    Genre = "Animation",
                    Language = "English",
                    Duration = 102,
                    Rating = 8.0m,
                    PosterPath = "e07ebec3-3773-4272-bf2a-2d2eea6c7407Puss in Boots.jpg"
                }
            };
        }
        public static List<Genre> GetMockGenreData()
        {
            return new List<Genre>
            {
                new Genre { GenreId = 1, GenreName = "Action" },
                new Genre { GenreId = 2, GenreName = "Animation" },
                new Genre { GenreId = 3, GenreName = "Comedy" },
                new Genre { GenreId = 4, GenreName = "Drama" },
                new Genre { GenreId = 5, GenreName = "Mystery" },
                new Genre { GenreId = 6, GenreName = "Science Fiction" }
            };
        }
        public static List<UserMaster> GetMockUserData()
        {
            return new List<UserMaster>
            {
                new UserMaster
                {
                    UserId = 1,
                    FirstName = "TestName",
                    Gender = "Male",
                    LastName = "TestLastName",
                    Password = "TestPassword1",
                    UserTypeName = "Admin",
                    Username = "TestUser"
                }
            };
        }
        public static List<Watchlist> GetMockWatchlistData()
        {
            return new List<Watchlist>
            {
                new Watchlist
                {
                    WatchlistId = "1",
                    DateCreated = DateTime.Now,
                    UserId = 1
                }
            };
        }
        public static List<WatchlistItem> GetMockWatchlistItemsData()
        {
            return new List<WatchlistItem>
            {
                new WatchlistItem
                {
                    WatchlistId = "1",
                    MovieId = 2027,
                    WatchlistItemId = 1
                },
                new WatchlistItem
                {
                    WatchlistId = "1",
                    MovieId = 2028,
                    WatchlistItemId = 2
                }
            };
        }
    }
}
