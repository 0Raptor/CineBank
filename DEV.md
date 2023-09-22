# CineBank Dev Manual

- [CineBank Dev Manual](#cinebank-dev-manual)
  - [Database Design](#database-design)
  - [Classes and their Objects](#classes-and-their-objects)
    - [Database](#database)
    - [Movie](#movie)
    - [LinkedFiles](#linkedfiles)
  - [Windows and Dialogs](#windows-and-dialogs)
    - [MainWindow](#mainwindow)
    - [SettingsWindow](#settingswindow)
    - [EntryWindow](#entrywindow)
    - [AddFielsDialog](#addfielsdialog)
    - [CreateDbDialog](#createdbdialog)
  - [PowerShell Scripts](#powershell-scripts)

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

## Classes and their Objects

### Database

Object that stores the database connection and should be used to perform all operations on the database.

> Each operation that interacts with the database should be implemented as a function inside the database class. This enables the simple implementation of other DBMSs through turning the current class into an abstract class and deriving from it for each DBMS.

### Movie

Objects that store the information about movies in the database during the execution of the application. Each entry in the database will be turned into a movie object when loaded.  
The objects have a function to be stored/ updated in the database to store them persistently.

> Currently, the m:n-connections in the database are represented as strings. In further releases, languages and actors should be implemented as their own classes, like the files.

### LinkedFiles

Internal representation of the files registred in the database. They always belong to a movie object and can be played (or shown in the case of a thumbnail).

## Windows and Dialogs

### MainWindow

Window that opens after executing the application.

- Displays a list of all movies (that can be filtered).
- Toolbar to manage the configuration, movies, ...
- Hotkeys to perform important actions
  - e.g. F5 to reload displayed movies (from database)
  - e.g. DEL to remove selected movie (from ui and database)
- Detailed information about a selected movie with option to playback connected files

### SettingsWindow

Can be accessed through the toolbar and a hotkey (F6).

Manage the behavior of the application. Stores them in the database.

Some settings have to be set in `config.xml`. Some settings can be overridden through commandline arguments. Compare with the user manual.

### EntryWindow

Can be accessed throgh the toolbar and hotkeys (F1 for new entry, F2 to edit the selected one).

Enter all information for the movie or obtain them from the TMDB-API. Furthermore, add files and link them.

### AddFielsDialog

Dialog to add files or a single folder.

The mediatype of the added element(s) should be defined. Files that are added together must have the same mediatype.

### CreateDbDialog

Dialog to create a new database. Will be started if no database is connected and the application is started.

## PowerShell Scripts

The PowerShell Scripts are used to execute the movie files that are stored in the database. Therefore, the path of the file (and the path of the folder containing the file) that was selected to be played will be supplied to the script.

There are default ones supplied. Each user may adapt them to their special needs (other software, different paths, ...)
