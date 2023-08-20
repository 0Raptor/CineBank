# open file in video player 1 (e.g. WindowsMediaPlayer for MP4)
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

$mediaplayer = "C:\Program Files (x86)\Windows Media Player\wmplayer.exe"

#Start-Process -FilePath $mediaplayer -ArgumentList """$path""" -WindowStyle Maximized
Start-Process -FilePath """$path""" -WindowStyle Maximized