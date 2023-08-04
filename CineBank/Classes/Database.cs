using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CineBank
{
    public class Database
    {
        public DatabseConfig Config { get; private set; }
        public SQLiteConnection Connection { get; private set; }

        public Database() { }
        public Database(string path)
        {
            // check file path
            if (!File.Exists(path)) throw new Exception("ERROR: No DB: The supplied file does not exist.");

            // connect to db
            Connection = CreateConnection(path);
        }

        private SQLiteConnection CreateConnection(string path)
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection(String.Format("Data Source={0}; Version = 3; FailIfMissing=True; Compress = True;", path));
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw new Exception("ERROR: DB Connection failed: Failed to connect to supplied database.");
            }
            return sqlite_conn;
        }

        /// <summary>
        /// Create a new SQLite-Database at the given location
        /// </summary>
        /// <param name="path">Path to the file storing the SQLite-Database</param>
        /// <param name="conf">(optional) Supply a configuration to change the defaults</param>
        public void Init(string path, DatabseConfig? conf = null)
        {

        }
    }

    public class DatabseConfig
    {

    }
}
