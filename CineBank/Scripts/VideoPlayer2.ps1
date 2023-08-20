# open file in video player 2 (e.g. VLC for MKVs)
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

$mediaplayer = "C:\Program Files\VideoLAN\VLC\vlc.exe"

Start-Process -FilePath $mediaplayer -ArgumentList """$path""" -WindowStyle Maximized