﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

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
        //private static MainWindow mainwindow;

        // Thread
        //public static Thread worker = null;

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
                mainwindow.textBlockProgressInfo.Text = progressInfo;
            });

            // Progress Bar
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                mainwindow.progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
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
                mainwindow.textBlockProgressInfo.Text = progressInfo;
            });

            //DownloadComplete(mainwindow);
        }


        // -------------------------
        // Progress Changed (Method)
        // -------------------------
        //public static void DownloadProgressChanged(MainWindow mainwindow, DownloadProgressChangedEventArgs e)
        //{
        //    mainwindow.Dispatcher.Invoke(new Action(delegate
        //    {
        //        mainwindow.textBlockProgressInfo.Text = progressInfo;
        //    }));

        //    // Progress Bar
        //    mainwindow.Dispatcher.Invoke(new Action(delegate
        //    {
        //        double bytesIn = double.Parse(e.BytesReceived.ToString());
        //        double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
        //        double percentage = bytesIn / totalBytes * 100;
        //        mainwindow.progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
        //    }));
        //}

        // -------------------------
        //  Download Complete (Method)
        // -------------------------
        //public static void DownloadComplete(MainWindow mainwindow)
        //{
        //    mainwindow.Dispatcher.Invoke(new Action(delegate
        //    {
        //        mainwindow.textBlockProgressInfo.Text = progressInfo;
        //    }));
        //}


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

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

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
        }



        // -------------------------
        // RetroArch Download (Method)
        // -------------------------
        public static void RetroArchDownload(MainWindow mainwindow)
        {
            // -------------------------
            // Download
            // -------------------------
            //mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            //{
            //waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)
            //});

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            Uri downloadUrl = new Uri(Parse.nightlyUrl); // Parse.nightlyUrl = x84/x86_64 + Parse.nightly7z
            //Uri downloadUrl = new Uri("http://127.0.0.1:8888/RetroArch.7z"); // TESTING Virtual Server URL

            //Async
            //wc.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            wc.Headers.Add("Accept-Encoding", "gzip,deflate");
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
                mainwindow.textBlockProgressInfo.Text = "RetroArch Complete";
                MainWindow.ClearRetroArchVars();
            });

            // End Thread
            //worker.Abort();
        }



        // -------------------------
        // Cores Download (Method)
        // -------------------------
        public static void CoresDownload(MainWindow mainwindow)
        {
            //waiter = new ManualResetEvent(false);

            // -------------------------
            // New Install
            // -------------------------
            // Change Cores List to All Available Buildbot Cores
            // Cross Thread
            mainwindow.Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
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


            // -------------------------
            // Download
            // -------------------------
            for (int i = 0; i < Queue.List_UpdatedCores_Name.Count; i++) //problem core count & Parse.nightly7z
            {
                //Reset Waiter, Must be here
                waiter.Reset();

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Uri downloadUrl2 = new Uri(Parse.parseCoresUrl + Queue.List_UpdatedCores_Name[i] + ".zip");
                //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + Queue.List_UpdatedCores_Name[i] + ".zip"); //TESTING
                //Async
                wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                wc2.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                wc2.DownloadFileAsync(downloadUrl2, Paths.tempPath + Queue.List_UpdatedCores_Name[i] + ".zip", i);

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
                        if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Install Complete";
                            mainwindow.textBlockProgressInfo.Text = progressInfo;
                        }

                        // RA+Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores")
                        {
                            // Progress Info
                            progressInfo = "RetroArch + Cores Update Complete";
                            mainwindow.textBlockProgressInfo.Text = progressInfo;
                        }

                        // Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Update Complete";
                            mainwindow.textBlockProgressInfo.Text = progressInfo;
                        }

                        // New Cores
                        //
                        else if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
                        {
                            // Progress Info
                            progressInfo = "Cores Install Complete";
                            mainwindow.textBlockProgressInfo.Text = progressInfo;
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
