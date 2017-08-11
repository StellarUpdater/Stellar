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

using System.IO;

namespace Stellar
{
    public partial class Paths
    {
        // Windows Temp Path
        public static string tempPath = System.IO.Path.GetTempPath();

        // System Paths
        public static string currentDir = Directory.GetCurrentDirectory().TrimEnd('\\') + @"\";
        public static string retroarchPath; // Location of User's RetroArch Folder
        public static string coresPath; // Location of User's cores folder

        // Buildbot
        // string buildbotCoresPage = string.Empty; //Parse.parseUrl + "latest/"
        public static string buildbotArchitecture; //x86 or x86_64\
        public static string buildbotArchitectureCores; //Used with latest/ element url. Used to fix w32.


        // -----------------------------------------------
        // Select Architecture, Change URL to Parse
        // -----------------------------------------------
        public static void SetArchitecture(MainWindow mainwindow) // Method
        {
            // -------------------------
            // If 32-bit Selected, change Download Architecture to x86
            // -------------------------
            if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "32-bit")
            {
                // Set Parse URL
                Parse.parseUrl = Parse.libretro_x86;
                Parse.parseCoresUrl = Parse.libretro_x86 + "latest/";
                Parse.indexextendedUrl = Parse.libretro_x86 + "latest/.index-extended";

                // Buildbot Architecture
                buildbotArchitecture = "x86";
                buildbotArchitectureCores = "x86";
            }

            // -------------------------
            // If 64-bit Selected, change Download Architecture to x86_64
            // -------------------------
            else if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "64-bit")
            {
                // Set Parse URL
                Parse.parseUrl = Parse.libretro_x86_64;
                Parse.parseCoresUrl = Parse.libretro_x86_64 + "latest/";
                Parse.indexextendedUrl = Parse.libretro_x86_64 + "latest/.index-extended";

                // Buildbot Architecture
                buildbotArchitecture = "x86_64";
                buildbotArchitectureCores = "x86_64";
            }

            // -------------------------
            // If win32 Selected, change Download Architecture to x86_64_w32
            // -------------------------
            else if ((string)mainwindow.comboBoxArchitecture.SelectedItem == "64 w32")
            {
                // Set Parse URL
                Parse.parseUrl = Parse.libretro_x86_64; //64-bit URL, w32 is cores-only
                Parse.parseCoresUrl = Parse.libretro_x86_64_w32 + "latest/";
                Parse.indexextendedUrl = Parse.libretro_x86_64_w32 + "latest/.index-extended";

                // Buildbot Architecture
                buildbotArchitecture = "x86_64"; //64-bit URL, w32 is cores-only
                buildbotArchitectureCores = "x86_64_w32";
            }
        }



        // -----------------------------------------------
        // Set URLs
        // -----------------------------------------------
        // Display URLs in Download Textbox
        public static void SetUrls(MainWindow mainwindow)
        {
            // If New Install Selected, Textbox will display URL
            //
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
            {
                mainwindow.textBoxDownload.Text = Parse.parseUrl;
            }

            // If RA+Cores or Cores Selected, Textbox will display URL
            //
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores" || (string)mainwindow.comboBoxDownload.SelectedItem == "RetroArch")
            {
                mainwindow.textBoxDownload.Text = Parse.parseUrl;
            }

            // If Cores Selected, Textbox will display URL
            //
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Cores" || (string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
            {
                mainwindow.textBoxDownload.Text = Parse.parseCoresUrl;
            }

            // If Redist Selected, Textbox will display URL
            //
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Redist")
            {
                mainwindow.textBoxDownload.Text = Parse.parseUrl;
            }

            // If Stellar Selected, Textbox will display URL
            //
            else if ((string)mainwindow.comboBoxDownload.SelectedItem == "Stellar")
            {
                mainwindow.textBoxDownload.Text = Parse.parseGitHubUrl;
            }

        }
    }
}
