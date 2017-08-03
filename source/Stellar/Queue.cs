using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
    public partial class Queue
    {
        // RetroArch Nightly
        public static List<string> NightliesList = new List<string>(); // Create Nightlies 7z List to be filled by Parse

        // PC Cores
        public static List<string> ListPcCoresName = new List<string>(); // PC File Name
        public static List<DateTime> ListPcCoresDateModified = new List<DateTime>(); // PC File Date & Time <DateTime> Important!
        public static List<string> ListPcCoresDateModifiedFormatted = new List<string>(); // PC File Date & Time Formatted

        // Buildbot Cores
        public static List<string> ListBuildbotCoresName = new List<string>(); // Buildbot File Name
        public static List<string> ListBuildbotCoresDate = new List<string>(); // Buildbot File Date & Time
        public static List<string> ListBuildbotID = new List<string>(); // Buildbot Core ID's

        // Updated Cores
        public static List<string> ListUpdatedCoresName = new List<string>(); // Updated Cores to Download
        public static ObservableCollection<string> CollectionUpdatedCoresName; // Displays List in Listbox
        public static List<string> ListExcludedCores = new List<string>(); // Updated Cores to Download

        // PC Cores / Buildbot Cores Name + Date
        public static List<DateTime> ListPcCoresDate = new List<DateTime>(); // PC File Date & Time <DateTime> Important!
        public static List<string> ListPcCoresDateFormatted = new List<string>(); // PC File Date & Time Formatted
        public static List<string> ListPcCoresNameDate = new List<string>(); // PC File Name+Date & Time
        public static ObservableCollection<string> CollectionPcCoresNameDate; // PC Cores Name+Date Observable Collection

        public static List<string> ListBuildbotCoresNameDate = new List<string>(); // Buildbot File Name + Date & Time
        public static ObservableCollection<string> CollectionBuildbotNameDate; // Buildbot Cores Name+Date Observable Collection


        // -----------------------------------------------
        // Create Updated Cores List
        // -----------------------------------------------
        public static void UpdatedCores(MainWindow mainwindow)
        {
            // This part gets complicated
            // For each Buildbot Date that is Greater than PC Date Modified Date, add a Buildbot Name to the Update List

            // Re-create ListbuilbotCoresName by comparing it to ListPcCoresName and keeping only Matches
            ListBuildbotCoresName = ListPcCoresName.Intersect(ListBuildbotCoresName).ToList();
            // ListBuildbotCoresName.Sort(); // Disable Sort

            // List Compare - Use the newly created ListbuilbotCoresName to draw Names from
            for (int i = 0; i < ListBuildbotCoresName.Count; i++)
            {
                // If PC Core Name List Contains Buildbot Core Name [i]
                if (ListPcCoresName.Contains(ListBuildbotCoresName[i]))
                {
                    // If Buildbot Core Modified Date greater than > PC Core Creation Date
                    if (DateTime.Parse(ListBuildbotCoresDate[i]) > DateTime.Parse(ListPcCoresDateModifiedFormatted[i]))
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


        // -----------------------------------------------
        // Create New/Missing Cores List
        // -----------------------------------------------
        public static void NewCores(MainWindow mainwindow)
        {
            // Make a List of All Buildbot Cores
            // Make a List of All PC Cores
            // Subtract PC List from Buildbot List
            ListUpdatedCoresName = ListBuildbotCoresName.Except(ListPcCoresName).ToList();
        }


        // -----------------------------------------------
        // Cores Up To Date Check
        // -----------------------------------------------
        public static void CoresUpToDateCheck(MainWindow mainwindow)
        {
            // If All Cores up to date
            // If the Update List is empty, but PC Cores have been found and scanned
            if (ListUpdatedCoresName.Count == 0 && ListPcCoresName.Count != 0)
            {
                if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
                {
                    System.Windows.MessageBox.Show("No New Cores available.");
                }
                else
                {
                    System.Windows.MessageBox.Show("Cores already lastest version.");
                }
            }
        }

    }
}