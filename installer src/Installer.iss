#define MyAppName "Crystal Folders"
#define MyAppVersion "1.3.4"
#define MyAppPublisher "Génesis Toxical"
#define MyAppURL "https://genesistoxical.github.io/crystal-folders/"
#define MyAppExeName "Crystal Folders.exe"

[Setup]
AppId={{96F31E5A-5556-46BB-9EB0-EF511EBEEDA7}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion=1.3.4.0
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
Compression=zip
SolidCompression=no
WizardStyle=modern      
WizardSmallImageFile=Wizard Small Image.bmp
WizardImageFile=Wizard Image File.bmp
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
Source: "Custom 📂 Folder.url"; DestDir: "{userdesktop}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon