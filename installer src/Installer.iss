#define MyAppName "Crystal Folders"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Génesis Toxical"
#define MyAppURL "https://genesistoxical.github.io/crystal-folders/"
#define MyAppExeName "Crystal Folders.exe"

[Setup]
AppId={{96F31E5A-5556-46BB-9EB0-EF511EBEEDA7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion=1.0.0.0
AppPublisher={#MyAppPublisher}
AppCopyright={#MyAppPublisher} © 2025
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
LicenseFile=Crystal Folders.txt
OutputDir=Output
OutputBaseFilename=Crystal-Folders-Installer
SetupIconFile=..\src\CrystalFolders\Resources\Icon.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern      
WizardSmallImageFile=Wizard Small Image.bmp
WizardImageFile=Wizard Image File.bmp
;DisableWelcomePage=no
;WizardSizePercent=114,100
WizardSizePercent=110,100 
DisableProgramGroupPage=yes
UninstallDisplayIcon={uninstallexe}
UsedUserAreasWarning=no                                    

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "..\src\CrystalFolders\bin\Release\*"; DestDir: "{app}"; Excludes: "Folders, *.ini"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\src\CrystalFolders\bin\Release\Config.ini"; DestDir: "{userappdata}\{#MyAppName}"; Flags: ignoreversion
Source: "..\src\CrystalFolders\bin\Release\Folders\*"; DestDir: "{userappdata}\{#MyAppName}\Folders"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

;[Messages]
;english.WelcomeLabel1=[name] Setup Wizard
;english.WelcomeLabel2=This will install [name/ver] on your computer.
;spanish.WelcomeLabel1=Asistente de Instalación de [name]
;spanish.WelcomeLabel2=Este programa instalará [name/ver] en su sistema.
; NOTA: El apartado de Messages solo se utilizará en caso de que la bienvenida esté habilitada