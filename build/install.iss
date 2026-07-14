#define MyAppName "Agnes AI Image Edit"

#ifndef MyAppVersion
  #define MyAppVersion "0.0.0-dev"
#endif
#ifndef PublishOutput
  #define PublishOutput "publish"
#endif

[Setup]
AppId={{A3F1C9E2-7B4D-4E8A-9C6B-1F2D3E4A5B6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher=Agnes AI Image Edit
DefaultDirName={localappdata}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=out
OutputBaseFilename=AgnesAIImageEdit-Setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayName={#MyAppName}
SetupLogging=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"

[Files]
Source: "{#PublishOutput}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\AgnesAIImageEdit.exe"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\AgnesAIImageEdit.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\AgnesAIImageEdit.exe"; Description: "Launch {#MyAppName}"; Flags: nowait postinstall skipifsilent
