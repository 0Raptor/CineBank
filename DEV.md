# CineBank Dev Manual

## Database Design

The following tables are created to store information.

- `movies`
- `files`
- `genres`
- `languages`

> The file-entries will delete themselfes if the connected movie-entry has been deleted (CASCADE).

The following tables are created to establish m:n-connection beetween the tables above.

> The foreign key constraints in these tables will delete the entry if the connected movie-entry was deleted (CASCADE), but block the deletion of an language-/genre-entry if it is connected to a movie-entry (RESTRICT).

- `movies2languages`
- `movies2genres`

> The conversion of the integer values in `movies.Type`, `files.Type` and `files.OpenWith` to text is handled by the enums in [Movie.cs](CineBank/Classes/Movie.cs).

```SQL
PRAGMA foreign_keys = off;
BEGIN TRANSACTION;

-- Table: files
CREATE TABLE IF NOT EXISTS files (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Movie INTEGER REFERENCES movies (Id) ON DELETE CASCADE NOT NULL, Type INTEGER NOT NULL, Open INTEGER NOT NULL, Path TEXT NOT NULL);

-- Table: genres
CREATE TABLE IF NOT EXISTS genres (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Name TEXT (25) UNIQUE NOT NULL);

-- Table: languages
CREATE TABLE IF NOT EXISTS languages (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Name TEXT (10) UNIQUE NOT NULL);

-- Table: movies
CREATE TABLE IF NOT EXISTS movies (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Title TEXT (255) UNIQUE NOT NULL, Description TEXT NOT NULL, Duration TEXT (10) NOT NULL, Type INTEGER NOT NULL, Released TEXT (10), Cast TEXT, Director TEXT, Score TEXT, MaxResolution TEXT (10), Age TEXT (10), Notes TEXT);

-- Table: movies2genres
CREATE TABLE IF NOT EXISTS movies2genres (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Genre INTEGER REFERENCES genres (Id) ON DELETE RESTRICT NOT NULL, Movie INTEGER REFERENCES movies (id) ON DELETE CASCADE NOT NULL);
CREATE UNIQUE INDEX UNQ_Genre ON movies2genres(Genre, Movie);

-- Table: movies2languages
CREATE TABLE IF NOT EXISTS movies2languages (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Language INTEGER REFERENCES languages (Id) ON DELETE RESTRICT NOT NULL, Movie INTEGER REFERENCES movies (id) ON DELETE CASCADE NOT NULL, Type TEXT (1) NOT NULL);
CREATE UNIQUE INDEX UNQ_Languages ON movies2languages(Language, Movie, Type);

-- Table: settings
CREATE TABLE IF NOT EXISTS settings (Id INTEGER PRIMARY KEY AUTOINCREMENT UNIQUE NOT NULL, Key TEXT (25) UNIQUE NOT NULL, Value TEXT NOT NULL);
INSERT INTO settings(Key, Value) VALUES('version', "1.0")
INSERT INTO settings(Key, Value) VALUES('baseDir', "")

COMMIT TRANSACTION;
PRAGMA foreign_keys = on;
```
