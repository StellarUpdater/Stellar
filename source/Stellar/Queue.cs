using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        // Create Nightlies 7z List to be populated by Parse
        public static List<string> NightliesList = new List<string>(); 

        // -----------------------------------------------
        // PC Cores
        // -----------------------------------------------
        // Name
        public static List<string> ListPcCoresName = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresName;
        // Date
        public static List<string> ListPcCoresDate = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresDate;
        // Name+Date
        public static List<string> ListPcCoresNameDate = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresNameDate; 
        // Unknown Name+Date
        public static List<string> ListPcCoresUnknownNameDate = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresUnknownNameDate;

        // -----------------------------------------------
        // Buildbot Cores
        // -----------------------------------------------
        // Name
        public static List<string> ListBuildbotCoresName = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresName;
        // Date
        public static List<string> ListBuildbotCoresDate = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresDate;
        // ID
        public static List<string> ListBuildbotID = new List<string>();
        // Name+Date
        public static List<string> ListBuildbotCoresNameDate = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresNameDate; 
        // New Name (Debugger)
        public static List<string> ListBuildbotCoresNewName = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresNewName;

        // -----------------------------------------------
        // Excluded Cores (Already Up To Date or Mismatched/Unknown)
        // -----------------------------------------------
        // Name
        public static List<string> ListExcludedCoresName = new List<string>();
        public static ObservableCollection<string> CollectionExcludedCoresName;
        // Name+Date
        public static List<string> ListExcludedCoresNameDate = new List<string>();

        // -----------------------------------------------
        // Rejected Cores (Checkbox)
        // -----------------------------------------------
        // Name
        public static List<string> ListRejectedCores = new List<string>();

        // -----------------------------------------------
        // Updated Cores to Download
        // -----------------------------------------------
        // Name
        public static List<string> ListUpdatedCoresName = new List<string>();
        public static ObservableCollection<string> CollectionUpdatedCoresName;



        // -----------------------------------------------
        // Create Updated Cores List
        // -----------------------------------------------
        public static void UpdatedCores(MainWindow mainwindow)
        {
            // For each Buildbot Date that is Greater than PC Modified Date, add a Buildbot Name to the Update List

            // Re-create ListbuilbotCoresName by comparing it to ListPcCoresName and keeping only Matches
            ListBuildbotCoresName = ListPcCoresName.Intersect(ListBuildbotCoresName).ToList();

            // List Compare - Use the newly created ListbuilbotCoresName to draw Names from
            for (int i = 0; i < ListBuildbotCoresName.Count; i++)
            {
                // If PC Core Name List Contains the Buildbot Core Name [i]
                if (ListPcCoresName.Contains(ListBuildbotCoresName[i]))
                {
                    // If Buildbot Core Date Greater Than > PC Core Modified Date
                    // Add Buildbot Core Name to Update List
                    if (DateTime.ParseExact(ListBuildbotCoresDate[i], "yyyy-MM-dd", CultureInfo.InvariantCulture) > DateTime.ParseExact(ListPcCoresDate[i], "yyyy-MM-dd", CultureInfo.InvariantCulture))
                    {
                        ListUpdatedCoresName.Add(ListBuildbotCoresName[i]);
                    }
                    // Less Than, Add Buildbot Core Name to Exclusion List
                    else
                    {
                        ListExcludedCoresName.Add(ListBuildbotCoresName[i]);
                    }
                }
                else
                {
                    // Remove [i] Item from Buildbot Name List (Prevent Index out of range)
                    ListBuildbotCoresName.Remove(ListBuildbotCoresName[i]);
                }

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
            // -------------------------
            // Cores - Already Up to Date
            // -------------------------
            // Update List is empty, but PC Cores have been found and scanned
            if (ListUpdatedCoresName.Count == 0 
                && ListPcCoresName.Count != 0 
                && (string)mainwindow.comboBoxDownload.SelectedItem != "New Cores")
            {
                System.Windows.MessageBox.Show("Cores already lastest version.");

                // Prevent Updated Cores from doubling up on next check
                if((string)mainwindow.comboBoxDownload.SelectedItem != "RA+Cores") //ignore RA+Cores
                {
                    MainWindow.ClearAll();
                    MainWindow.ClearNameDates();
                }
            }

            // -------------------------
            // New Cores - Not Found
            // -------------------------
            // Updat List is Empty
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores" 
                && ListUpdatedCoresName.Count == 0)
            {
                System.Windows.MessageBox.Show("No New Cores available.");

                // Prevent Updated Cores from doubling up on next check
                MainWindow.ClearAll();
                MainWindow.ClearNameDates();
            }

        }

    }
}