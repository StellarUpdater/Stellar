using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        // Scan PC Cores Directory
        // -----------------------------------------------
        public static void ScanPcCoresDir(MainWindow mainwindow)
        {
            // Cores Folder
            Paths.coresPath = Paths.retroarchPath + "cores\\"; //end with backslash RetroArch\cores\

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
                Queue.ListPcCoresDateModified = Directory.GetFiles(Paths.coresPath, "*_libretro.dll") //match ending of a core name
                    .Select(p => File.GetLastWriteTime(p))
                    .Select(p => new DateTime(p.Year, p.Month, p.Day))
                    .ToList();
            }
            catch
            {
                //System.Windows.MessageBox.Show("Error.");
                MainWindow.ready = 0;
            }

            // Popup Error Message if PC Cores Name List has no items 0
            if (Queue.ListPcCoresName.Count == 0 && (string)mainwindow.comboBoxDownload.SelectedItem != "New Cores") // Ignore for New Cores
            {
                System.Windows.MessageBox.Show("Cores not found. \n\nPlease select your RetroArch main folder.");
                MainWindow.ready = 0;
            }

            // Convert Core Dates to String to allow Date Formatting yyyy-MM-dd HH:mm Military Time
            Queue.ListPcCoresDateModifiedFormatted = Queue.ListPcCoresDateModified.ConvertAll<string>(x => x.ToString("yyyy-MM-dd")); //Keep Here


            // -------------------------
            // PC Cores Join Name + Date List
            // -------------------------
            // Convert Core Dates to String to allow Date Formatting yyyy-MM-dd HH:mm Military Time
            // Join Lists PC Name + PC Date (Formatted)
            for (int i = 0; i < Queue.ListPcCoresName.Count; i++)
            {
                Queue.ListPcCoresNameDate.Add(Queue.ListPcCoresName[i] + " " + Queue.ListPcCoresDateModifiedFormatted[i]);
            }


        }



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
                element = "<td class='fb-n'><a href='/nightly/windows/" + Paths.buildbotArchitecture + "/(.*?)'>"; // HTML Table containing Dated 7z, (.*?) is the text to keep

                // Add each 7z Date/Time to the Nightlies List
                foreach (Match match in Regex.Matches(page, element))
                    Queue.NightliesList.Add(match.Groups[1].Value);

                // Remove from the List all 7z files that do not contain _RetroArch.7z (filters out unwanted)
                Queue.NightliesList.RemoveAll(u => !u.Contains("_RetroArch.7z"));
                // Sort the Nighlies List, lastest 7z is first
                Queue.NightliesList.Sort(); //do not disable this sort

                // Get First Element of Nightlies List 
                nightly7z = Queue.NightliesList[Queue.NightliesList.Count - 1];
            }
            catch
            {
                System.Windows.MessageBox.Show("Error: Cannot connect to Server.");
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
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "Upgrade")
            {
                // Fetch the RetroArch + Redist (not dated)
                nightly7z = "RetroArch.7z";
            }

            // -------------------------
            // Redist Selected
            // -------------------------
            // Redistributable
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "Redist")
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
                // Last Item in Nightlies List is available
                if (!string.IsNullOrEmpty(nightly7z))
                {

                }
                // Last Item in Nightlies List cannot be found
                else
                {
                    // Clear Download Textbox
                    mainwindow.textBoxDownload.Text = "";
                }

                // Create URL string for Uri
                nightlyUrl = libretro_x86 + nightly7z;

            }

            // If 64-bit OR 64 w32 Selected, change Download URL to x86_64
            //
            if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "64-bit" || (string)mainwindow.comboBoxArchitecture.SelectedItem == "64 w32")
            {
                // Last Item in Nightlies List is available
                if (!string.IsNullOrEmpty(nightly7z))
                {

                }
                // Last Item in Nightlies List cannot be found
                else
                {
                    // Clear Download Textbox
                    mainwindow.textBoxDownload.Text = "";
                }

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
                // Parse the index-extended cores text file from parseUrl
                // -------------------------

                //string buildbotCoresPage = Download.wc.DownloadString(parseCoresUrl); // HTML Page
                string buildbotCoresIndex = Download.wc.DownloadString(indexextendedUrl); // index-extended cores text file

                // Check if index-extended failed or is empty
                if (string.IsNullOrEmpty(buildbotCoresIndex))
                {
                    System.Windows.MessageBox.Show("Error: Cores list is empty or failed to donwload index-extended.");
                }

                // Split the index-extended into 3 Lists, BuildbotCoreDate, BuildbotID, BuildbotCoresName
                //
                string[] lines = buildbotCoresIndex.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] arr = line.Split(' ');
                    Queue.ListBuildbotCoresDate.Add(arr[0]);
                    Queue.ListBuildbotID.Add(arr[1]);
                    Queue.ListBuildbotCoresName.Add(arr[2]);
                }

                // Debug
                //var message = string.Join(Environment.NewLine, Queue.ListBuildbotCoresDate);
                //System.Windows.MessageBox.Show(message);


                // Remove from the List all that do no tcontain .dll.zip (filters out unwanted)
                Queue.ListBuildbotCoresName.RemoveAll(u => !u.Contains(".dll.zip"));

                // Remove .zip from all in List
                for (int i = 0; i < Queue.ListBuildbotCoresName.Count; i++)
                {
                    if (Queue.ListBuildbotCoresName[i].Contains(".zip"))
                        Queue.ListBuildbotCoresName[i] = Queue.ListBuildbotCoresName[i].Replace(".zip", "");
                }

                // Sort the Nighlies List, lastest core first
                //ListBuildbotCoresName.Sort(); // Disable Sort


                // -------------------------
                // Buildbot Cores Name + Date List
                // -------------------------
                // Populate empty NameDate List to be modified for Join (Important!)
                for (int i = 0; i < Queue.ListBuildbotCoresName.Count; i++)
                {
                    Queue.ListBuildbotCoresNameDate.Add(Queue.ListBuildbotCoresName[i]);
                }
                // Join Lists Name & Date
                for (int i = 0; i < Queue.ListBuildbotCoresName.Count; i++)
                {
                    Queue.ListBuildbotCoresNameDate[i] = Queue.ListBuildbotCoresName[i] + " " + Queue.ListBuildbotCoresDate[i];
                }

                // Sort
                Queue.ListBuildbotCoresNameDate.Sort();

            }
            catch
            {
                System.Windows.MessageBox.Show("Error: Cannot connect to Server.");
            }
        }


    }
}
