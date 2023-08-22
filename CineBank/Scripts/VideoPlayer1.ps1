# open file in video player 1 (e.g. WindowsMediaPlayer for MP4)
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

#$mediaplayer = "shell:AppsFolder\Microsoft.ZuneMusic_8wekyb3d8bbwe!Microsoft.ZuneMusic"
$mediaplayer = "C:\Program Files (x86)\Windows Media Player\wmplayer.exe"

#Start-Process -FilePath $mediaplayer -ArgumentList """$path""" -WindowStyle Maximized
Start-Process -FilePath """$path""" -WindowStyle Maximized