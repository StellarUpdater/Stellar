using System.IO;

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
    public partial class Archiver
    {
        public static string archiver; // 7-Zip / WinRar
        public static string extract; // Archiver CLI arguments
        public static string sevenZipPath; // 7-Zip Config Settings Path
        public static string winRARPath; // 7-Zip Config Settings Path

        // -----------------------------------------------
        // Check if Archiver Exists, If true set string
        // -----------------------------------------------
        public static void SetArchiver(MainWindow mainwindow)
        {
            // Null Checker
            if (string.IsNullOrEmpty(Configure.sevenZipPath))
            {
                MainWindow.ready = 0;
                System.Windows.MessageBox.Show("Please set 7-Zip Path in Settings.");
            }

            // Null Checker
            if (string.IsNullOrEmpty(Configure.winRARPath))
            {
                MainWindow.ready = 0;
                System.Windows.MessageBox.Show("Please set WinRAR Path in Settings.");
            }

            // -------------------------
            // Auto
            // -------------------------
            // If 7-Zip or WinRAR Path is Configure Settings <Auto>
            //
            if (Configure.sevenZipPath == "<auto>" && Configure.winRARPath == "<auto>")
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
                    MainWindow.ready = 0;
                    System.Windows.MessageBox.Show("Please install 7-Zip or WinRAR to extract files.\n\nOr set Path in Settings.");
                }
            }

            // -------------------------
            // User Select
            // -------------------------
            // If 7-Zip Path is (Not Auto) and is Configure Settings <User Selected Path>
            //
            if (Configure.sevenZipPath != "<auto>" && !string.IsNullOrEmpty(Configure.sevenZipPath)) // Check null again
            {
                try
                {
                    // Get 7-Zip Path from the Configure Window (Pass Data)
                    sevenZipPath = Configure.sevenZipPath;
                }
                catch
                {
                    MainWindow.ready = 0;
                    System.Windows.MessageBox.Show("Error: Could not load 7-Zip. Please restart the program.");
                }

                // Path to 7-Zip
                archiver = Configure.sevenZipPath;
                // CLI Arguments unzip files
                extract = "7-Zip"; //args selector
            }

            // -------------------------
            // Not Auto
            // -------------------------
            // If WinRAR Path is (Not Auto) and is Configure Settings <User Selected Path> ####################
            else if (Configure.winRARPath != "<auto>" && !string.IsNullOrEmpty(Configure.winRARPath)) // Check null again
            {
                try
                {
                    // Get 7-Zip Path from the Configure Window (Pass Data)
                    winRARPath = Configure.winRARPath;
                }
                catch
                {
                    MainWindow.ready = 0;
                    System.Windows.MessageBox.Show("Error: Could not load WinRAR. Please restart the program.");
                }

                // Path to WinRAR
                archiver = Configure.winRARPath;
                // CLI Arguments unzip files
                extract = "WinRAR"; //args selector
            }

        }
    }
}
