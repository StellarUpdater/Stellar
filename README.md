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
* It analyzes http://buildbot.libretro.com/nightly/windows/
* Gets the latest 7z file
* Extracts and overwrites exe's to your RetroArch folder
* Updates latest cores

### How to use
1. Select your RetroArch folder
2. Select the Download from the menu
3. Click Update

* It will download the latest Nightly 7z and extract only retroarch.exe and retroarch_debug.exe to your folder.
* The Check button will preview the file URL before downloading.
* It should not overwrite your configs, but you should keep a backup before updating.
* If RetroArch is installed in Program Files folder, you may need to Run As Administrator.

### Cores Update
1. Select your RetroArch folder
2. Select Cores from the Download menu
3. Click the Check button to preview download (optional)
4. Click Update

* It will check if Server Core Dates are more recent than PC Core Dates.  
* Downloads only Cores you currently have, won't add more.  
* Temp zip files are stored in %appdata% and deleted when complete.

#### To Exclude Core from Updates Download List
* Check Button → Uncheck the Cores you don't want to update → Close Out → Click Update
* It won't save the checkbox states (for now), you will need to exclude again each time you update.
* Keep a backup of your important cores in case of accidentally overwriting


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