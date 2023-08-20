# open folder in audio player (e.g. scores)
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

$mediaplayer = "C:\Program Files\VideoLAN\VLC\vlc.exe"

# check weather a single sile is supplied or a folder containing multiple files (depends on what user imported into db)
$file = ""
if ((Get-Item $path) -is [System.IO.DirectoryInfo]) { # is directory
    $file = $path
}
else { # is file
    # determine if parent directory only contains file of same kind
    # if true expect that only one file from the complete score was supplied --> open dir
    # some users might prefer to put $path directly in "Start-Process"-Call for example if they store all title songs in the same dir or want to import all songs as seperate files --> then delete this whole if/else
    
    $folder = Split-Path $path
    
    $useFolder = $true
    foreach ($var in (Get-ChildItem -Path $folder)) {
        if (-not $var.ToString().EndsWith($path.Substring($path.Length - 3))) { # end like supplied file --> expect they are of same kind
            $useFolder = $false
        }
    }

    if ($useFolder) {
        $file = $folder
    }
    else {
        $file = $path
    }
}

Start-Process -FilePath $mediaplayer -ArgumentList """$file""" -WindowStyle Minimized