; Commande avant d'executer ce script
;dotnet publish -c Release -r win-x64 --self-contained true -o ./publish

; DÉFINITION DES VARIABLES
#define MyAppName "Epicur'app"
#define MyAppVersion "1.0"
#define MyAppPublisher "Home Sweet Home"
#define MyAppExeName "EpicurAppIHM.exe"

[Setup]
; --- Identité de l'application ---
AppId={{A1B2C3D4-E5F6-7890-1234-567890ABCDEF}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL="https://iutdijon.u-bourgogne.fr"

; --- Dossier d'installation ---
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}

; --- Configuration de l'installeur ---
OutputDir=.\Output
OutputBaseFilename=Install_EpicurApp_
SetupIconFile=.\Images\logo.ico

Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=admin

[Languages]
Name: "french"; MessagesFile: "compiler:Languages\French.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; --- COPIE DES FICHIERS ---
Source: ".\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs


[Icons]
; --- RACCOURCIS ---
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; WorkingDir: "{app}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"

Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon; WorkingDir: "{app}"

[Run]
; --- LANCEMENT FINAL  ---
Filename: "{app}\{#MyAppExeName}"; Description: "Lancer {#MyAppName} maintenant"; Flags: nowait postinstall skipifsilent unchecked; WorkingDir: "{app}"


