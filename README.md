# memory
Helps you to remember books, quotes, trainings etc.

## Relevant files after build:
* Memory.exe - console app
* MemoryDesktop.exe - windows app
* MemoryLibrary.dll - shared library needed for both console and windows app
* MemoryConfiguration.ini - text file with paths to notes

## Command Prompt Setup:
%windir%\system32\cmd.exe /K D:\setenv.cmd && Memory.exe
where setenv.cmd will setup path to Memory.exe.

## Desktop app:
Add MemoryDesktop.exe as startup app. 
winkey+r, run "shell:startup"
add shortcut to MemoryDesktop.exe
