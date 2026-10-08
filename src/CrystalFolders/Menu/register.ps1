# register.ps1 - Auto-registro silencioso del menu contextual
$ErrorActionPreference = "SilentlyContinue"
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$installDir = Join-Path $env:LOCALAPPDATA "CrystalFolders"
$cerPath = Join-Path $scriptDir "CrystalFolders.cer"
$msixPath = Join-Path $scriptDir "CrystalFolders.msix"

# 1. Asegurar directorios e iconos en LocalAppData
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
}
$localFolders = Join-Path $installDir "Folders"
if (-not (Test-Path $localFolders)) {
    New-Item -ItemType Directory -Force -Path $localFolders | Out-Null
}
$baseFolders = Join-Path $scriptDir "..\Folders"
if (Test-Path $baseFolders) {
    Copy-Item (Join-Path $baseFolders "*.ico") $localFolders -Force
}
Copy-Item (Join-Path $scriptDir "CrystalContextMenu.dll") (Join-Path $installDir "CrystalContextMenu.dll") -Force

# 2. Confianza de certificado
if (Test-Path $cerPath) {
    Import-Certificate -FilePath $cerPath -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
}

# 3. Latencia de dibujo
Set-ItemProperty -Path 'HKCU:\Control Panel\Desktop' -Name 'MenuShowDelay' -Value '20'

# 4. Desplegar Sparse MSIX
$pkg = Get-AppxPackage *CrystalFolders.ModernMenu*
if ($pkg) {
    Remove-AppxPackage $pkg.PackageFullName
}
Add-AppxPackage -Path $msixPath -ExternalLocation $installDir
