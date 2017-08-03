using Stellar.Properties;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
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
        // Windows
        public static Configure configure;
        public static Checklist checklist;

        // Ready Check
        public static int ready = 1; // If 0 halt progress


        // -----------------------------------------------
        // Window Defaults
        // -----------------------------------------------
        public MainWindow() // Pass Exclude Cores Data from Checklist
        {
            InitializeComponent();

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
            e.Cancel = true;
            System.Windows.Forms.Application.ExitThread();
            Environment.Exit(0);
        }

        // -----------------------------------------------
        // Clear All
        // -----------------------------------------------
        public static void ClearAll()
        {
            // Lists
            if (Queue.NightliesList != null)
            {
                Queue.NightliesList.Clear();
                Queue.NightliesList.TrimExcess();
            }

            if (Queue.ListPcCoresName != null)
            {
                Queue.ListPcCoresName.Clear();
                Queue.ListPcCoresName.TrimExcess();
            }

            if (Queue.ListPcCoresDateModified != null)
            {
                Queue.ListPcCoresDateModified.Clear();
                Queue.ListPcCoresDateModified.TrimExcess();
            }

            if (Queue.ListPcCoresDateModifiedFormatted != null)
            {
                Queue.ListPcCoresDateModifiedFormatted.Clear();
                Queue.ListPcCoresDateModifiedFormatted.TrimExcess();
            }

            if (Queue.ListBuildbotCoresName != null)
            {
                Queue.ListBuildbotCoresName.Clear();
                Queue.ListBuildbotCoresName.TrimExcess();
            }

            if (Queue.ListBuildbotCoresDate != null)
            {
                Queue.ListBuildbotCoresDate.Clear();
                Queue.ListBuildbotCoresDate.TrimExcess();
            }

            if (Queue.ListBuildbotID != null)
            {
                Queue.ListBuildbotID.Clear();
                Queue.ListBuildbotID.TrimExcess();
            }

            if (Queue.ListUpdatedCoresName != null)
            {
                Queue.ListUpdatedCoresName.Clear();
                Queue.ListUpdatedCoresName.TrimExcess();
            }

            if (Queue.CollectionUpdatedCoresName != null)
            {
                Queue.CollectionUpdatedCoresName = null;
            }

            // Do Not Clear
            //
            // ListExcludedCores
            // ListPcCoresDateCreated
            // ListPcCoresDateCreatedFormatted

            // Strings
            Parse.parseUrl = string.Empty;
            Paths.buildbotArchitecture = string.Empty;
            Parse.nightly7z = string.Empty;
        }


        // -----------------------------------------------
        // Folder Browser Popup 
        // -----------------------------------------------
        public void FolderBrowser() // Method
        {
            // Open Folder
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            DialogResult result = folderBrowserDialog.ShowDialog();

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
            System.Windows.MessageBox.Show("RetroArch Nightly Updater (Unofficial) \nby wyzrd \n\nNew versions at https://stellarupdater.github.io. \n\nPlease install 7-Zip for this program to properly extract files. \nhttp://www.7-zip.org \n\nThis software is licensed under GNU GPLv3. \nSource code is included in the archive with this executable. \n\nImage Credit: \nESO/José Francisco (josefrancisco.org), Milky Way \nESO, NGC 1232, Galaxy \nNASA, IC 405 Flaming Star \nNASA, NGC 5189, Spiral Nebula \nNASA, M100, Galaxy \nNASA, IC 405, Lagoon \nNASA, Solar Flare \nNASA, Rho Ophiuchi, Dark Nebula \nNASA, N159, Star Dust \nNASA, NGC 6357, Chaos \n\nThis software comes with no warranty, express or implied, and the author makes no representation of warranties. The author claims no responsibility for damages resulting from any use or misuse of the software.");
        }

        // -----------------------------------------------
        // Configure Settings Window Button
        // -----------------------------------------------
        private void buttonConfigure_Click(object sender, RoutedEventArgs e)
        {
            // Open Configure Window
            configure = new Configure();
            configure.Left = this.Left + 25;
            configure.Top = this.Top - 205;
            configure.Owner = Window.GetWindow(this);
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
                System.Windows.MessageBox.Show("Please choose RetroArch Folder location first.");
            }
        }

        // -----------------------------------------------
        // BuildBot Web Server Open Button
        // -----------------------------------------------
        private void buttonBuildBotDir_Click(object sender, RoutedEventArgs e)
        {
            // Call SetArchitecture Method
            // If Dropdown Combobox is 32-bit, parseURL is x86 URL. If 64-bit, x86_64 URL.
            Paths.SetArchitecture(this);

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
            // Clear All for next pass
            ClearAll();

            // Call SetArchitecture Method
            Paths.SetArchitecture(this);

            Paths.SetUrls(this);

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
        }

        // -----------------------------------------------
        // Combobox Download (On Changed)
        // -----------------------------------------------
        private void comboBoxDownload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset Update Button Text to "Update"
            buttonUpdateTextBlock.Text = "Update";

            // Get the currently selected item
            //comboBoxDownloadItem = comboBoxDownload.SelectedItem.ToString();

            // Save Download Combobox Settings for next launch
            Settings.Default["download"] = comboBoxDownload.SelectedItem;
            Settings.Default.Save();
            Settings.Default.Reload();

            // Cross Thread
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                // New Install
                //
                if ((string)comboBoxDownload.SelectedItem== "New Install")
                {
                    // Change Update Button Text
                    buttonUpdateTextBlock.Text = "Install";

                    // Warn user about New Install
                    System.Windows.MessageBox.Show("This will install a New Nightly RetroArch + Cores. \n\nIt will overwrite any existing files/configs in the selected folder. \n\nDo not use the New Install option to Update.");

                    // Save Download Combobox Settings back to RA+Cores instead of New Install for next launch
                    Settings.Default["download"] = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }

                // Upgrade
                //
                if ((string)comboBoxDownload.SelectedItem == "Upgrade")
                {
                    // Change Update Button Text
                    buttonUpdateTextBlock.Text = "Upgrade";

                    // Warn user about Upgrade
                    System.Windows.MessageBox.Show("Backup your configs and custom shaders! Large Download. \n\nThis will fully upgrade RetroArch to the latest version. \n\nUpdate Cores separately using \"Cores\" menu option. \nFor small updates use the \"RetroArch\" or \"RA+Cores\" menu option.");

                    // Save Download Combobox Settings back to RA+Cores instead of Upgrade for next launch
                    Settings.Default["download"] = "RA+Cores";
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }

                // New Cores
                //
                if ((string)comboBoxDownload.SelectedItem == "New Cores")
                {
                    // Change Update Button Text to "Upgrade"
                    buttonUpdateTextBlock.Text = "Download";

                    // Message
                    //System.Windows.MessageBox.Show("Press the Check Button to see if New or Missing Cores are available.");

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

            // Clear All if checked/unchecked for next pass
            ClearAll();
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
            // Clear All to prevent Lists doubling up
            ClearAll();

            // Progress Info
            labelProgressInfo.Content = "Checking...";

            // Call SetArchitecture Method
            Paths.SetArchitecture(this);

            // If Downloading RetroArch and NOT Cores
            if ((string)comboBoxDownload.SelectedItem == "New Install"
                || (string)comboBoxDownload.SelectedItem == "Upgrade" 
                || (string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "RetroArch" 
                || (string)comboBoxDownload.SelectedItem == "Redist")
            {
                // Call parse Page (HTML) Method
                Parse.ParseBuildbotPage(this);

                // Display message if download available
                if (!string.IsNullOrEmpty(Parse.nightly7z)) // If Last Item in Nightlies List is available
                {
                    System.Windows.MessageBox.Show("Download Available \n\n" + Paths.buildbotArchitecture + "\n\n" + Parse.nightly7z);
                }
                else // If Last Item in Nightlies List cannot be found
                {
                    System.Windows.MessageBox.Show("Could not find download.");
                }
            }

            // Clear the string to allow Checking again
            //Parse.nightly7z = string.Empty;

            // Progress Info
            labelProgressInfo.Content = "";

            // -------------------------
            // CORES
            // -------------------------
            // If Download Combobox Cores or RA+Cores selected
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "Cores"
                || (string)comboBoxDownload.SelectedItem == "New Cores")
            {
                // Call Methods - Build Cores Lists
                Parse.ScanPcCoresDir(this);

                // Call Parse Builtbot Page Method
                Parse.ParseBuildbotCoresIndex(this);

                // -------------------------
                // Core Check
                // -------------------------
                // Call New Cores Method
                if ((string)comboBoxDownload.SelectedItem == "New Cores")
                {
                    Queue.NewCores(this);
                }
                // Call Updated Cores Method
                else
                {
                    Queue.UpdatedCores(this);
                }


                // Call Cores Up To Date Method
                // If All Cores up to date, display message
                Queue.CoresUpToDateCheck(this);

                // -------------------------
                // Window Checklist Popup
                // -------------------------
                // If Update List greater than 0, Popup Checklist
                if (Queue.ListUpdatedCoresName.Count != 0)
                {
                    // Trim List if new
                    Queue.ListUpdatedCoresName.TrimExcess();

                    // Add Updated Cores List to List Box (pass to constructor)
                    // (old winforms way was just to add myListBox.DataSource = myList. Without ObservableCollection)
                    Queue.CollectionUpdatedCoresName = new ObservableCollection<string>(Queue.ListUpdatedCoresName);

                    // Add PC Cores Name+Date to List Box (pass to constructor)
                    Queue.CollectionPcCoresNameDate = new ObservableCollection<string>(Queue.ListPcCoresNameDate);

                    // Add Buildbot Cores Name+Date to List Box (pass to constructor)
                    Queue.CollectionBuildbotNameDate = new ObservableCollection<string>(Queue.ListBuildbotCoresNameDate);


                    // Open Checklist Window
                    checklist = new Checklist();
                    checklist.Owner = Window.GetWindow(this);
                    checklist.ShowDialog();


                    // -------------------------
                    // Clear Name+Date Lists to prevent doubling up on next pass
                    // -------------------------
                    if (Queue.ListPcCoresNameDate != null)
                    {
                        Queue.ListPcCoresNameDate.Clear();
                        Queue.ListPcCoresNameDate.TrimExcess();
                    }

                    if (Queue.ListBuildbotID != null)
                    {
                        Queue.ListBuildbotID.Clear();
                        Queue.ListBuildbotID.TrimExcess();
                    }

                    if (Queue.ListBuildbotCoresNameDate != null)
                    {
                        Queue.ListBuildbotCoresNameDate.Clear();
                        Queue.ListBuildbotCoresNameDate.TrimExcess();
                    }

                    if (Queue.CollectionPcCoresNameDate != null)
                    {
                        Queue.CollectionPcCoresNameDate.Clear();
                    }

                    if (Queue.CollectionBuildbotNameDate != null)
                    {
                        Queue.CollectionBuildbotNameDate.Clear();
                    }
                }
            }

            // Clear All again to prevent doubling up on Update button
            //
            ClearAll();
        }


        // -----------------------------------------------
        // Update Button - Launches Download and 7-Zip Extraction
        // -----------------------------------------------
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected item
            //comboBoxDownloadItem = comboBoxDownload.SelectedItem.ToString();


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
                ready = 0;
                System.Windows.MessageBox.Show("Please select your RetroArch main folder.");
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




            // -----------------------------------------------
            // If New Install (RetroArch + Cores)
            // -----------------------------------------------
            if ((string)comboBoxDownload.SelectedItem == "New Install")
            {
                // Set Cores Folder (Dont Scan PC)
                Paths.coresPath = Paths.retroarchPath + "cores\\";
                // Call Parse Builtbot Page Method
                Parse.ParseBuildbotCoresIndex(this);
            }



            // -----------------------------------------------
            // If RetroArch or Cores Update
            // -----------------------------------------------
            // If Update Download Combobox Cores or RA+Cores selected
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "Cores" 
                || (string)comboBoxDownload.SelectedItem == "New Cores")
            {
                // Call Methods - Build Cores Lists
                Parse.ScanPcCoresDir(this);

                // Call Parse Builtbot Page Method
                Parse.ParseBuildbotCoresIndex(this);

                // -------------------------
                // Core Check
                // -------------------------
                // Call New Cores Method
                if ((string)comboBoxDownload.SelectedItem == "New Cores")
                {
                    Queue.NewCores(this);
                }
                // Call Updated Cores Method
                else
                {
                    Queue.UpdatedCores(this);
                }


                // Call Cores Up To Date Method
                // If All Cores up to date, display message
                //
                Queue.CoresUpToDateCheck(this);

                // -------------------------
                // Clear Name+Date Lists to prevent doubling up on next pass
                // -------------------------
                if (Queue.ListPcCoresNameDate != null)
                {
                    Queue.ListPcCoresNameDate.Clear();
                    Queue.ListPcCoresNameDate.TrimExcess();
                }

                if (Queue.ListBuildbotCoresNameDate != null)
                {
                    Queue.ListBuildbotCoresNameDate.Clear();
                    Queue.ListBuildbotCoresNameDate.TrimExcess();
                }

                if (Queue.CollectionPcCoresNameDate != null)
                {
                    Queue.CollectionPcCoresNameDate.Clear();
                }

                if (Queue.CollectionPcCoresNameDate != null)
                {
                    Queue.CollectionBuildbotNameDate.Clear();
                }
            }



            // -----------------------------------------------
            // Ready
            // -----------------------------------------------
            if (ready == 1)
            {
                // Start Download
                Download.StartDownload(this);
            }
            else
            {
                // Restart & Reset ready value
                ready = 1;
                // Call Garbage Collector
                GC.Collect();
            }
        }

    }
}