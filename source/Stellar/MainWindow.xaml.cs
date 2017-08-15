using Stellar.Properties;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Stellar Current Version
        public static Version currentVersion = new Version("0.8.6.0");
        // Alpha, Beta, Stable
        public static string currentBuildPhase = "beta";

        // Windows
        public static Configure configure;
        public static Checklist checklist;

        // MainWindow Title
        public string TitleVersion
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static string stellarCurrentVersion;

        // Ready Check
        public static bool ready = true; // If 0 halt progress


        // -----------------------------------------------
        // Window Defaults
        // -----------------------------------------------
        public MainWindow() // Pass Exclude Cores Data from Checklist
        {
            InitializeComponent();

            TitleVersion = "Stellar ~ RetroArch Nightly Updater (" + Convert.ToString(currentVersion) + "-" + currentBuildPhase + ")";
            DataContext = this;

            this.MinWidth = 500;
            this.MinHeight = 225;
            this.MaxWidth = 500;
            this.MaxHeight = 225;

            // -----------------------------------------------
            // Prevent Loading Corrupt App.Config
            // -----------------------------------------------
            try
            {
                ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            }
            catch (ConfigurationErrorsException ex)
            {
                string filename = ex.Filename;

                if (File.Exists(filename) == true)
                {
                    File.Delete(filename);
                    Properties.Settings.Default.Upgrade();
                    // Properties.Settings.Default.Reload();
                }
                else
                {

                }
            }

            // --------------------------------------------------
            // Load Saved Settings
            // --------------------------------------------------

            // Window Position
            // First time use
            if (Convert.ToDouble(Settings.Default["Left"]) == 0 
                || Convert.ToDouble(Settings.Default["Top"]) == 0)
            {
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            // Theme CombBox
            Configure.ConfigTheme(configure);

            // 7-Zip Path
            Configure.Config7zipPath(configure);

            // WinRAR Path
            Configure.ConfigWinRARPath(configure);

            // Log CheckBox
            Configure.ConfigLogToggle(configure);

            // Log Path
            Configure.ConfigLogPath(configure);


            // -----------------------------------------------
            // Load RetroArch Path
            // -----------------------------------------------
            try
            {
                if (!string.IsNullOrEmpty(Settings.Default["retroarchPath"].ToString()))
                {
                    textBoxLocation.Text = Settings.Default["retroarchPath"].ToString();
                    Paths.retroarchPath = Settings.Default["retroarchPath"].ToString();
                }
            }
            catch
            {

            }

            // -----------------------------------------------
            // Load Dropdown Combobox Downloads (RA+Cores, RetroArch, Cores)
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default["download"].ToString())) // null check
                {
                    comboBoxDownload.SelectedItem = "RA+Cores";
                }
                // Saved Settings
                else
                {
                    comboBoxDownload.SelectedItem = Settings.Default["download"];
                }
            }
            catch
            {

            }

            // -----------------------------------------------
            // Load Dropdown Combobox Architecture (32-bit, 64-bit, 64 w32)
            // -----------------------------------------------
            // If Dropdown Combobox Architecture is Empty, set Default to 64-bit
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default["architecture"].ToString())) // null check
                {
                    comboBoxArchitecture.SelectedItem = "64-bit";
                }
                // Saved Settings
                else
                {
                    comboBoxArchitecture.SelectedItem = Settings.Default["architecture"];
                }
            }
            catch
            {

            }
           
        }

        // ----------------------------------------------------------------------------------------------
        // METHODS 
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Close All
        // -----------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.Save();

            e.Cancel = true;
            System.Windows.Forms.Application.ExitThread();
            Environment.Exit(0);
        }

        // -----------------------------------------------
        // Clear RetroArch Variables
        // -----------------------------------------------
        public static void ClearRetroArchVars()
        {
            Parse.parseUrl = string.Empty;
            Parse.nightly7z = string.Empty;
            Download.extractArgs = string.Empty;

            Parse.stellar7z = string.Empty;
            Parse.stellarUrl = string.Empty;
            //Parse.stellarLatestVersion = string.Empty;
            Parse.latestVersion = null;
            //Parse.currentVer = null;
        }

        // -----------------------------------------------
        // Clear Cores Variables
        // -----------------------------------------------
        public static void ClearCoresVars()
        {
            Parse.parseCoresUrl = string.Empty;
            Parse.indexextendedUrl = string.Empty;
            Download.extractArgs = string.Empty;
        }

        // -----------------------------------------------
        // Clear Lists
        // -----------------------------------------------

        public static void ClearLists()
        {
            // RetroArch
            if (Queue.NightliesList != null)
            {
                Queue.NightliesList.Clear();
                Queue.NightliesList.TrimExcess();
            }

            // Larget List Compre
            Queue.largestList = null;

            // PC & Buildbot Core Sublists
            Queue.pcArr = null;
            Queue.bbArr = null;

            // PC Core Name
            if (Queue.ListPcCoresName != null)
            {
                Queue.ListPcCoresName.Clear();
                Queue.ListPcCoresName.TrimExcess();
            }
            // PC Core Date
            if (Queue.ListPcCoresDate != null)
            {
                Queue.ListPcCoresDate.Clear();
                Queue.ListPcCoresDate.TrimExcess();
            }
            // PC Cores Name+Date
            if (Queue.ListPcCoresNameDate != null)
            {
                Queue.ListPcCoresNameDate.Clear();
                Queue.ListPcCoresNameDate.TrimExcess();
            }
            // PC Cores Name+Date Collection
            if (Queue.CollectionPcCoresNameDate != null)
            {
                Queue.CollectionPcCoresNameDate = null;
            }
            // PC Unknown Name+Date
            if (Queue.ListPcCoresUnknownName != null)
            {
                Queue.ListPcCoresUnknownName.Clear();
                Queue.ListPcCoresUnknownName.TrimExcess();
            }
            // PC Cores Unknown Name+Date
            if (Queue.ListPcCoresUnknownName != null)
            {
                Queue.ListPcCoresUnknownName.Clear();
            }
            // PC Cores Unknown Name+Date Collection
            if (Queue.CollectionPcCoresUnknownNameDate != null)
            {
                Queue.CollectionPcCoresUnknownNameDate = null;
            }


            // Buildbot Core Name
            if (Queue.ListBuildbotCoresName != null)
            {
                Queue.ListBuildbotCoresName.Clear();
                Queue.ListBuildbotCoresName.TrimExcess();
            }
            // Buildbot Core Date
            if (Queue.ListBuildbotCoresDate != null)
            {
                Queue.ListBuildbotCoresDate.Clear();
                Queue.ListBuildbotCoresDate.TrimExcess();
            }
            // Buildbot Cores Name+Date
            if (Queue.ListBuildbotCoresNameDate != null)
            {
                Queue.ListBuildbotCoresNameDate.Clear();
                Queue.ListBuildbotCoresNameDate.TrimExcess();
            }
            // Buildbot Cores Name+Date Collection
            if (Queue.CollectionBuildbotCoresNameDate != null)
            {
                Queue.CollectionBuildbotCoresNameDate = null;
            }
            // Buildbot Core New Name
            if (Queue.ListBuildbotCoresNewName != null)
            {
                Queue.ListBuildbotCoresNewName.Clear();
                Queue.ListBuildbotCoresNewName.TrimExcess();
            }
            // Buildbot Core ID
            //if (Queue.ListBuildbotID != null)
            //{
            //    Queue.ListBuildbotID.Clear();
            //    Queue.ListBuildbotID.TrimExcess();
            //}


            // Excluded Core Name
            if (Queue.ListExcludedCoresName != null)
            {
                Queue.ListExcludedCoresName.Clear();
                Queue.ListExcludedCoresName.TrimExcess();
            }
            // Excluded Core Name ObservableCollection
            if (Queue.CollectionExcludedCoresName != null)
            {
                Queue.CollectionExcludedCoresName = null;
            }
            // Excluded Core Name+Date
            if (Queue.ListExcludedCoresNameDate != null)
            {
                Queue.ListExcludedCoresNameDate.Clear();
                Queue.ListExcludedCoresNameDate.TrimExcess();
            }


            // Updated Cores Name
            if (Queue.ListUpdatedCoresName != null)
            {
                Queue.ListUpdatedCoresName.Clear();
                Queue.ListUpdatedCoresName.TrimExcess();
            }
            // Updated Cores Name Collection
            if (Queue.CollectionUpdatedCoresName != null)
            {
                Queue.CollectionUpdatedCoresName = null;
            }


            // Do Not Clear
            //
            // ListRejectedCores
        }


        // -----------------------------------------------
        // Folder Browser Popup 
        // -----------------------------------------------
        public void FolderBrowser() // Method
        {
            // Open Folder
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                textBoxLocation.Text = folderBrowserDialog.SelectedPath.TrimEnd('\\') + @"\";

                // Set the Paths.retroarchPath string
                Paths.retroarchPath = textBoxLocation.Text;

                try
                {
                    // Save RetroArch path for next launch
                    Settings.Default["retroarchPath"] = textBoxLocation.Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }




        

        // ----------------------------------------------------------------------------------------------
        // CONTROLS
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Info Button
        // -----------------------------------------------
        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("RetroArch Nightly Updater (Unofficial)" 
                + "\nby wyzrd \n\nNew versions at https://stellarupdater.github.io." 
                + "\n\nPlease install 7-Zip for this program to properly extract files." 
                + "\nhttp://www.7-zip.org \n\nThis software is licensed under GNU GPLv3." 
                + "\nSource code is included in the archive with this executable." 
                + "\n\nImage Credit: \nESO/José Francisco (josefrancisco.org), Milky Way" 
                + "\nESO, NGC 1232, Galaxy \nNASA, IC 405 Flaming Star" 
                + "\nNASA, NGC 5189, Spiral Nebula"
                + "\nNASA, M100, Galaxy" 
                + "\nNASA, IC 405, Lagoon" 
                + "\nNASA, Solar Flare" 
                + "\nNASA, Rho Ophiuchi, Dark Nebula" 
                + "\nNASA, N159, Star Dust" 
                + "\nNASA, NGC 6357, Chaos" 
                + "\n\nThis software comes with no warranty, express or implied, and the author makes no representation of warranties." 
                + "The author claims no responsibility for damages resulting from any use or misuse of the software.");
        }

        // -----------------------------------------------
        // Configure Settings Window Button
        // -----------------------------------------------
        private void buttonConfigure_Click(object sender, RoutedEventArgs e)
        {
            // Detect which screen we're on
            var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
            var thisScreen = allScreens.SingleOrDefault(s => this.Left >= s.WorkingArea.Left && this.Left < s.WorkingArea.Right);

            // Open Configure Window
            configure = new Configure();

            // Position Relative to MainWindow
            // Keep from going off screen
            configure.Left = Math.Max((this.Left + (this.Width - configure.Width) / 2), thisScreen.WorkingArea.Left);
            configure.Top = Math.Max(this.Top - configure.Height - 12, thisScreen.WorkingArea.Top);

            // Keep Window on Top
            configure.Owner = Window.GetWindow(this);

            // Open Winndow
            configure.ShowDialog();
        }

        // -----------------------------------------------
        // Stellar Website Button
        // -----------------------------------------------
        private void buttonStellarWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://stellarupdater.github.io");
        }

        // -----------------------------------------------
        // RetroArch Website Button
        // -----------------------------------------------
        private void buttonRetroArchWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.retroarch.com");
        }

        // -----------------------------------------------
        // RetroArch Folder Open Button
        // -----------------------------------------------
        private void buttonRetroArchExplorer_Click(object sender, RoutedEventArgs e)
        {
            // Paths.retroarchPath string is set when user chooses location from textbox
            if (Paths.retroarchPath != "")
            {
                Process.Start("explorer.exe", @Paths.retroarchPath);
            }
            else
            {
                MessageBox.Show("Please choose RetroArch Folder location first.");
            }
        }

        // -----------------------------------------------
        // BuildBot Web Server Open Button
        // -----------------------------------------------
        private void buttonBuildBotDir_Click(object sender, RoutedEventArgs e)
        {
            // Open the URL
            Process.Start(textBoxDownload.Text);
        }

        // -----------------------------------------------
        // Location RetroArch Label (On Click)
        // -----------------------------------------------
        private void labelLocation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Call Folder Browser Popup
            FolderBrowser();
        }

        // -----------------------------------------------
        // Location RetroArch TextBox (On Click/Mouse Down)
        // -----------------------------------------------
        private void textBoxLocation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            FolderBrowser();
        }

        // -----------------------------------------------
        // Dropdown Combobox Platform/Architecture 32-bit, 64-bit
        // -----------------------------------------------
        private void comboBoxArchitecture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                // Save Selected Arhitecture
                Settings.Default["architecture"] = comboBoxArchitecture.SelectedItem;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }

            // Set Architecture to show in URL Textbox
            Paths.SetArchitecture(this);
            // Show URL in Textbox
            Paths.SetUrls(this);

            // Clear for next pass
            ClearLists();
        }

        // -----------------------------------------------
        // Combobox Download (On Changed)
        // -----------------------------------------------
        private void comboBoxDownload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset Update Button Text to "Update"
            buttonUpdateTextBlock.Text = "Update";

            // Save Download Combobox Settings for next launch
            Settings.Default["download"] = comboBoxDownload.SelectedItem;
            Settings.Default.Save();
            Settings.Default.Reload();


            // Stellar Self-Update Selected, Disable Architecture ComboBox
            if ((string)comboBoxDownload.SelectedItem == "Stellar")
            {
                comboBoxArchitecture.IsEnabled = false;
            }
            else
            {
                comboBoxArchitecture.IsEnabled = true;
            }

            // Cross Thread
            Dispatcher.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate
            {
                // New Install
                //
                if ((string)comboBoxDownload.SelectedItem== "New Install")
                {
                    // Change Update Button Text
                    buttonUpdateTextBlock.Text = "Install";

                    // Warn user about New Install
                    MessageBox.Show("This will install a New Nightly RetroArch + Cores." 
                                    + "\n\nIt will overwrite any existing files/configs in the selected folder." 
                                    + "\n\nDo not use the New Install option to Update.");

                    // Save Download Combobox Settings back to RA+Cores instead of New Install for next launch
                    Settings.Default["download"] = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }

                // Upgrade
                //
                else if ((string)comboBoxDownload.SelectedItem == "Upgrade")
                {
                    // Change Update Button Text
                    buttonUpdateTextBlock.Text = "Upgrade";

                    // Warn user about Upgrade
                    MessageBox.Show("Backup your configs and custom shaders! Large Download." 
                                    + "\n\nThis will fully upgrade RetroArch to the latest version."
                                    + "\nFor small updates use the \"RetroArch\" or \"RA+Cores\" menu option."
                                    + "\n\nUpdate Cores separately using \"Cores\" menu option.");

                    // Save Download Combobox Settings back to RA+Cores instead of Upgrade for next launch
                    Settings.Default["download"] = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }

                // New Cores
                //
                else if ((string)comboBoxDownload.SelectedItem == "New Cores")
                {
                    // Change Update Button Text to "Upgrade"
                    buttonUpdateTextBlock.Text = "Download";

                    // Save Download Combobox Settings back to RA+Cores instead of New Cores for next launch
                    Settings.Default["download"] = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
            });


            // Call Set Architecture
            Paths.SetArchitecture(this);
            // Call Set Urls Method
            Paths.SetUrls(this);

            // Clear if checked/unchecked for next pass
            ClearLists();
        }

        // -----------------------------------------------
        // Textbox RetroArch Location (On Click Change)
        // -----------------------------------------------
        private void textBoxLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set the Paths.retroarchPath string
            Paths.retroarchPath = textBoxLocation.Text; //end with backslash

            // Save RetroArch Path for next launch
            Settings.Default["retroarchPath"] = textBoxLocation.Text;
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        // -----------------------------------------------
        // Check Button - Tests Download URL
        // -----------------------------------------------
        private void buttonCheck_Click(object sender, RoutedEventArgs e)
        {
            // Progress Info
            labelProgressInfo.Content = "Checking...";

            // Call SetArchitecture Method
            Paths.SetArchitecture(this);


            // -------------------------
            // Stellar Self-Update
            // -------------------------
            if ((string)comboBoxDownload.SelectedItem == "Stellar")
            {
                // Parse GitHub Page HTML
                Parse.ParseGitHubReleases(this);

                // Check if Stellar is the Latest Version
                if (MainWindow.currentVersion != null && Parse.latestVersion != null)
                {
                    if (Parse.latestVersion > MainWindow.currentVersion)
                    {
                        MessageBox.Show("Update Available \n\n" + "v" + Parse.latestVersion + "-" + Parse.latestBuildPhase);
                    }
                    else if (Parse.latestVersion <= MainWindow.currentVersion)
                    {
                        MessageBox.Show("This version is up to date.");
                    }
                    else // null
                    {
                        MessageBox.Show("Could not find download.");
                    }
                }
            }


            // -------------------------
            // RetroArch Part
            // -------------------------
            if ((string)comboBoxDownload.SelectedItem == "New Install"
                || (string)comboBoxDownload.SelectedItem == "Upgrade" 
                || (string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "RetroArch" 
                || (string)comboBoxDownload.SelectedItem == "Redist")
            {
                // Parse Page (HTML) Method
                Parse.ParseBuildbotPage(this);

                // Display message if download available
                if (!string.IsNullOrEmpty(Parse.nightly7z)) // If Last Item in Nightlies List is available
                {
                    MessageBox.Show("Download Available \n\n" + Paths.buildbotArchitecture + "\n\n" + Parse.nightly7z);
                }
                else
                {
                    MessageBox.Show("Could not find download.");
                }
            }

            // Progress Info
            labelProgressInfo.Content = "";


            // -------------------------
            // Cores Part
            // -------------------------
            // If Download Combobox Cores or RA+Cores selected
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "Cores"
                || (string)comboBoxDownload.SelectedItem == "New Cores")
            {
                // Create Builtbot Cores List
                Parse.ParseBuildbotCoresIndex(this);

                // Create PC Cores Lists
                Parse.ScanPcCoresDir(this);

                // Create Cores to Update List
                Queue.UpdatedCores(this);

                // Check if Cores Up To Date
                // If All Cores up to date, display message
                Queue.CoresUpToDateCheck(this); //Note there are Clears() in this method

                // -------------------------
                // Window Checklist Popup
                // -------------------------
                // If Update List greater than 0, Popup Checklist
                if (Queue.ListUpdatedCoresName.Count != 0)
                {
                    // Detect which screen we're on
                    var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
                    var thisScreen = allScreens.SingleOrDefault(s => this.Left >= s.WorkingArea.Left && this.Left < s.WorkingArea.Right);

                    // Start Window
                    checklist = new Checklist();

                    // Keep Window on Top
                    checklist.Owner = Window.GetWindow(this);

                    // Position Relative to MainWindow
                    checklist.Left = Math.Max((this.Left + (this.Width - checklist.Width) / 2), thisScreen.WorkingArea.Left);
                    checklist.Top = Math.Max((this.Top + (this.Height - checklist.Height) / 2), thisScreen.WorkingArea.Top);

                    // Open Window
                    checklist.ShowDialog();
                }
            }

            // Clear All again to prevent doubling up on Update button
            ClearRetroArchVars();
            ClearCoresVars();
            ClearLists();
        }


        // -----------------------------------------------
        // Update Button
        // -----------------------------------------------
        // Launches Download and 7-Zip Extraction
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Add backslash to Location Textbox path if missing
            if (!textBoxLocation.Text.EndsWith("\\") && textBoxLocation.Text != "")
            {
                textBoxLocation.Text = textBoxLocation.Text + "\\";
            }
            // Load the User's RetroArch Location from Text Box / Saved Settings
            Paths.retroarchPath = textBoxLocation.Text; //end with backslash


            // If RetroArch Path is empty, halt progress
            if (string.IsNullOrEmpty(Paths.retroarchPath))
            {
                ready = false;
                MessageBox.Show("Please select your RetroArch main folder.");
            }

            // MUST BE IN THIS ORDER: 1. SetArchitecture -> 2. parsePage -> 3. SetArchiver  ##################
            // If you checkArchiver before parsePage, it will not set the Parse.nightly7z string in the CLI Arguments first
            // Maybe solve this by putting CLI Arguments in another method?

            // 1. Call SetArchitecture Method
            Paths.SetArchitecture(this);

            // 2. Call parse Page (HTML) Method
            Parse.ParseBuildbotPage(this);

            // 3. Call checkArchiver Method
            // If Archiver exists, Set string
            Archiver.SetArchiver(this);


            // -------------------------
            // Stellar Self-Update
            // -------------------------
            if ((string)comboBoxDownload.SelectedItem == "Stellar")
            {
                // Parse GitHub Page HTML
                Parse.ParseGitHubReleases(this);

                if (Parse.latestVersion != null && MainWindow.currentVersion != null)
                {
                    // Check if Stellar is the Latest Version
                    if (Parse.latestVersion > MainWindow.currentVersion)
                    {
                        // Yes/No Dialog Confirmation
                        //
                        MessageBoxResult result = MessageBox.Show("v" + Parse.latestVersion + "-" + Parse.latestBuildPhase + "\n\nDownload Update?", "Update Available", MessageBoxButton.YesNo);
                        switch (result)
                        {
                            case MessageBoxResult.Yes:
                                // Proceed
                                break;
                            case MessageBoxResult.No:
                                // Lock
                                MainWindow.ready = false;
                                break;
                        }
                    }
                    else if (Parse.latestVersion <= MainWindow.currentVersion)
                    {
                        // Lock
                        MainWindow.ready = false;
                        MessageBox.Show("This version is up to date.");
                    }
                    else // null
                    {
                        // Lock
                        MainWindow.ready = false;
                        MessageBox.Show("Could not find download. Try updating manually.");
                    }
                }              
            }


            // -----------------------------------------------
            // If New Install (RetroArch + Cores)
            // -----------------------------------------------
            if ((string)comboBoxDownload.SelectedItem == "New Install")
            {
                // -------------------------
                // Create Cores Folder
                // -------------------------
                using (Process execMakeCoresDir = new Process())
                {
                    execMakeCoresDir.StartInfo.UseShellExecute = false;
                    execMakeCoresDir.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                    execMakeCoresDir.StartInfo.CreateNoWindow = true;
                    execMakeCoresDir.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                    execMakeCoresDir.StartInfo.FileName = "cmd.exe";
                    execMakeCoresDir.StartInfo.Arguments = "/c cd " + "\"" + Paths.retroarchPath + "\"" + " && mkdir cores";
                    execMakeCoresDir.Start();
                    execMakeCoresDir.WaitForExit();
                    execMakeCoresDir.Close();
                }

                // Set Cores Folder (Dont Scan PC)
                Paths.coresPath = Paths.retroarchPath + "cores\\";
                // Call Parse Builtbot Page Method
                Parse.ParseBuildbotCoresIndex(this);
            }



            // -----------------------------------------------
            // If RetroArch+Cores or Cores Only Update
            // -----------------------------------------------
            // RA+Cores or Cores Selected
            if ((string)comboBoxDownload.SelectedItem == "New Install"
                || (string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "Cores" 
                || (string)comboBoxDownload.SelectedItem == "New Cores")
            {
                // Create Builtbot Cores List
                Parse.ParseBuildbotCoresIndex(this);

                // Create PC Cores List
                Parse.ScanPcCoresDir(this);

                // Create Cores to Update List
                Queue.UpdatedCores(this);

                // Check if Cores Up To Date
                // If All Cores up to date, display message
                Queue.CoresUpToDateCheck(this); //Note there are Clears() in this method
            }



            // -----------------------------------------------
            // Ready
            // -----------------------------------------------
            if (ready == true)
            {
                Download.StartDownload(this);
            }
            else
            {
                // Restart & Reset ready value
                ready = true;
                // Call Garbage Collector
                GC.Collect();
            }
        }

    }
}