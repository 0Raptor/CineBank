# CineBank

Open source local video management solution for Windows. Manage all your movies and series inside a central tool, store metadata and launch them in your preferred media player.  
Mounting your ISOs or playing any type of media file can be achieved through completely customizable PowerShell-scripts. To avoid that unauthorized users mess with your scripts, integrity checks can be enabled. 

Third implementation of its kind. Last try: [LocalCineBank](https://git.0raptor.earth/Raptor/LocalCineBank) (never published)

For information about the design of the application check [Developer Manual](DEV.md).

## Installation

1. Download latest release from [Missing]()
2. Extract software on your machine
3. Adapt powershell scripts
4. Run initial configuration
5. (optional) Create a desktop shortcut
6. Start applicaton

## Initial configuration

Next to the main executable there must be a file called `config.xml`. This is the primary configuration file.  
The file will be created automatically if you run the `UpdateConfiguration.ps1`-script during installation!

> After changing the configuration run `UpdateConfiguration.ps1`-script to update the checksum in the registry! Otherwise the changed config will be rejected as malicious.
> If you don not want to validate the scripts and config you can disable them through the script and configuration

```XML
<?xml version="1.0" encoding="utf-8"?>
<xml>
	<config>
		<validateChecksums>true</validateChecksums><!-- if true prgram will validate that config has not been changed using checksum in HKLM:\SOFTWARE\CineBank\ConfigCksm -->
		<baseDir></baseDir><!-- OPTIONAL: Specify a baseDir and store relative paths in the databse. This parameter overrides the baseDir specified in the database -->
		<dbPath></dbPath> <!-- OPTIONAL: Path to SQLite-database to load at startup. If not specified must be supplied via commandline parameter -->
	</config>
	<checksums>
		<!-- contains checksums of powershell skripts used by the program to play files - program will check their integrity during start -->
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

## License

This application is published undes GNU GNU GENERAL PUBLIC LICENSE Version 3 as refered in the LICENSE-File.

Copyright (C) 2023 0Raptor

This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see [www.gnu.org/licenses/](https://www.gnu.org/licenses/).