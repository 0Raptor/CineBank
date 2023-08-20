# CineBank Config Editor

# FUNCTIONS
$files = {
    { "Video1", "$PSScriptRoot\Scripts\VideoPlayer1.ps1", "" },
    { "Video2", "$PSScriptRoot\Scripts\VideoPlayer2.ps1", "" },
    { "DVDPlayer", "$PSScriptRoot\Scripts\DVDPlayer.ps1", "" },
    { "BRPlayer", "$PSScriptRoot\Scripts\BRPlayer.ps1", "" },
    { "AudioPlayer", "$PSScriptRoot\Scripts\AudioPlayer.ps1", "" },
    { "Setup", "$PSScriptRoot\UpdateConfiguration.ps1", "" }
};
function Create-Config {
    Write-Host "Creating new config..."
    $confPath = "$PSScriptRoot\config.xml"

    # create new file
    $xml = New-Object System.Xml.XmlTextWriter($confPath, $null) # create new xml file
    $xml.Formatting = "Indented" # format xml so a human can read it more easily
    $xml.Indentation = 1
    $xml.IndentChar = "`t"

    # add content
    $xml.WriteStartDocument()

    $xml.WriteStartElement("xml") # xml

    $xml.WriteStartElement("config") # config

    $xml.WriteElementString("validateChecksums", "true")
    $xml.WriteElementString("baseDir", "")
    $xml.WriteElementString("dbPath", "")
    $xml.WriteElementString("tmdbApiKey", "")

    $xml.WriteEndElement() # /config

    $xml.WriteStartElement("checksums") # checksums

    foreach ($item in $files) {
        $xml.WriteElementString($item[0], "")
    }

    $xml.WriteEndElement() # /checksums

    $xml.WriteEndElement() # /xml

    $xml.WriteEndDocument()

    # save
    $xml.Flush()
    $xml.Close()
    Write-Host "Successfully created config."
}
function Update-Config {
    Write-Host "Conduct a poll to set configuration..."
    $confPath = "$PSScriptRoot\config.xml"

    # open config
    [xml]$xml = Get-Content $confPath

    # check if already configured
    $oldCheckCksm = $xml.SelectSingleNode("xml/config/validateChecksums").InnerText
    $oldBaseDir = $xml.SelectSingleNode("xml/config/baseDir").InnerText
    $oldDbPath = $xml.SelectSingleNode("xml/config/dbPath").InnerText
    $oldApikey = $xml.SelectSingleNode("xml/config/tmdbApiKey").InnerText
    if (-not [String]::IsNullOrWhiteSpace($oldCheckCksm) -or -not [String]::IsNullOrWhiteSpace($oldBaseDir) -or -not [String]::IsNullOrWhiteSpace($oldCheckCksm) -or -not [String]::IsNullOrWhiteSpace($oldCheckCksm)) {
        Write-Host "INFO: Found an exisitng configuration that is displayed below. If you do not want to change a option, enter a underscore (_) in the promt!"
        Write-Host "  1: $oldCheckCksm"
        Write-Host "  2: $oldBaseDir"
        Write-Host "  3: $oldDbPath"
        Write-Host "  4: $oldApikey"
    }

    # prompt user for configuration
    Write-Host "[1/4]: Checksum Validation"
    Write-Host "The checksum-validation will ensure that your execution script and config have not been changed since the last execution of this function. Only users with administrative privileges are able to update the checksums and therefore change your files without triggering a warning. This feature is strongly recommended to prevent unauthorized people from inserting malicious code into your execution scripts!"
    $checkCksm = Read-Host -Prompt "Enable validation? [Y/n]"
    if ($checkCksm -eq "_" -and -not [String]::IsNullOrWhiteSpace($oldCheckCksm)) { $checkCksm = $oldCheckCksm}

    Write-Host "[2/4]: Base Directory"
    Write-Host "The baseDir is the folder containing all media files you want to manage. The paths to the media files will be stored relative to the baseDir. If used, all media files must be INSIDE the baseDir. This enables you to move the files to a different location and only change one variable as long as the files' position inside the baseDir stays the same. This value can also be defined inside the database or be delivered as a commandline argument on startup. If you prefer one of these methods or want to store files with their absolute path, just press enter without entering a character."
    $baseDir = ""
    while (1) {
        $baseDir = Read-Host -Prompt "Path to baseDir"
        # validate input
        if ([String]::IsNullOrWhiteSpace($checkCksm)) { break } # leaft empty
        elseif ((Get-Item $baseDir) -is [System.IO.DirectoryInfo]) { break } # entered a valid directory
        elseif ($baseDir -eq "_" -and -not [String]::IsNullOrWhiteSpace($oldBaseDir)) { $baseDir = $oldBaseDir; break } # use old value
        else { Write-Host "Entered path is invalid/ not existing. Please leave empty or enter a valid and existing path." } 
    }

    Write-Host "[3/4]: Database location"
    Write-Host "Location where the sqlite database containing your data will be stored. This value can also be supplied as a commandline argument. Normally, a sqlite database has the file-ending '.db'"
    $dbPath = ""
    while (1) {
        $dbPath = Read-Host -Prompt "Path to Database"
        # validate input
        if ([String]::IsNullOrWhiteSpace($dbPath)) { break } # leaft empty
        elseif ((Get-Item (Split-Path $dbPath)) -is [System.IO.DirectoryInfo]) { break } # entered a valid directory
        elseif ($dbPath -eq "_" -and -not [String]::IsNullOrWhiteSpace($oldDbPath)) { $dbPath = $oldDbPath; break } # use old value
        else { Write-Host "Entered path is invalid (checked for existing of parent directory). Please leave empty or enter a valid filename in an existing directory." } 
    }

    Write-Host "[4/4]: 'The Movie Database' (TMDB) API Key"
    Write-Host "This application can automatically import metadata of your movies from TMDB. To use this service you have to obtain an API-key as described in 'https://developer.themoviedb.org/docs' and enter it here. If left empty, the API-service will be disabled."
    $apikey = Read-Host -Prompt "API-Key"
    if ($apikey -eq "_" -and -not [String]::IsNullOrWhiteSpace($oldApikey)) { $apikey = $oldApikey}

    # store input
    if ($checkCksm.ToLower() -eq "n") { $xml.SelectSingleNode("xml/config/validateChecksums").InnerText = "false" } # 1
    else { $xml.SelectSingleNode("xml/config/validateChecksums").InnerText = "true" }
    $xml.SelectSingleNode("xml/config/baseDir").InnerText = $baseDir # 2
    $xml.SelectSingleNode("xml/config/dbPath").InnerText = $dbPath # 3
    $xml.SelectSingleNode("xml/config/tmdbApiKey").InnerText = $apikey # 4

    # save config
    $xml.Save($confPath)
    Write-Host "Configuration updated."

    # update checksums if user enabled feature
    if ($checkCksm.ToLower() -ne "n") {
        Update-ConfigCksm
    }
}
function Update-ConfigCksm {
    Write-Host "Updating checksums..."
    $confPath = "$PSScriptRoot\config.xml"
    if ($isAdmin) {
        # get checksum of all scripts (stored in $files)
        foreach ($item in $files) {
            $hash = [Main]::GetFileHash($item[1]) # calculate hash
            $item[2] = $hash # save hash
        }

        # insert checksums into config
        [xml]$xml = Get-Content $confPath
        $xml.SelectSingleNode("xml/config/validateChecksums").InnerText = "true" # enable validation
        foreach ($item in $files) {
            $xml.SelectSingleNode("xml/checksums/$($item[0])").InnerText = $item[2] # insert obtained checksums
        }
        $xml.Save($confPath)

        # get hash of config containing hashs of scripts
        $confCksm = [Main]::GetFileHash("$PSScriptRoot\config.xml")

        # store hash of config in registry
        # create CineBank node if not existing
        if (-not (Test-Path -Path "HKLM:\SOFTWARE\CineBank")) {
            New-Item -Path "HKLM:\SOFTWARE" -Name "CineBank"
        }
        # create checksum key of not existing, else update
        if (-not (Test-Path -Path "HKLM:\SOFTWARE\CineBank\ConfigCksm" -PathType Leaf)) {
            New-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value $confChksm -PropertyType "String"
        }
        else {
            Set-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value $confChksm
        }
    }
    else {
        Write-Warning "Skipping building of checksums due to insufficient privileges. Please run the script later as admin and use operation 3!"
    }
    Write-Host "Update completed."
}
function Disable-CksmValidation {
    Write-Host "Updating checksums..."
    $confPath = "$PSScriptRoot\config.xml"
    if ($isAdmin) {
        # update config
        [xml]$xml = Get-Content $confPath
        $xml.SelectSingleNode("xml/config/validateChecksums").InnerText = "false" # disable validation
        $xml.Save($confPath)

        # remove hash from config in registry
        # create CineBank node if not existing
        if (-not (Test-Path -Path "HKLM:\SOFTWARE\CineBank")) {
            New-Item -Path "HKLM:\SOFTWARE" -Name "CineBank"
        }
        # create checksum key of not existing, else update
        if (-not (Test-Path -Path "HKLM:\SOFTWARE\CineBank\ConfigCksm" -PathType Leaf)) {
            New-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value "" -PropertyType "String"
        }
        else {
            Set-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value ""
        }
    }
    else {
        Write-Warning "Failed to remove registry key due to insufficient privileges. Please run the script as admin or warnings will be raised hen executing the application!"
    }
    Write-Host "Checksum-validation disable."
}

$assemblies = ("System.IO","System.Security")
$source = @"
using System;
using System.IO;
using System.Security.Cryptography;
public static class Main {
    /// <summary>
    /// Genereates the SHA256-Hash of a file
    /// </summary>
    /// <param name="path">Path to the file to hash</param>
    /// <returns>SHA256-Hash of the file as Base64 string</returns>
    public static string GetFileHash(string path)
    {
        using (SHA256 sha = SHA256.Create())
        {
            // get filestream from path
            using (FileStream fileStream = File.OpenRead(path))
            {
                try
                {
                    // maske sure that filestream is at start
                    fileStream.Position = 0;
                    // Compute the hash of the fileStream.
                    byte[] hashValue = sha.ComputeHash(fileStream);
                    // return hash value of the file
                    return Convert.ToBase64String(hashValue);
                }
                catch (IOException e)
                {
                    Console.WriteLine("I/O Exception: " + e.Message);
                }
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine("Access Exception: " + e.Message);
                }
            }
        }
        return "ERROR";
    }
}
"@
Add-Type -ReferencedAssemblies $assemblies -TypeDefinition $source -Language CSharp

# MAIN
Write-Host "   === CineBank Config Editor ===   "

#checing access level
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
	Write-Warning "You are not running this script with elevated privileges! You will not be able to store the checksum of the config file in your registry. This is recommended to ensure nobody changed your config or scripts!"
	$confirm = Read-Host -Prompt "Procede without elevated privileges? [y/N]"
    if ($confirm.ToLower() -ne "y") {
        Write-Host "Aborting."
        EXIT
    }
}

while (1) {
    Write-Host ""
    Write-Host ""

    # select option
    Write-Host "Select operation to perform (e.g. 1):"
    Write-Host " [1] Initial configuration"
    Write-Host " [2] Update config"
    Write-Host " [3] Update checksums / Enable validation if disabled"
    Write-Host " [4] Disable checksum validation"
    Write-Host " [e] Exit"
    $op = Read-Host -Prompt "Operation"
    Write-Host ""

    # execute operation
    if ($op -eq "1") { # initial configuration
        Create-Config
        Update-Config
    }
    elseif ($op -eq "2") { # update config
        Update-Config
    }
    elseif ($op -eq "3") { # update checksum of configuration file in registry
        Update-ConfigCksm
    }
    elseif ($op -eq "4") { # disable checksum validation
        Disable-CksmValidation
    }
    elseif ($op.ToLower().StartsWith("e")) { # exit
        EXIT
    }
    else {
        Write-Host "Unknown Operation '$op'!"
        $confirm = Read-Host -Prompt "Exit? [y/N]"
        if ($confirm.ToLower() -eq "y") {
            EXIT
        }
    }
}