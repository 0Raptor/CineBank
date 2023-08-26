using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBank.Classes
{
    /// <summary>
    /// Class that represents the movies/ series stored in the database.
    /// </summary>
    public class Movie
    {
        public long Id { get; private set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverPath { get; set; }
        public string Genre { get; set; }
        public string Duration { get; set; } // screentime in h:mm:ss OR number of episodes
        public MovieType Type { get; set; }
        public string Released { get; set; }
        public string Cast { get; set; }
        public string Director { get; set; }
        public string Score { get; set; }
        public string Languages { get; set; }
        public string Subtitles { get; set; }
        public string AudioDescription { get; set; }
        public string MaxResolution { get; set; }
        public string Format { get; private set; }
        public string Age { get; set; }
        public string Notes { get; set; }
        public LinkedFile[] Files { get; set; }


        /// <summary>
        /// Constructor to create a new Movie-Object. This object is not represented in the database unless saved.
        /// </summary>
        public Movie()
        {
            Id = default(long);
            Title = "";
            Description = "";
            CoverPath = "";
            Genre = "";
            Duration = "";
            Type = MovieType.Movie;
            Released = "";
            Cast = "";
            Director = "";
            Score = "";
            Languages = "";
            Subtitles = "";
            AudioDescription = "";
            MaxResolution = "";
            Format = "";
            Age = "";
            Notes = "";
            Files = new LinkedFile[] { };
        }

        /// <summary>
        /// Constructor to create a Movie-Object that is already represented in the database. obj.SolveForeignKeys() should be called afterwards to get information that are linked via foreign keys.
        /// </summary>
        /// <param name="id">Database Id of the object. Required to search for foreign keys. -1 when crating a new object that is not yet stored in the database.</param>
        public Movie(long id)
        {
            Id = id;
            Title = "";
            Description = "";
            CoverPath = "";
            Genre = "";
            Duration = "";
            Type = MovieType.Movie;
            Released = "";
            Cast = "";
            Director = "";
            Score = "";
            Languages = "";
            Subtitles = "";
            AudioDescription = "";
            MaxResolution = "";
            Format = "";
            Age = "";
            Notes = "";
            Files = new LinkedFile[] {};
        }

        /// <summary>
        /// Constructor to create a new movie object and set all parameters. Private so it can only be used internal e.g. for deep copy.
        /// </summary>
        private Movie(long id, string title, string description, string coverPath, string genre, string duration, MovieType type,
            string released, string cast, string director, string score, string languages, string subtitles, string audioDescripton,
            string maxResolution, string format, string age, string notes, LinkedFile[] files)
        {
            Id = id;
            Title = title;
            Description = description;
            CoverPath = coverPath;
            Genre = genre;
            Duration = duration;
            Type = type;
            Released = released;
            Cast = cast;
            Director = director;
            Score = score;
            Languages = languages;
            Subtitles = subtitles;
            AudioDescription = audioDescripton;
            MaxResolution = maxResolution;
            Format = format;
            Age = age;
            Notes = notes;
            Files = files;
        }

        /// <summary>
        /// Update the string Format based on the file-objects in Files
        /// </summary>
        private void UpdateFormat()
        {
            string format = "";
            foreach (var file in Files)
            {
                if (file.Open == LinkedFile.OpenWith.None) // skip unopenable files
                    continue;

                switch (file.Type) // get the name of the format based on file type and method used to open it
                {
                    case LinkedFile.FileType.Image:
                        if (!format.Contains("Image File, ")) format += "Image File, ";
                        break;
                    case LinkedFile.FileType.Audio:
                        if (!format.Contains("Audio File, ")) format += "Audio File, ";
                        break;
                    case LinkedFile.FileType.ISO:
                        if (file.Open == LinkedFile.OpenWith.DVDPlayer)
                            if (!format.Contains("DVD ISO, ")) format += "DVD ISO, ";
                        if (file.Open == LinkedFile.OpenWith.BRPlayer)
                            if (!format.Contains("Blu-ray ISO, ")) format += "Blu-ray ISO, ";
                        else
                            if (!format.Contains("ISO, ")) format += "ISO, ";
                        break;
                    case LinkedFile.FileType.DVDFolder:
                        if (!format.Contains("DVD Folder, ")) format += "DVD Folder, ";
                        break;
                    case LinkedFile.FileType.BRFolder:
                        if (!format.Contains("Blu-ray Folder, ")) format += "Blu -ray Folder, ";
                        break;
                    case LinkedFile.FileType.AVCHDFolder:
                        if (!format.Contains("AVCHD Folder, ")) format += "AVCHD Folder, ";
                        break;
                    case LinkedFile.FileType.Video:
                        if (file.Open == LinkedFile.OpenWith.Video1)
                            if (!format.Contains("Video File (Typ 1), ")) format += "Video File (Typ 1), ";
                        if (file.Open == LinkedFile.OpenWith.Video2)
                            if (!format.Contains("Video File (Typ 2), ")) format += "Video File (Typ 2), ";
                        if (file.Open == LinkedFile.OpenWith.Undefined)
                            if (!format.Contains("Unknown Video File, ")) format += "Unknown Video File, ";
                        break;
                }
            }
            if (format.Length > 2) format = format.Substring(0, format.Length - 2);
            Format = format;
        }

        /// <summary>
        /// Create a DeepCopy of the current object and return it
        /// </summary>
        /// <returns>Return copy of the current object that can be modified without changing the original</returns>
        public Movie DeepCopy()
        {
            // copy linked files
            List<LinkedFile> files = new List<LinkedFile>();
            if (Files != null)
            {
                foreach (var file in Files)
                {
                    files.Add(new LinkedFile(file.Id, file.Type, file.Open, file.Path));
                }
            }

            // copy object
            Movie deepcopy = new Movie(Id, Title, Description, CoverPath, Genre, Duration, Type, Released, Cast, Director, Score,
                Languages, Subtitles, AudioDescription, MaxResolution, Format, Age, Notes, files.ToArray());

            // return
            return deepcopy;
        }

        /// <summary>
        /// Set all members to the values of a reference object.
        /// </summary>
        /// <param name="src">Reference object to copy the members values from</param>
        public void SetMembers(Movie src)
        {
            Id = src.Id;
            Title = src.Title;
            Description = src.Description;
            CoverPath = src.CoverPath;
            Genre = src.Genre;
            Duration = src.Duration;
            Type = src.Type;
            Released = src.Released;
            Cast = src.Cast;
            Director = src.Director;
            Score = src.Score;
            Languages = src.Languages;
            Subtitles = src.Subtitles;
            AudioDescription = src.AudioDescription;
            MaxResolution = src.MaxResolution;
            Format = src.Format;
            Age = src.Age;
            Notes = src.Notes;
            Files = src.Files;
        }

        /// <summary>
        /// Solve m:n foreign key to e.g. obtain linked files from other db tables.
        /// </summary>
        /// <param name="id">Primary key (databse) of the entry.</param>
        public void SolveForeignKeys(Database db)
        {
            // get genres
            string[][] res = db.Query("SELECT g.Name FROM genres g, movies m, movies2genres mg WHERE m.Id = " + Id + " AND m.Id = mg.Movie AND mg.Genre = g.Id;");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get genres from movie with ID " + Id);
            else
            {
                Genre = "";
                for (int i = 1; i < res.Length; i++) // add each genre name to the list
                {
                    Genre += res[i][0] + ", ";
                }
                Genre = Genre.Substring(0, Genre.Length - 2); // remove tailing ", "
            }

            // get languages
            res = db.Query("SELECT l.Name FROM languages l, movies m, movies2languages ml WHERE ml.Type = \"L\" AND m.Id = " + Id + " AND m.Id = ml.Movie AND ml.Language = l.Id;");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get spoken languages from movie with ID " + Id);
            else
            {
                Languages = "";
                for (int i = 1; i < res.Length; i++) // add each language name to the list
                {
                    Languages += res[i][0] + ", ";
                }
                Languages = Languages.Substring(0, Languages.Length - 2); // remove tailing ", "
            }

            // get subtitles
            res = db.Query("SELECT l.Name FROM languages l, movies m, movies2languages ml WHERE ml.Type = \"S\" AND m.Id = " + Id + " AND m.Id = ml.Movie AND ml.Language = l.Id;");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get subtitles from movie with ID " + Id);
            else
            {
                Subtitles = "";
                for (int i = 1; i < res.Length; i++) // add each language name to the list
                {
                    Subtitles += res[i][0] + ", ";
                }
                Subtitles = Subtitles.Substring(0, Subtitles.Length - 2); // remove tailing ", "
            }

            // get audio deskription
            res = db.Query("SELECT l.Name FROM languages l, movies m, movies2languages ml WHERE ml.Type = \"A\" AND m.Id = " + Id + " AND m.Id = ml.Movie AND ml.Language = l.Id;");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get audio deskrition languages from movie with ID " + Id);
            else
            {
                AudioDescription = "";
                for (int i = 1; i < res.Length; i++) // add each language name to the list
                {
                    AudioDescription += res[i][0] + ", ";
                }
                AudioDescription = AudioDescription.Substring(0, AudioDescription.Length - 2); // remove tailing ", "
            }

            // check if absolute path is used or obtain basedir
            string baseDir = ""; // empty string so can be safely added before each filepath even if not required
            if (!String.IsNullOrWhiteSpace(db.Config.BaseDir)) // if required value will be inserted here
                baseDir = db.Config.BaseDir;

            // get cover and set CoverPath
            res = db.Query("SELECT Path FROM files WHERE Movie = " + Id + " AND Open = " + (int)LinkedFile.OpenWith.None + ";");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get cover from movie with ID " + Id);
            else
            {
                if (res[1].Length == 1) // check column was found
                {
                    CoverPath = baseDir + res[1][0];
                }
            }

            // get media files and set Files
            res = db.Query("SELECT * FROM files WHERE Movie = " + Id + ";");
            if (res == null || res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get linked files from movie with ID " + Id);
            else
            {
                List<LinkedFile> files = new List<LinkedFile>();
                for (int i = 1; i < res.Length; i++) // add each linked file as new object to the list
                {
                    files.Add(new LinkedFile(Convert.ToInt64(res[i][0]), (LinkedFile.FileType)Convert.ToInt32(res[i][2]),
                        (LinkedFile.OpenWith)Convert.ToInt32(res[i][3]), res[i][4]));
                }
                Files = files.ToArray();
            }

            // set Format
            UpdateFormat();
        }

        /// <summary>
        /// Delete m:n-connections
        /// </summary>
        /// <param name="db">Database to delete the connection from</param>
        /// <param name="table">Select wich table to delete from (language, genre). When language is selected add (":L" for languagem, ":S" for subtitle and ":A" for audio description)</param>
        /// <param name="name">Name of the language or genre to delete</param>
        public void RemoveForeignKeys(Database db, string table, string name)
        {
            if (table.StartsWith("language"))
            {
                // delete from movies2languages
                string query = "DELETE FROM movies2languages WHERE Language = (SELECT Id FROM languages WHERE Name = @lang) AND Type = @type AND Movie = @id;";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", name.Trim() }, { "@type", table.Split(':')[1].Trim() }, { "@id", Id.ToString() }
                });
                db.Query(query);
            }
            else if (table.StartsWith("genre"))
            {
                // delete from movies2genres
                string query = "DELETE FROM movies2genres WHERE Genre = (SELECT Id FROM genres WHERE Name = @g) AND Movie = @id;";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@g", name.Trim() }, { "@id", Id.ToString() }
                });
                db.Query(query);
            }
        }

        /// <summary>
        /// Updates the entry of this object in the database.
        /// </summary>
        /// <param name="db">Database to store information in</param>
        public void UpdateInDB(Database db)
        {
            // update main entry
            if (Id == default(long))
            {
                // new entry --> insert into db
                string res = db.Insert("movies", new Dictionary<string, string> {
                    {"Title", Title},
                    {"Description", Description},
                    {"Duration", Duration},
                    {"Type", ((int)Type).ToString()},
                    {"Released", Released},
                    {"Cast", Cast},
                    {"Director", Director},
                    {"Score", Score},
                    {"MaxResolution", MaxResolution},
                    {"Age", Age},
                    {"Notes", Notes}
                });

                // store id of new object
                if (res != null)
                {
                    Id = Convert.ToInt64(res);
                }
            }
            else
            {
                // exisitng entry --> update
                db.Update("movies", "Id", Id.ToString(), new Dictionary<string, string> {
                    {"Title", Title},
                    {"Description", Description},
                    {"Duration", Duration},
                    {"Type", ((int)Type).ToString()},
                    {"Released", Released},
                    {"Cast", Cast},
                    {"Director", Director},
                    {"Score", Score},
                    {"MaxResolution", MaxResolution},
                    {"Age", Age},
                    {"Notes", Notes}
                });
            }

            // update foreign keys
            // languages
            string[] languges = Languages.Split(',');
            foreach (string lang in languges)
            {
                // skip empty entries
                if (String.IsNullOrWhiteSpace(lang))
                    continue;

                // make sure language is exisitng
                string query = "INSERT OR IGNORE INTO languages(Name) VALUES(@lang);";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }
                });
                db.Insert("languages", query);

                // add connection between language and movie
                query = "INSERT OR IGNORE INTO movies2languages(Language, Movie, Type) VALUES((SELECT Id FROM languages WHERE Name = @lang), @id, \"L\");";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }, { "@id", Id.ToString() }
                });
                db.Insert("movies2languages", query);
            }
            // subtitles
            string[] subtitles = Subtitles.Split(',');
            foreach (string lang in subtitles)
            {
                // skip empty entries
                if (String.IsNullOrWhiteSpace(lang))
                    continue;

                // make sure language is exisitng
                string query = "INSERT OR IGNORE INTO languages(Name) VALUES(@lang);";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }
                });
                db.Insert("languages", query);

                // add connection between language and movie
                query = "INSERT OR IGNORE INTO movies2languages(Language, Movie, Type) VALUES((SELECT Id FROM languages WHERE Name = @lang), @id, \"S\");";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }, { "@id", Id.ToString() }
                });
                db.Insert("movies2languages", query);
            }
            // audio description
            string[] audioDesc = AudioDescription.Split(',');
            foreach (string lang in audioDesc)
            {
                // skip empty entries
                if (String.IsNullOrWhiteSpace(lang))
                    continue;

                // make sure language is exisitng
                string query = "INSERT OR IGNORE INTO languages(Name) VALUES(@lang);";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }
                });
                db.Insert("languages", query);

                // add connection between language and movie
                query = "INSERT OR IGNORE INTO movies2languages(Language, Movie, Type) VALUES((SELECT Id FROM languages WHERE Name = @lang), @id, \"A\");";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@lang", lang.Trim() }, { "@id", Id.ToString() }
                });
                db.Insert("movies2languages", query);
            }
            // genres
            string[] genre = Genre.Split(',');
            foreach (string g in genre)
            {
                // skip empty entries
                if (String.IsNullOrWhiteSpace(g))
                    continue;

                // make sure genre is exisitng
                string query = "INSERT OR IGNORE INTO genres(Name) VALUES(@g);";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@g", g.Trim() }
                });
                db.Insert("genres", query);

                // add connection between genre and movie
                query = "INSERT OR IGNORE INTO movies2genres(Genre, Movie) VALUES((SELECT Id FROM genres WHERE Name = @g), @id);";
                query = db.PrepareSecureSQLStatement(query, new Dictionary<string, string>
                {
                    { "@g", g.Trim() }, { "@id", Id.ToString() }
                });
                db.Insert("movies2genres", query);
            }
            // files
            foreach (LinkedFile lf in Files)
            {
                lf.UpdateInDB(db, Id);
            }
            // format (based on files)
            UpdateFormat();
        }

        /// <summary>
        /// Removes the item from the database
        /// </summary>
        /// <param name="db">Database to remove information from</param>
        public void Delete(Database db)
        {
            if (Id != default(long)) // check that id is set --> means that item aready in db
            {
                // delete self from database --> constraints will automatically remove m:n-connections
                db.Delete("movies", "Id", Id.ToString());

                // delete all connected files
                foreach (LinkedFile lf in Files)
                    lf.Delete(db);
            }
        }

        public enum MovieType : ushort
        {
            Movie = 0,
            Series = 1
        }

        /// <summary>
        /// Reads all movie entries from the given database.
        /// </summary>
        /// <param name="db">Database-Object to qurey from</param>
        /// <param name="filter">(OPTIONAL) Search parameter to limit the results</param>
        /// <returns>A list of movie objects</returns>
        public static List<Movie> GetMovies(Database db, string filter = "")
        {
            List<Movie> movies = new List<Movie>();

            // prepare sql statemet
            string sql = "SELECT * FROM movies";
            if (!String.IsNullOrWhiteSpace(filter))
            {
                sql += " WHERE Title LIKE @filter";
                sql = db.PrepareSecureSQLStatement(sql, new Dictionary<string, string>() { { "@filter", "%" + filter + "%" } });
            }

            // get data and check results
            string[][] res = db.Query(sql + ";");
            if (res == null || res.Length <= 1)
            {
                Console.WriteLine("INFO: No movies where found.");
                return movies;
            }

            // loop over all results skipping the column names
            for (int i = 1; i < res.Length; i++)
            {
                // create new movie object (FKs will be resolved by the constructor)
                Movie tmp = new Movie(Convert.ToInt64(res[i][0]));

                // add data (which does not require FKs)
                tmp.Title = res[i][1];
                tmp.Description = res[i][2];
                tmp.Duration = res[i][3];
                tmp.Type = (MovieType)Convert.ToInt32(res[i][4]);
                tmp.Released = res[i][5];
                tmp.Cast = res[i][6];
                tmp.Director = res[i][7];
                tmp.Score = res[i][8];
                tmp.MaxResolution = res[i][9];
                tmp.Age = res[i][10];
                tmp.Notes = res[i][11];

                // resolve foreign keys
                tmp.SolveForeignKeys(db);

                // add to list
                movies.Add(tmp);
            }

            return movies;
        }

        #region override operators
        // https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
        public override bool Equals(object obj) => this.Equals(obj as Movie);

        public bool Equals(Movie o)
        {
            if (o is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, o))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != o.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.

            // compare elements in Files-Array
            bool filesMatch = true;
            if (Files.Length != o.Files.Length)
            {
                filesMatch = false;
            }
            else
            {
                for (int i = 0; i < Files.Length; i++)
                {
                    if (Files[i].Id != o.Files[i].Id || Files[i].Type != o.Files[i].Type ||
                        Files[i].Open != o.Files[i].Open || Files[i].Path != o.Files[i].Path)
                    {
                        filesMatch = false;
                        break;
                    }
                }
            }

            // compare other variables and return
            return (Title == o.Title) && (Description == o.Description) && (Description == o.Description) &&
                CoverPath == o.CoverPath && Genre == o.Genre && Duration == o.Duration && Type == o.Type &&
                Released == o.Released && Cast == o.Cast && Director == o.Director && Score == o.Score &&
                Languages == o.Languages && Subtitles == o.Subtitles && AudioDescription == o.AudioDescription &&
                MaxResolution == o.MaxResolution && Format == o.Format && Age == o.Age && Notes == o.Notes &&
                filesMatch;
        }

        public static bool operator ==(Movie lhs, Movie rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            // And compares content
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Movie lhs, Movie rhs) => !(lhs == rhs);
        #endregion
    }
}
