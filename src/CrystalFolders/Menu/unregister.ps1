# unregister.ps1 - Retirar menu contextual
$ErrorActionPreference = "SilentlyContinue"
$pkg = Get-AppxPackage *CrystalFolders.ModernMenu*
if ($pkg) {
    Remove-AppxPackage $pkg.PackageFullName
}
