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
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Linq;

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

        public static string failedImportMessage;


        public Configure()
        {
            InitializeComponent();

            //this.mainwindow = mainwindow;

            this.MinWidth = 450;
            this.MinHeight = 235;
            this.MaxWidth = 450;
            this.MaxHeight = 235;


            // -----------------------------------------------------------------
            /// <summary>
            ///     Control Binding
            /// </summary>
            // -----------------------------------------------------------------
            //DataContext = vm;

            // --------------------------------------------------
            // Load Update Auto Check Text
            // --------------------------------------------------
            if (VM.MainView.UpdateAutoCheck_IsChecked == true)
            {
                VM.MainView.UpdateAutoCheck_Text = "On";
            }
            else if (VM.MainView.UpdateAutoCheck_IsChecked == false)
            {
                VM.MainView.UpdateAutoCheck_Text = "Off";
            }
        }


        /// <summary>
        ///    Updates Auto Check - Checked
        /// </summary>
        private void tglUpdateAutoCheck_Checked(object sender, RoutedEventArgs e)
        {
            // Update Toggle Text
            VM.MainView.UpdateAutoCheck_Text = "On";
        }
        /// <summary>
        ///    Updates Auto Check - Unchecked
        /// </summary>
        private void tglUpdateAutoCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            // Update Toggle Text
            VM.MainView.UpdateAutoCheck_Text = "Off";
        }


        // ----------------------------------------------------------------------------------------------
        // METHODS 
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Load Control Defaults
        // -----------------------------------------------
        public static void LoadDefaults(MainWindow mainwindow )
        {
            // Main Window
            mainwindow.Top = 0;
            mainwindow.Left = 0;
            VM.MainView.RetroArchPath_Text = string.Empty;
            VM.MainView.Download_SelectedItem = "RetroArch";
            VM.MainView.Architecture_SelectedItem = "64-bit";
            VM.MainView.Server_SelectedItem = "buildbot";
            VM.MainView.SevenZipPath_Text = "<auto>";
            VM.MainView.WinRARPath_Text = "<auto>";
            VM.MainView.LogPath_IsChecked = false;
            VM.MainView.LogPath_Text = string.Empty;
            //VM.MainView.LogPath_IsEnabled = false;
            VM.MainView.Theme_SelectedItem = "Milky Way";
            VM.MainView.UpdateAutoCheck_IsChecked = true;
            VM.MainView.Update_Text = "Update";
        }

        // -----------------------------------------------
        // 7-Zip Folder Browser Popup 
        // -----------------------------------------------
        public void sevenZipFolderBrowser() // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                VM.MainView.SevenZipPath_Text = OpenFileDialog.FileName;

                // Set the sevenZipPath string
                sevenZipPath = VM.MainView.SevenZipPath_Text;
            }
        }

        // -----------------------------------------------
        // WinRAR Folder Browser Popup 
        // -----------------------------------------------
        public void winRARFolderBrowser() // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                VM.MainView.WinRARPath_Text = OpenFileDialog.FileName;

                // Set the winRARPath string
                winRARPath = VM.MainView.WinRARPath_Text;
            }
        }

        // -----------------------------------------------
        // Log Folder Browser Popup 
        // -----------------------------------------------
        public void logFolderBrowser() // Method
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                VM.MainView.LogPath_Text = folderBrowserDialog.SelectedPath + "\\"; //end with backslash

                // Set the winRARPath string
                logPath = VM.MainView.LogPath_Text;
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
            sevenZipFolderBrowser();
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
            // Display Folder Path in Textbox
            VM.MainView.SevenZipPath_Text = "<auto>";

            // Set the sevenZipPath string
            sevenZipPath = VM.MainView.SevenZipPath_Text; //<auto>
        }



        // -----------------------------------------------
        // WinRAR Textbox Click
        // -----------------------------------------------
        private void textBoxWinRARConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            

            winRARFolderBrowser();
        }

        // -----------------------------------------------
        // WinRAR Auto Path (On Click)
        // -----------------------------------------------
        private void buttonWinRARAuto_Click(object sender, RoutedEventArgs e)
        {
            // Display Folder Path in Textbox
            VM.MainView.WinRARPath_Text = "<auto>";

            // Set the winRARPath string
            winRARPath = VM.MainView.WinRARPath_Text; //<auto>
        }



        // -----------------------------------------------
        // Log Checkbox (Checked)
        // -----------------------------------------------
        private void checkBoxLogConfig_Checked(object sender, RoutedEventArgs e)
        {
            

            // Enable the Log
            logEnable = true;
        }

        // -----------------------------------------------
        // Log Checkbox (Unchecked)
        // -----------------------------------------------
        private void checkBoxLogConfig_Unchecked(object sender, RoutedEventArgs e)
        {
            // Disable the Log
            logEnable = false;
        }

        // -----------------------------------------------
        // Log Textbox (On Click)
        // -----------------------------------------------
        private void textBoxLogConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            logFolderBrowser();
        }

        // -----------------------------------------------
        // Log Auto Path (On Click)
        // -----------------------------------------------
        private void buttonLogAuto_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck Log Checkbox
            VM.MainView.LogPath_IsChecked = false;
            checkBoxLogConfig.IsChecked = false;

            // Clear Path in Textbox
            VM.MainView.LogPath_Text = string.Empty;

            // Set the logPath string
            logPath = string.Empty;
        }

        // -----------------------------------------------
        // Clear All Saved Settings Button
        // -----------------------------------------------
        private void buttonClearAllSavedSettings_Click(object sender, RoutedEventArgs e)
        {
            // Check if config.ini Exists
            if (File.Exists(Paths.configFile))
            {
                // Yes/No Dialog Confirmation
                //
                MessageBoxResult result = MessageBox.Show(
                    "Delete: \n\n" + Paths.configFile,
                    "Delete config.ini Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);
                switch (result)
                {
                    // Create
                    case MessageBoxResult.Yes:
                        File.Delete(Paths.configFile);

                        // Reload Control Defaults
                        LoadDefaults(mainwindow);

                        // Restart Program
                        Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                        break;

                    // Use Default
                    case MessageBoxResult.No:
                        break;
                }
            }
            // If config.ini Not Found
            else
            {
                MessageBox.Show("No Previous Config File Found.",
                                "Notice",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                return;
            }
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
            // Black
            if (VM.MainView.Theme_SelectedItem == "Black") //not used
            {
                // Call Method
                //removeTheme();
            }

            // Milky Way
            else if (VM.MainView.Theme_SelectedItem == "Milky Way")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Spiral Galaxy")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Spiral Nebula")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Solar Flare")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Flaming Star")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Dark Galaxy")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Lagoon")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Dark Nebula")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Star Dust")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Chaos")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

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
            else if (VM.MainView.Theme_SelectedItem == "Cosmic Web")
            {
                theme = VM.MainView.Theme_SelectedItem.Replace(" ", string.Empty);

                // Change Theme Resource
                App.Current.Resources.MergedDictionaries.Clear();
                App.Current.Resources.MergedDictionaries.Add(new ResourceDictionary()
                {
                    Source = new Uri("Theme" + theme + ".xaml", UriKind.RelativeOrAbsolute)
                });

                // Image Credit
                labelTheme.Content = "Volker Springel, MPA";
            }
        }


        /// <summary>
        ///    INI Reader
        /// </summary>
        /*
        * Source: GitHub Sn0wCrack
        * https://gist.github.com/Sn0wCrack/5891612
        */
        public partial class INIFile
        {
            public string path { get; private set; }

            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
            [DllImport("kernel32", CharSet = CharSet.Unicode)]
            private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            public INIFile(string INIPath)
            {
                path = INIPath;
            }
            public void Write(string Section, string Key, string Value)
            {
                WritePrivateProfileString(Section, Key, Value, this.path);
            }

            public string Read(string Section, string Key)
            {
                StringBuilder temp = new StringBuilder(255);
                int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
                return temp.ToString();
            }
        }



        /// <summary>
        ///    Export Config
        /// </summary>
        public static void ExportConfig(MainWindow mainwindow, /*MainViewModel vm,*/ string configFile)
        {
            // Check if Profiles Directory exists
            //bool exists = Directory.Exists(Paths.configDir);

            // Check if Directory Exists
            if (!Directory.Exists(Paths.configDir))
            {
                // Yes/No Dialog Confirmation
                //
                MessageBoxResult resultExport = MessageBox.Show("Config Folder does not exist. Automatically create it?",
                                                                "Directory Not Found",
                                                                MessageBoxButton.YesNo,
                                                                MessageBoxImage.Information);
                switch (resultExport)
                {
                    // Create
                    case MessageBoxResult.Yes:
                        try
                        {
                            Directory.CreateDirectory(Paths.configDir);
                        }
                        catch
                        {
                            MessageBox.Show("Could not create Config folder. May require Administrator privileges.",
                                            "Error",
                                            MessageBoxButton.OK,
                                            MessageBoxImage.Error);
                        }
                        break;
                    // Use Default
                    case MessageBoxResult.No:
                        break;
                }
            }

            // If Dir Exists, Save config file
            else if (Directory.Exists(Paths.configDir))
            {
                // Start INI File Write
                INIFile inif = new INIFile(configFile);

                // -------------------------
                // Main Window
                // -------------------------
                // Window Position Top
                inif.Write("Main Window", "Position_Top", Convert.ToString(mainwindow.Top));

                // Window Position Left
                inif.Write("Main Window", "Position_Left", Convert.ToString(mainwindow.Left));

                // RetroArch Path
                inif.Write("Main Window", "RetroArchPath_Text", VM.MainView.RetroArchPath_Text);

                // Server
                inif.Write("Main Window", "Server_SelectedItem", VM.MainView.Server_SelectedItem);

                // Download
                inif.Write("Main Window", "Download_SelectedItem", VM.MainView.Download_SelectedItem);

                // Architecture
                inif.Write("Main Window", "Architecture_SelectedItem", VM.MainView.Architecture_SelectedItem);


                // -------------------------
                // Configure Window
                // -------------------------
                // 7-Zip Path
                inif.Write("Configure Window", "SevenZipPath_Text", VM.MainView.SevenZipPath_Text);

                // WinRAR Path
                inif.Write("Configure Window", "WinRARPath_Text", VM.MainView.WinRARPath_Text);

                // Log Path CheckBox
                inif.Write("Configure Window", "LogPath_IsChecked", Convert.ToString(VM.MainView.LogPath_IsChecked).ToLower());

                // Log Path
                inif.Write("Configure Window", "LogPath_Text", VM.MainView.LogPath_Text);

                // Theme
                inif.Write("Configure Window", "Theme_SelectedItem", VM.MainView.Theme_SelectedItem);

                // Update Auto Check
                inif.Write("Configure Window", "UpdateAutoCheck_IsChecked", Convert.ToString(VM.MainView.UpdateAutoCheck_IsChecked).ToLower());
            }   
        }


        /// <summary>
        ///    Import Config
        /// </summary>
        public static void ImportConfig(MainWindow mainwindow,/* MainViewModel vm,*/ string configFile)
        {
            try
            {
                List<string> listFailedImports = new List<string>();

                // Start INI File Read
                INIFile inif = null;
                if (File.Exists(configFile) == true)
                {
                    inif = new INIFile(configFile);
                }

                // -------------------------
                // Main Window
                // -------------------------
                // Window Position Top
                double? top = Double.Parse(inif.Read("Main Window", "Position_Top"));
                if (top != null)
                {
                    mainwindow.Top = Convert.ToDouble(inif.Read("Main Window", "Position_Top"));
                }

                // Window Position Left
                double? left = Double.Parse(inif.Read("Main Window", "Position_Left"));
                if (left != null)
                {
                    mainwindow.Left = Convert.ToDouble(inif.Read("Main Window", "Position_Left"));
                }

                // RetroArch Path
                VM.MainView.RetroArchPath_Text = inif.Read("Main Window", "RetroArchPath_Text");

                // Server
                string server = inif.Read("Main Window", "Server_SelectedItem");
                if (VM.MainView.Server_Items.Contains(server))
                    VM.MainView.Server_SelectedItem = server;
                else
                    listFailedImports.Add("Main Window: Server");

                // Download
                string download = inif.Read("Main Window", "Download_SelectedItem");
                if (VM.MainView.Download_Items.Contains(download))
                    VM.MainView.Download_SelectedItem = download;
                else
                    listFailedImports.Add("Main Window: Download");

                // Architecture
                string architecture = inif.Read("Main Window", "Architecture_SelectedItem");
                if (VM.MainView.Architecture_Items.Contains(architecture))
                    VM.MainView.Architecture_SelectedItem = architecture;
                else
                    listFailedImports.Add("Main Window: Architecture");

                // -------------------------
                // Configure Window
                // -------------------------
                // 7-Zip Path
                VM.MainView.SevenZipPath_Text = inif.Read("Configure Window", "SevenZipPath_Text");

                // WinRAR Path
                VM.MainView.WinRARPath_Text = inif.Read("Configure Window", "WinRARPath_Text");

                // Logh Path
                VM.MainView.LogPath_IsChecked = Convert.ToBoolean(inif.Read("Configure Window", "LogPath_IsChecked").ToLower());

                // Log Path CheckBox
                VM.MainView.LogPath_Text = inif.Read("Configure Window", "LogPath_Text");

                // Theme
                string theme = inif.Read("Configure Window", "Theme_SelectedItem");
                if (VM.MainView.Theme_Items.Contains(theme))
                    VM.MainView.Theme_SelectedItem = theme;
                else
                    listFailedImports.Add("Configure Window: Theme");

                // Update Auto Check
                VM.MainView.UpdateAutoCheck_IsChecked = Convert.ToBoolean(inif.Read("Configure Window", "UpdateAutoCheck_IsChecked").ToLower());


                // --------------------------------------------------
                // Failed Imports
                // --------------------------------------------------
                if (listFailedImports.Count > 0 && listFailedImports != null)
                {
                    failedImportMessage = string.Join(Environment.NewLine, listFailedImports);

                    // Detect which screen we're on
                    var allScreens = System.Windows.Forms.Screen.AllScreens.ToList();
                    var thisScreen = allScreens.SingleOrDefault(s => mainwindow.Left >= s.WorkingArea.Left && mainwindow.Left < s.WorkingArea.Right);

                    // Start Window
                    FailedImportWindow failedimportwindow = new FailedImportWindow();

                    // Position Relative to MainWindow
                    failedimportwindow.Left = Math.Max((mainwindow.Left + (mainwindow.Width - failedimportwindow.Width) / 2), thisScreen.WorkingArea.Left);
                    failedimportwindow.Top = Math.Max((mainwindow.Top + (mainwindow.Height - failedimportwindow.Height) / 2), thisScreen.WorkingArea.Top);

                    // Open Window
                    failedimportwindow.Show();
                }
            }

            // Error Loading config.ini
            //
            catch
            {
                // Delete config.ini and Restart
                // Check if config.ini Exists
                if (File.Exists(Paths.configFile))
                {
                    // Yes/No Dialog Confirmation
                    //
                    MessageBoxResult result = MessageBox.Show(
                        "Could not load config.ini. \n\nDelete config and reload defaults?",
                        "Error",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Error);
                    switch (result)
                    {
                        // Create
                        case MessageBoxResult.Yes:
                            File.Delete(Paths.configFile);

                            // Reload Control Defaults
                            LoadDefaults(mainwindow);

                            // Restart Program
                            Process.Start(Application.ResourceAssembly.Location);
                            Application.Current.Shutdown();
                            break;

                        // Use Default
                        case MessageBoxResult.No:
                            // Force Shutdown
                            System.Windows.Forms.Application.ExitThread();
                            Environment.Exit(0);
                            return;
                    }
                }
                // If config.ini Not Found
                else
                {
                    MessageBox.Show("No Previous Config File Found.",
                                    "Notice",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Information);

                    return;
                }
            }
        }


    }

}