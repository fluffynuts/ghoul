; Script generated by the Inno Script Studio Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Ghoul"
#define MyAppVersion "1.0"
#define MyAppPublisher "fluffynuts"
#define MyAppURL "https://github.com/fluffynuts/ghoul"
#define MyAppExeName "Ghoul.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{5500FAC8-D651-4A13-8376-A779BBBECF68}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}}
AppUpdatesURL={#MyAppURL}
DefaultDirName={localappdata}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=LICENSE
OutputDir=dist
OutputBaseFilename=Setup-Ghoul-{#MyAppVersion}
SetupIconFile=src\Ghoul\Resources\minecraft-zombie.ico
Compression=lzma
SolidCompression=yes
InfoAfterFile=C:\code\opensource\Ghoul\README.md
PrivilegesRequired=admin

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "src\Ghoul\bin\Debug\Ghoul.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\Ghoul.exe.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\PeanutButter.INI.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\PeanutButter.TinyEventAggregator.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\PeanutButter.TrayIcon.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\PeanutButter.Utils.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "src\Ghoul\bin\Debug\System.ValueTuple.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: runascurrentuser nowait postinstall skipifsilent