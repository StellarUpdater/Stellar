using Stellar.Properties;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Ready Check
        public static int ready = 1; //If 0 halt progress

        // Windows Temp Path
        public static string tempPath = System.IO.Path.GetTempPath();

        // Configure Settings
        public static Configure configure = new Configure(); //must be "new" for pass data, do not change or remove

        // Lists - RetroArch Nightly
        public static List<string> NightliesList = new List<string>(); // Create Nightlies 7z List to be filled by Parse

        // Lists - PC Cores
        public static List<string> ListPcCoresName = new List<string>(); // PC File Name

        public static List<DateTime> ListPcCoresDateModified = new List<DateTime>(); // PC File Date & Time <DateTime> Important!
        public static List<string> ListPcCoresDateModifiedFormatted = new List<string>(); // PC File Date & Time Formatted

        //List<DateTime> ListPcCoresDateCreated = new List<DateTime>(); // PC File Date & Time <DateTime> Important!
        //List<string> ListPcCoresDateCreatedFormatted = new List<string>(); // PC File Date & Time Formatted

        // Lists - Buildbot Cores
        public static List<string> ListBuildbotCoresName = new List<string>(); // Buildbot File Name
        public static List<string> ListBuildbotCoresDate = new List<string>(); // Buildbot File Date & Time
        public static List<string> ListBuildbotCoresFullInstall = new List<string>(); // Buildbot Name Full Install List

        // Lists Updated Cores
        List<string> ListUpdatedCoresName = new List<string>(); // Updated Cores to Download
        public static ObservableCollection<string> CollectionUpdatedCoresName; // Displays List in Listbox
        public static Checklist checklist; // Checklist Window, do not change
        public static List<string> ListExcludedCores = new List<string>(); // Updated Cores to Download


        // Lists PC Cores / Buildbot Cores Name + Date
        public static List<DateTime> ListPcCoresDate = new List<DateTime>(); // PC File Date & Time <DateTime> Important!
        public static List<string> ListPcCoresDateFormatted = new List<string>(); // PC File Date & Time Formatted
        public static List<string> ListPcCoresNameDate = new List<string>(); // PC File Name+Date & Time
        public static ObservableCollection<string> CollectionPcCoresNameDate; // PC Cores Name+Date Observable Collection

        public static List<string> ListBuildbotCoresNameDate = new List<string>(); // Buildbot File Name + Date & Time
        public static ObservableCollection<string> CollectionBuildbotNameDate; // Buildbot Cores Name+Date Observable Collection

        // Web Downloads
        public static WebClient wc = new WebClient();
        public static WebClient wc2 = new WebClient();
        public static ManualResetEvent waiter = new ManualResetEvent(false); // Download one at a time

        public static string parseUrl = "https://buildbot.libretro.com/nightly/windows/x86_64/"; // Parse URL to be changed with Dropdown Combobox. Default is 64-bit.
        public static string parseCoresUrl = string.Empty; // Buildbot Cores URL to be parsed
        public static string libretro_x86 = "https://buildbot.libretro.com/nightly/windows/x86/"; // Download URL 32-bit
        public static string libretro_x86_64 = "https://buildbot.libretro.com/nightly/windows/x86_64/"; // Download URL 64-bit
        public static string libretro_x86_64_w32 = "https://buildbot.libretro.com/nightly/windows/x86_64_w32/"; // Download URL 64-bit w32

        // System Program Paths
        //not yet used
        public static string programsPath;
        public static string program_x86 = "C:\\Program Files (x86)\\";
        public static string program_x86_64 = "C:\\Program Files\\";

        // Buildbot
        //string buildbotCoresPage = string.Empty; //parseUrl + "latest/"
        public static string buildbotArchitecture; //x86 or x86_64\
        public static string buildbotArchitectureCores; //Used with latest/ element url. Used to fix w32.

        // Parse Strings
        public static string page;
        public static string element;

        // RetroArch Nightly
        public static string nightly7z; // The Parsed Dated 7z Nightly Filename
        public static string nightlyUrl; // Download URL + Dated 7z Filename

        // System Paths
        public static string retroarchPath; // Location of User's RetroArch Folder
        public static string coresPath; // Location of User's cores folder

        // Archiver
        public static string archiver; // 7-Zip / WinRar
        public static string extract; // Archiver CLI arguments
        public static string sevenZipPath; // 7-Zip Config Settings Path
        public static string winRARPath; // 7-Zip Config Settings Path

        // Progress Label Info
        public static string progressInfo;

        // Currently Selected String
        // Prevents cross-thread error and object reference not set error
        //Object comboBoxDownloadItem; //notused
        //Object comboBoxArchitectureItem = null; //notused

        String comboBoxDownloadItem;

        // Log
        public static bool logEnable; // Log Enable/Disable from Config Settings
        public static string logPath; // Log Config Settings Path

        // Themes
        public static string themeResource; //default background


        // ###############################################
        // Window Defaults
        //###############################################
        public MainWindow() // Pass Exclude Cores Data from Checklist
        {
            InitializeComponent();

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

            // ###############################################
            // Themes
            // ###############################################
            try
            {
                // Load Background if Saved Settings not empty, else use the default string
                if (!string.IsNullOrEmpty(Settings.Default["themes"].ToString()))
                {
                    themeResource = Settings.Default["themes"].ToString(); // Load Saved Settings Override
                }
                else
                {
                    themeResource = "Resources/bgMilkyWay.jpg";
                }

                // Background Image (no blur)
                ImageBrush changeTheme = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(this), themeResource)));
                changeTheme.TileMode = TileMode.None;
                changeTheme.Stretch = Stretch.None;
                changeTheme.ViewportUnits = BrushMappingMode.Absolute;
                // Center Image, convert double to int or decimals will cause blur
                changeTheme.Viewport = new Rect(Convert.ToInt32(-(changeTheme.ImageSource.Width - this.Width)), Convert.ToInt32(-(changeTheme.ImageSource.Height - this.Height)), changeTheme.ImageSource.Width, changeTheme.ImageSource.Height);
                this.Background = changeTheme;

            }
            catch
            {
                // do nothing
            }

            // ###############################################
            // Load RetroArch Path from Saved Settings 
            // ###############################################
            try
            {
                textBoxLocation.Text = Settings.Default["retroarchPath"].ToString();
                retroarchPath = Settings.Default["retroarchPath"].ToString();
            }
            catch
            {

            }


            // ###############################################
            // Load 7-Zip Path from Saved Settings
            // ###############################################
            // Load 7-Zip Path
            configure.sevenZipPath = "<auto>"; // first time use

            try
            {
                // If string has value besides <auto> or none
                if (Settings.Default["sevenZipPath"].ToString() != "<auto>" && !string.IsNullOrEmpty(Settings.Default["sevenZipPath"].ToString())) // auto/null check
                {
                    // Load Saved Settings Override
                    configure.sevenZipPath = Settings.Default["sevenZipPath"].ToString();
                }
            }
            catch
            {

            }


            // ###############################################
            // Load WinRAR Path from Saved Settings
            // ###############################################
            // Load Win-RAR Path
            configure.winRARPath = "<auto>"; // first time use

            try
            {
                // If string has value besides <auto> or none
                if (Settings.Default["winRARPath"].ToString() != "<auto>" && !string.IsNullOrEmpty(Settings.Default["winRARPath"].ToString())) // auto/null check
                {
                    // Load Saved Settings Override
                    configure.winRARPath = Settings.Default["winRARPath"].ToString();
                }
            }
            catch
            {

            }


            // ###############################################
            // Load Log Enable/Disable from Saved Settings
            // ###############################################
            configure.logEnable = false; // first time use
            configure.logEnable = Settings.Default.logEnable; // Load Saved Settings Override

            // ###############################################
            // Load Log Path from Saved Settings
            // ###############################################
            // Load Log Path
            configure.logPath = string.Empty; // first time use
            // If string has value besides <auto> or none
            if (!string.IsNullOrEmpty(Settings.Default["logPath"].ToString())) // null check
            {
                configure.logPath = Settings.Default["logPath"].ToString(); // Load Saved Settings Override
            }

            // ###############################################
            // Combobox Architecture Color
            // ###############################################
            comboBoxArchitecture.Foreground = Brushes.White;
            comboBoxArchitecture.Resources.Add(SystemColors.WindowBrushKey, Brushes.Black);

            // ###############################################
            // Combobox Download Color
            // ###############################################
            comboBoxDownload.Foreground = Brushes.White;
            comboBoxDownload.Resources.Add(SystemColors.WindowBrushKey, Brushes.Black);

            // ###############################################
            // Load Dropdown Combobox Architecture (32-bit, 64-bit, 64 w32) from Saved Settings
            // ###############################################

            comboBoxArchitecture.SelectedItem = Settings.Default["architecture"];
            // If Dropdown Combobox Architecture is Empty, set Default to 64-bit
            if (comboBoxArchitecture.SelectedItem == null)
            {
                comboBoxArchitecture.SelectedItem = "64-bit";
            }

            // ###############################################
            // Load Dropdown Combobox Downloads (RA+Cores, RetroArch, Cores) from Saved Settings
            // ###############################################

            comboBoxDownload.SelectedItem = Settings.Default["download"];
            // If Dropdown Combobox Architecture is Empty, set Default to 64-bit
            if (comboBoxDownload.SelectedItem == null)
            {
                comboBoxDownload.SelectedItem = "RA+Cores";
            }
        }

        // #####################################################################################################################
        // METHODS 
        // #####################################################################################################################

        // ###############################################
        // Close All
        // ###############################################
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            System.Windows.Forms.Application.ExitThread();
            Environment.Exit(0);
        }

        // ###############################################
        // Clear All
        // ###############################################
        public void clearAll()
        {
            // Lists
            NightliesList.Clear();
            NightliesList.TrimExcess();
            ListPcCoresName.Clear();
            ListPcCoresName.TrimExcess();
            ListPcCoresDateModified.Clear();
            ListPcCoresDateModified.TrimExcess();
            ListPcCoresDateModifiedFormatted.Clear();
            ListPcCoresDateModifiedFormatted.TrimExcess();
            //ListPcCoresDateCreated.Clear();
            //ListPcCoresDateCreated.TrimExcess();
            //ListPcCoresDateCreatedFormatted.Clear();
            //ListPcCoresDateCreatedFormatted.TrimExcess();
            ListBuildbotCoresName.Clear();
            ListBuildbotCoresName.TrimExcess();
            ListBuildbotCoresDate.Clear();
            ListBuildbotCoresDate.TrimExcess();
            ListUpdatedCoresName.Clear();
            ListUpdatedCoresName.TrimExcess();
            CollectionUpdatedCoresName = null;
            //ListExcludedCores.Clear(); // Dont clear Excluded Cores after Check
            //ListExcludedCores.TrimExcess();

            // Strings
            parseUrl = string.Empty;
            buildbotArchitecture = string.Empty;
            nightly7z = string.Empty;
        }


        // ###############################################
        // Folder Browser Popup 
        //###############################################
        public void folderBrowser() // Method
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = folderBrowserDialog.ShowDialog();

            // Popup Folder Browse Window
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                // Display Folder Path in Textbox
                textBoxLocation.Text = folderBrowserDialog.SelectedPath.TrimEnd('\\') + @"\";

                // Set the retroarchPath string
                retroarchPath = textBoxLocation.Text;

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

        // ###############################################
        // Scan PC Cores Directory
        // ###############################################
        public void scanPcCoresDir() // METHOD
        {
            // Cores Folder
            coresPath = retroarchPath + "cores\\"; //end with backslash RetroArch\cores\

            try // program will crash if files not found
            {
                // Add Core Name to List ##############################
                ListPcCoresName = Directory.GetFiles(coresPath, "*_libretro.dll") //match ending of a core name //Try EnumerateFiles
                        .Select(System.IO.Path.GetFileName)
                        .ToList();

                // Add Core Modified Dates to List ##############################
                // Extracts original File Modified Date when overwriting
                ListPcCoresDateModified = Directory.GetFiles(coresPath, "*_libretro.dll") //match ending of a core name
                    .Select(File.GetLastWriteTime) // Update all cores once to sync times, use File.GetCreationTime(path, DateTime.Now); //also might use File.GetLastWriteTime
                    .ToList();

                // Add Core Created Dates to List ##############################
                // Extracts original File Modified Date when overwriting
                //ListPcCoresDateCreated = Directory.GetFiles(coresPath, "*_libretro.dll") //match ending of a core name
                //    .Select(File.GetCreationTime) // Update all cores once to sync times, use File.GetCreationTime(path, DateTime.Now); //also might use File.GetLastWriteTime
                //    .ToList();
            }
            catch
            {
                //System.Windows.MessageBox.Show("Error.");
                ready = 0;
            }

            // Popup Error Message if PC Cores Name List has no items 0
            if (ListPcCoresName.Count == 0)
            {
                System.Windows.MessageBox.Show("Cores not found. \n\nPlease select your RetroArch main folder.");
                ready = 0;
            }

            // Convert Core Dates to String to allow Date Formatting yyyy-MM-dd HH:mm Military Time
            ListPcCoresDateModifiedFormatted = ListPcCoresDateModified.ConvertAll<string>(x => x.ToString("yyyy-MM-dd HH:mm")); //Keep Here


            // #########################
            // PC Cores Join Name + Date List
            // #########################
            // Convert Core Dates to String to allow Date Formatting yyyy-MM-dd HH:mm Military Time
            // Join Lists PC Name + PC Date (Formatted)
            for (int i = 0; i < ListPcCoresName.Count; i++)
            {
                ListPcCoresNameDate.Add(ListPcCoresName[i] + " " + ListPcCoresDateModifiedFormatted[i]);
            }


        }

        // ###############################################
        // Parse Builbot Cores HTML
        // ###############################################
        public void parseBuildbotCoresPage() // Method
        {
            // Begin Parse ############################
            // If No Internet Connect, program will crash. Use Try & Catch to display Error.
            try
            {
                // Parse the HTML Page from parseUrl ############################
                //parseCoresUlr is now defined in setArchitecture() method
                //parseCoresUrl = parseUrl + "latest/"; //DONT USE, old

                string buildbotCoresPage = wc.DownloadString(parseCoresUrl); // HTML Page

                // HTML Elements
                string elementName = "<td class='fb-n'><a href='/nightly/windows/" + buildbotArchitectureCores + "/latest/(.*?)'>"; // HTML Table containing Core_Name.zip, (.*?) is the text to keep
                string elementDate = "<td class='fb-d'>(.*?)</td>"; // HTML Table containing Date Time                

                // Add each Core Name to the List
                foreach (Match match in Regex.Matches(buildbotCoresPage, elementName))
                    ListBuildbotCoresName.Add(match.Groups[1].Value);

                // Remove from the List all that do no tcontain .dll.zip (filters out unwanted)
                ListBuildbotCoresName.RemoveAll(u => !u.Contains(".dll.zip"));

                // Remove .zip from all in List
                for (int i = 0; i < ListBuildbotCoresName.Count; i++)
                {
                    if (ListBuildbotCoresName[i].Contains(".zip"))
                        ListBuildbotCoresName[i] = ListBuildbotCoresName[i].Replace(".zip", "");
                }

                // Add each Parsed Core Date to the List
                foreach (Match match in Regex.Matches(buildbotCoresPage, elementDate))
                    ListBuildbotCoresDate.Add(match.Groups[1].Value);

                // Remove from the List all any blank (filters out unwanted)
                ListBuildbotCoresDate.RemoveAll(u => u == string.Empty);

                // Sort the Nighlies List, lastest core first
                //ListBuildbotCoresName.Sort(); //Disable Sort???

                // Create Full Install Cores List
                ListBuildbotCoresFullInstall = ListBuildbotCoresName;

                // Log Buildbot Cores
                //File.WriteAllLines("buildbotcores.log", ListBuildbotCoresFullInstall);


                // #########################
                // Buildbot Cores Name + Date List
                // #########################
                // Populate empty NameDate List to be modified for Join (Important!)
                for (int i = 0; i < ListBuildbotCoresName.Count; i++)
                {
                    ListBuildbotCoresNameDate.Add(ListBuildbotCoresName[i]);
                }
                // Join Lists Name & Date
                for (int i = 0; i < ListBuildbotCoresName.Count; i++)
                {
                    ListBuildbotCoresNameDate[i] = ListBuildbotCoresName[i] + " " + ListBuildbotCoresDate[i];
                }

            }
            catch
            {
                System.Windows.MessageBox.Show("Error: Cannot connect to Server.");
            }
        }

        // ###############################################
        // Create Updated Cores List
        // ###############################################
        public void updatedCores()
        {
            // This part gets complicated #############
            // For each Buildbot Date that is Greater than PC Date Modified Date, add a Buildbot Name to the Update List

            // Re-create ListbuilbotCoresName by comparing it to ListPcCoresName and keeping only Matches
            ListBuildbotCoresName = ListPcCoresName.Intersect(ListBuildbotCoresName).ToList(); //WORKING
            // ListBuildbotCoresName.Sort(); //Disable Sort???

            // List Compare - Use the newly created ListbuilbotCoresName to draw Names from
            for (int i = 0; i < ListBuildbotCoresName.Count; i++)
            {
                //If PC Core Name List Contains Buildbot Core Name [i]
                if (ListPcCoresName.Contains(ListBuildbotCoresName[i]))
                {
                    // If Buildbot Core Modified Date greater than > PC Core Creation Date
                    if (DateTime.Parse(ListBuildbotCoresDate[i]) >= DateTime.Parse(ListPcCoresDateModifiedFormatted[i]))
                    {
                        // Add Buildbot Core Name to Update List
                        ListUpdatedCoresName.Add(ListBuildbotCoresName[i]);
                    }
                }
                else
                {
                    // Remove [i] Item from Buildbot Name List (Prevent Index out of range)
                    ListBuildbotCoresName.Remove(ListBuildbotCoresName[i]);
                }

                // Add Non-Matching Cores to Rejected List
                //ListExcludedCores = ListPcCoresName.Except(ListBuildbotCoresName).ToList();
                //ListExcludedCores.Sort(); //Disable Sort???
            }
        }

        // ###############################################
        // Cores Up To Date Check
        // ###############################################
        public void coresUpToDateCheck()
        {
            // If All Cores up to date
            // If the Update List is empty, but PC Cores have been found and scanned
            if (ListUpdatedCoresName.Count == 0 && ListPcCoresName.Count != 0)
            {
                System.Windows.MessageBox.Show("Cores already lastest version.");
            }
        }

        // ###############################################
        // Select Architecture, Change URL to Parse
        // ###############################################
        public void setArchitecture() // Method
        {
            // ######### 32-bit & 64-bit Dropdown Comboboxes ##########
            // If 32-bit Selected, change Download Architecture to x86
            if ((string)comboBoxArchitecture.SelectedItem == "32-bit")
            {
                // Set Parse URL
                parseUrl = libretro_x86;
                parseCoresUrl = libretro_x86 + "latest/";
                // Set Programs Path
                programsPath = program_x86; //not yet used
                // Buildbot Architecture
                buildbotArchitecture = "x86";
                buildbotArchitectureCores = "x86";
            }
            // If 64-bit Selected, change Download Architecture to x86_64
            if ((string)comboBoxArchitecture.SelectedItem == "64-bit")
            {
                // Set Parse URL
                parseUrl = libretro_x86_64;
                parseCoresUrl = libretro_x86_64 + "latest/";
                // Set Programs Path
                programsPath = program_x86_64; //not yet used
                // Buildbot Architecture
                buildbotArchitecture = "x86_64";
                buildbotArchitectureCores = "x86_64";
            }
            // If win32 Selected, change Download Architecture to x86_64_w32
            if ((string)comboBoxArchitecture.SelectedItem == "64 w32")
            {
                // Set Parse URL
                parseUrl = libretro_x86_64; //64-bit URL, w32 is cores-only
                parseCoresUrl = libretro_x86_64_w32 + "latest/";
                // Set Programs Path
                programsPath = program_x86_64; //not yet used
                // Buildbot Architecture
                buildbotArchitecture = "x86_64"; //64-bit URL, w32 is cores-only
                buildbotArchitectureCores = "x86_64_w32";
            }
        }


        // ###############################################
        // Check if Archiver Exists, If true set string
        // ###############################################
        public void setArchiver() // Method
        {
            // Null Checker
            if (string.IsNullOrEmpty(configure.sevenZipPath))
            {
                ready = 0;
                System.Windows.MessageBox.Show("Please set 7-Zip Path in Settings.");
            }

            // Null Checker
            if (string.IsNullOrEmpty(configure.winRARPath))
            {
                ready = 0;
                System.Windows.MessageBox.Show("Please set WinRAR Path in Settings.");
            }

            // Auto
            // If 7-Zip or WinRAR Path is Configure Settings <Auto> ####################
            if (configure.sevenZipPath == "<auto>" && configure.winRARPath == "<auto>")
            {
                // Check for 7zip 32-bit
                if (File.Exists("C:\\Program Files (x86)\\7-Zip\\7z.exe"))
                {
                    // Path to 7-Zip
                    archiver = "C:\\Program Files (x86)\\7-Zip\\7z.exe";
                    // CLI Arguments unzip files
                    extract = "7-Zip"; //args selector
                }
                // Check for 7zip 64-bit
                else if (File.Exists("C:\\Program Files\\7-Zip\\7z.exe"))
                {
                    // Path to 7-Zip
                    archiver = "C:\\Program Files\\7-Zip\\7z.exe";
                    // CLI Arguments unzip files
                    extract = "7-Zip"; //args selector
                }
                // Check for WinRAR 32-bit
                else if (File.Exists("C:\\Program Files (x86)\\WinRAR\\WinRAR.exe"))
                {
                    // Path to WinRAR
                    archiver = "C:\\Program Files (x86)\\WinRAR\\WinRAR.exe";
                    // CLI Arguments unzip files
                    extract = "WinRAR"; //args selector
                }
                // Check for WinRAR 64-bit
                else if (File.Exists("C:\\Program Files\\WinRAR\\WinRAR.exe"))
                {
                    // Path to WinRAR
                    archiver = "C:\\Program Files\\WinRAR\\WinRAR.exe";
                    // CLI Arguments unzip files
                    extract = "WinRAR"; //args selector
                }
                else
                {
                    ready = 0;
                    System.Windows.MessageBox.Show("Please install 7-Zip or WinRAR to extract files.\n\nOr set Path in Settings.");
                }
            }

            // User Select
            // If 7-Zip Path is (Not Auto) and is Configure Settings <User Selected Path> ####################
            if (configure.sevenZipPath != "<auto>" && !string.IsNullOrEmpty(configure.sevenZipPath)) // Check null again
            {
                try
                {
                    // Get 7-Zip Path from the Configure Window (Pass Data)
                    sevenZipPath = configure.sevenZipPath;
                }
                catch
                {
                    ready = 0;
                    System.Windows.MessageBox.Show("Error: Could not load 7-Zip. Please restart the program.");
                }

                // Path to 7-Zip
                archiver = configure.sevenZipPath;
                // CLI Arguments unzip files
                extract = "7-Zip"; //args selector
            }

            // If WinRAR Path is (Not Auto) and is Configure Settings <User Selected Path> ####################
            else if (configure.winRARPath != "<auto>" && !string.IsNullOrEmpty(configure.winRARPath)) // Check null again
            {
                try
                {
                    // Get 7-Zip Path from the Configure Window (Pass Data)
                    winRARPath = configure.winRARPath;
                }
                catch
                {
                    ready = 0;
                    System.Windows.MessageBox.Show("Error: Could not load WinRAR. Please restart the program.");
                }

                // Path to WinRAR
                archiver = configure.winRARPath;
                // CLI Arguments unzip files
                extract = "WinRAR"; //args selector
            }

        }

        // ###############################################
        // Parse Page HTML
        // ###############################################
        public void parseBuildbotPage() // Method
        {
            // Begin Parse ############################
            // If No Internet Connect, program will crash. Use Try & Catch to display Error.
            // ###############################################
            // Update Selected
            // ###############################################
            try
            {
                // Parse the HTML Page from parseUrl
                page = wc.DownloadString(parseUrl); // HTML Page
                element = "<td class='fb-n'><a href='/nightly/windows/" + buildbotArchitecture + "/(.*?)'>"; // HTML Table containing Dated 7z, (.*?) is the text to keep

                // Add each 7z Date/Time to the Nightlies List
                foreach (Match match in Regex.Matches(page, element))
                    NightliesList.Add(match.Groups[1].Value);

                // Remove from the List all 7z files that do not contain _RetroArch.7z (filters out unwanted)
                NightliesList.RemoveAll(u => !u.Contains("_RetroArch.7z"));
                // Sort the Nighlies List, lastest 7z is first
                NightliesList.Sort(); //do not disable this sort

                // Get First Element of Nightlies List 
                nightly7z = NightliesList[NightliesList.Count - 1];
            }
            catch
            {
                System.Windows.MessageBox.Show("Error: Cannot connect to Server.");
            }

            // ###############################################
            // Full Install Selected
            // ###############################################
            // Get Current Selected Item
            //comboBoxDownloadItem = comboBoxDownload.SelectedItem;

            // RetroArch.exe
            if ((string)comboBoxDownload.SelectedItem == "Full Install")
            {
                // Fetch the Full RetroArch + Redist (not dated)
                nightly7z = "RetroArch.7z";
            }

            // Redistributable
            if ((string)comboBoxDownload.SelectedItem == "Redist")
            {
                // Fetch the Full RetroArch + Redist (not dated)
                nightly7z = "redist.7z";
            }

            // Set the URL's for 32-bit & 64-bit Dropdown Comboboxes ############################
            // If 32-bit Selected, change Download URL to x86
            if ((string)comboBoxArchitecture.SelectedItem == "32-bit")
            {
                if (!string.IsNullOrEmpty(nightly7z)) // If Last Item in Nightlies List is available
                {

                }
                else // If Last Item in Nightlies List cannot be found
                {
                    // Clear Download Textbox
                    textBoxDownload.Text = "";
                }

                // Create URL string for Uri
                nightlyUrl = libretro_x86 + nightly7z;

            }
            // If 64-bit OR 64 w32 Selected, change Download URL to x86_64
            if ((string)comboBoxArchitecture.SelectedItem == "64-bit" || (string)comboBoxArchitecture.SelectedItem == "64 w32")
            {
                if (!string.IsNullOrEmpty(nightly7z)) // If Last Item in Nightlies List is available
                {

                }
                else // If Last Item in Nightlies List cannot be found
                {
                    // Clear Download Textbox
                    textBoxDownload.Text = "";
                }

                // Create URL string for Uri
                nightlyUrl = libretro_x86_64 + nightly7z;
            }
        }

        // ###############################################
        // Set URLs
        // ###############################################
        // Display URLs in Download Textbox
        public void setUrls()
        {
            // If Full Install Selected, Textbox will display URL
            if ((string)comboBoxDownload.SelectedItem == "Full Install")
            {
                textBoxDownload.Text = parseUrl;
            }
            // If RA+Cores or Cores Selected, Textbox will display URL
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" || (string)comboBoxDownload.SelectedItem == "RetroArch")
            {
                textBoxDownload.Text = parseUrl;
            }
            // If Cores Selected, Textbox will display URL
            if ((string)comboBoxDownload.SelectedItem == "Cores")
            {
                textBoxDownload.Text = parseCoresUrl;
            }

            // If Cores Selected, Textbox will display URL
            if ((string)comboBoxDownload.SelectedItem == "Cores")
            {
                if ((string)comboBoxArchitecture.SelectedItem == "32-bit"
                    || (string)comboBoxArchitecture.SelectedItem == "64-bit"
                    || (string)comboBoxArchitecture.SelectedItem == "64 w32")
                {
                    textBoxDownload.Text = parseCoresUrl;
                }
            }

            // If Redist Selected, Textbox will display URL
            if ((string)comboBoxDownload.SelectedItem == "Redist")
            {
                textBoxDownload.Text = parseUrl;
            }

        }

        // ###############################################
        // Download Handlers
        // ###############################################
        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                labelProgressInfo.Content = progressInfo;
            });

            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                //labelProgress.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }
        void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Set the waiter Release
            // Must be here
            waiter.Set();

            Dispatcher.BeginInvoke((MethodInvoker)delegate
            {
                // Progress Info
                labelProgressInfo.Content = progressInfo;
            });
        }

        // ###############################################
        // Start Download Method
        // ###############################################
        private void startDownload()
        {
            this.Dispatcher.Invoke(() =>
            {
                // Start New Thread
                Thread thread = new Thread(() =>
                {
                    // ###############################################
                    // Full Install Download
                    // ###############################################
                    if ((string)comboBoxDownloadItem == "Full Install")
                    {
                        waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)

                        //////////////////////////////////////
                        // RetroArch Download
                        //////////////////////////////////////
                        Uri downloadUrl = new Uri(nightlyUrl); // nightlyUrl = x84/x86_64 + nightly7z
                        //Uri downloadUrl = new Uri("http://127.0.0.1:8888/2016-08-19_RetroArch.7z"); // TESTING Virtual Server URL
                        //Async
                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                        wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                        wc.DownloadFileAsync(downloadUrl, tempPath + nightly7z);

                        // Progress Info
                        progressInfo = "Downloading RetroArch...";

                        waiter.WaitOne();

                        // Progress Info
                        progressInfo = "RetroArch Complete";

                        // Extract ##################
                        using (Process execExtract = new Process())
                        {
                            // Allow 0.1 seconds before Extracting Files
                            Thread.Sleep(100);

                            //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                            execExtract.StartInfo.UseShellExecute = false;
                            execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                            execExtract.StartInfo.CreateNoWindow = true;
                            execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                            execExtract.StartInfo.FileName = archiver; //archiver string
                            // Extract -o and Overwrite -y Selected Files -r
                            if (extract == "7-Zip")
                            {
                                // Extract All Files *
                                execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + nightly7z + "\"" + " -o" + "\"" + retroarchPath + "\"" + " * -r -y";
                            }
                            else if (extract == "WinRAR")
                            {
                                // Extract All Files *
                                execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + nightly7z + "\"" + " * " + "\"" + retroarchPath + "\"" + " -y";
                            }
                            execExtract.Start();
                            execExtract.WaitForExit();
                            execExtract.Close();
                        }

                        // Delete Temporary Nightly 7z file ##################
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
                            deleteTemp.StartInfo.Arguments = "/c del " + "\"" + tempPath + nightly7z + "\"";
                            deleteTemp.Start();
                            deleteTemp.WaitForExit();
                            deleteTemp.Close();
                        }


                        //////////////////////////////////////
                        // Cores Download
                        /////////////////////////////////////
                        for (int i = 0; i < ListBuildbotCoresFullInstall.Count; i++) //problem core count & nightly7z
                        {
                            //Reset Waiter
                            //Must be here
                            waiter.Reset();

                            Uri downloadUrl2 = new Uri(parseCoresUrl + ListBuildbotCoresFullInstall[i] + ".zip");
                            //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + ListBuildbotCoresFullInstall[i] + ".zip"); //TESTING
                            //Async
                            wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                            wc2.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                            wc2.DownloadFileAsync(downloadUrl2, tempPath + ListBuildbotCoresFullInstall[i] + ".zip", i);

                            // Progress Info
                            progressInfo = "Downloading " + ListBuildbotCoresFullInstall[i];

                            //Wait until download is finished
                            waiter.WaitOne();

                            // If Last item in List
                            if (i == ListBuildbotCoresFullInstall.Count - 1)
                            {
                                // Progress Info
                                progressInfo = "RetroArch + Cores Install Complete";
                            }

                            // Extract ##################
                            using (Process execExtract = new Process())
                            {
                                // Allow 0.1 seconds before extraction
                                Thread.Sleep(100);

                                //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                                execExtract.StartInfo.UseShellExecute = false;
                                execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                                execExtract.StartInfo.CreateNoWindow = true;
                                execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                                execExtract.StartInfo.FileName = archiver; //archiver string
                                // Extract -o and Overwrite -y Selected Files -r
                                if (extract == "7-Zip")
                                {
                                    execExtract.StartInfo.Arguments = " e " + "\"" + tempPath + ListBuildbotCoresFullInstall[i] + ".zip" + "\"" + " -o" + "\"" + coresPath + "\"" + " -y"; //extract string
                                }
                                else if (extract == "WinRAR")
                                {
                                    execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + ListBuildbotCoresFullInstall[i] + ".zip" + "\"" + " " + "\"" + coresPath + "\"" + " -y";
                                }
                                execExtract.Start();
                                execExtract.WaitForExit();
                                execExtract.Close();

                                // Convert Local Time to Server Time
                                // This doesn't work in a Method
                                DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // .AddHours(12) Needs to be 6-12 hours ahead to be more recent than server? 24 Hour AM/PM Problem?

                                // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                                if (File.Exists(coresPath + ListBuildbotCoresFullInstall[i]))
                                {
                                    File.SetCreationTime(coresPath + ListBuildbotCoresFullInstall[i], libretroServerTime); // Created Date Time = Now, (used to be DateTime.Now)
                                    File.SetLastWriteTime(coresPath + ListBuildbotCoresFullInstall[i], libretroServerTime); //maybe disable modified date?
                                }
                            }

                            // Delete Temporary Nightly 7z file ##################
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
                                deleteTemp.StartInfo.Arguments = "/c del " + "\"" + tempPath + ListBuildbotCoresFullInstall[i] + ".zip" + "\"";
                                deleteTemp.Start();
                                deleteTemp.WaitForExit();
                                deleteTemp.Close();
                            }

                            // If Last item in List ####################################
                            if (i == ListBuildbotCoresFullInstall.Count - 1)
                            {
                                // Clear list to prevent doubling up ##################
                                clearAll();
                            }
                        }
                    }


                    // ####################################
                    // RetroArch Standalone Download
                    // ####################################
                    // If Download Combobox Cores or RA+Cores selected
                    if ((string)comboBoxDownloadItem == "RA+Cores" || (string)comboBoxDownloadItem == "RetroArch")
                    {
                        waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)

                        // RetroArch Download ################## 
                        Uri downloadUrl = new Uri(nightlyUrl); // nightlyUrl = x84/x86_64 + nightly7z
                        //Uri downloadUrl = new Uri("http://127.0.0.1:8888/2016-08-19_RetroArch.7z"); // TESTING Virtual Server URL
                        //Async
                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                        wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                        wc.DownloadFileAsync(downloadUrl, tempPath + nightly7z);

                        // Progress Info
                        progressInfo = "Downloading RetroArch...";

                        waiter.WaitOne();

                        // Progress Info
                        progressInfo = "RetroArch Complete";

                        // Extract ##################
                        using (Process execExtract = new Process())
                        {
                            // Allow 0.1 seconds before Extracting Files
                            Thread.Sleep(100);

                            //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                            execExtract.StartInfo.UseShellExecute = false;
                            execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                            execExtract.StartInfo.CreateNoWindow = true;
                            execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                            execExtract.StartInfo.FileName = archiver; //archiver string
                            // Extract -o and Overwrite -y Selected Files -r
                            if (extract == "7-Zip")
                            {
                                execExtract.StartInfo.Arguments = " e " + "\"" + tempPath + nightly7z + "\"" + " -o" + "\"" + retroarchPath + "\"" + " retroarch.exe retroarch_debug.exe -r -y";
                            }
                            else if (extract == "WinRAR")
                            {
                                execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + nightly7z + "\"" + " retroarch.exe retroarch_debug.exe " + "\"" + retroarchPath + "\"" + " -y";
                            }
                            execExtract.Start();
                            execExtract.WaitForExit();
                            execExtract.Close();

                            // Convert Local Time to Server Time
                            // This doesn't work in a Method
                            DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                            DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);

                            if (File.Exists(retroarchPath + "retroarch.exe"))
                            {
                                // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                                File.SetCreationTime(retroarchPath + "retroarch.exe", libretroServerTime); //Use Server Timezone, (used to be DateTime.Now)
                                File.SetLastWriteTime(retroarchPath + "retroarch.exe", libretroServerTime);
                            }
                            if (File.Exists(retroarchPath + "retroarch_debug.exe"))
                            {
                                // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                                File.SetCreationTime(retroarchPath + "retroarch_debug.exe", libretroServerTime);  //(used to be DateTime.Now)
                                File.SetLastWriteTime(retroarchPath + "retroarch_debug.exe", libretroServerTime);
                            }
                        }

                        // Delete Temporary Nightly 7z file ##################
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
                            deleteTemp.StartInfo.Arguments = "/c del " + "\"" + tempPath + nightly7z + "\"";
                            deleteTemp.Start();
                            deleteTemp.WaitForExit();
                            deleteTemp.Close();
                        }

                        // If Update Complete, and not downloading Cores, Clear All
                        if ((string)comboBoxDownloadItem == "RetroArch")
                        {
                            clearAll();

                            waiter = new ManualResetEvent(false);
                        }
                    }


                    // ####################################
                    // Cores Download
                    // ####################################
                    if ((string)comboBoxDownloadItem == "RA+Cores" || (string)comboBoxDownloadItem == "Cores")
                    {
                        // Compare Lists ####################################
                        // Change MainWindow's Excluded List to match Checklist's Excluded List
                        try
                        {
                            ListExcludedCores = checklist.ListExcludedCores; // If Checklist Window is not opened first before being called, program will crash
                        }
                        catch
                        {
                            //ListExcludedCores = ListExcludedCores; // If Checklist Window is not opened first, use the MainWindow List
                        }

                        // Remove Excluded Cores from the Update List
                        ListUpdatedCoresName = ListUpdatedCoresName.Except(ListExcludedCores).ToList();


                        // Begin ####################################
                        for (int i = 0; i < ListUpdatedCoresName.Count; i++) //problem core count & nightly7z
                        {
                            //Reset Waiter
                            //Must be here
                            waiter.Reset();
                        
                            Uri downloadUrl2 = new Uri(parseCoresUrl + ListUpdatedCoresName[i] + ".zip");
                            //Uri downloadUrl2 = new Uri("http://127.0.0.1:8888/latest/" + ListUpdatedCoresName[i] + ".zip"); //TESTING
                            //Async
                            wc2.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                            wc2.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                            wc2.DownloadFileAsync(downloadUrl2, tempPath + ListUpdatedCoresName[i] + ".zip", i);

                            // Progress Info
                            progressInfo = "Downloading " + ListUpdatedCoresName[i];

                            //Wait until download is finished
                            waiter.WaitOne();

                            // If Last item in List
                            if (i == ListUpdatedCoresName.Count - 1)
                            {
                                // If "RA+Cores" Combobox Download Selected
                                if ((string)comboBoxDownloadItem == "RA+Cores")
                                {
                                    // Progress Info
                                    progressInfo = "RetroArch + Cores Update Complete";
                                }
                                // If "Cores" Combobox Download Selected
                                if ((string)comboBoxDownloadItem == "Cores")
                                {
                                    // Progress Info
                                    progressInfo = "Cores Update Complete";
                                }
                            }

                            // Extract ##################
                            using (Process execExtract = new Process())
                            {
                                // Allow 0.1 seconds before extraction
                                Thread.Sleep(100);

                                //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                                execExtract.StartInfo.UseShellExecute = false;
                                execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                                execExtract.StartInfo.CreateNoWindow = true;
                                execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                                execExtract.StartInfo.FileName = archiver; //archiver string
                                // Extract -o and Overwrite -y Selected Files -r
                                if (extract == "7-Zip")
                                {
                                    execExtract.StartInfo.Arguments = " e " + "\"" + tempPath + ListUpdatedCoresName[i] + ".zip" + "\"" + " -o" + "\"" + coresPath + "\"" + " -y"; //extract string
                                }
                                else if (extract == "WinRAR")
                                {
                                    execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + ListUpdatedCoresName[i] + ".zip" + "\"" + " " + "\"" + coresPath + "\"" + " -y";
                                }
                                //else if (extract == "PeaZip") //not working
                                //{
                                //    exec7zip.StartInfo.Arguments = "cd " + "\"" + coresPath + "\"" + " && " + "\"" + archiver + "\"" + " -ext2here " + "\"" + tempPath + ListUpdatedCoresName[i] + ".zip" + "\"";
                                //}
                                execExtract.Start();
                                execExtract.WaitForExit();
                                execExtract.Close();

                                // Convert Local Time to Server Time
                                // This doesn't work in a Method
                                DateTime utcTime = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified);
                                TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                                DateTime libretroServerTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi); // .AddHours(12) Needs to be 6-12 hours ahead to be more recent than server? 24 Hour AM/PM Problem?

                                // Set the File Date Time Stamp - Very Important! Let's file sync compare for next update.
                                if (File.Exists(coresPath + ListUpdatedCoresName[i]))
                                {
                                    File.SetCreationTime(coresPath + ListUpdatedCoresName[i], libretroServerTime); // Created Date Time = Now, (used to be DateTime.Now)
                                    File.SetLastWriteTime(coresPath + ListUpdatedCoresName[i], libretroServerTime); //maybe disable modified date?
                                }
                            }

                            // Delete Temporary Nightly 7z file ##################
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
                                deleteTemp.StartInfo.Arguments = "/c del " + "\"" + tempPath + ListUpdatedCoresName[i] + ".zip" + "\"";
                                deleteTemp.Start();
                                deleteTemp.WaitForExit();
                                deleteTemp.Close();
                            }


                            // If Last item in List ####################################
                            if (i == ListUpdatedCoresName.Count - 1)
                            {
                                    // Write Log Append ##################
                                    if (configure.logEnable == true) // Only if Log is Enabled through Config Checkbox
                                    {
                                        using (FileStream fs = new FileStream(/*pass data*/configure.logPath + "stellar.log", FileMode.Append, FileAccess.Write))
                                        using (StreamWriter sw = new StreamWriter(fs))
                                        {
                                            sw.WriteLine(DateTime.Now);
                                            sw.WriteLine("######################################\n\n");
                                            // Append List
                                            for (int x = 0; x < ListUpdatedCoresName.Count; x++)
                                            {
                                                sw.WriteLine(ListUpdatedCoresName[x]);
                                            }
                                            sw.WriteLine(""); // Add return space
                                            sw.WriteLine("");
                                            // Close Log
                                            sw.Close();
                                        }
                                    }


                                // Clear list to prevent doubling up ##################
                                clearAll();
                            }

                        }
                    }


                    // ####################################
                    // Redistributable Download
                    // ####################################
                    if ((string)comboBoxDownloadItem == "Redist")
                    {
                        waiter = new ManualResetEvent(false); //start a new waiter for next pass (clicking update again)

                        // RetroArch Download ################## 
                        Uri downloadUrl = new Uri(nightlyUrl); // nightlyUrl = x84/x86_64 + nightly7z (redist.7z)
                        //Uri downloadUrl = new Uri("http://127.0.0.1:8888/2016-08-19_RetroArch.7z"); // TESTING Virtual Server URL
                        //Async
                        wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(wc_DownloadProgressChanged);
                        wc.DownloadFileCompleted += new AsyncCompletedEventHandler(wc_DownloadFileCompleted);
                        wc.DownloadFileAsync(downloadUrl, tempPath + nightly7z);

                        // Progress Info
                        progressInfo = "Downloading Redistributable...";

                        waiter.WaitOne();

                        // Progress Info
                        progressInfo = "Redistributable Complete";

                        // Extract ##################
                        using (Process execExtract = new Process())
                        {
                            // Allow 0.1 seconds before Extracting Files
                            Thread.Sleep(100);

                            //exec7zip.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //use with ShellExecute
                            execExtract.StartInfo.UseShellExecute = false;
                            execExtract.StartInfo.Verb = "runas"; //use with ShellExecute for admin
                            execExtract.StartInfo.CreateNoWindow = true;
                            execExtract.StartInfo.RedirectStandardOutput = true; //set to false if using ShellExecute
                            execExtract.StartInfo.FileName = archiver; //archiver string
                            // Extract -o and Overwrite -y Selected Files -r
                            if (extract == "7-Zip")
                            {
                                // Extract All Files *
                                execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + nightly7z + "\"" + " -o" + "\"" + retroarchPath + "\"" + " * -r -y";
                            }
                            else if (extract == "WinRAR")
                            {
                                // Extract All Files *
                                execExtract.StartInfo.Arguments = " x " + "\"" + tempPath + nightly7z + "\"" + " * " + "\"" + retroarchPath + "\"" + " -y";
                            }
                            execExtract.Start();
                            execExtract.WaitForExit();
                            execExtract.Close();
                        }

                        // Delete Temporary Nightly 7z file ##################
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
                            deleteTemp.StartInfo.Arguments = "/c del " + "\"" + tempPath + nightly7z + "\"";
                            deleteTemp.Start();
                            deleteTemp.WaitForExit();
                            deleteTemp.Close();
                        }

                        // If Update Complete, and not downloading Cores, Clear All
                        if ((string)comboBoxDownloadItem == "Redistributable")
                        {
                            clearAll();

                            waiter = new ManualResetEvent(false);
                        }
                    }

                }); //end thread

                // Start Download Thread
                thread.Start();

            }); //end dispatcher

        }



        // #####################################################################################################################
        // CONTROLS
        // #####################################################################################################################

        // ###############################################
        // Info Button
        // ###############################################
        private void buttonInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("RetroArch Nightly Updater (Unofficial) \nby wyzrd \n\nNew versions at http://x.co/nightly. \n\nPlease install 7-Zip for this program to properly extract files. \nhttp://www.7-zip.org \n\nThis software is licensed under GNU GPLv3. \nSource code is included in the archive with this executable. \n\nImage Credit: \nESO/José Francisco (josefrancisco.org), Milky Way \nESO, NGC 1232, Galaxy \nNASA, IC 405 Flaming Star \nNASA, NGC 5189, Spiral Nebula \nNASA, M100, Galaxy \nNASA, IC 405, Lagoon \nNASA, Solar Flare \nNASA, NGC 2818 \nNASA, Rho Ophiuchi \n\nThis software comes with no warranty, express or implied, and the author makes no representation of warranties. The author claims no responsibility for damages resulting from any use or misuse of the software.");
        }

        // ###############################################
        // Configure Settings Window Button
        // ###############################################
        private void buttonConfigure_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainwindow = this;
            // Open Configure Window ########################
            configure = new Configure(mainwindow);
            configure.Left = this.Left + 25;
            configure.Top = this.Top - 205;
            configure.Owner = Window.GetWindow(this);
            configure.ShowDialog();
        }

        // ###############################################
        // Stellar Website Button
        // ###############################################
        private void buttonStellarWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://x.co/nightly");
        }

        // ###############################################
        // RetroArch Website Button
        // ###############################################
        private void buttonRetroArchWebsite_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.libretro.com/index.php/retroarch-2/");
        }

        // ###############################################
        // RetroArch Folder Open Button
        // ###############################################
        private void buttonRetroArchExplorer_Click(object sender, RoutedEventArgs e)
        {
            // retroarchPath string is set when user chooses location from textbox
            if (retroarchPath != "")
            {
                Process.Start("explorer.exe", @retroarchPath);
            }
            else
            {
                System.Windows.MessageBox.Show("Please choose RetroArch Folder location first.");
            }
        }

        // ###############################################
        // BuildBot Web Server Open Button
        // ###############################################
        private void buttonBuildBotDir_Click(object sender, RoutedEventArgs e)
        {
            // Call setArchitecture Method
            // If Dropdown Combobox is 32-bit, parseURL is x86 URL. If 64-bit, x86_64 URL.
            setArchitecture();

            // Open the URL
            Process.Start(textBoxDownload.Text);
        }

        // ###############################################
        // Location RetroArch Label (On Click)
        // ###############################################
        private void labelLocation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Call Folder Browser Popup
            folderBrowser();
        }

        // ###############################################
        // Location RetroArch TextBox (On Click/Mouse Down)
        // ###############################################
        private void textBoxLocation_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            folderBrowser();
        }

        // ###############################################
        // Dropdown Combobox Platform/Architecture 32-bit, 64-bit
        // ###############################################
        private void comboBoxArchitecture_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear All for next pass
            clearAll();

            // Call setArchitecture Method
            setArchitecture();

            setUrls();

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

        // ###############################################
        // Combobox Download (On Changed)
        // ###############################################
        private void comboBoxDownload_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Reset Update Button Text to "Update"
            buttonUpdateTextBlock.Text = "Update";

            // Get the currently selected item
            comboBoxDownloadItem = comboBoxDownload.SelectedItem.ToString();

            // Save Download Combobox Settings for next launch
            Settings.Default["download"] = comboBoxDownload.SelectedItem;
            Settings.Default.Save();
            Settings.Default.Reload();

            // Full Install
            if ((string)comboBoxDownloadItem == "Full Install")
            {
                // Change Update Button Text to "Install"
                buttonUpdateTextBlock.Text = "Install";

                // Warn user about Full Install
                System.Windows.MessageBox.Show("This will install a New Nightly RetroArch + Cores. \n\nIt will overwrite any existing files/configs in the selected folder. \n\nDo not use the Full Install option to Update.");

                // Save Download Combobox Settings back to RA+Cores instead of Full Install for next launch
                Settings.Default["download"] = "RA+Cores";
                Settings.Default.Save();
                Settings.Default.Reload();
            }

            // Call Set Architecture
            setArchitecture();
            // Call Set Urls Method
            setUrls();

            // Clear All if checked/unchecked for next pass
            clearAll();
        }

        // ###############################################
        // Textbox RetroArch Location (On Click Change)
        // ###############################################
        private void textBoxLocation_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Set the retroarchPath string
            retroarchPath = textBoxLocation.Text; //end with backslash

            // Save RetroArch Path for next launch
            Settings.Default["retroarchPath"] = textBoxLocation.Text;
            Settings.Default.Save();
            Settings.Default.Reload();
        }

        // ###############################################
        // Check Button - Tests Download URL
        // ###############################################
        private void buttonCheck_Click(object sender, RoutedEventArgs e)
        {
            // Clear All to prevent Lists doubling up
            clearAll();

            // Progress Info
            labelProgressInfo.Content = "Checking...";

            // Call setArchitecture Method
            setArchitecture();

            // Call parse Page (HTML) Method
            parseBuildbotPage();

            // If Downloading RetroArch and NOT Cores
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" 
                || (string)comboBoxDownload.SelectedItem == "RetroArch" 
                || (string)comboBoxDownload.SelectedItem == "Redist")
            {
                // Display message if download available
                if (!string.IsNullOrEmpty(nightly7z)) // If Last Item in Nightlies List is available
                {
                    System.Windows.MessageBox.Show("Download Available \n\n" + buildbotArchitecture + "\n\n" + nightly7z);
                }
                else // If Last Item in Nightlies List cannot be found
                {
                    System.Windows.MessageBox.Show("Could not find download.");
                }
            }

            // Clear the string to allow Checking again
            //nightly7z = string.Empty;

            // Progress Info
            labelProgressInfo.Content = "";

            // CORES ######################
            // If Download Combobox Cores or RA+Cores selected
            if ((string)comboBoxDownload.SelectedItem == "RA+Cores" || (string)comboBoxDownload.SelectedItem == "Cores")
            {
                // Call Methods - Build Cores Lists
                scanPcCoresDir();

                // Call Parse Builtbot Page Method
                parseBuildbotCoresPage();
                // Call Updated Cores Method
                updatedCores();

                // Call Cores Up To Date Method
                // If All Cores up to date, display message
                coresUpToDateCheck();

                // Window Checklist Popup //############################################### 
                // If Update List greater than 0, Popup Checklist
                if (ListUpdatedCoresName.Count != 0)
                {
                    // Trim List if new
                    ListUpdatedCoresName.TrimExcess();

                    // Add Updated Cores List to List Box (pass to constructor)
                    // (old winforms way was just to add myListBox.DataSource = myList. Without ObservableCollection)
                    CollectionUpdatedCoresName = new ObservableCollection<string>(ListUpdatedCoresName);

                    // Add PC Cores Name+Date to List Box (pass to constructor)
                    CollectionPcCoresNameDate = new ObservableCollection<string>(ListPcCoresNameDate);

                    // Add Buildbot Cores Name+Date to List Box (pass to constructor)
                    CollectionBuildbotNameDate = new ObservableCollection<string>(ListBuildbotCoresNameDate);


                    // Pass list into the constructor for Checklist Window ########################
                    checklist = new Checklist(CollectionUpdatedCoresName, CollectionPcCoresNameDate, CollectionBuildbotNameDate);
                    checklist.Owner = Window.GetWindow(this);
                    checklist.ShowDialog();

                    // Clear Name+Date Lists to prevent doubling up on next pass
                    ListPcCoresNameDate.Clear();
                    ListPcCoresNameDate.TrimExcess();
                    ListBuildbotCoresNameDate.Clear();
                    ListBuildbotCoresNameDate.TrimExcess();
                    CollectionPcCoresNameDate.Clear();
                    CollectionBuildbotNameDate.Clear();

                    // Clear List to prevent doubling up
                    //ListUpdatedCoresName.Clear();
                    //ListUpdatedCoresName.TrimExcess();
                    //CollectionUpdatedCoresName = null;
                }
            }

            // Clear All again to prevent doubling up on Update button
            clearAll();
        }


        // ###############################################
        // Update Button - Launches Download and 7-Zip Extraction
        // ###############################################
        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Get the currently selected item
            comboBoxDownloadItem = comboBoxDownload.SelectedItem.ToString();


            // Add backslash to Location Textbox path if missing
            if (!textBoxLocation.Text.EndsWith("\\") && textBoxLocation.Text != "")
            {
                textBoxLocation.Text = textBoxLocation.Text + "\\";
            }
            // Load the User's RetroArch Location from Text Box / Saved Settings
            retroarchPath = textBoxLocation.Text; //end with backslash


            // If RetroArch Path is empty, halt progress
            if (string.IsNullOrEmpty(retroarchPath))
            {
                ready = 0;
                System.Windows.MessageBox.Show("Please select your RetroArch main folder.");
            }

            // MUST BE IN THIS ORDER: 1. setArchitecture -> 2. parsePage -> 3. setArchiver  ##################
            // If you checkArchiver before parsePage, it will not set the nightly7z string in the CLI Arguments first
            // Maybe solve this by putting CLI Arguments in another method?

            // 1. Call setArchitecture Method
            setArchitecture();

            // 2. Call parse Page (HTML) Method
            parseBuildbotPage();

            // 3. Call checkArchiver Method
            // If Archiver exists, Set string
            setArchiver();




            // ###############################################
            // If Full Install
            // ###############################################
            if ((string)comboBoxDownload.SelectedItem == "Full Install")
            {
                // Set Cores Folder (Dont Scan PC)
                coresPath = retroarchPath + "cores\\";
                // Call Parse Builtbot Page Method
                parseBuildbotCoresPage();
            }



            // ###############################################
            // If Update
            // ###############################################
            // If Update Download Combobox Cores or RA+Cores selected
            if ((string)comboBoxDownload.SelectedItem == "Cores" || (string)comboBoxDownload.SelectedItem == "RA+Cores")
            {
                // Call Methods - Build Cores Lists
                scanPcCoresDir();

                // Call Parse Builtbot Page Method
                parseBuildbotCoresPage();
                // Call Updated Cores Method
                updatedCores();

                // Call Cores Up To Date Method
                // If All Cores up to date, display message
                coresUpToDateCheck();

                // Clear Name+Date Lists to prevent doubling up on next pass
                ListPcCoresNameDate.Clear();
                ListPcCoresNameDate.TrimExcess();
                ListBuildbotCoresNameDate.Clear();
                ListBuildbotCoresNameDate.TrimExcess();
                CollectionPcCoresNameDate.Clear();
                CollectionBuildbotNameDate.Clear();
            }



            // ###############################################
            // Check if Ready
            // ###############################################
            if (ready == 1)
            {
                // Start Download
                startDownload();
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
