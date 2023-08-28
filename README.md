# CineBank

Open source local video management solution for Windows.
Manage all your movies and series inside a central tool, store metadata, and launch them in your preferred media player.

The execution is realized through customizable PowerShell scripts to grant you full control over how the files are going to be opened.
Mounting your ISOs or playing any type of media file with any software can be achieved easily.
To avoid unauthorized users messing with your scripts, integrity checks can be enabled.

This tool will give you full control over what happens to your information. All information about your movies is stored locally
on your device (except if you decide to store the database anywhere else). No information will be sent to external services.  
To obtain additional information for your entries, you can optionally use the API from [TMDB](https://www.themoviedb.org/). But this feature is
disabled until you supply an API key. Even then, the application will never fetch information automatically, but while adding or editing an entry you can
search for information using a search bar and a manual button click.

For information about the design of the application, check the [Developer Manual](DEV.md).

## Installation

1. Download the latest release from [GitHub](https://github.com/0Raptor/CineBank/releases)
2. Extract software on your machine
3. Adapt PowerShell scripts in "Scripts"-Subfolder

   > This step is only necessary if you use other media players or the players are installed under different paths on your system than listed in the table below.

   The scripts will be called from the application when a file should be played. Therefore, based on the available options you can assign each imported file to, you can configure five different methods to open files.  
   The scripts are preconfigured with the following configuration:

   | Filename             | Mediatype                         | Player                                                             |  Expected path                                                            |
   |----------------------|-----------------------------------|--------------------------------------------------------------------|---------------------------------------------------------------------------|
   | `VideoPlayer1.ps1`   | Videos: MP4, WMV                  | Windows Default                                                    | n/A \*                                                                    |
   | `VideoPlayer2.ps1`   | Videos: Almost everything         | [VLC media player](https://www.videolan.org/vlc/)                  | `C:\Program Files\VideoLAN\VLC\vlc.exe`                                   |
   | `AudioPlayer.ps1`    | Music                             | [VLC media player](https://www.videolan.org/vlc/)                  | `C:\Program Files\VideoLAN\VLC\vlc.exe`                                   |
   | `DVDPlayer.ps1`      | DVD-ISOs, DVD-Folders (VIDEO_TS)  | [VLC media player](https://www.videolan.org/vlc/)                  | `C:\Program Files\VideoLAN\VLC\vlc.exe`                                   |
   | `BRPlayer.ps1`       | BR-ISOs, BR-Folders (BDMV)        | [LEAWO Blu-ray Player](https://www.leawo.org/de/blu-ray-player/)   | `"C:\Program Files (x86)\Leawo\Blu-ray Player\Leawo Blu-ray Player.exe`   |

   *The players are suggestions and can be changed against any player you prefer. I do not get any compensation for suggesting those players. VLC is an open-source project, LEAWO Blue-Rry Player freeware.*  
   \* Since the new Windows media players ignore command line arguments the script is just starting the media file as a process: The system's default media player will be used.

   To change the scripts you can:

   - Change the path where the player is expected if you use the same player but it is installed at a different location:

      Add the path to the main executable on your system inside the quotes at `$mediaplayer = ""`

   - Use another media player

      Most applications/ players will open the file that is supplied as the first command line argument. In this case, you just have to add the path to the main executable in the `$mediaplayer`-parameter as shown above.  
      To validate your mediaplayer behaves this way hit `Win+R` type `cmd` and hit enter. In the console window enter the path of the media player-executable a space and the path to a media file (e. g. `"C:\Program Files\VideoLAN\VLC\vlc.exe" "E:\Videos\MLG.mp4"`). Then hit enter again. The player should open and play the file.

      If your player does not behave this way you have to change line `Start-Process` at the end of each script. Check the documentation of your player to find out how it can be opened via the command line.

   - Change the whole logic

      The benefit of the scripts is that you can perform any operation on the system to play your file. It is encrypted? Implement a dialog to ask for the password. It is stored inside a disk image? Mount it before opening the media player (as done in `DVDPlayer.ps1`).

      The only limitation is, that your script must start with the following lines to get the selected file from **CineBank**:

      ```PowerShell
      param(
          [Parameter(Mandatory)]
          [String]$path,
          [Parameter()]
          [String]$dir
      )
      ```

4. Run initial configuration

   After the application is extracted and the scripts have been adapted to your setup you have to configure **CineBank**. Use the script `UpdateConfiguration.ps1` in the root of the extracted directory (from step 2).

   If you want to use the implemented security features that will warn you if somebody tempered with your config or scripts (from step 3) you need an administrative PowerShell. Otherwise the unelevated will be sufficient.  
   In the root of your extracted folder (from step 2) click inside the box with the folder path. Type: `powershell` - this will open an unelevated PowerShell window.  
   In the PowerShell window enter `Start-Process powershell -Verb runas -ArgumentList "-NoExit -c cd '$pwd'"` and confirm to open a new elevated window.

   In the elevated windows type `.\UpdateConfiguration.ps1` and hit enter. This will start the setup process.

   If you really want to skip the script validation (NOT RECOMMENDED), you can execute `.\UpdateConfiguration.ps1` in the unelevated window and ignore the warning.

   Just follow the instructions displayed and answer the questions.

5. (optional) Create a desktop shortcut and/or add command line arguments

   If you want to be able to execute **CineBank** from your desktop, right-click on `CineBank.exe` and select `Copy` (Or select and hit `Ctrl+C`).  
   Navigate to your desktop, right-click, and select `Paste Shortcut`.

   If you want to supply the path to the database or *basedir* directly to the executable without using the configuration you can add command line-argumnts (as described in step 6). To add them to your shortcut, right-click on the shortcut and select properties (or select and hit `Alt+Enter`).

   In the section `Shortcut` (should be opened by default) click in the textbox `Target` and use the arrow keys to get to the end. After the path to your `CineBank.exe` you can add two parameters (keep in mind to surround paths with quotes if they contain spaces):  
   e. g. "Target: `"C:\Users\User\CineBank\CineBank.exe" "C:\Users\User\Videos\cinebank.db" ""C:\Users\User\Videos\"`"  
   The first parameter specifies the database file and the second is the directory that contains all your files. If the second parameter is supplied all files will be saved with paths relative to this directory instead of their absolute path.

6. Start application

   Use your created shortcut or the EXE to open the application.  
   When a command line is used you can supply command line arguments

   | arg[0]                                | arg[1]                                           | arg[2]                                                     |
   |---------------------------------------|--------------------------------------------------|------------------------------------------------------------|
   | Path to the **CineBank**-executable   | Path of the database file storing your library   | BaseDir/ Root-Directory when relative paths should be used |
   | `C:\Users\User\CineBank\CineBank.exe` | `C:\Users\User\Videos\cinebank.db`               | `C:\Users\User\Videos\`                                    |

   The paths are just examples. arg[0] is always the path to the EXE. You can specify the first argument without adding the second but NOT vice versa. If files with absolute or relative paths are already added you CANNOT switch (add or remove the arg) the mode.  
   Both arguments can be set in the config, so they must not be supplied at each start.  
   The second argument can also be configured in the database.

## Configuration

Next to the main executable, there must be a file called `config.xml`. This is the primary configuration file.  
The file will be created automatically if you run the `UpdateConfiguration.ps1`-script (as described [above](#installation) in step 4) during installation!

> After changing the configuration or scripts run `UpdateConfiguration.ps1`-script to update the checksum in the registry! Otherwise, the changed config will be rejected as malicious.
> If you do not want to validate the scripts and config you can disable them through the script and configuration

```XML
<?xml version="1.0" encoding="utf-8"?>
<xml>
 <config>
  <validateChecksums>true</validateChecksums><!-- if true prgram will validate that config has not been changed using checksum in HKLM:\SOFTWARE\CineBank\ConfigCksm -->
  <baseDir></baseDir><!-- OPTIONAL: Specify a baseDir and store relative paths in the database. This parameter overrides the baseDir specified in the database -->
  <dbPath></dbPath> <!-- OPTIONAL: Path to SQLite-database to load at startup. If not specified must be supplied via command line parameter -->
  <tmdbApiKey></tmdbApiKey> <!-- OPTIONAL: API-Key for https://www.themoviedb.org/ -->
 </config>
 <checksums>
  <!-- contains checksums of powershell scripts used by the program to play files - the program will check their integrity during start -->
  <!-- as the checksum of the config itself will be validated these checksums could not have been modified without administrative rights on your system -->
  <Video1></Video1> <!-- each node contains one checksum - auto generated! -->
  <Video2></Video2>
  <DVDPlayer></DVDPlayer>
  <BRPlayer></BRPlayer>
  <AudioPlayer></AudioPlayer>
  <Setup></Setup>
 </checksums>
</xml>
```

## Roadmap

Ideas that might be implemented in future releases:

1. Implement filtering for genre, languages, etc. during search
2. Implement additional DBMS by turning the Database class into a virtual class and creating derived classes for each DBMS.
3. Store information about people (Cast, Director, Score) in their own table.
4. Dark mode

## License

This application is published under GNU GENERAL PUBLIC LICENSE Version 3 as referred in the LICENSE-File.

Copyright (C) 2023 0Raptor

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see [www.gnu.org/licenses/](https://www.gnu.org/licenses/).
