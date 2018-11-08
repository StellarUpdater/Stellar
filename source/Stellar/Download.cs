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
using System.Windows.Forms;
using System.Windows.Threading;

namespace Stellar
{
    public partial class Download
    {
        private static MainWindow mainwindow = ((MainWindow)System.Windows.Application.Current.MainWindow);

        // Thread
        //public static Thread worker = null;

        // Web Downloads
        public static ManualResetEvent waiter = new ManualResetEvent(false); // Download one at a time
                                                                             
        public static string progressInfo; // Progress Label Info
        
        public static string extractArgs; // Unzip Arguments



        // -----------------------------------------------
        // Download Handlers
        // -----------------------------------------------
        // -------------------------
        // Progress Changed
        // -------------------------
        public static void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Progress Info
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                ViewModel vm = mainwindow.DataContext as ViewModel;
                vm.ProgressInfo_Text = progressInfo;
            });

            // Progress Bar
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                //ViewModel vm = mainwindow.DataContext as ViewModel;

                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                mainwindow.progressBar.Value = double.Parse(Math.Truncate(percentage).ToString());
                //vm.CurrentProgress_Value = double.Parse(Math.Truncate(percentage).ToString());
            });

            //DownloadProgressChanged(mainwindow, e);
        }

        // -------------------------
        // Download Complete
        // -------------------------
        public static void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            

            // Set the waiter Release
            // Must be here
            waiter.Set();

            // Progress Info
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                ViewModel vm = mainwindow.DataContext as ViewModel;
                vm.ProgressInfo_Text = progressInfo;
            });

            //DownloadComplete(mainwindow);
        }


        // -----------------------------------------------
        // Downloads
        // -----------------------------------------------

        // -----------------------------------------------
        // Start Download (Method)
        // -----------------------------------------------
        public static void StartDownload(ViewModel vm)
        {
            // -------------------------
            // RetroArch Standalone
            // -------------------------
            if (vm.Download_SelectedItem == "Upgrade"
                || vm.Download_SelectedItem == "RetroArch"
                || vm.Download_SelectedItem == "Redist")
            {
                // Start New Thread
                //
                Thread worker = new Thread(() =>
                {
                    RetroArchDownload(vm);
                });
                worker.IsBackground = true;
                

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // RetroArch + Cores
            // -------------------------
            else if (vm.Download_SelectedItem == "New Install"
                || vm.Download_SelectedItem == "RA+Cores")
            {
                // Start New Thread
                //
                Thread worker = new Thread(() =>
                {
                    RetroArchDownload(vm);

                    CoresDownload(vm);
                });
                worker.IsBackground = true;

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // Cores Only
            // -------------------------
            else if (vm.Download_SelectedItem == "Cores"
                || vm.Download_SelectedItem == "New Cores")
            {
                // Start New Thread
                Thread worker = new Thread(() =>
                {
                    CoresDownload(vm);
                });
                worker.IsBackground = true;

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // Stellar Self-Update
            // -------------------------
            else if (vm.Download_SelectedItem == "Stellar")
            {
                // Start New Thread
                Thread worker = new Thread(() =>
                {
                    StellarDownload(vm);
                });

                // Start Download Thread
                //
                worker.Start();
            }

        }


        // -------------------------
        // Stellar Self-Update Download (Method)
        // -------------------------
        public static void StellarDownload(ViewModel vm)
        {
            WebClient wc = new WebClient();

            wc.Proxy = null;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // UserAgent Header
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Self-Update");

            // -------------------------
            // Download
            // -------------------------
            waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)

            Uri downloadUrl = new Uri(Parse.stellarUrl); // Parse.stellarUrl = Version + Parse.stellar7z
            //Uri downloadUrl = new Uri("http://127.0.0.1:8888/Stellar.7z"); // TESTING Virtual Server URL
            //Async
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            wc.DownloadFileAsync(downloadUrl, Paths.tempPath + Parse.stellar7z);

            // Progress Info
            progressInfo = "Downloading Stellar...";

            waiter.WaitOne();


            // -------------------------
            // Extract
            // -------------------------
            // Progress Info
            progressInfo = "Extracting Stellar...";

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

            wc.Dispose();
        }



        // -------------------------
        // RetroArch Download (Method)
        // -------------------------
        public static void RetroArchDownload(ViewModel vm)
        {
            WebClient wc = new WebClient();

            wc.Proxy = null;

            // -------------------------
            // Download
            // -------------------------
            //mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            //{
            //waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)
            //});

            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // UserAgent Header
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Self-Update");

            progressInfo = "Preparing Download...";

            //MessageBox.Show(Parse.nightlyUrl); //debug
            Uri downloadUrl = new Uri(Parse.nightlyUrl); // Parse.nightlyUrl = x84/x86_64 + Parse.nightly7z

            //Uri downloadUrl = new Uri("http://127.0.0.1:8888/RetroArch.7z"); // TESTING Virtual Server URL

            //Async
            //wc.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            //wc.Headers.Add("Accept-Encoding", "gzip,deflate");

            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            wc.DownloadFileAsync(downloadUrl, Paths.tempPath + Parse.nightly7z);

            // Progress Info
            //mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            //{
            progressInfo = "Downloading RetroArch...";

            waiter.WaitOne();
            //});

            // -------------------------
            // Extract
            // -------------------------
            // Progress Info
            //mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            //{
            progressInfo = "Extracting RetroArch...";

            //});

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
                    if (vm.Download_SelectedItem == "New Install")
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
                    else if (vm.Download_SelectedItem == "Upgrade")
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
                    else if (vm.Download_SelectedItem == "RetroArch" 
                            || vm.Download_SelectedItem == "RA+Cores")
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
                    else if (vm.Download_SelectedItem == "Redist")
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
                    if (vm.Download_SelectedItem == "New Install")
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
                    else if (vm.Download_SelectedItem == "Upgrade")
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
                    else if (vm.Download_SelectedItem == "RetroArch"
                            || vm.Download_SelectedItem == "RA+Cores")
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
                    else if (vm.Download_SelectedItem == "Redist")
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
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                vm.ProgressInfo_Text = "RetroArch Complete";
                MainWindow.ClearRetroArchVars();
            });

            // End Thread
            //worker.Abort();

            wc.Dispose();
        }



        // -------------------------
        // Cores Download (Method)
        // -------------------------
        public static void CoresDownload(ViewModel vm)
        {
            //waiter = new ManualResetEvent(false);

            // -------------------------
            // New Install
            // -------------------------
            // Change Cores List to All Available Buildbot Cores
            // Cross Thread
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                if (vm.Download_SelectedItem == "New Install")
                {
                    Queue.List_UpdatedCores_Name = Queue.List_BuildbotCores_Name;
                    Queue.List_UpdatedCores_Name.TrimExcess();
                }
            });


            // -------------------------
            // Rejected
            // -------------------------
            // Remove Rejected Core Names from the Update List
            //Queue.List_UpdatedCores_Name = Queue.List_UpdatedCores_Name.Except(Queue.List_RejectedCores_Name).ToList();
            //Queue.List_UpdatedCores_Name.TrimExcess();

            //// Remove Rejected Names & Dates from the Update List
            try
            {
                int updateCount = Queue.List_UpdatedCores_Name.Count();
                for (int r = updateCount - 1; r >= 0; r--)
                {
                    if (Queue.List_RejectedCores_Name.Contains(Queue.List_UpdatedCores_Name[r]))
                    {
                        // Name
                        if (Queue.List_UpdatedCores_Name.Count() > r
                            && Queue.List_UpdatedCores_Name.Count() != 0) // null check
                        {
                            Queue.List_UpdatedCores_Name.RemoveAt(r);
                            Queue.List_UpdatedCores_Name.TrimExcess();
                        }

                        // Date
                        if (Queue.List_UpdatedCores_Date.Count() > r
                            && Queue.List_UpdatedCores_Date.Count() != 0) // null check
                        {
                            Queue.List_UpdatedCores_Date.RemoveAt(r);
                            Queue.List_UpdatedCores_Date.TrimExcess();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Error: Problem Excluding cores from Update List.");
            }


            //debug
            //var messageNames = string.Join(Environment.NewLine, Queue.List_UpdatedCores_Name);
            //MessageBox.Show(messageNames);
            //var messageDates = string.Join(Environment.NewLine, Queue.List_UpdatedCores_Date);
            //MessageBox.Show(messageDates);

            WebClient wc = new WebClient();

            wc.Proxy = null;

            //ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // UserAgent Header
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Self-Update");

            // -------------------------
            // Download
            // -------------------------
            for (int i = 0; i < Queue.List_UpdatedCores_Name.Count; i++) //problem core count & Parse.nightly7z
            {
                //Reset Waiter, Must be here
                waiter.Reset();

                Uri downloadUrl2 = new Uri(Parse.parseCoresUrl + Queue.List_UpdatedCores_Name[i] + ".zip");
                //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + Queue.List_UpdatedCores_Name[i] + ".zip"); //TESTING
                //Async
                wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc.DownloadFileAsync(downloadUrl2, Paths.tempPath + Queue.List_UpdatedCores_Name[i] + ".zip", i);

                // Progress Info
                progressInfo = "Downloading " + Queue.List_UpdatedCores_Name[i];

                //Wait until download is finished
                waiter.WaitOne();



                // If Last item in List
                //
                if (i == Queue.List_UpdatedCores_Name.Count - 1)
                {
                    // Cross Thread
                    mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
                    {
                        // New Install
                        //
                        if (vm.Download_SelectedItem == "New Install")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Install Complete";
                            vm.ProgressInfo_Text = progressInfo;
                        }

                        // RA+Cores
                        //
                        else if (vm.Download_SelectedItem == "RA+Cores")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Update Complete";
                            vm.ProgressInfo_Text = progressInfo;
                        }

                        // Cores
                        //
                        else if (vm.Download_SelectedItem == "Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Update Complete";
                            vm.ProgressInfo_Text = progressInfo;
                        }

                        // New Cores
                        //
                        else if (vm.Download_SelectedItem == "New Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Install Complete";
                            vm.ProgressInfo_Text = progressInfo;
                        }
                    });

                    wc.Dispose();
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
                                "\"" + Paths.tempPath + Queue.List_UpdatedCores_Name[i] + ".zip" + "\"",
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
                                "\"" + Paths.tempPath + Queue.List_UpdatedCores_Name[i] + ".zip" + "\"",
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
                    //DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // .AddHours(12) Needs to be 6-12 hours ahead to be more recent than server? 24 Hour AM/PM Problem?

                    // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                    if (File.Exists(Paths.coresPath + Queue.List_UpdatedCores_Name[i]))
                    {
                        File.SetCreationTime(Paths.coresPath + Queue.List_UpdatedCores_Name[i], buildbotServerTime); // Created Date Time = Now, (used to be DateTime.Now)
                        File.SetLastWriteTime(Paths.coresPath + Queue.List_UpdatedCores_Name[i], buildbotServerTime); //maybe disable modified date?
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
                    deleteTemp.StartInfo.Arguments = "/c del " + "\"" + Paths.tempPath + Queue.List_UpdatedCores_Name[i] + ".zip" + "\"";
                    deleteTemp.Start();
                    deleteTemp.WaitForExit();
                    deleteTemp.Close();
                }


                // If Last item in List
                //
                if (i == Queue.List_UpdatedCores_Name.Count - 1)
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
        }


    }

}
