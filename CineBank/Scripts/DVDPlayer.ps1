# open dvd player on folder or mount iso and open player there
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

$mediaplayer = "C:\Program Files\VideoLAN\VLC\vlc.exe"

# iso file or dvd folder
$file = ""
if ($path.EndsWith(".iso")) {
    # mount image
    $mnt = Mount-DiskImage -ImagePath $path
    # get drive letter
    $file = "$(($mnt | Get-Volume).DriveLetter):"
}
else {
    # select playable file in dvd folder (if not supplied)
    if (-not $path.EndsWith("VIDEO_TS\VIDEO_TS.IFO") -and -not $path.EndsWith("VIDEO_TS/VIDEO_TS.IFO")) {
        $file = "$path\VIDEO_TS\VIDEO_TS.IFO"
    }
    elseif (-not $path.EndsWith("VIDEO_TS.IFO")) {
        $file = "$path\VIDEO_TS.IFO"
    }
    else {
        $file = $path
    }
}

Start-Process -FilePath $mediaplayer -ArgumentList """$file""" -WindowStyle Maximized