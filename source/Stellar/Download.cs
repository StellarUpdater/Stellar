using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

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
    public partial class Download
    {
        private static MainWindow mainwindow = ((MainWindow)System.Windows.Application.Current.MainWindow);

        // Web Downloads
        public static WebClient wc = new WebClient();
        public static WebClient wc2 = new WebClient();
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
                mainwindow.labelProgressInfo.Content = progressInfo;
            });

            // Progress Bar
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                mainwindow.progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }

        // -------------------------
        // Download Complete
        // -------------------------
        public static void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Set the waiter Release
            // Must be here
            waiter.Set();

            //Progress Info
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                mainwindow.labelProgressInfo.Content = progressInfo;
            });
        }


        // -----------------------------------------------
        // Downloads
        // -----------------------------------------------

        // -----------------------------------------------
        // Start Download (Method)
        // -----------------------------------------------
        public static void StartDownload(MainWindow mainwindow)
        {
            // -------------------------
            // RetroArch Standalone
            // -------------------------
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "Upgrade"
                || (string)mainwindow.comboBoxDownload.SelectedItem == "RetroArch"
                || (string)mainwindow.comboBoxDownload.SelectedItem == "Redist")
            {
                // Start New Thread
                //
                Thread worker = new Thread(() =>
                {
                    RetroArchDownload(mainwindow);
                });

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // RetroArch + Cores
            // -------------------------
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install"
                || (string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores")
            {
                // Start New Thread
                //
                Thread worker = new Thread(() =>
                {
                    RetroArchDownload(mainwindow);

                    CoresDownload(mainwindow);
                });

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // Cores Only
            // -------------------------
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Cores"
                || (string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
            {
                // Start New Thread
                Thread worker = new Thread(() =>
                {
                    CoresDownload(mainwindow);
                });

                // Start Download Thread
                //
                worker.Start();
            }

            // -------------------------
            // Stellar Self-Update
            // -------------------------
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Stellar")
            {
                // Start New Thread
                Thread worker = new Thread(() =>
                {
                    StellarDownload(mainwindow);
                });

                // Start Download Thread
                //
                worker.Start();
            }

        }


        // -------------------------
        // Stellar Self-Update Download (Method)
        // -------------------------
        public static void StellarDownload(MainWindow mainwindow)
        {
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
                    "-o\"" + Paths.currentDir + "\"",
                    "*",
                    // Delete Temp
                    "&&",
                    "echo Deleting Temp File",
                    "&&",
                    "del " + "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    // Relaunch Stellar
                    "&&",
                    "\"" + Paths.currentDir + "Stellar.exe" + "\"",
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
                    "\"" + Paths.currentDir + "\"",
                    // Delete Temp
                    "&&",
                    "echo Deleting Temp File",
                    "&&",
                    "del " + "\"" + Paths.tempPath + Parse.stellar7z + "\"",
                    // Relaunch Stellar
                    "&&",
                    "\"" + Paths.currentDir + "Stellar.exe" + "\"",
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
        public static void RetroArchDownload(MainWindow mainwindow)
        {
            // -------------------------
            // Download
            // -------------------------
            waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)

            Uri downloadUrl = new Uri(Parse.nightlyUrl); // Parse.nightlyUrl = x84/x86_64 + Parse.nightly7z
            //Uri downloadUrl = new Uri("http://127.0.0.1:8888/RetroArch.7z"); // TESTING Virtual Server URL
            //Async
            wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
            wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
            wc.DownloadFileAsync(downloadUrl, Paths.tempPath + Parse.nightly7z);

            // Progress Info
            progressInfo = "Downloading RetroArch...";

            waiter.WaitOne();


            // -------------------------
            // Extract
            // -------------------------
            // Progress Info
            progressInfo = "Extracting RetroArch...";

            using (Process execExtract = new Process())
            {
                // Allow 0.1 seconds before Extracting Files
                Thread.Sleep(100);

                //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                execExtract.StartInfo.UseShellExecute = false;
                execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                execExtract.StartInfo.CreateNoWindow = true;
                execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                execExtract.StartInfo.FileName = Archiver.archiver;
                // Extract -o and Overwrite -y Selected Files -r

                // -------------------------
                // 7-Zip
                // -------------------------
                if (Archiver.extract == "7-Zip")
                {
                    // -------------------------
                    // New Install
                    // -------------------------
                    if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "New Install")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "Upgrade")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "RetroArch" 
                            || (string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "RA+Cores")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "Redist")
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
                    if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "New Install")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "Upgrade")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "RetroArch"
                            || (string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "RA+Cores")
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
                    else if ((string)mainwindow.comboBoxDownload.Dispatcher.Invoke((() => { return mainwindow.comboBoxDownload.SelectedItem; })) == "Redist")
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
                mainwindow.labelProgressInfo.Content = "RetroArch Complete";
                MainWindow.ClearRetroArchVars();
            }); 
        }



        // -------------------------
        // Cores Download (Method)
        // -------------------------
        public static void CoresDownload(MainWindow mainwindow)
        {
            waiter = new ManualResetEvent(false);

            // -------------------------
            // New Install
            // -------------------------
            // Change Cores List to All Available Buildbot Cores
            // Cross Thread
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
                {
                    Queue.ListUpdatedCoresName = Queue.ListBuildbotCoresName;
                    Queue.ListUpdatedCoresName.TrimExcess();
                }
            });


            // -------------------------
            // Rejected
            // -------------------------
            // Remove Rejected Cores from the Update List
            Queue.ListUpdatedCoresName = Queue.ListUpdatedCoresName.Except(Queue.ListRejectedCores).ToList();
            Queue.ListUpdatedCoresName.TrimExcess();

            // -------------------------
            // Download
            // -------------------------
            for (int i = 0; i < Queue.ListUpdatedCoresName.Count; i++) //problem core count & Parse.nightly7z
            {
                //Reset Waiter, Must be here
                waiter.Reset();

                Uri downloadUrl2 = new Uri(Parse.parseCoresUrl + Queue.ListUpdatedCoresName[i] + ".zip");
                //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + Queue.ListUpdatedCoresName[i] + ".zip"); //TESTING
                //Async
                wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc2.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc2.DownloadFileAsync(downloadUrl2, Paths.tempPath + Queue.ListUpdatedCoresName[i] + ".zip", i);

                // Progress Info
                progressInfo = "Downloading " + Queue.ListUpdatedCoresName[i];

                //Wait until download is finished
                waiter.WaitOne();



                // If Last item in List
                //
                if (i == Queue.ListUpdatedCoresName.Count - 1)
                {
                    // Cross Thread
                    mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
                    {
                        // New Install
                        //
                        if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Install Complete";
                            mainwindow.labelProgressInfo.Content = progressInfo;
                        }

                        // RA+Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Update Complete";
                            mainwindow.labelProgressInfo.Content = progressInfo;
                        }

                        // Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Update Complete";
                            mainwindow.labelProgressInfo.Content = progressInfo;
                        }

                        // New Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Install Complete";
                            mainwindow.labelProgressInfo.Content = progressInfo;
                        }
                    });
                }

                // -------------------------
                // Extract
                // -------------------------
                using (Process execExtract = new Process())
                {
                    // Allow 0.1 seconds before extraction
                    Thread.Sleep(100);

                    //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                    execExtract.StartInfo.UseShellExecute = false;
                    execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                    execExtract.StartInfo.CreateNoWindow = true;
                    execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                    execExtract.StartInfo.FileName = Archiver.archiver;

                    // Extract -o and Overwrite -y Selected Files -r
                    // -------------------------
                    // 7-Zip
                    // -------------------------
                    if (Archiver.extract == "7-Zip")
                    {
                        List<string> extractArgs = new List<string>() {
                                "-y e",
                                "\"" + Paths.tempPath + Queue.ListUpdatedCoresName[i] + ".zip" + "\"",
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
                                "\"" + Paths.tempPath + Queue.ListUpdatedCoresName[i] + ".zip" + "\"",
                                "\"" + Paths.coresPath + "\"",
                            };

                        // Join List with Spaces
                        execExtract.StartInfo.Arguments = string.Join(" ", extractArgs.Where(s => !string.IsNullOrEmpty(s)));
                    }

                    execExtract.Start();
                    execExtract.WaitForExit();
                    execExtract.Close();

                    // Convert Local Time to Server Time
                    // This doesn't work in a Method
                    DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                    //TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                    //DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // .AddHours(12) Needs to be 6-12 hours ahead to be more recent than server? 24 Hour AM/PM Problem?

                    // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                    if (File.Exists(Paths.coresPath + Queue.ListUpdatedCoresName[i]))
                    {
                        File.SetCreationTime(Paths.coresPath + Queue.ListUpdatedCoresName[i], utcTime); // Created Date Time = Now, (used to be DateTime.Now)
                        File.SetLastWriteTime(Paths.coresPath + Queue.ListUpdatedCoresName[i], utcTime); //maybe disable modified date?
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
                    deleteTemp.StartInfo.Arguments = "/c del " + "\"" + Paths.tempPath + Queue.ListUpdatedCoresName[i] + ".zip" + "\"";
                    deleteTemp.Start();
                    deleteTemp.WaitForExit();
                    deleteTemp.Close();
                }


                // If Last item in List
                //
                if (i == Queue.ListUpdatedCoresName.Count - 1)
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
                    Queue.ListRejectedCores.Clear();
                    Queue.ListRejectedCores.TrimExcess();
                }

            } // end for loop
        }


    }
}
