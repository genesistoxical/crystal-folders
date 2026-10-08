@echo off
:: Solicitar permisos de administrador si no se tienen
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo Solicitando permisos de administrador...
    powershell -Command "Start-Process '%~f0' -Verb RunAs"
    exit /b
)

echo Restaurando iconos originales de carpetas de Windows...
reg delete "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons" /f >nul 2>&1
reg delete "HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Shell Icons" /f >nul 2>&1

echo Refrescando cache de iconos...
ie4uinit.exe -show

echo.
echo ========================================================
echo  Icono predeterminado de Windows restaurado con exito.
echo ========================================================
pause
