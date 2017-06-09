using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Stellar.Properties;
using System.Configuration;

/* ----------------------------------------------------------------------
    Stellar ~ RetroArch Nightly Updater by wyzrd
    http://x.co/nightly
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
    /// Interaction logic for Configure.xaml
    /// </summary>
    public partial class Configure : Window
    {
        public string sevenZipPath; // 7-Zip Config Settings Path (public - pass data)
        public string winRARPath; // WinRAR Config Settings Path (public - pass data)
        public string logPath; // stellar.log Config Settings Path (public - pass data)
        public bool logEnable; //checkBoxLogConfig, Enable or Disable Log, true or false - (public - pass data)

        string themeResource = string.Empty;
        private MainWindow mainwindow;

        public Configure()
        {
            // Configure, dont remove
        }

        public Configure(MainWindow mainwindow) // Pass Constructor from MainWindow
        {
            InitializeComponent();

            // Pass Constructor from MainWindow
            this.mainwindow = mainwindow;


            // ##################################################
            // Prevent Loading Corrupt App.Config
            // ##################################################
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
                    // you could optionally restart the app instead
                }
                else
                {

                }
            }


            // Combobox Themes #####
            //comboBoxArchitecture.Background = Brushes.Black;
            comboBoxThemeConfig.Foreground = Brushes.White;
            //comboBoxArchitecture.BorderBrush = Brushes.White;
            comboBoxThemeConfig.Resources.Add(SystemColors.WindowBrushKey, Brushes.Black);
            comboBoxThemeConfig.Resources.Add(SystemColors.HighlightBrushKey, Brushes.Green);

            try
            {
                // Combobox Theme Load Saved Settings
                if (!string.IsNullOrEmpty(Settings.Default["comboboxThemes"].ToString())) // auto/null check
                {
                    comboBoxThemeConfig.SelectedItem = Settings.Default["comboboxThemes"];
                }
                // Else Select Default Theme
                else if ((string)comboBoxThemeConfig.SelectedItem == null)
                {
                    comboBoxThemeConfig.SelectedItem = "Milky Way";
                }


                // 7-Zip Path
                sevenZipPath = "<auto>"; // First time use
                textBox7zipConfig.Text = sevenZipPath; // First time use

                // Load 7-Zip Path from saved settings
                if (Settings.Default["sevenZipPath"].ToString() != "<auto>" && !string.IsNullOrEmpty(Settings.Default["sevenZipPath"].ToString())) // auto/null check
                {
                    textBox7zipConfig.Text = Settings.Default["sevenZipPath"].ToString();
                    sevenZipPath = Settings.Default["sevenZipPath"].ToString();
                }

                // WinRAR Path
                winRARPath = "<auto>"; // First time use
                textBoxWinRARConfig.Text = winRARPath; // First time use

                // Load WinRAR Path from saved settings
                if (Settings.Default["winRARPath"].ToString() != "<auto>" && !string.IsNullOrEmpty(Settings.Default["winRARPath"].ToString())) // auto/null check
                {
                    textBoxWinRARConfig.Text = Settings.Default["winRARPath"].ToString();
                    winRARPath = Settings.Default["winRARPath"].ToString();
                }

                // Log Checkbox
                checkBoxLogConfig.IsChecked = false; // First time use
                // Load Log Checkbox from saved settings
                checkBoxLogConfig.IsChecked = Settings.Default.checkBoxLog;

                // Log Path
                logPath = ""; // First time use
                textBoxLogConfig.Text = logPath; // First time use

                // Load Log Path from saved settings
                if (!string.IsNullOrEmpty(Settings.Default["logPath"].ToString())) // null check
                {
                    textBoxLogConfig.Text = Settings.Default["logPath"].ToString();
                    logPath = Settings.Default["logPath"].ToString();
                }
            }
            catch
            {

            }
        }

        // #####################################################################################################################
        // METHODS 
        // #####################################################################################################################

        // ###############################################
        // 7-Zip Folder Browser Popup 
        //###############################################
        public void sevenZipFolderBrowser() // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                textBox7zipConfig.Text = OpenFileDialog.FileName;

                // Set the sevenZipPath string
                sevenZipPath = textBox7zipConfig.Text;

                try
                {
                    // Save 7-zip Path for next launch
                    Settings.Default["sevenZipPath"] = textBox7zipConfig.Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }

            }
        }

        // ###############################################
        // WinRAR Folder Browser Popup 
        //###############################################
        public void winRARFolderBrowser() // Method
        {
            var OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            System.Windows.Forms.DialogResult result = OpenFileDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                textBoxWinRARConfig.Text = OpenFileDialog.FileName;

                // Set the winRARPath string
                winRARPath = textBoxWinRARConfig.Text;

                try
                {
                    // Save WinRAR Path for next launch
                    Settings.Default["winRARPath"] = textBoxWinRARConfig.Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }

        // ###############################################
        // Log Folder Browser Popup 
        //###############################################
        public void logFolderBrowser() // Method
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                textBoxLogConfig.Text = folderBrowserDialog.SelectedPath + "\\"; //end with backslash

                // Set the winRARPath string
                logPath = textBoxLogConfig.Text;

                try
                {
                    // Save 7-zip Path for next launch
                    Settings.Default["logPath"] = textBoxLogConfig.Text;
                    Settings.Default.Save();
                    Settings.Default.Reload();
                }
                catch
                {

                }
            }
        }



        // #####################################################################################################################
        // CONTROLS
        // #####################################################################################################################

        // ###############################################
        // 7-Zip Textbox Click
        //###############################################
        private void textBox7zipConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sevenZipFolderBrowser();
        }

        // ###############################################
        // 7-Zip Textbox (Text Changed)
        //###############################################
        private void textBox7zipConfig_TextChanged(object sender, TextChangedEventArgs e)
        {
            // dont use
        }

        // ###############################################
        // 7-Zip Auto Path (On Click)
        //###############################################
        private void button7zipAuto_Click(object sender, RoutedEventArgs e)
        {
            // Display Folder Path in Textbox
            textBox7zipConfig.Text = "<auto>";

            // Set the sevenZipPath string
            sevenZipPath = textBox7zipConfig.Text; //<auto>

            try
            {
                // Save 7-zip Path path for next launch
                Settings.Default["sevenZipPath"] = textBox7zipConfig.Text;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }



        // ###############################################
        // WinRAR Textbox Click
        //###############################################
        private void textBoxWinRARConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            winRARFolderBrowser();
        }

        // ###############################################
        // WinRAR Auto Path (On Click)
        //###############################################
        private void buttonWinRARAuto_Click(object sender, RoutedEventArgs e)
        {
            // Display Folder Path in Textbox
            textBoxWinRARConfig.Text = "<auto>";

            // Set the winRARPath string
            winRARPath = textBoxWinRARConfig.Text; //<auto>

            try
            {
                // Save 7-zip Path path for next launch
                Settings.Default["winRARPath"] = textBoxWinRARConfig.Text;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }



        // ###############################################
        // Log Checkbox (Checked)
        // ###############################################
        private void checkBoxLogConfig_Checked(object sender, RoutedEventArgs e)
        {
            // Enable the Log
            logEnable = true;

            // must be done this way or you get "convert object to bool error"
            if (checkBoxLogConfig.IsChecked == true)
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
            else if (checkBoxLogConfig.IsChecked == false)
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

        // ###############################################
        // Log Checkbox (Unchecked)
        // ###############################################
        private void checkBoxLogConfig_Unchecked(object sender, RoutedEventArgs e)
        {
            // Disable the Log
            logEnable = false;

            // must be done this way or you get "convert object to bool error"
            if (checkBoxLogConfig.IsChecked == true)
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
            else if (checkBoxLogConfig.IsChecked == false)
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

        // ###############################################
        // Log Textbox (On Click)
        //###############################################
        private void textBoxLogConfig_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            logFolderBrowser();
        }

        // ###############################################
        // Log Auto Path (On Click)
        //###############################################
        private void buttonLogAuto_Click(object sender, RoutedEventArgs e)
        {
            // Uncheck Log Checkbox
            checkBoxLogConfig.IsChecked = false;

            // Clear Path in Textbox
            textBoxLogConfig.Text = string.Empty;

            // Set the sevenZipPath string
            logPath = string.Empty;
            try
            {
                // Save Log Path path for next launch
                Settings.Default["logPath"] = string.Empty;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }

        // ###############################################
        // Clear All Saved Settings Button
        //###############################################
        private void buttonClearAllSavedSettings_Click(object sender, RoutedEventArgs e)
        {
            // Revert 7-Zip
            textBox7zipConfig.Text = "<auto>";
            sevenZipPath = textBox7zipConfig.Text;

            // Revert WinRAR
            textBoxWinRARConfig.Text = "<auto>";
            winRARPath = textBoxWinRARConfig.Text;

            // Revert Log
            checkBoxLogConfig.IsChecked = false;
            textBoxLogConfig.Text = string.Empty;
            logPath = string.Empty;

            Properties.Settings.Default.Reset();
        }

        // ###############################################
        // Set Theme (Method)
        //###############################################
        public void setTheme()
        {
            //MainWindow mainwindow = this.Owner as MainWindow;

            ImageBrush changeTheme = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), themeResource)));
            changeTheme.TileMode = TileMode.None;
            changeTheme.Stretch = Stretch.None;
            changeTheme.ViewportUnits = BrushMappingMode.Absolute;
            // Center Image, convert double to int or decimals will cause blur
            changeTheme.Viewport = new Rect(Convert.ToInt32(-(changeTheme.ImageSource.Width - mainwindow.Width)), Convert.ToInt32(-(changeTheme.ImageSource.Height - mainwindow.Height)), changeTheme.ImageSource.Width, changeTheme.ImageSource.Height);

            // Set Background to MainWindow
            mainwindow.Background = changeTheme;

            try
            {
                // Save Background string to Settings
                Settings.Default["themes"] = themeResource;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }
        }

        // ###############################################
        // Remove Theme
        //###############################################
        public void removeTheme()
        {
            // Set Color
            mainwindow.Background = null;
            mainwindow.Background = Brushes.Black;

            themeResource = string.Empty;

            try
            {
                Settings.Default["themes"] = string.Empty;
            }
            catch
            {

            }
        }

        // ###############################################
        // Set Theme (Combobox)
        //###############################################
        private void comboBoxThemeConfig_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the Main Window, may require configure window's owner set in MainWindow.xaml.cs
            MainWindow mainwindow = this.Owner as MainWindow;

            //Application.Current.MainWindow = Configure;

            // Black
            if ((string)comboBoxThemeConfig.SelectedItem == "Black") //not used
            {
                // Call Method
                removeTheme();
            }

            // Milky Way
            else if ((string)comboBoxThemeConfig.SelectedItem == "Milky Way")
            {
                // Background Resource Image
                themeResource = "Resources/bgMilkyWay.jpg";
                // Image Credit
                labelTheme.Content = "ESO";

                // Call Method
                setTheme();
            }

            // Spiral Galaxy
            else if ((string)comboBoxThemeConfig.SelectedItem == "Spiral Galaxy")
            {
                // Background Resource Image
                themeResource = "Resources/bgSpiralGalaxy.jpg";
                // Image Credit
                labelTheme.Content = "ESO, NGC 1232";

                // Call Method
                setTheme();
            }

            // Spiral Nebula
            else if ((string)comboBoxThemeConfig.SelectedItem == "Spiral Nebula")
            {
                // Background Resource Image
                themeResource = "Resources/bgSpiralNebula.jpg";
                // Image Credit
                labelTheme.Content = "NASA, NGC 5189";

                // Call Method
                setTheme();
            }

            // Solar Flare
            else if ((string)comboBoxThemeConfig.SelectedItem == "Solar Flare")
            {
                // Background Resource Image
                themeResource = "Resources/bgSolarFlare.jpg";
                // Image Credit
                labelTheme.Content = "NASA";

                // Call Method
                setTheme();
            }

            // Flaming Star
            else if ((string)comboBoxThemeConfig.SelectedItem == "Flaming Star")
            {
                // Background Resource Image
                themeResource = "Resources/bgFlamingStar.jpg";
                // Image Credit
                labelTheme.Content = "NASA, IC 405";

                // Call Method
                setTheme();
            }

            // Dark Galaxy
            else if ((string)comboBoxThemeConfig.SelectedItem == "Dark Galaxy")
            {
                // Background Resource Image
                themeResource = "Resources/bgDarkGalaxy.jpg";
                // Image Credit
                labelTheme.Content = "NASA, M100";

                // Call Method
                setTheme();
            }

            // Lagoon
            else if ((string)comboBoxThemeConfig.SelectedItem == "Lagoon")
            {
                // Background Resource Image
                themeResource = "Resources/bgLagoon.jpg";
                // Image Credit
                labelTheme.Content = "NASA, IC 405";

                // Call Method
                setTheme();
            }

            // Dark Nebula
            else if ((string)comboBoxThemeConfig.SelectedItem == "Dark Nebula")
            {
                // Background Resource Image
                themeResource = "Resources/bgDarkNebula.jpg";
                // Image Credit
                labelTheme.Content = "NASA, Rho Ophiuchi";

                // Call Method
                setTheme();
            }

            try
            {
                // Save Selected Theme
                Settings.Default["comboboxThemes"] = comboBoxThemeConfig.SelectedItem;
                Settings.Default.Save();
                Settings.Default.Reload();
            }
            catch
            {

            }

        }
    }

}
