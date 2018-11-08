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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO;
using Stellar.Properties;
using System.Configuration;
using System.Diagnostics;

namespace Stellar
{
    /// <summary>
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class Configure : Window
    {
        private MainWindow mainwindow;
        public static Debugger debugger; // Debug Window

        public static string sevenZipPath; // 7-Zip Config Settings Path
        public static string winRARPath; // WinRAR Config Settings Path

        public static string logPath; // stellar.log Config Settings Path
        public static bool logEnable; //checkBoxLogConfig, Enable or Disable Log, true or false

        public static string theme; // Background Theme Image


        public Configure(MainWindow mainwindow, ViewModel vm)
        {
            InitializeComponent();

            this.mainwindow = mainwindow;

            this.MinWidth = 450;
            this.MinHeight = 235;
            this.MaxWidth = 450;
            this.MaxHeight = 235;


            // -----------------------------------------------------------------
            /// <summary>
            ///     Control Binding
            /// </summary>
            // -----------------------------------------------------------------
            DataContext = vm;

            // --------------------------------------------------
            // StartUp Defaults
            // --------------------------------------------------
            //vm.Theme_SelectedItem = "Milky Way";


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
            // Load From Saved Settings
            // --------------------------------------------------
            // Theme CombBox
            Configure.ConfigTheme(this, vm);

            // 7-Zip Path
            Configure.Config7zipPath(this, vm);

            // WinRAR Path
            Configure.ConfigWinRARPath(this, vm);

            // Log CheckBox
            Configure.ConfigLogToggle(this, vm);

            // Log Path
            Configure.ConfigLogPath(this, vm);

            // Log Path
            Configure.UpdateAutoCheck(this, vm);
        }


        /// <summary>
        /// Load Theme
        /// </summary>
        public static void ConfigTheme(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load Theme
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.themes.ToString())) // null check
                {
                    //System.Windows.MessageBox.Show("Debug");

                    Configure.theme = "MilkyWay";

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.Theme_SelectedItem = "Milky Way";
                    //}

                    // Change Theme Resource
                    App.Current.Resources.MergedDictionaries.Clear();
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                    {
                        Source = new Uri("Theme" + Configure.theme + ".xaml", UriKind.RelativeOrAbsolute)
                    });

                    // Save Theme for next launch
                    Settings.Default.themes = Configure.theme; // Theme
                    Settings.Default.comboBoxThemes = "Milky Way"; // ComboBox Selected Item
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                // Saved Settings
                else
                {
                    //System.Windows.MessageBox.Show("Debug");

                    Configure.theme = Settings.Default.themes.ToString();

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.Theme_SelectedItem = Settings.Default.comboBoxThemes;
                    //}

                    // Change Theme Resource
                    App.Current.Resources.MergedDictionaries.Clear();
                    App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                    {
                        Source = new Uri("Theme" + Configure.theme + ".xaml", UriKind.RelativeOrAbsolute)
                    });
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Load 7-Zip Path
        /// </summary>
        public static void Config7zipPath(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load 7-Zip Path from Saved Settings
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.sevenZipPath.ToString())) // null check
                {
                    // Load Saved Settings Override
                    Configure.sevenZipPath = "<auto>";

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.SevenZip_Text = Configure.sevenZipPath;
                    //}
                }
                // Saved Settings
                else
                {
                    Configure.sevenZipPath = Settings.Default.sevenZipPath.ToString();

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.SevenZip_Text = Configure.sevenZipPath;
                    //}
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Load WinRAR Path
        /// </summary>
        public static void ConfigWinRARPath(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load WinRAR Path from Saved Settings
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.winRARPath.ToString())) // null check
                {
                    // Load Saved Settings Override
                    Configure.winRARPath = "<auto>";

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.WinRAR_Text = Configure.winRARPath;
                    //}

                }
                // Saved Settings
                else
                {
                    Configure.winRARPath = Settings.Default.winRARPath.ToString();

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.WinRAR_Text = Configure.winRARPath;
                    //}
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Load Log Toggle
        /// </summary>
        public static void ConfigLogToggle(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load Log Enable/Disable from Saved Settings
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.logEnable.ToString())) // null check
                {
                    Configure.logEnable = false;

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.LogPath_IsChecked = false;
                    //}
                }
                // Saved Settings
                else
                {
                    Configure.logEnable = Settings.Default.logEnable;

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.LogPath_IsChecked = Settings.Default.checkBoxLog;
                    //}
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// Load Log Path
        /// </summary>
        public static void ConfigLogPath(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load Log Path from Saved Settings
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.logPath.ToString())) // null check
                {
                    Configure.logPath = string.Empty;

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.LogPath_Text = Configure.logPath;
                    //}
                }
                // Saved Settings
                else
                {

                    Configure.logPath = Settings.Default.logPath.ToString();

                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.LogPath_Text = Configure.logPath;
                    //}
                }
            }
            catch
            {

            }
        }


        /// <summary>
        ///    Updates Auto Check - Checked
        /// </summary>
        private void tglUpdateAutoCheck_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Update Toggle Text
            vm.UpdateAutoCheck_Text = "On";
            Settings.Default.UpdateAutoCheckLabel = "On";

            //Prevent Loading Corrupt App.Config
            try
            {
                // Save Toggle Settings
                // must be done this way or you get "convert object to bool error"
                if (vm.UpdateAutoCheck_IsChecked == true)
                {
                    Settings.Default.UpdateAutoCheck = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                else if (vm.UpdateAutoCheck_IsChecked == false)
                {
                    Settings.Default.UpdateAutoCheck = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                // Delete Old App.Config
                string filename = ex.Filename;

                if (File.Exists(filename) == true)
                {
                    File.Delete(filename);
                    Settings.Default.Upgrade();
                    // Properties.Settings.Default.Reload();
                }
                else
                {

                }
            }
        }
        /// <summary>
        ///    Updates Auto Check - Unchecked
        /// </summary>
        private void tglUpdateAutoCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Update Toggle Text
            vm.UpdateAutoCheck_Text = "Off";
            Settings.Default.UpdateAutoCheckLabel = "Off";

            // Prevent Loading Corrupt App.Config
            try
            {
                // Save Toggle Settings
                // must be done this way or you get "convert object to bool error"
                if (vm.UpdateAutoCheck_IsChecked == true)
                {
                    Settings.Default.UpdateAutoCheck = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                else if (vm.UpdateAutoCheck_IsChecked == false)
                {
                    Settings.Default.UpdateAutoCheck = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
            }
            catch (ConfigurationErrorsException ex)
            {
                // Delete Old App.Config
                string filename = ex.Filename;

                if (File.Exists(filename) == true)
                {
                    File.Delete(filename);
                    Settings.Default.Upgrade();
                    // Properties.Settings.Default.Reload();
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// Updates Auto Check
        /// </summary>
        public static void UpdateAutoCheck(Configure configure, ViewModel vm)
        {
            // -----------------------------------------------
            // Load Log Path from Saved Settings
            // -----------------------------------------------
            try
            {
                // First Time Use
                if (string.IsNullOrEmpty(Settings.Default.UpdateAutoCheck.ToString())) // null check
                {
                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.UpdateAutoCheck_IsChecked = true;
                        vm.UpdateAutoCheck_Text = "On";
                        Settings.Default.UpdateAutoCheckLabel = "On";
                    //}
                }
                // Saved Settings
                else
                {
                    // Set ComboBox if Configure Window is Open
                    //if (configure != null)
                    //{
                        vm.UpdateAutoCheck_IsChecked = Settings.Default.UpdateAutoCheck;
                        vm.UpdateAutoCheck_Text = Settings.Default.UpdateAutoCheckLabel;
                    //}
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
        // 7-Zip Folder Browser Popup 
        // -----------------------------------------------
        public void sevenZipFolderBrowser(ViewModel vm) // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                vm.SevenZip_Text = OpenFileDialog.FileName;

                // Set the sevenZipPath string
                sevenZipPath = vm.SevenZip_Text;

                try
                {
                    // Save 7-zip Path for next launch
                    Settings.Default.sevenZipPath = vm.SevenZip_Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }

            }
        }

        // -----------------------------------------------
        // WinRAR Folder Browser Popup 
        // -----------------------------------------------
        public void winRARFolderBrowser(ViewModel vm) // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                vm.WinRAR_Text = OpenFileDialog.FileName;

                // Set the winRARPath string
                winRARPath = vm.WinRAR_Text;

                try
                {
                    // Save WinRAR Path for next launch
                    Settings.Default.winRARPath = vm.WinRAR_Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }

        // -----------------------------------------------
        // Log Folder Browser Popup 
        // -----------------------------------------------
        public void logFolderBrowser(ViewModel vm) // Method
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                vm.LogPath_Text = folderBrowserDialog.SelectedPath + "\\"; //end with backslash

                // Set the winRARPath string
                logPath = vm.LogPath_Text;

                try
                {
                    // Save 7-zip Path for next launch
                    Settings.Default.logPath = vm.LogPath_Text;
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
        // 7-Zip Textbox Click
        // -----------------------------------------------
        private void textBox7zipConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            sevenZipFolderBrowser(vm);
        }

        // -----------------------------------------------
        // 7-Zip Textbox (Text Changed)
        // -----------------------------------------------
        private void textBox7zipConfig_TextChanged(object sender, TextChangedEventArgs e)
        {
            // dont use
        }

        // -----------------------------------------------
        // 7-Zip Auto Path (On Click)
        // -----------------------------------------------
        private void button7zipAuto_Click(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Display Folder Path in Textbox
            vm.SevenZip_Text = "<auto>";

            // Set the sevenZipPath string
            sevenZipPath = vm.SevenZip_Text; //<auto>

            try
            {
                // Save 7-zip Path path for next launch
                Settings.Default.sevenZipPath = vm.SevenZip_Text;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }



        // -----------------------------------------------
        // WinRAR Textbox Click
        // -----------------------------------------------
        private void textBoxWinRARConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            winRARFolderBrowser(vm);
        }

        // -----------------------------------------------
        // WinRAR Auto Path (On Click)
        // -----------------------------------------------
        private void buttonWinRARAuto_Click(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Display Folder Path in Textbox
            vm.WinRAR_Text = "<auto>";

            // Set the winRARPath string
            winRARPath = vm.WinRAR_Text; //<auto>

            try
            {
                // Save 7-zip Path path for next launch
                Settings.Default.winRARPath = vm.WinRAR_Text;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }



        // -----------------------------------------------
        // Log Checkbox (Checked)
        // -----------------------------------------------
        private void checkBoxLogConfig_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Enable the Log
            logEnable = true;

            // must be done this way or you get "convert object to bool error"
            if (vm.LogPath_IsChecked == true)
            {
                try
                {
                    // Save Checkbox Settings
                    Settings.Default.checkBoxLog = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();

                    // Save Log Enable Settings
                    Settings.Default.logEnable = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
            else if (vm.LogPath_IsChecked == false)
            {
                try
                {
                    // Save Checkbox Settings
                    Settings.Default.checkBoxLog = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();

                    // Save Log Enable Settings
                    Settings.Default.logEnable = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }

        // -----------------------------------------------
        // Log Checkbox (Unchecked)
        // -----------------------------------------------
        private void checkBoxLogConfig_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Disable the Log
            logEnable = false;

            // must be done this way or you get "convert object to bool error"
            if (vm.LogPath_IsChecked == true)
            {
                try
                {
                    // Save Checkbox Settings
                    Settings.Default.checkBoxLog = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();

                    // Save Log Enable Settings
                    Settings.Default.logEnable = true;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
            else if (vm.LogPath_IsChecked == false)
            {
                try
                {
                    // Save Checkbox Settings
                    Settings.Default.checkBoxLog = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();

                    // Save Log Enable Settings
                    Settings.Default.logEnable = false;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }

        // -----------------------------------------------
        // Log Textbox (On Click)
        // -----------------------------------------------
        private void textBoxLogConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            logFolderBrowser(vm);
        }

        // -----------------------------------------------
        // Log Auto Path (On Click)
        // -----------------------------------------------
        private void buttonLogAuto_Click(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Uncheck Log Checkbox
            vm.LogPath_IsChecked = false;

            // Clear Path in Textbox
            vm.LogPath_Text = string.Empty;

            // Set the sevenZipPath string
            logPath = string.Empty;
            try
            {
                // Save Log Path path for next launch
                Settings.Default.logPath = string.Empty;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }

        // -----------------------------------------------
        // Clear All Saved Settings Button
        // -----------------------------------------------
        private void buttonClearAllSavedSettings_Click(object sender, RoutedEventArgs e)
        {
            string userProfile = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%");
            string appDataPath = "\\AppData\\Local\\Stellar";

            // Check if Directory Exists
            if (Directory.Exists(userProfile + appDataPath))
            {
                // Show Yes No Window
                System.Windows.Forms.DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "Delete " + userProfile + appDataPath, "Delete Directory Confirm", System.Windows.Forms.MessageBoxButtons.YesNo);
                // Yes
                if (dialogResult == System.Windows.Forms.DialogResult.Yes)
                {
                    // Delete leftover 2 Pass Logs in Program's folder and Input Files folder
                    using (Process delete = new Process())
                    {
                        delete.StartInfo.UseShellExecute = false;
                        delete.StartInfo.CreateNoWindow = false;
                        delete.StartInfo.RedirectStandardOutput = true;
                        delete.StartInfo.FileName = "cmd.exe";
                        delete.StartInfo.Arguments = "/c RD /Q /S " + "\"" + userProfile + appDataPath;
                        delete.Start();
                        delete.WaitForExit();
                        //delete.Close();
                    }

                    // Restart Program
                    Process.Start(Application.ResourceAssembly.Location);
                    Application.Current.Shutdown();
                }
                // No
                else if (dialogResult == System.Windows.Forms.DialogResult.No)
                {
                    //do nothing
                }
            }
            // If Stellar Folder Not Found
            else
            {
                MessageBox.Show("No Previous Settings Found.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }

            //// Revert 7-Zip
            //vm.SevenZip_Text = "<auto>";
            //sevenZipPath = vm.SevenZip_Text;

            //// Revert WinRAR
            //vm.WinRAR_Text = "<auto>";
            //winRARPath = vm.WinRAR_Text;

            //// Revert Log
            //vm.LogPath_IsChecked = false;
            //vm.LogPath_Text = string.Empty;
            //logPath = string.Empty;

            //// Save Current Window Location
            //// Prevents MainWindow from moving to Top 0 Left 0 while running
            //double left = mainwindow.Left;
            //double top = mainwindow.Top;

            //// Reset AppData Settings
            //Settings.Default.Reset();
            //Settings.Default.Reload();

            //// Set Window Location
            //mainwindow.Left = left;
            //mainwindow.Top = top;
        }

        // -----------------------------------------------
        // Debug Button
        // -----------------------------------------------
        private void btnDebug_Click(object sender, RoutedEventArgs e)
        {
            // Open Debugger Window
            debugger = new Debugger();
            debugger.ShowDialog();
        }

        // -----------------------------------------------
        // Set Theme (Combobox)
        // -----------------------------------------------
        private void comboBoxThemeConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // Black
            if (vm.Theme_SelectedItem == "Black") //not used
            {
                // Call Method
                //removeTheme();
            }

            // Milky Way
            else if (vm.Theme_SelectedItem == "Milky Way")
            {
                theme = "MilkyWay";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                //// Image Credit
                labelTheme.Content = "ESO";
            }

            // Spiral Galaxy
            else if (vm.Theme_SelectedItem == "Spiral Galaxy")
            {
                theme = "SpiralGalaxy";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                //// Image Credit
                labelTheme.Content = "ESO, NGC 1232";
            }

            // Spiral Nebula
            else if (vm.Theme_SelectedItem == "Spiral Nebula")
            {
                theme = "SpiralNebula";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, NGC 5189";
            }

            // Solar Flare
            else if (vm.Theme_SelectedItem == "Solar Flare")
            {
                theme = "SolarFlare";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA";
            }

            // Flaming Star
            else if (vm.Theme_SelectedItem == "Flaming Star")
            {
                theme = "FlamingStar";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, IC 405";
            }

            // Dark Galaxy
            else if (vm.Theme_SelectedItem == "Dark Galaxy")
            {
                theme = "DarkGalaxy";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, M100";
            }

            // Lagoon
            else if (vm.Theme_SelectedItem == "Lagoon")
            {
                theme = "Lagoon";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, IC 405";
            }

            // Dark Nebula
            else if (vm.Theme_SelectedItem == "Dark Nebula")
            {
                theme = "DarkNebula";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, Rho Ophiuchi";
            }
            // Star Dust
            else if (vm.Theme_SelectedItem == "Star Dust")
            {
                theme = "StarDust";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, N159";
            }
            // Chaos
            else if (vm.Theme_SelectedItem == "Chaos")
            {
                theme = "Chaos";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "NASA, NGC 6357";
            }
            // Cosmic Web
            else if (vm.Theme_SelectedItem == "Cosmic Web")
            {
                theme = "CosmicWeb";

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "Volker Springel, MPA";
            }

            // -------------------------
            // Save Selected Theme
            // -------------------------
            try
            {
                // Save Theme
                Settings.Default.themes = Configure.theme;
                Settings.Default.Save();
                Settings.Default.Reload();

                // Save ComboBox Selected Item
                Settings.Default.comboBoxThemes = vm.Theme_SelectedItem;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }

        }


    }

}