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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Stellar
{
    public partial class Download
    {
        // Web Downloads
        public static ManualResetEvent waiter = new ManualResetEvent(false); // Download one at a time
                                                                             
        public static string extractArgs; // Unzip Arguments

        /// <summary>
        ///     WebClient Custom
        /// </summary>
        public class WebClientCustom : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
                request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                if (request is HttpWebRequest)
                {
                    ((HttpWebRequest)request).KeepAlive = false;
                }

                return request;
            }
        }


        // -----------------------------------------------
        // Download Handlers
        // -----------------------------------------------
        // -------------------------
        // Progress Changed
        // -------------------------
        public static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            VM.MainView.Progress_Value = double.Parse(Math.Truncate(percentage).ToString());
        }

        // -------------------------
        // Download Complete
        // -------------------------
        public static void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Set the waiter Release
            // Must be here
            waiter.Set();
        }


        // -----------------------------------------------
        // Start Download (Method)
        // -----------------------------------------------
        /// <summary>
        ///    Start Process
        /// </summary>
        public static async Task<int> StartDownloadProcess()
        {
            int count = 0;
            await Task.Factory.StartNew(() =>
            {
                using (var wc = new WebClientCustom())
                {
                    waiter = new ManualResetEvent(false);

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    wc.Proxy = null;

                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);

                    // -------------------------
                    // RetroArch Standalone
                    // -------------------------
                    if (VM.MainView.Download_SelectedItem == "Upgrade" ||
                    VM.MainView.Download_SelectedItem == "RetroArch" ||
                    VM.MainView.Download_SelectedItem == "Redist")
                    {
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        RetroArchDownload(wc);
                    }

                    // -------------------------
                    // RetroArch + Cores
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "New Install" ||
                             VM.MainView.Download_SelectedItem == "RA+Cores")
                    {
                        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                        RetroArchDownload(wc);

                        CoresDownload(wc);
                    }

                    // -------------------------
                    // Cores Only
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "Cores" ||
                             VM.MainView.Download_SelectedItem == "New Cores")
                    {
                        CoresDownload(wc);
                    }

                    // -------------------------
                    // Stellar Self-Update
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "Stellar")
                    {
                        StellarDownload(wc);
                    }

                    wc.Dispose();

                } // End WebClient
            });

            return count;
        }

        public static async void StartDownload()
        {
            Task<int> task = StartDownloadProcess();
            int count = await task;
        }


        // -------------------------
        // Stellar Self-Update Download (Method)
        // -------------------------
        public static void StellarDownload(WebClientCustom wc)
        {
            // Headers
            //wc.Headers.Add("Host", "www.example.com");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Self-Update");
            wc.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            //wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //wc.Headers.Add("dnt", "1");
            //wc.Headers.Add(HttpRequestHeader.Upgrade, "1");

            // -------------------------
            // Download
            // -------------------------
            //start a new waiter for next pass (clicking update again)
            waiter = new ManualResetEvent(false); 

            Uri downloadUrl = new Uri(Parse.stellarUrl); //Uri downloadUrl = new Uri("http://127.0.0.1:8888/Stellar.7z"); // TESTING Virtual Server URL

            // Download File
            wc.DownloadFileAsync(downloadUrl, Paths.tempPath + Parse.stellar7z);

            // Progress Info
            VM.MainView.ProgressInfo_Text = "Downloading Stellar...";

            // Wait until download is finished
            waiter.WaitOne();

            // -------------------------
            // Extract
            // -------------------------
            // Progress Info
            VM.MainView.ProgressInfo_Text = "Extracting Stellar...";

            // -------------------------
            // 7-Zip
            // -------------------------
            if (Archiver.extract == "7-Zip")
            {
                List<string> extractArgs = new List<string>() {
                    "/c",
                    "echo Updating Stellar to version " + Convert.ToString(Parse.latestVersion) + ".",
                    "&&",
                    "echo Please wait for program to close.",
                    // Wait
                    "&&",
                    "timeout /t 3",
                    // Extract
                    "&&",
                    "\"" + Archiver.archiver + "\"",
                    "-r -y e",
                    "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    "-o\"" + Paths.appDir + "\"",
                    "*",
                    // Delete Temp
                    "&&",
                    "echo Deleting Temp File",
                    "&&",
                    "del " + "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    // Relaunch Stellar
                    "&&",
                    "\"" + Paths.appDir + "Stellar.exe" + "\"",
                    // Complete
                    "&&",
                    "echo Update Complete"
                };

                // Join List with Spaces
                string arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));

                // Start
                Process.Start("cmd.exe", arguments);

                // Close Stellar before updating exe
                Environment.Exit(0);
            }
            // -------------------------
            // WinRAR
            // -------------------------
            else if (Archiver.extract == "WinRAR")
            {
                List<string> extractArgs = new List<string>() {
                    "/c",
                    "echo Updating Stellar to version " + Convert.ToString(Parse.latestVersion) + ".",
                    "&&",
                    "echo Please wait for program to close.",
                    // Wait
                    "&&",
                    "timeout /t 3",
                    // Extract
                    "&&",
                    "\"" + Archiver.archiver + "\"",
                    "-y x",
                    "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    "*",
                    "\"" + Paths.appDir + "\"",
                    // Delete Temp
                    "&&",
                    "echo Deleting Temp File",
                    "&&",
                    "del " + "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    // Relaunch Stellar
                    "&&",
                    "\"" + Paths.appDir + "Stellar.exe" + "\"",
                    // Complete
                    "&&",
                    "echo Update Complete"
                };

                // Join List with Spaces
                string arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));

                // Start
                Process.Start("cmd.exe", arguments);

                // Close Stellar before updating exe
                Environment.Exit(0);
            }
        }



        // -------------------------
        // RetroArch Download (Method)
        // -------------------------
        public static void RetroArchDownload(WebClientCustom wc)
        {
            // Headers
            //wc.Headers.Add("Host", "www.example.com");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " RetroArch Download");
            wc.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            //wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //wc.Headers.Add("dnt", "1");
            //wc.Headers.Add(HttpRequestHeader.Upgrade, "1");

            // Progress Info
            VM.MainView.ProgressInfo_Text = "Preparing Download...";

            //MessageBox.Show(Parse.nightlyUrl); //debug
            Uri downloadUrl = new Uri(Parse.nightlyUrl);
            //Uri downloadUrl = new Uri("http://127.0.0.1:8888/RetroArch.7z"); // TESTING Virtual Server URL

            // Download File
            wc.DownloadFileAsync(downloadUrl, Paths.tempPath + Parse.nightly7z);

            // Progress Info
            VM.MainView.ProgressInfo_Text = "Downloading RetroArch...";

            // Wait until download is finished
            waiter.WaitOne();

            // -------------------------
            // Extract
            // -------------------------
            // Progress Info
            VM.MainView.ProgressInfo_Text = "Extracting RetroArch...";

            using (Process execExtract = new Process())
            {
                // Allow 0.1 seconds before Extracting Files
                Thread.Sleep(100);

                // Extract -o and Overwrite -y Selected Files -r
                //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                execExtract.StartInfo.UseShellExecute = false;
                execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                execExtract.StartInfo.CreateNoWindow = true;
                execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                execExtract.StartInfo.FileName = Archiver.archiver;

                // -------------------------
                // 7-Zip
                // -------------------------
                if (Archiver.extract == "7-Zip")
                {
                        // -------------------------
                        // New Install
                        // -------------------------
                        if (VM.MainView.Download_SelectedItem == "New Install")
                        {
                            // Extract All Files
                            List<string> extractArgs = new List<string>() {
                                                            "-r -y x",
                                                            "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                            "-o\"" + Paths.retroarchPath + "\"",
                                                            "*",
                                                            };

                            // Join List with Spaces
                            execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                        }
                        // -------------------------
                        // Upgrade
                        // -------------------------
                        else if (VM.MainView.Download_SelectedItem == "Upgrade")
                        {
                            // Extract All Files, Exclude Configs
                            List<string> extractArgs = new List<string>() {
                                                        "-r -y x",
                                                        "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                        "-xr!config -xr!saves -xr!states -xr!retroarch.default.cfg -xr!retroarch.cfg", //exclude files
                                                        "-o\"" + Paths.retroarchPath + "\"",
                                                        "*"
                                                        };

                            // Join List with Spaces
                            execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                        }
                        // -------------------------
                        // Update
                        // -------------------------
                        else if (VM.MainView.Download_SelectedItem == "RetroArch" ||
                                VM.MainView.Download_SelectedItem == "RA+Cores")
                        {
                            // Extract only retroarch.exe & retroarch_debug.exe
                            List<string> extractArgs = new List<string>() {
                                                            "-r -y e",
                                                            "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                            "-o\"" + Paths.retroarchPath + "\"",
                                                            "retroarch.exe retroarch_debug.exe"
                                                        };

                            // Join List with Spaces
                            execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                        }
                        // -------------------------
                        // Redist
                        // -------------------------
                        else if (VM.MainView.Download_SelectedItem == "Redist")
                        {
                            // Extract All Files
                            List<string> extractArgs = new List<string>() {
                                                        "-r -y e",
                                                        "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                        "-o\"" + Paths.retroarchPath + "\"",
                                                        "*"
                                                    };

                            // Join List with Spaces
                            execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                        }
                    }

                    // -------------------------
                    // WinRAR
                    // -------------------------
                    else if (Archiver.extract == "WinRAR")
                    {
                        // -------------------------
                        // New Install
                        // -------------------------
                        if (VM.MainView.Download_SelectedItem == "New Install")
                        {
                            // Extract All Files
                            List<string> extractArgs = new List<string>() {
                                                            "-y x",
                                                            "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                            "*",
                                                            "\"" + Paths.retroarchPath + "\"",
                                                        };

                            // Join List with Spaces
                            execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }
                    // -------------------------
                    // Upgrade
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "Upgrade")
                    {
                        // Extract All Files, Exclude Configs
                        List<string> extractArgs = new List<string>() {
                                                        "-y x",
                                                        "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                        "-xconfig -xsaves -xstates -xretroarch.default.cfg -xretroarch.cfg", //exclude files
                                                        "\"" + Paths.retroarchPath + "\""
                                                    };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));

                    }
                    // -------------------------
                    // Update
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "RetroArch" ||
                            VM.MainView.Download_SelectedItem == "RA+Cores")
                    {
                        // Extract only retroarch.exe & retroarch_debug.exe
                        List<string> extractArgs = new List<string>() {
                                                        "-y x",
                                                        "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                        "retroarch.exe retroarch_debug.exe",
                                                        "\"" + Paths.retroarchPath + "\""
                                                    };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }
                    // -------------------------
                    // Redist
                    // -------------------------
                    else if (VM.MainView.Download_SelectedItem == "Redist")
                    {
                        // Extract only retroarch.exe & retroarch_debug.exe
                        List<string> extractArgs = new List<string>() {
                                                        "-y x",
                                                        "\"" + Paths.tempPath + Parse.nightly7z + "\"",
                                                        "*",
                                                        "\"" + Paths.retroarchPath + "\"",
                                                    };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }
                }

                // Start Extract
                execExtract.Start();
                execExtract.WaitForExit();
                execExtract.Close();


                // -------------------------
                // Set File Time
                // -------------------------
                // Convert Local Time to Server Time
                // This doesn't work in a Method
                DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Universal Time Coordinated");
                //DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                if (File.Exists(Paths.retroarchPath + "retroarch.exe"))
                {
                    // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                    File.SetCreationTime(Paths.retroarchPath + "retroarch.exe", utcTime); //Use Server Timezone, (used to be DateTime.Now)
                    File.SetLastWriteTime(Paths.retroarchPath + "retroarch.exe", utcTime);
                }

                if (File.Exists(Paths.retroarchPath + "retroarch_debug.exe"))
                {
                    // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                    File.SetCreationTime(Paths.retroarchPath + "retroarch_debug.exe", utcTime);  //(used to be DateTime.Now)
                    File.SetLastWriteTime(Paths.retroarchPath + "retroarch_debug.exe", utcTime);
                }
            }

            // -------------------------
            // Delete Temporary Nightly 7z file
            // -------------------------
            using (Process deleteTemp = new Process())
            {
                // Allow 0.1 seconds before Deleting Temporary Files
                Thread.Sleep(100);

                //deleteTemp.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                deleteTemp.StartInfo.UseShellExecute = false;
                deleteTemp.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                deleteTemp.StartInfo.CreateNoWindow = true;
                deleteTemp.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                deleteTemp.StartInfo.FileName = "cmd.exe";
                deleteTemp.StartInfo.Arguments = "/c del " + "\"" + Paths.tempPath + Parse.nightly7z + "\"";
                deleteTemp.Start();
                deleteTemp.WaitForExit();
                deleteTemp.Close();
            }


            // -------------------------
            // RetroArch Download Complete
            // -------------------------
            // Cross Thread
            VM.MainView.ProgressInfo_Text = "RetroArch Complete";
            MainWindow.ClearRetroArchVars();
        }



        // -------------------------
        // Cores Download (Method)
        // -------------------------
        public static void CoresDownload(WebClientCustom wc)
        {
            //waiter = new ManualResetEvent(false);

            // -------------------------
            // New Install
            // -------------------------
            // Change Cores List to All Available Buildbot Cores
            if (VM.MainView.Download_SelectedItem == "New Install")
            {
                Queue.List_CoresToUpdate_Name = Queue.List_BuildbotCores_Name;
                Queue.List_CoresToUpdate_Name.TrimExcess();
            }

            // -------------------------
            // Core To Update Empty Check
            // -------------------------
            //if (Queue.List_CoresToUpdate_Name.Count > 0 &&
            //    Queue.List_CoresToUpdate_Name != null)
            //{

                // -------------------------
                // Rejected
                // -------------------------
                // Remove Rejected Names & Dates from the Update List
                try
                {
                    int updateCount = Queue.List_CoresToUpdate_Name.Count();
                    for (int r = updateCount - 1; r >= 0; r--)
                    {
                        if (Queue.List_RejectedCores_Name.Contains(Queue.List_CoresToUpdate_Name[r]))
                        {
                            // Name
                            if (Queue.List_CoresToUpdate_Name.Count() > r &&
                                Queue.List_CoresToUpdate_Name.Count() != 0) // null check
                            {
                                Queue.List_CoresToUpdate_Name.RemoveAt(r);
                                Queue.List_CoresToUpdate_Name.TrimExcess();
                            }

                            // Date
                            if (Queue.List_UpdatedCores_Date.Count() > r &&
                                Queue.List_UpdatedCores_Date.Count() != 0) // null check
                            {
                                Queue.List_UpdatedCores_Date.RemoveAt(r);
                                Queue.List_UpdatedCores_Date.TrimExcess();
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Problem Excluding cores from Update List.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }

            //debug
            //var messageNames = string.Join(Environment.NewLine, Queue.List_CoresToUpdate_Name);
            //MessageBox.Show(messageNames);
            //var messageDates = string.Join(Environment.NewLine, Queue.List_UpdatedCores_Date);
            //MessageBox.Show(messageDates);

            // Headers
            // wc.Headers.Add("Host", "www.example.com");
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Cores Download");
            wc.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
            //wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
            //wc.Headers.Add("dnt", "1");
            //wc.Headers.Add(HttpRequestHeader.Upgrade, "1");


            // -------------------------
            // Download
            // -------------------------
            for (int i = 0; i < Queue.List_CoresToUpdate_Name.Count; i++) //problem core count & Parse.nightly7z
            {
                //Reset Waiter, Must be here
                waiter.Reset();

                Uri downloadUrl2 = new Uri(Parse.parseCoresUrl + Queue.List_CoresToUpdate_Name[i] + ".zip");
                //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + Queue.List_CoresToUpdate_Name[i] + ".zip"); //TESTING

                // Download File
                wc.DownloadFileAsync(downloadUrl2, Paths.tempPath + Queue.List_CoresToUpdate_Name[i] + ".zip", i);

                // Progress Info
                VM.MainView.ProgressInfo_Text = "Downloading " + Queue.List_CoresToUpdate_Name[i];

                // Wait until download is finished
                waiter.WaitOne();



                // If Last item in List
                //
                if (i == Queue.List_CoresToUpdate_Name.Count - 1)
                {
                    // New Install
                    //
                    if (VM.MainView.Download_SelectedItem == "New Install")
                    {
                        // Progress Info
                        VM.MainView.ProgressInfo_Text = "RetroArch + Cores Install Complete";
                    }

                    // RA+Cores
                    //
                    else if (VM.MainView.Download_SelectedItem == "RA+Cores")
                    {
                        // Progress Info
                        VM.MainView.ProgressInfo_Text = "RetroArch + Cores Update Complete";
                    }

                    // Cores
                    //
                    else if (VM.MainView.Download_SelectedItem == "Cores")
                    {
                        // Progress Info
                        VM.MainView.ProgressInfo_Text = "Cores Update Complete";
                    }

                    // New Cores
                    //
                    else if (VM.MainView.Download_SelectedItem == "New Cores")
                    {
                        // Progress Info
                        VM.MainView.ProgressInfo_Text = "Cores Install Complete";
                    }
                }

                // -------------------------
                // Extract
                // -------------------------
                using (Process execExtract = new Process())
                {
                    // Allow 0.1 seconds before extraction
                    Thread.Sleep(100);

                    // Extract -o and Overwrite -y Selected Files -r
                    //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                    execExtract.StartInfo.UseShellExecute = false;
                    execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                    execExtract.StartInfo.CreateNoWindow = true;
                    execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                    execExtract.StartInfo.FileName = Archiver.archiver;

                    // -------------------------
                    // 7-Zip
                    // -------------------------
                    if (Archiver.extract == "7-Zip")
                    {
                        List<string> extractArgs = new List<string>() {
                                                    "-y e",
                                                    "\"" + Paths.tempPath + Queue.List_CoresToUpdate_Name[i] + ".zip" + "\"",
                                                    "-o\"" + Paths.coresPath + "\"",
                                                };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }
                    // -------------------------
                    // WinRAR
                    // -------------------------
                    else if (Archiver.extract == "WinRAR")
                    {
                        List<string> extractArgs = new List<string>() {
                                                        "-y x",
                                                        "\"" + Paths.tempPath + Queue.List_CoresToUpdate_Name[i] + ".zip" + "\"",
                                                        "\"" + Paths.coresPath + "\"",
                                                    };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }

                    execExtract.Start();
                    execExtract.WaitForExit();
                    execExtract.Close();


                    // -------------------------
                    // Set File Time
                    // -------------------------
                    // Default UTC
                    DateTime buildbotServerTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);

                    // Try to set to BuildBot index-extended Server Time
                    if (Queue.List_UpdatedCores_Date.Count() > i && Queue.List_UpdatedCores_Date.Count() != 0) //index range check
                    {
                        if (!string.IsNullOrEmpty(Queue.List_UpdatedCores_Date[i]))
                        {
                            buildbotServerTime = Convert.ToDateTime(Queue.List_UpdatedCores_Date[i]);
                        }
                    }

                    // Convert Local Time to Server Time
                    // This doesn't work in a Method
                    // DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    // TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    // DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // .AddHours(12) Needs to be 6-12 hours ahead to be more recent than server? 24 Hour AM/PM Problem?

                    // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                    if (File.Exists(Paths.coresPath + Queue.List_CoresToUpdate_Name[i]))
                    {
                        File.SetCreationTime(Paths.coresPath + Queue.List_CoresToUpdate_Name[i], buildbotServerTime); // Created Date Time = Now, (used to be DateTime.Now)
                        File.SetLastWriteTime(Paths.coresPath + Queue.List_CoresToUpdate_Name[i], buildbotServerTime); //maybe disable modified date?
                    }
                }

                // Delete Temporary Nightly 7z file
                //
                using (Process deleteTemp = new Process())
                {
                    // Allow 0.1 seconds before Deleting Temporary Files
                    Thread.Sleep(100);

                    //deleteTemp.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                    deleteTemp.StartInfo.UseShellExecute = false;
                    deleteTemp.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                    deleteTemp.StartInfo.CreateNoWindow = true;
                    deleteTemp.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                    deleteTemp.StartInfo.FileName = "cmd.exe";
                    deleteTemp.StartInfo.Arguments = "/c del " + "\"" + Paths.tempPath + Queue.List_CoresToUpdate_Name[i] + ".zip" + "\"";
                    deleteTemp.Start();
                    deleteTemp.WaitForExit();
                    deleteTemp.Close();
                }


                // If Last item in List
                //
                if (i == Queue.List_CoresToUpdate_Name.Count - 1)
                {
                    // Write Log Append
                    //
                    Log.WriteLog();

                    // Clear list to prevent doubling up
                    //
                    MainWindow.ClearRetroArchVars();
                    MainWindow.ClearCoresVars();
                    MainWindow.ClearLists();

                    // Clear Checklist Checkbox Rejected Cores
                    //
                    Queue.List_RejectedCores_Name.Clear();
                    Queue.List_RejectedCores_Name.TrimExcess();
                }

            } // end for loop

            // -------------------------
            // Error: Cores Empty
            // -------------------------
            //else
            //{
            //    MessageBox.Show("No cores selected.",
            //                    "Error",
            //                    MessageBoxButton.OK,
            //                    MessageBoxImage.Error);
            //}
        }


    }

}
