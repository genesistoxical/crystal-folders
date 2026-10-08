# sign_and_register.ps1 - Despliegue de ultra-alto rendimiento
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$installDir = Join-Path $env:LOCALAPPDATA "CrystalFolders"

Write-Host "1. Cerrando temporalmente Explorer..." -ForegroundColor Cyan
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Milliseconds 600

Write-Host "2. Preparando binarios en: $installDir" -ForegroundColor Cyan
if (-not (Test-Path $installDir)) {
    New-Item -ItemType Directory -Force -Path $installDir | Out-Null
}

Copy-Item (Join-Path $scriptDir "CrystalContextMenu.dll") (Join-Path $installDir "CrystalContextMenu.dll") -Force
$mainExe = Join-Path $scriptDir "..\..\src\CrystalFolders\bin\Release\Crystal Folders.exe"
if (Test-Path $mainExe) {
    Copy-Item $mainExe (Join-Path $installDir "Crystal Folders.exe") -Force
}

# Copiar iconos a LocalAppData para asignacion fisica en disco (desktop.ini)
$iconsTarget = Join-Path $installDir "Folders"
if (-not (Test-Path $iconsTarget)) {
    New-Item -ItemType Directory -Force -Path $iconsTarget | Out-Null
}
$srcFolders = Join-Path $scriptDir "..\..\src\CrystalFolders\Folders"
if (Test-Path $srcFolders) {
    Copy-Item (Join-Path $srcFolders "*.ico") $iconsTarget -Force
}

Write-Host "3. Optimizando latencia de dibujo del sistema (MenuShowDelay = 20ms)..." -ForegroundColor Cyan
Set-ItemProperty -Path 'HKCU:\Control Panel\Desktop' -Name 'MenuShowDelay' -Value '20'

Write-Host "4. Empaquetando y firmando MSIX..." -ForegroundColor Cyan
$windowsKitBin = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin\10.*" -ErrorAction SilentlyContinue | Sort-Object Name -Descending | Select-Object -First 1
if ($windowsKitBin) {
    $makeappx = Join-Path $windowsKitBin.FullName "x64\makeappx.exe"
    $signtool = Join-Path $windowsKitBin.FullName "x64\signtool.exe"
} else {
    $makeappx = "makeappx.exe"
    $signtool = "signtool.exe"
}

$pkgDir = Join-Path $scriptDir "Package"
$msixPath = Join-Path $scriptDir "CrystalFolders.msix"

& $makeappx pack /d $pkgDir /p $msixPath /nv /o | Out-Null

$cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -match 'CN=CrystalFolders' } | Select-Object -First 1
if (-not $cert) {
    $cert = New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=CrystalFolders" -CertStoreLocation "Cert:\CurrentUser\My"
}
$cerLocalPath = Join-Path $scriptDir "CrystalFolders.cer"
Export-Certificate -Cert $cert -FilePath $cerLocalPath -Force | Out-Null

$inTrusted = Get-ChildItem Cert:\CurrentUser\TrustedPeople | Where-Object { $_.Thumbprint -eq $cert.Thumbprint }
if (-not $inTrusted) {
    Import-Certificate -FilePath $cerLocalPath -CertStoreLocation "Cert:\CurrentUser\TrustedPeople" | Out-Null
}

& $signtool sign /fd SHA256 /sha1 $cert.Thumbprint /v $msixPath | Out-Null

# Sincronizar paquete y certificado con src/CrystalFolders/Menu
$menuDir = Join-Path $scriptDir "..\..\src\CrystalFolders\Menu"
if (Test-Path $menuDir) {
    Copy-Item $msixPath (Join-Path $menuDir "CrystalFolders.msix") -Force
    Copy-Item $cerLocalPath (Join-Path $menuDir "CrystalFolders.cer") -Force
}

Write-Host "5. Registrando Sparse Package en Windows 11..." -ForegroundColor Cyan
$existing = Get-AppxPackage -Name "CrystalFolders.ModernMenu" -ErrorAction SilentlyContinue
if ($existing) {
    Remove-AppxPackage -Package $existing.PackageFullName
}

Add-AppxPackage -Path $msixPath -ExternalLocation $installDir

Write-Host "6. Reiniciando Explorador de Windows..." -ForegroundColor Cyan
Start-Process explorer

Write-Host "TODO COMPLETADO EXITOSAMENTE! Menu contextual activo." -ForegroundColor Green
