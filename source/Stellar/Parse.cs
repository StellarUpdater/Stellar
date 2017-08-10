using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

/* ----------------------------------------------------------------------
    Stellar ~ RetroArch Nightly Updater by wyzrd
    https://stellarupdater.github.io
    https://forums.libretro.com/users/wyzrd

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.If not, see <http://www.gnu.org/licenses/>. 

    Image Credit: ESO & NASA (CC)
   ---------------------------------------------------------------------- */

namespace Stellar
{
    public partial class Parse
    {
        public static string page;
        public static string element;

        public static string nightly7z; // The Parsed Dated 7z Nightly Filename
        public static string nightlyUrl; // Download URL + Dated 7z Filename

        public static string parseUrl = "https://buildbot.libretro.com/nightly/windows/x86_64/"; // Parse URL to be changed with Dropdown Combobox. Default is 64-bit.
        public static string indexextendedUrl = string.Empty; // index-extended Cores Text File
        public static string parseCoresUrl = string.Empty; // Buildbot Cores URL to be parsed
        public static string libretro_x86 = "https://buildbot.libretro.com/nightly/windows/x86/"; // Download URL 32-bit
        public static string libretro_x86_64 = "https://buildbot.libretro.com/nightly/windows/x86_64/"; // Download URL 64-bit
        public static string libretro_x86_64_w32 = "https://buildbot.libretro.com/nightly/windows/x86_64_w32/"; // Download URL 64-bit w32



        // -----------------------------------------------
        // Parse Builbot Page HTML
        // -----------------------------------------------
        public static void ParseBuildbotPage(MainWindow mainwindow)
        {
            // -------------------------
            // Begin Parse
            // -------------------------
            // If No Internet Connect, program will crash. Use Try & Catch to display Error.

            // -------------------------
            // Update Selected
            // -------------------------
            try
            {
                // Parse the HTML Page from parseUrl
                page = Download.wc.DownloadString(parseUrl); // HTML Page
                element = "<a href='/nightly/windows/" + Paths.buildbotArchitecture + "/(.*?)'>"; // HTML Table containing Dated 7z, (.*?) is the text to keep

                // Add each 7z Date/Time to the Nightlies List
                foreach (Match match in Regex.Matches(page, element))
                    Queue.NightliesList.Add(match.Groups[1].Value);

                // Remove from the List all 7z files that do not contain _RetroArch.7z (filters out unwanted)
                Queue.NightliesList.RemoveAll(u => !u.Contains("_RetroArch.7z"));
                Queue.NightliesList.TrimExcess();

                // Sort the Nighlies List, lastest 7z is first
                Queue.NightliesList.Sort(); //do not disable this sort


                // Get First Element of Nightlies List 
                nightly7z = Queue.NightliesList.First();
            }
            catch
            {
                MessageBox.Show("Error: Problem creating RetroArch list from HTML.");
            }

            // -------------------------
            // New Install Selected
            // -------------------------
            // RetroArch.exe
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
            {
                // Fetch the RetroArch + Redist (not dated)
                nightly7z = "RetroArch.7z";
            }

            // -------------------------
            // Upgrade Selected
            // -------------------------
            // Partial Unpack RetroArch.7z
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Upgrade")
            {
                // Fetch the RetroArch + Redist (not dated)
                nightly7z = "RetroArch.7z";
            }

            // -------------------------
            // Redist Selected
            // -------------------------
            // Redistributable
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Redist")
            {
                // Fetch the RetroArch + Redist (not dated)
                nightly7z = "redist.7z";
            }

            // -------------------------
            // Set the URL's for 32-bit & 64-bit Dropdown Comboboxes
            // -------------------------
            // If 32-bit Selected, change Download URL to x86
            //
            if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "32-bit")
            {
                // Create URL string for Uri
                nightlyUrl = libretro_x86 + nightly7z;

            }

            // If 64-bit OR 64 w32 Selected, change Download URL to x86_64
            //
            else if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "64-bit")
            {
                // Create URL string for Uri
                nightlyUrl = libretro_x86_64 + nightly7z;
            }
        }



        // -----------------------------------------------
        // Parse Builbot Cores Page HTML
        // -----------------------------------------------
        public static void ParseBuildbotCoresIndex(MainWindow mainwindow)
        {
            // -------------------------
            // Begin Parse
            // -------------------------
            // If No Internet Connect, program will crash. Use Try & Catch to display Error.
            try
            {
                // -------------------------
                // Download
                // -------------------------
                // index-extended cores text file
                string buildbotCoresIndex = Download.wc.DownloadString(indexextendedUrl);
                // Trim ending linebreak
                buildbotCoresIndex = buildbotCoresIndex.TrimEnd('\n');

                // Check if index-extended failed or is empty
                if (string.IsNullOrEmpty(buildbotCoresIndex))
                {
                    MessageBox.Show("Error: Cores list is empty or failed to donwload index-extended.");
                }

                // -------------------------
                // Sort
                // -------------------------
                // Split the index-extended by LineBreak Array
                // Sort the Array by Core Name (3rd word in Line)
                var lines = buildbotCoresIndex.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    .Select(tag => tag.Trim())
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .OrderBy(s => s.Split(' ')[2])
                    .ToArray();

                // Split the index-extended into 3 Lists, BuildbotCoreDate, BuildbotID, BuildbotCoresName
                foreach (string line in lines)
                {
                    string[] arr = line.Split(' ');
                    Queue.ListBuildbotCoresDate.Add(arr[0]);
                    Queue.ListBuildbotCoresDate.TrimExcess();

                    //Queue.ListBuildbotID.Add(arr[1]);
                    //Queue.ListBuildbotID.TrimExcess();

                    Queue.ListBuildbotCoresName.Add(arr[2]);
                    Queue.ListBuildbotCoresName.TrimExcess();
                }

                // -------------------------
                // Modify
                // -------------------------
                // Remove from the List all that do not contain .dll.zip (filters out unwanted)
                Queue.ListBuildbotCoresName.RemoveAll(u => !u.Contains(".dll.zip"));
                Queue.ListBuildbotCoresName.TrimExcess();

                // Remove .zip from all in List
                for (int i = 0; i < Queue.ListBuildbotCoresName.Count; i++)
                {
                    if (Queue.ListBuildbotCoresName[i].Contains(".zip"))
                        Queue.ListBuildbotCoresName[i] = Queue.ListBuildbotCoresName[i].Replace(".zip", "");
                }

                // -------------------------
                // Combine
                // -------------------------
                // Join Lists Name & Date
                for (int i = 0; i < Queue.ListBuildbotCoresName.Count; i++)
                {
                    Queue.ListBuildbotCoresNameDate.Add(Queue.ListBuildbotCoresName[i] + " " + Queue.ListBuildbotCoresDate[i]);
                }

                // -------------------------
                // Sort Correction
                // -------------------------
                Queue.ListBuildbotCoresNameDate.Sort();
                Queue.ListBuildbotCoresNameDate.TrimExcess();
            }
            catch
            {
                MessageBox.Show("Error: Cannot connect to Server.");
            }
        }



        // -----------------------------------------------
        // Scan PC Cores Directory
        // -----------------------------------------------
        // Creates the PC Name+Date List
        public static void ScanPcCoresDir(MainWindow mainwindow)
        {
            // Cores Folder
            // end with backslash RetroArch\cores\
            Paths.coresPath = Paths.retroarchPath + "cores\\"; 

            try // program will crash if files not found
            {
                // Add Core Name to List
                //
                Queue.ListPcCoresName = Directory.GetFiles(Paths.coresPath, "*_libretro.dll") //match ending of a core name //Try EnumerateFiles
                        .Select(System.IO.Path.GetFileName)
                        .ToList();

                // Add Core Modified Dates to List
                // Extracts original File Modified Date when overwriting
                //
                Queue.ListPcCoresDate = Directory.GetFiles(Paths.coresPath, "*_libretro.dll") //match ending of a core name
                        .Select(p => File.GetLastWriteTime(p)
                        .ToString("yyyy-MM-dd"))
                        .ToList();
            }
            catch
            {
                MainWindow.ready = 0;
                MessageBox.Show("Error: Problem scanning PC Cores Name & Dates.");
            }

            // Popup Error Message if PC Cores Name List has no items 0
            if (Queue.ListPcCoresName.Count == 0
                && (string)mainwindow.comboBoxDownload.SelectedItem != "New Install" // Ignore
                && (string)mainwindow.comboBoxDownload.SelectedItem != "New Cores") // Ignore
            {
                MessageBox.Show("Cores not found. \n\nPlease select your RetroArch main folder.");
                MainWindow.ready = 0;
            }

            // -------------------------
            // PC Cores Join Name + Date List
            // -------------------------
            // Join Lists PC Name + PC Date (Formatted)
            for (int i = 0; i < Queue.ListPcCoresName.Count; i++)
            {
                Queue.ListPcCoresNameDate.Add(Queue.ListPcCoresName[i] + " " + Queue.ListPcCoresDate[i]);
            }

            // -------------------------
            // Sort Correction
            // -------------------------
            Queue.ListPcCoresNameDate.Sort();
            Queue.ListPcCoresNameDate.TrimExcess();
        }

    }
}
