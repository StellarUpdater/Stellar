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
using System.IO;
using System.Windows;

namespace Stellar
{
    public partial class Log
    {
        public static void WriteLog()
        {
            // Write Log Append
            // Only if Log is Enabled through Config Checkbox
            if (VM.MainView.LogPath_IsChecked == true && 
                !string.IsNullOrEmpty(VM.MainView.LogPath_Text)) 
            {
                // Check if Path Exists
                if (Directory.Exists(VM.MainView.LogPath_Text))
                {
                    // Check for Save Error
                    try
                    {
                        using (FileStream fs = new FileStream(VM.MainView.LogPath_Text + "stellar.log", FileMode.Append, FileAccess.Write))
                        using (StreamWriter sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(DateTime.Now);
                            sw.WriteLine("--------------------------------------\r\n");

                            // Append List
                            for (int x = 0; x < Queue.List_CoresToUpdate_Name.Count; x++)
                            {
                                sw.WriteLine(Queue.List_CoresToUpdate_Name[x]);
                            }

                            sw.WriteLine("\r\n");

                            sw.Close();
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Error Saving Output Log to " + "\"" + VM.MainView.LogPath_Text + "\"" + ". May require Administrator Privileges.",
                                        "Error",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                    }
                }

                // Path does not exist
                else
                {
                    MessageBox.Show("Directory " + "\"" + VM.MainView.LogPath_Text + "\"" + " does not exist.",
                                    "Error",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

        }
    }

}
