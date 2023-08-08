using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBank
{
    /// <summary>
    /// Class that represents the movies/ series stored in the database.
    /// </summary>
    public class Movie
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CoverPath { get; private set; }
        public string Genre { get; private set; }
        public string Duration { get; set; } // screentime in h:mm:ss OR number of episodes
        public MovieType Type { get; set; }
        public string Released { get; set; }
        public string Cast { get; set; }
        public string Director { get; set; }
        public string Score { get; set; }
        public string Languages { get; private set; }
        public string Subtitles { get; private set; }
        public string AudioDescription { get; private set; }
        public string MaxResolution { get; set; }
        public string Format { get; private set; }
        public LinkedFile[] Files { get; private set; }

        /// <summary>
        /// Solve m:n foreign key to e.g. obtain linked files from other db tables.
        /// </summary>
        /// <param name="id">Primary key (databse) of the entry.</param>
        private void SolveForeignKeys(long id)
        {
            // get genres

            // get languages, subtitles, etc.

            // check if absolute path is used or obtain basedir

            // get cover and set CoverPath

            // get media files and set Files

            // set Format
        }

        /// <summary>
        /// Constructor to create a new Movie-Object. Will resolve connections to other tables.
        /// </summary>
        /// <param name="id">Database Id of the object. Required to search for foreign keys.</param>
        public Movie(long id)
        {
            Id = id;

            SolveForeignKeys(id);
        }

        /// <summary>
        /// Updates the entry of this object in the database.
        /// </summary>
        public void UpdateInDB()
        {

        }

        /// <summary>
        /// Removes the item from the database
        /// </summary>
        public void Delete()
        {

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
                // TODO extract filter options
                throw new NotImplementedException("Usings filters to search for special movies is not implemented yet.");
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

                // add to list
                movies.Add(tmp);
            }

            return movies;
        }
    }

    /// <summary>
    /// Class that represents files and their properties that are linked to a movie in the database
    /// </summary>
    public class LinkedFile
    {
        public long Id { get; set; }
        public FileType Type { get; set; }
        public OpenWith Open { get; set; }
        public string Path { get; set; }

        /// <summary>
        /// Names of different types the file can be. May be required to play file
        /// </summary>
        public enum FileType : ushort
        {
            Generic = 0,
            Image = 1,
            Audio = 2,
            Video = 3,
            ISO = 4,
            DVDFolder = 5,
            AVCHDFolder = 6,
            BRFolder = 7
        }

        /// <summary>
        /// Names of tools the file can be opened with. Required to play file.
        /// </summary>
        public enum OpenWith : ushort
        {
            Undefined = 0,
            None = 1,
            Video1 = 2,
            Video2 = 3,
            DVDPlayer = 4,
            BRPlayer = 5,
            AudioPlayer = 6
        }
    }
}
