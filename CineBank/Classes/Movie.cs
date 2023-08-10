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
        public long Id { get; private set; }
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
        /// Constructor to create a new Movie-Object. Will resolve connections to other tables.
        /// </summary>
        /// <param name="id">Database Id of the object. Required to search for foreign keys. -1 when crating a new object that is not yet stored in the database.</param>
        public Movie(long id)
        {
            Id = id;
        }

        /// <summary>
        /// Solve m:n foreign key to e.g. obtain linked files from other db tables.
        /// </summary>
        /// <param name="id">Primary key (databse) of the entry.</param>
        public void SolveForeignKeys(Database db)
        {
            // get genres
            string[][] res = db.Query("SELECT g.Name FROM genres g, movies m, movies2genres mg WHERE m.Id = " + Id + " AND m.Id = mg.Movie AND mg.Genre = g.Id;");
            if (res.Length < 2) // validate items where found
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
            if (res.Length < 2) // validate items where found
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
            if (res.Length < 2) // validate items where found
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
            if (res.Length < 2) // validate items where found
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
            res = db.Query("SELECT Path FROM files WHERE Id = " + Id + ";");
            if (res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get cover from movie with ID " + Id);
            else
            {
                if (res[1].Length == 1) // check column was found
                {
                    CoverPath = baseDir + res[1][0];
                }
            }

            // get media files and set Files
            res = db.Query("SELECT * FROM files WHERE Movie = " + Id + " AND Open != " + (int)LinkedFile.OpenWith.None + ";");
            if (res.Length < 2) // validate items where found
                Console.WriteLine("WARNING: Movie: Failed to get linked files from movie with ID " + Id);
            else
            {
                List<LinkedFile> files = new List<LinkedFile>();
                for (int i = 1; i < res.Length; i++) // add each linked file as new object to the list
                {
                    files.Add(new LinkedFile(Convert.ToInt64(res[i][0]), (LinkedFile.FileType)Convert.ToInt32(res[i][2]),
                        (LinkedFile.OpenWith)Convert.ToInt32(res[i][3]), baseDir + res[i][4]));
                }
                Files = files.ToArray();
            }

            // set Format
            foreach (var file in Files)
            {
                string format = "";
                switch (file.Type) // get the name of the format based on file type and method used to open it
                {
                    case LinkedFile.FileType.Image:
                        format += "Image File, ";
                        break;
                    case LinkedFile.FileType.Audio:
                        format += "Audio File, ";
                        break;
                    case LinkedFile.FileType.ISO:
                        if (file.Open == LinkedFile.OpenWith.DVDPlayer)
                            format += "DVD ISO, ";
                        else if (file.Open == LinkedFile.OpenWith.BRPlayer)
                            format += "Blu-ray ISO, ";
                        else
                            format += "ISO, ";
                        break;
                    case LinkedFile.FileType.DVDFolder:
                        format += "DVD Folder, ";
                        break;
                    case LinkedFile.FileType.BRFolder:
                        format += "Blu-ray Folder, ";
                        break;
                    case LinkedFile.FileType.AVCHDFolder:
                        format += "AVCHD Folder, ";
                        break;
                    case LinkedFile.FileType.Video:
                        if (file.Open == LinkedFile.OpenWith.Video1)
                            format += "Video File (Typ 1), ";
                        else if (file.Open == LinkedFile.OpenWith.Video2)
                            format += "Video File (Typ 2), ";
                        else if (file.Open == LinkedFile.OpenWith.Undefined)
                            format += "Unknown Video File, ";
                        break;
                }
                format = format.Substring(0, format.Length - 2);
                Format = format;
            }
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

                // resolve foreign keys
                tmp.SolveForeignKeys(db);

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

        public LinkedFile(long id, FileType type, OpenWith open, string path)
        {
            Id = id;
            Type = type;
            Open = open;
            Path = path;
        }

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
