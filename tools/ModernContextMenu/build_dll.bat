@echo off
setlocal
if not defined DevEnvDir (
    if exist "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
    ) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat"
    ) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvars64.bat"
    ) else if exist "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvars64.bat" (
        call "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvars64.bat"
    )
)

cd /d "%~dp0"

echo [1/3] Compilando recursos Win32 (iconos embebidos)...
rc.exe /r /fo CrystalContextMenu.res CrystalContextMenu.rc
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Fallo la compilacion de recursos
    exit /b 1
)

echo [2/3] Compilando CrystalContextMenu.dll optimizada (/O2, /MT, recursos embebidos)...
cl.exe /O2 /MT /EHsc /std:c++17 /LD CrystalContextMenu.cpp CrystalContextMenu.res /Fe:CrystalContextMenu.dll /link /DEF:CrystalContextMenu.def /OPT:REF /OPT:ICF shell32.lib shlwapi.lib ole32.lib oleaut32.lib user32.lib advapi32.lib
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Fallo la compilacion de la DLL
    exit /b 1
)

echo [3/3] Sincronizando con src/CrystalFolders/Menu...
if not exist "..\..\src\CrystalFolders\Menu" mkdir "..\..\src\CrystalFolders\Menu"
copy /y CrystalContextMenu.dll "..\..\src\CrystalFolders\Menu\CrystalContextMenu.dll" >nul

echo Limpiando archivos intermedios...
del /q CrystalContextMenu.obj CrystalContextMenu.res CrystalContextMenu.exp CrystalContextMenu.lib 2>nul

echo [OK] CrystalContextMenu.dll compilada y sincronizada con exito.
endlocal
