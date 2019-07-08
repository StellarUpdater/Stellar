# Stellar Guide
### RetroArch Nightly Updater for Windows
---

### Requirements
Please update only once per day to conserve Libretro server bandwidth.  
Program needs [7-zip](http://www.7-zip.org/download.html) installed in order to unzip the cores.

- 7-Zip or WinRAR to extract 7z
- Older Windows may need .NET Framework 4.5 installed.
- Old versions of Windows 7 may need to be updated to SP1
- `Run as Administrator` if accessing Program Files path (should be default).
<br>

### How it works
- It analyzes http://buildbot.libretro.com/nightly/windows/
- Gets the latest 7z file
- Extracts and overwrites exe's to your RetroArch folder
- Updates latest cores
<br>

### How to use
- All you need is the `exe`. It's portable, no install.
- Select your RetroArch folder, click Update.
- It will download the latest Nightly 7z and extract only `retroarch.exe` and `retroarch_debug.exe` to your folder.
- The `Check` button will preview the file URL before downloading.
- It should not overwrite your configs, but keep a backup before updating.
- If RetroArch is installed in Program Files folder, you may need to `Run As Administrator`.
<br>

### Menu Options
- `New Install` - Installs RetroArch, Redistributables, & Cores. Replaces Configs with default.
- `Upgrade` - Upgrades RetroArch to the latest version, including Redistributables and Configs.
- `RetroArch` - Updates RetroArch to the latest version, excluding Redistributables and Configs.
- `RA + Cores` - Updates RetroArch and currently installed Cores.
- `Cores` - Updates currently installed Cores.
- `New Cores` - Installs Cores that are newly released or missing from your current install.
- `Redist` - Installs Redistributables. Helpful if RetroArch won't start after update.
- `Stellar` - Updates Stellar
<br>

### New Install
To install RetroArch and Cores for first time use (Large download):

1. Create a RetroArch folder on your computer
2. Select the folder
3. Select `New Install` from the `Download menu`.

Don't use this option to Update or if you already have it installed. Please conserve Libretro bandwidth.  
If you don't need Nightly alpha builds, consider using the Stable version before installing.  
<br>
<br>

### Cores Update
1. Select your RetroArch main folder
2. Select `Cores` from the `Download menu`
3. Click the `Check` button to preview download (optional)
4. Click `Update`

It will check if Server Core Dates are more recent than PC Core Dates.  
Downloads only Cores you currently have, won't add more.  
Temp zip files are stored in `%appdata%` and deleted when complete.  

***To Exclude Core from Updates Download List***
- Click `Check` Button → Uncheck the Cores you don't want to update → Close Out → Click `Update`
- It won't save the checkbox states (for now), you will need to exclude again each time you update.
- Keep a backup of your important cores in case of accidentally overwriting
<br>

### Troubleshooting
- If the program starts with a white background, go to `Configure` → `Clear Saved`, restart program.  
Or go to `C:\Users\[Your Name]\AppData\Local\Stellar` and delete the old configs.
- If you receive "Error: Cannot connect to Server", try a few times, it may have failed to download the cores list.
- If certain cores are throwing the updater out of sync, delete them and use "New Cores" option to download a new version.
- If you Update cores, but it still says they are out of date, it might be a Time Zone problem.