# CineBank Config Editor

Write-Host "   === CineBank Config Editor ===   "

#checing access level
$isAdmin = ([Security.Principal.WindowsPrincipal][Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
	Write-Warning "You are not running this script with elevated privileges! You will not be able to store the checksum of the config file in your registry. This is recommended to ensure nobody changed your config or scripts!"
	$confirm = Read-Host -Prompt "Procede without elevated privileges? [y/N]"
    if ($confirm -ne "y") {
        Write-Host "Aborting."
        EXIT
    }
}

# select option
Write-Host "Select operation to perform (e.g. 1):"
Write-Host " [1] Initial configuration"
Write-Host " [2] Update config"
Write-Host " [3] Update checksums"
Write-Host " [4] Disable checksum validation"
$op = Read-Host -Prompt "Operation"

# update checksum of configuration file in registry
#  config containes checksums of other scripts, so their integridy is also ensured
if ($isAdmin) {
    # create CineBank node if not existing
    if (-not Test-Path -Path "HKLM:\SOFTWARE\CineBank") {
        New-Item -Path "HKLM:\SOFTWARE" -Name "CineBank"
    }
    # create checksum key of not existing, else update
    if (-not Test-Path -Path "HKLM:\SOFTWARE\CineBank\ConfigCksm" -PathType Leaf) {
        New-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value "newval" -PropertyType "String"
    }
    else {
        Set-ItemProperty -Path "HKLM:\SOFTWARE\CineBank" -Name "ConfigCksm" -Value "newval"
    }
}