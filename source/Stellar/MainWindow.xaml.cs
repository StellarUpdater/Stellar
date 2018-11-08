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

using Stellar.Properties;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
// Disable XML Comment warnings
#pragma warning disable 1591
#pragma warning disable 1587
#pragma warning disable 1570

namespace Stellar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // View Model
        public ViewModel vm = new ViewModel();

        // Current Version
        public static Version currentVersion;
        // GitHub Latest Version
        public static Version latestVersion;
        // Alpha, Beta, Stable
        public static string currentBuildPhase = "beta";
        public static string latestBuildPhase;
        public static string[] splitVersionBuildPhase;

        // MainWindow Title
        public string TitleVersion
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static string stellarCurrentVersion;

        // Windows
        public static Configure configure;
        public static Checklist checklist;

        // Ready Check
        public static bool ready = true; // If 0 halt progress


        // -----------------------------------------------
        // Window Defaults
        // -----------------------------------------------
        public MainWindow() // Pass Exclude Cores Data from Checklist
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string assemblyVersion = fvi.FileVersion;
            currentVersion = new Version(assemblyVersion);

            // -------------------------
            // Title + Version
            // -------------------------
            TitleVersion = "Stellar ~ RetroArch Nightly Updater (" + Convert.ToString(currentVersion) + "-" + currentBuildPhase + ")";

            MinWidth = 500;
            MinHeight = 225;
            MaxWidth = 500;
            MaxHeight = 225;

            // -----------------------------------------------------------------
            /// <summary>
            ///     Control Binding
            /// </summary>
            // -----------------------------------------------------------------
            DataContext = vm;

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

            // -------------------------
            // Window Position
            // -------------------------
            try {
                // First time use
                if (Convert.ToDouble(Settings.Default.Left) == 0 
                    && Convert.ToDouble(Settings.Default.Top) == 0)
                {
                    WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            catch
            {

            }

            // Theme CombBox
            Configure.ConfigTheme(configure, vm);

            // 7-Zip Path
            Configure.Config7zipPath(configure, vm);

            // WinRAR Path
            Configure.ConfigWinRARPath(configure, vm);

            // Log CheckBox
            Configure.ConfigLogToggle(configure, vm);

            // Log Path
            Configure.ConfigLogPath(configure, vm);

            // Update Auto Check
            Configure.UpdateAutoCheck(configure, vm);


            // -----------------------------------------------
            // Load RetroArch Path
            // -----------------------------------------------
            try
            {
                if (!string.IsNullOrEmpty(Settings.Default.retroarchPath.ToString()))
                {
                    vm.Location_Text = Settings.Default.retroarchPath.ToString();
                    Paths.retroarchPath = Settings.Default.retroarchPath.ToString();
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
                if (string.IsNullOrEmpty(Settings.Default.download.ToString())) // null check
                {
                    vm.Download_SelectedItem = "RA+Cores";
                }
                // Saved Settings
                else
                {
                    vm.Download_SelectedItem = Settings.Default.download;
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
                if (string.IsNullOrEmpty(Settings.Default.architecture.ToString())) // null check
                {
                    vm.Architecture_SelectedItem = "64-bit";
                }
                // Saved Settings
                else
                {
                    vm.Architecture_SelectedItem = Settings.Default.architecture;
                }
            }
            catch
            {

            }


            // -----------------------------------------------
            // Load Download Server (auto, raw, buildbot)
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.downloadServer.ToString())) // null check
                {
                    //vm.Server_SelectedItem = "auto";
                    vm.Server_SelectedItem = "buildbot";
                }
                // Saved Settings
                else
                {
                    vm.Server_SelectedItem = Settings.Default.downloadServer;
                }
            }
            catch
            {

            }

        }

        // ----------------------------------------------------------------------------------------------
        // METHODS 
        // ----------------------------------------------------------------------------------------------
        /// <summary>
        ///    Window Loaded
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // -------------------------
            // Check for Available Updates
            // -------------------------
            Task.Factory.StartNew(() =>
            {
                UpdateAvailableCheck();
            });
        }

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


        /// <summary>
        ///     Check For Internet Connection
        /// </summary>
        [System.Runtime.InteropServices.DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

        public static bool CheckForInternetConnection()
        {
            int desc;
            return InternetGetConnectedState(out desc, 0);
        }

        /// <summary>
        ///    Update Available Check
        /// </summary>
        public void UpdateAvailableCheck()
        {
            if (CheckForInternetConnection() == true)
            {
                //if (tglUpdatesAutoCheck.IsChecked == true)
                //if (tglUpdateAutoCheck.Dispatcher.Invoke((() => { return tglUpdateAutoCheck.IsChecked; })) == true)
                if (vm.UpdateAutoCheck_IsChecked == true)
                {
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    WebClient wc = new WebClient();
                    // UserAgent Header
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "Stellar Updater (https://github.com/StellarUpdater/Stellar)" + " v" + MainWindow.currentVersion + "-" + MainWindow.currentBuildPhase + " Self-Update");
                    //wc.Headers.Add("Accept-Encoding", "gzip,deflate"); //error

                    wc.Proxy = null;

                    // -------------------------
                    // Parse GitHub .version file
                    // -------------------------
                    string parseLatestVersion = string.Empty;

                    try
                    {
                        parseLatestVersion = wc.DownloadString("https://raw.githubusercontent.com/StellarUpdater/Stellar/master/.version");
                    }
                    catch
                    {
                        return;
                    }

                    // -------------------------
                    // Split Version & Build Phase by dash
                    // -------------------------
                    if (!string.IsNullOrEmpty(parseLatestVersion)) //null check
                    {
                        try
                        {
                            // Split Version and Build Phase
                            splitVersionBuildPhase = Convert.ToString(parseLatestVersion).Split('-');

                            // Set Version Number
                            latestVersion = new Version(splitVersionBuildPhase[0]); //number
                            latestBuildPhase = splitVersionBuildPhase[1]; //alpha
                        }
                        catch
                        {
                            return;
                        }

                        // Check if Stellar is the Latest Version
                        // Update Available
                        if (latestVersion > currentVersion)
                        {
                            //updateAvailable = " ~ Update Available: " + "(" + Convert.ToString(latestVersion) + "-" + latestBuildPhase + ")";

                            Dispatcher.Invoke(new Action(delegate
                            {
                                TitleVersion = "Stellar ~ RetroArch Nightly Updater (" + Convert.ToString(currentVersion) + "-" + currentBuildPhase + ")"
                                                + " UPDATE";
                            }));
                        }
                        // Update Not Available
                        else if (latestVersion <= currentVersion)
                        {
                            return;
                        }
                    }
                }
            }

            // Internet Connection Failed
            else
            {
                MessageBox.Show("Could not detect Internet Connection.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);

                return;
            }
        }



        // -----------------------------------------------
        // Clear RetroArch Variables
        // -----------------------------------------------
        public static void ClearRetroArchVars()
        {
            Parse.parseUrl = string.Empty;
            Parse.page = string.Empty;
            Parse.element = string.Empty;
            Parse.nightly7z = string.Empty;
            Parse.nightlyUrl = string.Empty;
            Download.extractArgs = string.Empty;

            Parse.stellar7z = string.Empty;
            Parse.stellarUrl = string.Empty;
            Parse.latestVersion = null;
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
            Queue.largestList = 0;

            // PC & Buildbot Core Sublists
            Queue.pcArr = null;
            Queue.bbArr = null;

            // PC Core Name
            if (Queue.List_PcCores_Name != null)
            {
                Queue.List_PcCores_Name.Clear();
                Queue.List_PcCores_Name.TrimExcess();
            }
            // PC Core Date
            if (Queue.List_PcCores_Date != null)
            {
                Queue.List_PcCores_Date.Clear();
                Queue.List_PcCores_Date.TrimExcess();
            }
            // PC Cores Name+Date
            if (Queue.List_PcCores_NameDate != null)
            {
                Queue.List_PcCores_NameDate.Clear();
                Queue.List_PcCores_NameDate.TrimExcess();
            }
            // PC Cores Name+Date Collection
            if (Queue.Collection_PcCores_NameDate != null)
            {
                Queue.Collection_PcCores_NameDate = null;
            }
            // PC Unknown Name+Date
            if (Queue.List_PcCores_UnknownName != null)
            {
                Queue.List_PcCores_UnknownName.Clear();
                Queue.List_PcCores_UnknownName.TrimExcess();
            }
            // PC Cores Unknown Name+Date
            if (Queue.List_PcCores_UnknownName != null)
            {
                Queue.List_PcCores_UnknownName.Clear();
            }
            // PC Cores Unknown Name+Date Collection
            if (Queue.CollectionPcCoresUnknownNameDate != null)
            {
                Queue.CollectionPcCoresUnknownNameDate = null;
            }


            // Buildbot Core Name
            if (Queue.List_BuildbotCores_Name != null)
            {
                Queue.List_BuildbotCores_Name.Clear();
                Queue.List_BuildbotCores_Name.TrimExcess();
            }
            // Buildbot Core Date
            if (Queue.List_BuildbotCores_Date != null)
            {
                Queue.List_BuildbotCores_Date.Clear();
                Queue.List_BuildbotCores_Date.TrimExcess();
            }
            // Buildbot Cores Name+Date
            if (Queue.List_BuildbotCores_NameDate != null)
            {
                Queue.List_BuildbotCores_NameDate.Clear();
                Queue.List_BuildbotCores_NameDate.TrimExcess();
            }
            // Buildbot Cores Name+Date Collection
            if (Queue.Collection_BuildbotCores_NameDate != null)
            {
                Queue.Collection_BuildbotCores_NameDate = null;
            }
            // Buildbot Core New Name
            if (Queue.List_BuildbotCores_NewName != null)
            {
                Queue.List_BuildbotCores_NewName.Clear();
                Queue.List_BuildbotCores_NewName.TrimExcess();
            }
            // Buildbot Core ID
            //if (Queue.ListBuildbotID != null)
            //{
            //    Queue.ListBuildbotID.Clear();
            //    Queue.ListBuildbotID.TrimExcess();
            //}


            // Excluded Core Name
            if (Queue.List_ExcludedCores_Name != null)
            {
                Queue.List_ExcludedCores_Name.Clear();
                Queue.List_ExcludedCores_Name.TrimExcess();
            }
            // Excluded Core Name ObservableCollection
            if (Queue.Collection_ExcludedCores_Name != null)
            {
                Queue.Collection_ExcludedCores_Name = null;
            }
            // Excluded Core Name+Date
            if (Queue.List_ExcludedCores_NameDate != null)
            {
                Queue.List_ExcludedCores_NameDate.Clear();
                Queue.List_ExcludedCores_NameDate.TrimExcess();
            }


            // Updated Cores Name
            if (Queue.List_UpdatedCores_Name != null)
            {
                Queue.List_UpdatedCores_Name.Clear();
                Queue.List_UpdatedCores_Name.TrimExcess();
            }
            // Updated Cores Date
            if (Queue.List_UpdatedCores_Date != null)
            {
                Queue.List_UpdatedCores_Date.Clear();
                Queue.List_UpdatedCores_Date.TrimExcess();
            }
            // Updated Cores Name Collection
            if (Queue.Collection_UpdatedCores_Name != null)
            {
                Queue.Collection_UpdatedCores_Name = null;
            }


            // Do Not Clear
            //
            // List_RejectedCores_Name
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
                vm.Location_Text = folderBrowserDialog.SelectedPath.TrimEnd('\\') + @"\";

                // Set the Paths.retroarchPath string
                Paths.retroarchPath = vm.Location_Text;

                try
                {
                    // Save RetroArch path for next launch
                    Settings.Default.retroarchPath = vm.Location_Text;
                    Settings.Default.Save();
                    //Settings.Default.Reload();
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
                + "\n\nThis software comes with no warranty, express or implied, and the author makes no representation of warranties. " 
                + "The author claims no responsibility for damages resulting from any use or misuse of the software.");
        }

        // -----------------------------------------------
        // Configure Settings Window Button
        // -----------------------------------------------
        private void buttonConfigure_Click(object sender, RoutedEventArgs e)
        {
            // Prevent Monitor Resolution Window Crash
            //
            try
            {
                // Detect which screen we're on
                var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
                var thisScreen = allScreens.SingleOrDefault(s => Left >= s.WorkingArea.Left && Left < s.WorkingArea.Right);

                // Open Configure Window
                configure = new Configure(this, vm);

                // Keep Window on Top
                configure.Owner = Window.GetWindow(this);

                // Position Relative to MainWindow
                // Keep from going off screen
                configure.Left = Math.Max((Left + (Width - configure.Width) / 2), thisScreen.WorkingArea.Left);
                configure.Top = Math.Max(Top - configure.Height - 12, thisScreen.WorkingArea.Top);

                // Open Winndow
                configure.ShowDialog();
            }
            // Simplified
            catch
            {
                // Open Configure Window
                configure = new Configure(this, vm);

                // Keep Window on Top
                configure.Owner = Window.GetWindow(this);

                // Position Relative to MainWindow
                // Keep from going off screen
                configure.Left = Math.Max((Left + (Width - configure.Width) / 2), Left);
                configure.Top = Math.Max(Top - configure.Height - 12, Top);

                // Open Winndow
                configure.ShowDialog();
            }
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
                MessageBox.Show("Please choose RetroArch Folder location first.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

        // -----------------------------------------------
        // BuildBot Web Server Open Button
        // -----------------------------------------------
        private void buttonBuildBotDir_Click(object sender, RoutedEventArgs e)
        {
            // Open the URL
            Process.Start(vm.DownloadURL_Text);
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
                Settings.Default.architecture = vm.Architecture_SelectedItem.ToString();
                Settings.Default.Save();
                //Settings.Default.Reload();
            }
            catch
            {

            }

            // Set Architecture to show in URL Textbox
            Paths.SetArchitecture(vm);
            // Show URL in Textbox
            Paths.SetUrls(vm);

            // Clear for next pass
            ClearLists();
        }

        // -----------------------------------------------
        // Combobox Download (On Changed)
        // -----------------------------------------------
        private void comboBoxDownload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Disable Server ComboBox
            if (vm.Download_SelectedItem == "Stellar")
            {
                cboServer.IsEnabled = false;
            }
            // Enable Server ComboBox
            else
            {
                cboServer.IsEnabled = true;
            }


            // Reset Update Button Text to "Update"
            vm.Update_Text = "Update";

            // Save Download Combobox Settings for next launch
            Settings.Default.download = vm.Download_SelectedItem.ToString();
            Settings.Default.Save();
            //Settings.Default.Reload();


            // Stellar Self-Update Selected, Disable Architecture ComboBox
            if (vm.Download_SelectedItem == "Stellar")
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
                if (vm.Download_SelectedItem== "New Install")
                {
                    // Change Update Button Text
                    vm.Update_Text = "Install";

                    // Warn user about New Install
                    MessageBox.Show("This will install a New Nightly RetroArch + Cores." 
                                    + "\n\nIt will overwrite any existing files/configs in the selected folder." 
                                    + "\n\nDo not use the New Install option to Update.",
                                    "Notice",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    // Save Download Combobox Settings back to RA+Cores instead of New Install for next launch
                    Settings.Default.download = "RA+Cores";
                    Settings.Default.Save();
                    //Settings.Default.Reload();
                }

                // Upgrade
                //
                else if (vm.Download_SelectedItem == "Upgrade")
                {
                    // Change Update Button Text
                    vm.Update_Text = "Upgrade";

                    // Warn user about Upgrade
                    MessageBox.Show("Backup your configs and custom shaders! Large Download." 
                                    + "\n\nThis will fully upgrade RetroArch to the latest version."
                                    + "\nFor small updates use the \"RetroArch\" or \"RA+Cores\" menu option."
                                    + "\n\nUpdate Cores separately using \"Cores\" menu option.",
                                    "Notice",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    // Save Download Combobox Settings back to RA+Cores instead of Upgrade for next launch
                    Settings.Default.download = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }

                // New Cores
                //
                else if (vm.Download_SelectedItem == "New Cores")
                {
                    // Change Update Button Text to "Upgrade"
                    vm.Update_Text = "Download";

                    // Save Download Combobox Settings back to RA+Cores instead of New Cores for next launch
                    Settings.Default.download = "RA+Cores";
                    Settings.Default.Save();
                    //Settings.Default.Reload();
                }
            });


            // Call Set Architecture
            Paths.SetArchitecture(vm);
            // Call Set Urls Method
            Paths.SetUrls(vm);

            // Clear if checked/unchecked for next pass
            ClearLists();
        }


        // -----------------------------------------------
        // Textbox RetroArch Location (On Click Change)
        // -----------------------------------------------
        private void textBoxLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set the Paths.retroarchPath string
            Paths.retroarchPath = vm.Location_Text; //end with backslash

            // Save RetroArch Path for next launch
            Settings.Default.retroarchPath = vm.Location_Text;
            Settings.Default.Save();
            //Settings.Default.Reload();
        }


        // -----------------------------------------------
        // Server Switch
        // -----------------------------------------------
        private void cboServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Set Architecture
            Paths.SetArchitecture(vm);

            // Set URLs
            Paths.SetUrls(vm);

            Settings.Default.downloadServer = vm.Server_SelectedItem.ToString();
            Settings.Default.Save();
        }


        // -----------------------------------------------
        // Check Button - Tests Download URL
        // -----------------------------------------------
        private void buttonCheck_Click(object sender, RoutedEventArgs e)
        {
            //Download.waiter.Reset();
            //Download.waiter = new ManualResetEvent(false);

            // Clear RetroArch Nightlies List before each run
            if (Queue.NightliesList != null)
            {
                Queue.NightliesList.Clear();
                Queue.NightliesList.TrimExcess();
            }

            //var message = string.Join(Environment.NewLine, Queue.NightliesList); //debug
            //MessageBox.Show(message); //debug

            // Progress Info
            vm.ProgressInfo_Text = "Checking...";

            // Call SetArchitecture Method
            Paths.SetArchitecture(vm);


            // -------------------------
            // Stellar Self-Update
            // -------------------------
            if (vm.Download_SelectedItem == "Stellar")
            {
                // Parse GitHub Page HTML
                Parse.ParseGitHubReleases(vm);

                // Check if Stellar is the Latest Version
                if (MainWindow.currentVersion != null && Parse.latestVersion != null)
                {
                    if (Parse.latestVersion > MainWindow.currentVersion)
                    {
                        MessageBox.Show("Update Available \n\n" + "v" + Parse.latestVersion + "-" + Parse.latestBuildPhase);
                    }
                    else if (Parse.latestVersion <= MainWindow.currentVersion)
                    {
                        MessageBox.Show("This version is up to date.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    }
                    else // null
                    {
                        MessageBox.Show("Could not find download.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                    }
                }
            }


            // -------------------------
            // RetroArch Part
            // -------------------------
            if (vm.Download_SelectedItem == "New Install"
                || vm.Download_SelectedItem == "Upgrade" 
                || vm.Download_SelectedItem == "RA+Cores" 
                || vm.Download_SelectedItem == "RetroArch" 
                || vm.Download_SelectedItem == "Redist")
            {
                // Parse Page (HTML) Method
                Parse.ParseBuildbotPage(vm);

                // Display message if download available
                if (!string.IsNullOrEmpty(Parse.nightly7z)) // If Last Item in Nightlies List is available
                {
                    MessageBox.Show("Download Available \n\n" + Paths.buildbotArchitecture + "\n\n" + Parse.nightly7z);
                }
                else
                {
                    MessageBox.Show("Could not find download.",
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                }
            }

            // Progress Info
            vm.ProgressInfo_Text = "";


            // -------------------------
            // Cores Part
            // -------------------------
            // If Download Combobox Cores or RA+Cores selected
            if (vm.Download_SelectedItem == "RA+Cores" 
                || vm.Download_SelectedItem == "Cores"
                || vm.Download_SelectedItem == "New Cores")
            {
                // Create Builtbot Cores List
                Parse.ParseBuildbotCoresIndex(vm);

                // Create PC Cores Lists
                Parse.ScanPcCoresDir(vm);

                // Create Cores to Update List
                Queue.UpdatedCores(vm);

                // Check if Cores Up To Date
                // If All Cores up to date, display message
                Queue.CoresUpToDateCheck(vm); //Note there are Clears() in this method

                // -------------------------
                // Window Checklist Popup
                // -------------------------
                // If Update List greater than 0, Popup Checklist
                if (Queue.List_UpdatedCores_Name.Count != 0)
                {
                    // Prevent Monitor Resolution Window Crash
                    //
                    try
                    {
                        // Detect which screen we're on
                        var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
                        var thisScreen = allScreens.SingleOrDefault(s => Left >= s.WorkingArea.Left && Left < s.WorkingArea.Right);

                        // Start Window
                        checklist = new Checklist();

                        // Keep Window on Top
                        checklist.Owner = Window.GetWindow(this);

                        // Position Relative to MainWindow
                        checklist.Left = Math.Max((Left + (Width - checklist.Width) / 2), thisScreen.WorkingArea.Left);
                        checklist.Top = Math.Max((Top + (Height - checklist.Height) / 2), thisScreen.WorkingArea.Top);

                        // Open Window
                        checklist.ShowDialog();
                    }
                    // Simplified
                    catch
                    {
                        // Start Window
                        checklist = new Checklist();

                        // Keep Window on Top
                        checklist.Owner = Window.GetWindow(this);

                        // Position Relative to MainWindow
                        checklist.Left = Math.Max((Left + (Width - checklist.Width) / 2), Left);
                        checklist.Top = Math.Max((Top + (Height - checklist.Height) / 2), Top);

                        // Open Window
                        checklist.ShowDialog();
                    }
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
            Download.waiter.Reset();
            Download.waiter = new ManualResetEvent(false);

            // Clear RetroArch Nightlies List before each run
            if (Queue.NightliesList != null)
            {
                Queue.NightliesList.Clear();
                Queue.NightliesList.TrimExcess();
            }

            //var message = string.Join(Environment.NewLine, Queue.NightliesList); //debug
            //MessageBox.Show(message); //debug

            // Add backslash to Location Textbox path if missing
            if (!vm.Location_Text.EndsWith("\\") && !string.IsNullOrWhiteSpace(vm.Location_Text))
            {
                vm.Location_Text = vm.Location_Text + "\\";
            }
            // Load the User's RetroArch Location from Text Box / Saved Settings
            Paths.retroarchPath = vm.Location_Text; //end with backslash


            // If RetroArch Path is empty, halt progress
            if (string.IsNullOrEmpty(Paths.retroarchPath) 
                && vm.Download_SelectedItem != "Stellar") // ignore if Stellar Self Update
            {
                ready = false;
                MessageBox.Show("Please select your RetroArch main folder.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }

            // MUST BE IN THIS ORDER: 1. SetArchitecture -> 2. parsePage -> 3. SetArchiver  ##################
            // If you checkArchiver before parsePage, it will not set the Parse.nightly7z string in the CLI Arguments first
            // Maybe solve this by putting CLI Arguments in another method?

            // 1. Call SetArchitecture Method
            Paths.SetArchitecture(vm);

            // 2. Call parse Page (HTML) Method
            if (vm.Download_SelectedItem == "New Install"
                || vm.Download_SelectedItem == "Upgrade"
                || vm.Download_SelectedItem == "RA+Cores"
                || vm.Download_SelectedItem == "RetroArch"
                || vm.Download_SelectedItem == "Redist")
            {
                // Progress Info
                vm.ProgressInfo_Text = "Fetching RetroArch List...";

                Parse.ParseBuildbotPage(vm);
            }

            // 3. Call checkArchiver Method
            // If Archiver exists, Set string
            Archiver.SetArchiver(this);


            // -------------------------
            // Stellar Self-Update
            // -------------------------
            if (vm.Download_SelectedItem == "Stellar")
            {
                // Parse GitHub Page HTML
                Parse.ParseGitHubReleases(vm);

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
                        MessageBox.Show("This version is up to date.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                    }
                    else // null
                    {
                        // Lock
                        MainWindow.ready = false;
                        MessageBox.Show("Could not find download. Try updating manually.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                    }
                }              
            }


            // -----------------------------------------------
            // If New Install (RetroArch + Cores)
            // -----------------------------------------------
            if (vm.Download_SelectedItem == "New Install")
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
                Parse.ParseBuildbotCoresIndex(vm);
            }



            // -----------------------------------------------
            // RetroArch+Cores or Cores Only Update
            // -----------------------------------------------
            if (vm.Download_SelectedItem == "New Install"
                || vm.Download_SelectedItem == "RA+Cores" 
                || vm.Download_SelectedItem == "Cores" 
                || vm.Download_SelectedItem == "New Cores")
            {
                // Progress Info
                vm.ProgressInfo_Text = "Fetching Cores List...";

                // Create Builtbot Cores List
                Parse.ParseBuildbotCoresIndex(vm);

                // Create PC Cores List
                Parse.ScanPcCoresDir(vm);

                // Create Cores to Update List
                Queue.UpdatedCores(vm);

                // Check if Cores Up To Date
                // If All Cores up to date, display message
                Queue.CoresUpToDateCheck(vm); //Note there are Clears() in this method
            }



            // -----------------------------------------------
            // Ready
            // -----------------------------------------------
            if (ready == true)
            {
                // Start Download
                if (CheckForInternetConnection() == true)
                {
                    Download.StartDownload(vm);
                }
                // Internet Connection Failed
                else
                {
                    MessageBox.Show("Could not detect Internet Connection.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);

                    return;
                }
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