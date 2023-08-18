using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBank.Classes
{
    /// <summary>
    /// Class that represents files and their properties that are linked to a movie in the database
    /// </summary>
    public class LinkedFile
    {
        public long Id { get; private set; }
        public FileType Type { get; set; }
        public OpenWith Open { get; set; }
        public string Path { get; set; }

        /// <summary>
        /// Constructor to create a LinkedFile-Object. Is not represented in the database unless saved.
        /// </summary>
        /// <param name="type">Type of file.</param>
        /// <param name="open">Method of opning the file.</param>
        /// <param name="path">Path to the file. Might be relative if database is correctly configured.</param>
        public LinkedFile(FileType type, OpenWith open, string path)
        {
            Type = type;
            Open = open;
            Path = path;
        }

        /// <summary>
        /// Constructor to create a LinkedFile-Object that is already represented in the database.
        /// </summary>
        /// <param name="id">Primary key in the database.</param>
        /// <param name="type">Type of file.</param>
        /// <param name="open">Method of opning the file.</param>
        /// <param name="path">Path to the file. Might be relative if database is correctly configured.</param>
        public LinkedFile(long id, FileType type, OpenWith open, string path)
        {
            Id = id;
            Type = type;
            Open = open;
            Path = path;
        }

        /// <summary>
        /// Updates the entry of this object in the database.
        /// </summary>
        /// <param name="db">Database to store information in</param>
        /// <param name="linkToMovie">Database-Id of movie to link this file to.</param>
        public void UpdateInDB(Database db, long linkToMovie)
        {
            if (Id == default(long))
            {
                // new entry --> insert into db
                string res = db.Insert("files", new Dictionary<string, string> {
                    {"Movie", linkToMovie.ToString()},
                    {"Type", ((int)Type).ToString()},
                    {"Open", ((int)Open).ToString()},
                    {"Path", Path}
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
                db.Update("files", "Id", Id.ToString(), new Dictionary<string, string> {
                    {"Movie", linkToMovie.ToString()},
                    {"Type", ((int)Type).ToString()},
                    {"Open", ((int)Open).ToString()},
                    {"Path", Path}
                });
            }
        }

        /// <summary>
        /// Removes the item from the database
        /// </summary>
        /// <param name="db">Database to remove information from</param>
        public void Delete(Database db)
        {
            if (Id != default(long)) // check that id is set --> means that item aready in db
            {
                db.Delete("files", "Id", Id.ToString());
            }
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
