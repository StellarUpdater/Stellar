using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

        public static int? largestList = null;

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

        public static string[] pcArr = null;

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
        //public static List<string> ListBuildbotID = new List<string>();
        // Name+Date
        public static List<string> ListBuildbotCoresNameDate = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresNameDate; 
        // New Name (Debugger)
        public static List<string> ListBuildbotCoresNewName = new List<string>();
        public static ObservableCollection<string> CollectionBuildbotCoresNewName;

        public static string[] bbArr = null;

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
        // Remove PC Unknown Cores
        // -----------------------------------------------
        public static void RemovePCUnknownCores()
        {
            // Always use Largest List as Loop Counter
            largestList = Math.Max(ListBuildbotCoresNameDate.Count(), ListPcCoresNameDate.Count());

            for (int i = 0; i < largestList; i++)
            {
                if (Queue.ListPcCoresNameDate.Count() > i && Queue.ListPcCoresNameDate.Count() != 0) //index range check
                {
                    string[] pcArr = Convert.ToString(Queue.ListPcCoresNameDate[i]).Split(' ');

                    // If Buildbot List Does NOT Contain a PC Core Name
                    if (!Queue.ListBuildbotCoresName.Contains(pcArr[0]))
                    {
                        // Name
                        if (Queue.ListPcCoresName.Count() > i
                            && Queue.ListPcCoresName.Count() != 0) // null check
                        {
                            Queue.ListPcCoresName.RemoveAt(i);
                            Queue.ListPcCoresName.TrimExcess();
                        }
                        // Date
                        if (Queue.ListPcCoresDate.Count() > i
                            && Queue.ListPcCoresDate.Count() != 0) // null check
                        {
                            Queue.ListPcCoresDate.RemoveAt(i);
                            Queue.ListPcCoresDate.TrimExcess();
                        }
                        // Name+Date
                        if (Queue.ListPcCoresNameDate.Count() > i
                            && Queue.ListPcCoresNameDate.Count() != 0) // null check
                        {
                            Queue.ListPcCoresNameDate.RemoveAt(i);
                            Queue.ListPcCoresNameDate.TrimExcess();
                        }

                        // PC Unknown Name
                        // Check if List already contains Core
                        if (!ListPcCoresUnknownNameDate.Contains(pcArr[0]))
                        {
                            Queue.ListPcCoresUnknownNameDate.Add(pcArr[0]);
                        }
                    }
                }
            }
        }


        // -----------------------------------------------
        // Remove Buildbot Missing Cores
        // -----------------------------------------------
        public static void RemoveBuildbotMissingCores()
        {
            // Always use Largest List as Loop Counter
            largestList = Math.Max(ListBuildbotCoresNameDate.Count(), ListPcCoresNameDate.Count());

            for (int i = 0; i < largestList; i++)
            {
                if (Queue.ListBuildbotCoresName.Count() > i && Queue.ListBuildbotCoresName.Count() != 0) //index range check
                {
                    Queue.bbArr = Convert.ToString(Queue.ListBuildbotCoresName[i]).Split(' ');


                    // If PC List Does NOT Contain a Buildbot Core Name
                    if (!Queue.ListPcCoresName.Contains(bbArr[0]))
                    {
                        // Name
                        if (Queue.ListBuildbotCoresName.Count() > i
                            && Queue.ListBuildbotCoresName.Count() != 0) // null check
                        {
                            Queue.ListBuildbotCoresName.RemoveAt(i);
                            Queue.ListBuildbotCoresName.TrimExcess();
                        }
                        // Date
                        if (Queue.ListBuildbotCoresDate.Count() > i
                            && Queue.ListBuildbotCoresDate.Count() != 0) // null check
                        {
                            Queue.ListBuildbotCoresDate.RemoveAt(i);
                            Queue.ListBuildbotCoresDate.TrimExcess();
                        }
                        // Name+Date
                        if (Queue.ListBuildbotCoresNameDate.Count() > i
                            && Queue.ListBuildbotCoresNameDate.Count() != 0) // null check
                        {
                            Queue.ListBuildbotCoresNameDate.RemoveAt(i);
                            Queue.ListBuildbotCoresNameDate.TrimExcess();
                        }

                        // Buildbot Excluded Name
                        // Check if List already contains Core
                        if (!ListExcludedCoresName.Contains(bbArr[0]))
                        {
                            Queue.ListExcludedCoresName.Add(bbArr[0]);
                        }
                    }
                }
            }
            
        }


        // -----------------------------------------------
        // Create Updated Cores List
        // -----------------------------------------------
        public static void UpdatedCores(MainWindow mainwindow)
        {
            // -------------------------
            // New / Missing Cores
            // -------------------------
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores" 
                || (string)mainwindow.comboBoxDownload.SelectedItem == "New Install")
            {
                // Make a List of All Buildbot Cores
                // Make a List of All PC Cores
                // Subtract PC List from Buildbot List
                ListUpdatedCoresName = ListBuildbotCoresName.Except(ListPcCoresName).ToList();
            }

            // -------------------------
            // Update Cores
            // -------------------------
            else
            {
                // -------------------------
                // Remove PC Unknown Cores
                // -------------------------
                try
                {
                    RemovePCUnknownCores();

                    // Double Check
                    for (int i = 0; i < Queue.ListBuildbotCoresName.Count(); i++)
                    {
                        if (Queue.ListPcCoresNameDate.Count() > Queue.ListBuildbotCoresName.Count())
                        {
                            RemovePCUnknownCores();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Problem removing Unknown PC Cores from list.");
                }


                // -------------------------
                // Remove Buildbot Missing Cores
                // -------------------------
                try
                {
                    RemoveBuildbotMissingCores();

                    // Double Check
                    for (int i = 0; i < Queue.ListBuildbotCoresName.Count(); i++)
                    {
                        if (Queue.ListBuildbotCoresName.Count() > Queue.ListPcCoresNameDate.Count())
                        {
                            RemoveBuildbotMissingCores();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Problem removing Missing Buildbot Cores from list.");
                }
                


                // -------------------------
                // Create and Compare Name & Date Lists
                // -------------------------
                try
                {
                    // Always use Largest List as Loop Counter
                    largestList = Math.Max(ListBuildbotCoresNameDate.Count(), ListPcCoresNameDate.Count());

                    for (int i = 0; i < largestList; i++)
                    {
                        // Buildbot
                        if (ListBuildbotCoresNameDate.Count() > i) //index range check
                        {
                            bbArr = Convert.ToString(ListBuildbotCoresNameDate[i]).Split(' ');
                        }

                        // PC
                        if (ListPcCoresNameDate.Count() > i) //index range check
                        {
                            pcArr = Convert.ToString(ListPcCoresNameDate[i]).Split(' ');
                        }

                        // -------------------------
                        // Compare
                        // -------------------------
                        if (ListPcCoresNameDate.Count() > i) //index range check
                        {
                            // If Buildbot Name+Date List Contains a PC Name
                            //
                            if (ListBuildbotCoresName.Any(p => p.Contains(pcArr[0])))
                            {
                                if (bbArr != null && pcArr != null) // null check
                                {
                                    // Updated Cores
                                    if (DateTime.Parse(bbArr[1]) > DateTime.Parse(pcArr[1]))
                                    {
                                        // Add Core to Updated
                                        ListUpdatedCoresName.Add(bbArr[0]);

                                        // Debug
                                        //MessageBox.Show(Convert.ToString(bbArr[0] + " " + bbArr[1] + " > " + pcArr[0] + " " + pcArr[1]));
                                    }

                                    // Excluded Cores
                                    else
                                    {
                                        // Check if Excluded already contains Core
                                        if (!ListExcludedCoresName.Contains(bbArr[0]))
                                        {
                                            // Add Core to Excluded
                                            ListExcludedCoresName.Add(bbArr[0]);
                                        }
                                    }
                                }
                            }
                            // -------------------------
                            // Uknown/Excluded Cores
                            // -------------------------
                            else
                            {
                                // PC Unknown Name
                                ListPcCoresUnknownNameDate.Add(pcArr[0]);
                                // Buildbot Excluded Name
                                ListExcludedCoresName.Add(pcArr[0]);
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Error: Problem creating Update & Exclusion list.");
                }


                // -------------------------
                // Sort Unknown & Excluded
                // -------------------------
                Queue.ListExcludedCoresName.Sort();
                Queue.ListExcludedCoresName.TrimExcess();
                Queue.ListPcCoresUnknownNameDate.Sort();
                Queue.ListPcCoresUnknownNameDate.TrimExcess();

                // -------------------------
                // Create Update List
                // -------------------------
                ListUpdatedCoresName = ListUpdatedCoresName.Except(ListExcludedCoresName).ToList();

            }

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
                MessageBox.Show("Cores already lastest version.");

                // Prevent Updated Cores from doubling up on next check
                if((string)mainwindow.comboBoxDownload.SelectedItem != "RA+Cores") //ignore RA+Cores
                {
                    MainWindow.ClearCoresVars();
                    MainWindow.ClearLists();
                }
            }

            // -------------------------
            // New Cores - Not Found
            // -------------------------
            // Updat List is Empty
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "New Cores" 
                && ListUpdatedCoresName.Count == 0)
            {
                MessageBox.Show("No New Cores available.");

                // Prevent Updated Cores from doubling up on next check
                MainWindow.ClearCoresVars();
                MainWindow.ClearLists();
            }

        }

    }
}