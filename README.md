![https://stellarupdater.github.io/](https://stellarupdater.github.io/img/logo.png?raw=true)

# Stellar
RetroArch Nightly Updater

* [Overview](#overview)
* [Downloads](#downloads)
* [Installation](#installation)
* [Build](#build)

## Overview

![Stellar](https://stellarupdater.github.io/img/program.jpg?raw=true)

### How it works
It analyzes http://buildbot.libretro.com/nightly/windows/  
Gets the latest 7z file  
Extracts and overwrites exe's to your RetroArch folder  
Updates latest cores  

### How to use
All you need is the exe. It's portable, no install.  
Select your RetroArch folder, click Update.  
It will download the latest Nightly 7z and extract only retroarch.exe and retroarch_debug.exe to your folder.  
The Check button will preview the file URL before downloading.  
It should not overwrite your configs, but keep a backup before updating.  
If RetroArch is installed in Program Files folder, you may need to Run As Administrator.  

## Downloads
#### Binary Releases
https://github.com/StellarUpdater/Stellar/releases

Requires [Microsoft .NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)

## Installation
Stellar is portable and can be run from any location on the computer.

## Build
Visual Studio 2013
<br />
WPF, C#, XAML