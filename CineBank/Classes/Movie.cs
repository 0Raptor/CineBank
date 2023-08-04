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

        public enum MovieType : ushort
        {
            Movie = 0,
            Series = 1
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
            Video = 2,
            ISO = 3,
            DVDFolder = 4,
            AVCHDFolder = 5,
            BRFolder = 6
        }

        /// <summary>
        /// Names of tools the file can be opened with. Required to play file.
        /// </summary>
        public enum OpenWith : ushort
        {
            Undefined = 0,
            None = 1,
            MP4Player = 2,
            MKVPlayer = 3,
            DVDPlayer = 4,
            BRPlayer = 5
        }
    }
}
