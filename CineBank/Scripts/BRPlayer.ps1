# open br player on folder or mount iso and open player there
param(
    [Parameter(Mandatory)]
    [String]$path,
    [Parameter()]
    [String]$dir
)

$mediaplayer = "C:\Program Files (x86)\Leawo\Blu-ray Player\Leawo Blu-ray Player.exe"

# iso file or br folder
$file = ""
if ($path.EndsWith(".iso")) {
    # mount image
    $mnt = Mount-DiskImage -ImagePath $path
    # get drive letter
    $file = "$(($mnt | Get-Volume).DriveLetter):"
}
else {
    # select playable file in br folder (if not supplied)
    if (-not $path.EndsWith("BDMV\index.bdmv") -and -not $path.EndsWith("BDMV/index.bdmv")) {
        $file = "$path\BDMV\index.bdmv"
    }
    elseif (-not $path.EndsWith("index.bdmv")) {
        $file = "$path\index.bdmv"
    }
    else {
        $file = $path
    }
}
Start-Process -FilePath $mediaplayer -ArgumentList """$file""" -WindowStyle Maximized