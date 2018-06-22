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

using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace Stellar
{
    public partial class Parse
    {
        public static string page;
        public static string element;

        public static Version latestVersion; // Stellar GitHub Latest Version
        public static string latestBuildPhase; // Alpha, Beta, Stable
        public static string[] splitVersionBuildPhase;

        public static string stellar7z; // Self-Update File
        public static string stellarUrl; // Self-Update Url

        public static string nightly7z; // The Parsed Dated 7z Nightly Filename
        public static string nightlyUrl; // Download URL + Dated 7z Filename

        public static string parseUrl;
        public static string parseGitHubUrl = "https://github.com/StellarUpdater/Stellar/releases/"; // Self-Update
        public static string indexextendedUrl = string.Empty; // index-extended Cores Text File
        public static string parseCoresUrl = string.Empty; // Buildbot Cores URL to be parsed
        public static string libretro_x86; // Download URL 32-bit
        public static string libretro_x86_64; // Download URL 64-bit
        public static string libretro_x86_64_w32; // Download URL 64-bit w32


        // -------------------------
        // Check For Internet Connection
        // -------------------------
        public static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (client.OpenRead("http://clients3.google.com/generate_204"))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // -----------------------------------------------
        // Parse GitHub Release Tags Page HTML
        // -----------------------------------------------
        public static void ParseGitHubReleases(MainWindow mainwindow)
        {
            // Update Version Number at 4 places, Format 0.0.0.0
            // MainWindow CurrentVersion
            // GitHub Release tag
            // GitHub ./version file
            // Stellar Website Download Links

            WebClient wc = new WebClient();

            // -------------------------
            // Update Selected
            // -------------------------
            if (CheckForInternetConnection() == true)
            {
                // Parse the HTML Page from parseUrl
                //
                string parseLatestVersion = string.Empty;

                try
                {
                    parseLatestVersion = wc.DownloadString("https://raw.githubusercontent.com/StellarUpdater/Stellar/master/.version");
                }
                catch
                {
                    MessageBox.Show("GitHub version not found.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    return;
                }


                //Split Version & Build Phase by dash
                //
                if (!string.IsNullOrEmpty(parseLatestVersion)) //null check
                {
                    try
                    {
                        // Split Version and Build Phase
                        splitVersionBuildPhase = Convert.ToString(parseLatestVersion).Split('-');

                        // Set Version Number
                        latestVersion = new Version(splitVersionBuildPhase[0]); //number
                        latestBuildPhase = splitVersionBuildPhase[1]; //beta
                    }
                    catch
                    {
                        MessageBox.Show("Error reading version.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);

                        return;
                    }

                    // Debug
                    //MessageBox.Show(Convert.ToString(latestVersion));
                    //MessageBox.Show(latestBuildPhase);

                }
                // Version is Null
                else
                {
                    MessageBox.Show("GitHub version returned empty.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    return;
                }
            }
            else
            {
                //MainWindow.ready = false;
                MessageBox.Show("Could not detect Internet Connection.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);

                return;
            }


            // -------------------------
            // Stellar Selected
            // -------------------------
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "Stellar")
            {
                stellar7z = "Stellar.7z";
                stellarUrl = "https://github.com/StellarUpdater/Stellar/releases/download/" + "v" + Convert.ToString(latestVersion) + "-" + latestBuildPhase + "/" + stellar7z;
                // .../0.8.5.3-beta/Stellar.7z
            }

            wc.Dispose();
        }


        // -----------------------------------------------
        // Download Server Page
        // -----------------------------------------------
        public static void DownloadServerPage(MainWindow mainwindow)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // -------------------------
            // If GZip disabled, switch to Uncomprssed
            // -------------------------
            //try
            //{
            //    // -------------------------
            //    // GZip
            //    // -------------------------

            //    //MessageBox.Show(parseUrl); //debug

            //    // Parse the HTML Page from parseUrl
            //    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(parseUrl);
            //    req.UserAgent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
            //    req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            //    req.Headers.Add("Accept-Encoding", "gzip,deflate");

            //    GZipStream zip = new GZipStream(req.GetResponse().GetResponseStream(), CompressionMode.Decompress);
            //    //zip.Position = 0;
            //    StreamReader reader = new StreamReader(zip);

            //    //MessageBox.Show("Using GZip"); //debug
            //    mainwindow.textBlockProgressInfo.Text = "Using GZip";

            //    page = reader.ReadToEnd(); // Error here if uncompressed, triggers catch

            //    // Close
            //    req.Abort();
            //    zip.Dispose();
            //    reader.Dispose();

            //    //MessageBox.Show("complete"); //debug
            //}

            //// GZip Disabled
            //catch
            //{
                //MessageBox.Show("Page Reader Error Detected"); //debug

                // -------------------------
                // Uncompressed
                // -------------------------

                //MessageBox.Show("Using Uncompressed"); //debug
                //mainwindow.textBlockProgressInfo.Text = "Using Uncompressed";

                // Switch to Uncompressed download method
                //WebClient wc = new WebClient();
                //wc.Headers[HttpRequestHeader.UserAgent] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                //page = wc.DownloadString(parseUrl);
                //wc.Dispose();

                //MessageBox.Show("complete"); //debug



                // Parse the HTML Page from parseUrl
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(parseUrl);
                req.UserAgent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //req.Headers.Add("Accept-Encoding", "gzip,deflate");

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    page = sr.ReadToEnd();
                }

                //MessageBox.Show(page); //debug
            //}
        }



        // -----------------------------------------------
        // Create Nightlies List
        // -----------------------------------------------
        public static void FetchNightlyFile(MainWindow mainwindow)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            try
            {
                // Parse the HTML Page from parseUrl
                //DownloadServerPage(mainwindow);

                // -------------------------
                // Parse the HTML Page from parseUrl
                // -------------------------
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(parseUrl);
                req.UserAgent = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                //req.Headers.Add("Accept-Encoding", "gzip,deflate");

                HttpWebResponse response = (HttpWebResponse)req.GetResponse();

                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    //page = sr.ReadToEnd();

                    while (sr.Peek() >= 0)
                    {
                        page = sr.ReadToEnd();
                    }
                }


                // -------------------------
                // Build Nightlies Dated Files List
                // -------------------------
                // HTML Tag containing Dated 7z, (.*?) is the text to keep
                element = "<a href=\"/nightly/windows/" + Paths.buildbotArchitecture + "/(.*?)\">";

                // Find 7z Matches in HTML tags
                MatchCollection matches = Regex.Matches(page, element);

                //var message = string.Join(Environment.NewLine, Queue.NightliesList); //debug
                //MessageBox.Show(message); //debug

                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        Queue.NightliesList.Add(m.Groups[1].Value);
                    }

                    //MessageBox.Show("Matches found: {0}", string.Join(Environment.NewLine, matches.Count)); //debug

                    // Remove from the List all 7z files that do not contain _RetroArch.7z (filters out unwanted)
                    Queue.NightliesList.RemoveAll(u => !u.Contains("_RetroArch.7z"));

                    Queue.NightliesList.TrimExcess();
                }

                try
                {
                    // Sort the Nighlies List, lastest 7z is first
                    Queue.NightliesList.Sort(); //do not disable this sort

                    // Get Lastest Element of Nightlies List 
                    nightly7z = Queue.NightliesList.Last();
                }
                catch
                {
                    MainWindow.ready = false;

                    // Ignore Error Message if Server Auto (Will display on second pass when switch turns on)
                    if ((string)mainwindow.cboServer.SelectedItem != "auto")
                    {
                        MessageBox.Show("Problem creating RetroArch list from HTML. \n\nTry another server such as raw or buildbot.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }


                    return;
                }
            }
            catch
            {
                MainWindow.ready = false;

                // Ignore Error Message if Server Auto (Will display on second pass when switch turns on)
                if ((string)mainwindow.cboServer.SelectedItem != "auto")
                {
                    MessageBox.Show("Problem connecting to Network.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

                return;
            }
        }



        // -----------------------------------------------
        // Parse Builbot Page HTML
        // -----------------------------------------------
        public static void ParseBuildbotPage(MainWindow mainwindow)
        {
            // If No Internet Connect, program will crash.
            // Try Catch Errors

            // -------------------------
            // New Install Selected
            // -------------------------
            // RetroArch.exe
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "RetroArch"
                || (string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores")
            {
                // Clear RetroArch Nightlies List
                if (Queue.NightliesList != null)
                {
                    Queue.NightliesList.Clear();
                    Queue.NightliesList.TrimExcess();
                }

                // -------------------------
                // auto Server
                // -------------------------
                if ((string)mainwindow.cboServer.SelectedItem == "auto")
                {
                    // -------------------------
                    // Default Raw
                    // -------------------------

                    // Progress Info
                    mainwindow.textBlockProgressInfo.Text = "Using raw server";

                    // Create List
                    FetchNightlyFile(mainwindow);

                    // -------------------------
                    // Switch to Buildbot
                    // -------------------------
                    if (Queue.NightliesList.Count == 0)
                    {
                        Download.waiter.Reset();
                        Download.waiter = new ManualResetEvent(false);

                        // Switch Server
                        mainwindow.cboServer.SelectedItem = "buildbot";

                        // Progress Info
                        mainwindow.textBlockProgressInfo.Text = "Using buildbot server";

                        // Create List
                        FetchNightlyFile(mainwindow);
                    }
                }

                // -------------------------
                // raw Server
                // -------------------------
                else if ((string)mainwindow.cboServer.SelectedItem == "raw")
                {
                    // Progress Info
                    mainwindow.textBlockProgressInfo.Text = "Using raw server";

                    // Create List
                    FetchNightlyFile(mainwindow);
                }

                // -------------------------
                // buildbot Server
                // -------------------------
                else if ((string)mainwindow.cboServer.SelectedItem == "buildbot")
                {
                    // Progress Info
                    mainwindow.textBlockProgressInfo.Text = "Using buildbot server";

                    // Create List
                    FetchNightlyFile(mainwindow);
                }
            }


            // -------------------------
            // New Install Selected
            // -------------------------
            // RetroArch.exe
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
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

                //MessageBox.Show(libretro_x86_64); //debug
                //MessageBox.Show(nightly7z); //debug
                //MessageBox.Show(nightlyUrl); //debug
            }


            // Prevents Threading Crash
            Download.waiter = new ManualResetEvent(false);
        }



        // -----------------------------------------------
        // Parse Builbot Cores Page HTML
        // -----------------------------------------------
        public static void ParseBuildbotCoresIndex(MainWindow mainwindow)
        {
            WebClient wc = new WebClient();

            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // -------------------------
            // Download
            // -------------------------
            // If No Internet Connect, program will crash. Use Try & Catch to display Error.
            string buildbotCoresIndex = string.Empty;

            try
            {
                // index-extended cores text file
                //Download.webclient.Headers["Accept-Encoding"] = "gzip,deflate";
                buildbotCoresIndex = wc.DownloadString(indexextendedUrl);

                // Trim ending linebreak
                buildbotCoresIndex = buildbotCoresIndex.TrimEnd('\n');
            }
            catch
            {
                MainWindow.ready = false;
                MessageBox.Show("Buildbot index-extended file may be offline.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }


            if (!string.IsNullOrEmpty(buildbotCoresIndex)) //null check
            {
                try
                {
                    // index-extended to array
                    var lines = buildbotCoresIndex.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(tag => tag.Trim())
                        .Where(tag => !string.IsNullOrEmpty(tag))
                        .ToArray();

                    // -------------------------
                    // Corrupt Fix
                    // -------------------------
                    // Remove line if it does not contain a space, might be corrupt
                    for (int i = lines.Count() - 1; i >= 0; --i)
                    {
                        if (lines.Count() > i && lines.Count() != 0) // null check
                        {
                            if (!lines[i].Contains(' '))
                            {
                                lines = lines.Where(w => w != lines[i]).ToArray();
                            }
                        }
                    }

                    // -------------------------
                    // Sort
                    // -------------------------
                    // Split the index-extended by LineBreak Array
                    // Sort the Array by Core Name (3rd word in Line)
                    //var lines = buildbotCoresIndex.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                    //    .Select(tag => tag.Trim())
                    //    .Where(tag => !string.IsNullOrEmpty(tag))
                    //    .OrderBy(s => s.Split(' ')[2]) //sort by Name
                    //    .ToArray();
                    lines = lines.OrderBy(s => s.Split(' ')[2]).ToArray();

                    // Split the index-extended into 3 Lists, BuildbotCoreDate, BuildbotID, BuildbotCoresName
                    foreach (string line in lines)
                    {
                        string[] arr = line.Split(' ');

                        if (arr[0] != null)
                        {
                            Queue.List_BuildbotCores_Date.Add(arr[0]);
                            Queue.List_BuildbotCores_Date.TrimExcess();
                        }

                        //Queue.ListBuildbotID.Add(arr[1]);
                        //Queue.ListBuildbotID.TrimExcess();

                        if (arr[2] != null)
                        {
                            Queue.List_BuildbotCores_Name.Add(arr[2]);
                            Queue.List_BuildbotCores_Name.TrimExcess();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Problem sorting Buildbot Cores.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }


                try
                {
                    // -------------------------
                    // Modify
                    // -------------------------
                    // Remove from the List all that do not contain .dll.zip (filters out unwanted)
                    Queue.List_BuildbotCores_Name.RemoveAll(u => !u.Contains(".dll.zip"));
                    Queue.List_BuildbotCores_Name.TrimExcess();

                    // Remove .zip from all in List
                    for (int i = 0; i < Queue.List_BuildbotCores_Name.Count; i++)
                    {
                        if (Queue.List_BuildbotCores_Name[i].Contains(".zip"))
                            Queue.List_BuildbotCores_Name[i] = Queue.List_BuildbotCores_Name[i].Replace(".zip", "");
                    }
                }
                catch
                {
                    MessageBox.Show("Problem modifying Buildbot Cores.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                

                try
                {
                    // -------------------------
                    // Combine
                    // -------------------------
                    // Join Lists Name & Date
                    for (int i = 0; i < Queue.List_BuildbotCores_Name.Count; i++)
                    {
                        Queue.List_BuildbotCores_NameDate.Add(Queue.List_BuildbotCores_Name[i] + " " + Queue.List_BuildbotCores_Date[i]);
                    }
                }
                catch
                {
                    MessageBox.Show("Problem combining Buildbot Cores.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
                

                // -------------------------
                // Sort Correction
                // -------------------------
                Queue.List_BuildbotCores_NameDate.Sort();
                Queue.List_BuildbotCores_NameDate.TrimExcess();
            }

            // index-extended null
            else
            {
                MainWindow.ready = false;
                MessageBox.Show("Cores list is empty or failed to download index-extended.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            // Prevents Threading Crash
            Download.waiter = new ManualResetEvent(false);

            wc.Dispose();
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
                Queue.List_PcCores_Name = Directory.GetFiles(Paths.coresPath, "*_libretro.dll") //match ending of a core name //Try EnumerateFiles
                        .Select(System.IO.Path.GetFileName)
                        .ToList();

                // Add Core Modified Dates to List
                // Extracts original File Modified Date when overwriting
                //
                Queue.List_PcCores_Date = Directory.GetFiles(Paths.coresPath, "*_libretro.dll") //match ending of a core name
                        .Select(p => File.GetLastWriteTime(p)
                        .ToString("yyyy-MM-dd"))
                        .ToList();
            }
            catch
            {
                MainWindow.ready = false;
                MessageBox.Show("Problem scanning PC Cores Name & Dates.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }

            // Popup Error Message if PC Cores Name List has no items 0
            if (Queue.List_PcCores_Name.Count == 0
                && (string)mainwindow.comboBoxDownload.SelectedItem != "New Install" // Ignore
                && (string)mainwindow.comboBoxDownload.SelectedItem != "New Cores") // Ignore
            {
                MainWindow.ready = false;
                MessageBox.Show("Cores not found. \n\nPlease select your RetroArch main folder.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }

            // -------------------------
            // PC Cores Join Name + Date List
            // -------------------------
            // Join Lists PC Name + PC Date (Formatted)
            for (int i = 0; i < Queue.List_PcCores_Name.Count; i++)
            {
                Queue.List_PcCores_NameDate.Add(Queue.List_PcCores_Name[i] + " " + Queue.List_PcCores_Date[i]);
            }

            // -------------------------
            // Sort Correction
            // -------------------------
            Queue.List_PcCores_NameDate.Sort();
            //Queue.List_PcCores_NameDate.TrimExcess();
        }

    }
}
