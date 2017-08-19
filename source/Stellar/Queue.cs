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
        // GitHub Self-Update
        public static List<string> ListGitHub = new List<string>();

        // RetroArch Nightly
        // Create Nightlies 7z List to be populated by Parse
        public static List<string> NightliesList = new List<string>();

        // List Count Comparison
        public static int largestList = 0;

        // -----------------------------------------------
        // PC Cores
        // -----------------------------------------------
        // Name
        public static List<string> List_PcCores_Name = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresName;
        // Date
        public static List<string> List_PcCores_Date = new List<string>();
        public static ObservableCollection<string> CollectionPcCoresDate;
        // Name+Date
        public static List<string> List_PcCores_NameDate = new List<string>();
        public static ObservableCollection<string> Collection_PcCores_NameDate; 
        // Unknown Name
        public static HashSet<string> List_PcCores_UnknownName = new HashSet<string>();
        public static ObservableCollection<string> CollectionPcCoresUnknownNameDate;

        // Array for Name/Date Sublists
        public static string[] pcArr = null;

        // -----------------------------------------------
        // Buildbot Cores
        // -----------------------------------------------
        // Name
        public static List<string> List_BuildbotCores_Name = new List<string>();
        public static ObservableCollection<string> Collection_BuildbotCores_Name;
        // Date
        public static List<string> List_BuildbotCores_Date = new List<string>();
        public static ObservableCollection<string> Collection_BuildbotCores_Date;
        // ID
        //public static List<string> ListBuildbotID = new List<string>();
        // Name+Date
        public static List<string> List_BuildbotCores_NameDate = new List<string>();
        public static ObservableCollection<string> Collection_BuildbotCores_NameDate; 
        // New Name (Debugger)
        public static List<string> List_BuildbotCores_NewName = new List<string>();
        public static ObservableCollection<string> Collection_BuildbotCores_NewName;

        // Array for Name/Date Sublists
        public static string[] bbArr = null;

        // -----------------------------------------------
        // Excluded Cores (Already Up To Date or Mismatched/Unknown)
        // -----------------------------------------------
        // Name
        public static HashSet<string> List_ExcludedCores_Name = new HashSet<string>();
        public static ObservableCollection<string> Collection_ExcludedCores_Name;
        // Name+Date
        public static HashSet<string> List_ExcludedCores_NameDate = new HashSet<string>();

        // -----------------------------------------------
        // Rejected Cores (Checkbox)
        // -----------------------------------------------
        // Name
        public static HashSet<string> List_RejectedCores_Name = new HashSet<string>();

        // -----------------------------------------------
        // Updated Cores to Download
        // -----------------------------------------------
        // Name
        public static List<string> List_UpdatedCores_Name = new List<string>();
        public static ObservableCollection<string> Collection_UpdatedCores_Name;
        // Date
        public static List<string> List_UpdatedCores_Date = new List<string>();



        // -----------------------------------------------
        // Remove PC Unknown Cores
        // -----------------------------------------------
        public static void RemovePCUnknownCores()
        {
            try
            {
                // Always use Largest List as Loop Counter
                //largestList = Math.Max(List_BuildbotCores_NameDate.Count(), List_PcCores_NameDate.Count());

                // Remove in reverse
                //for (int i = largestList - 1; i >= 0; i--)
                for (int i = List_PcCores_NameDate.Count() - 1; i >= 0; --i)
                {
                    if (List_PcCores_NameDate.Count() > i && List_PcCores_NameDate.Count() != 0) //index range check
                    {
                        pcArr = Convert.ToString(List_PcCores_NameDate[i]).Split(' ');

                        // If Buildbot List Does NOT Contain a PC Core Name
                        if (!List_BuildbotCores_Name.Contains(pcArr[0]))
                        {
                            // Name
                            if (List_PcCores_Name.Count() > i && List_PcCores_Name.Count() != 0) // null check
                            {
                                List_PcCores_Name.RemoveAt(i);
                            }
                            // Date
                            if (List_PcCores_Date.Count() > i && List_PcCores_Date.Count() != 0) // null check
                            {
                                List_PcCores_Date.RemoveAt(i);
                            }
                            // Name+Date
                            if (List_PcCores_NameDate.Count() > i && List_PcCores_NameDate.Count() != 0) // null check
                            {
                                List_PcCores_NameDate.RemoveAt(i);
                            }

                            // PC Unknown Name
                            // Check if List already contains Core
                            if (!List_PcCores_UnknownName.Contains(pcArr[0]))
                            {
                                List_PcCores_UnknownName.Add(pcArr[0]);
                            }
                        }
                    }

                } //end loop

                List_PcCores_Name.TrimExcess();
                List_PcCores_Date.TrimExcess();
                List_PcCores_NameDate.TrimExcess();
            }
            catch
            {
                MessageBox.Show("Error: Problem removing Unknown PC Cores from list.");
            }
        }


        // -----------------------------------------------
        // Remove Buildbot Missing Cores
        // -----------------------------------------------
        public static void RemoveBuildbotMissingCores()
        {
            try
            {
                // Always use Largest List as Loop Counter
                //largestList = Math.Max(List_BuildbotCores_NameDate.Count(), List_PcCores_NameDate.Count());

                // Remove in reverse
                //for (int i = largestList - 1; i >= 0; i--)
                for (int i = List_BuildbotCores_NameDate.Count() - 1; i >= 0; --i)
                {
                    if (List_BuildbotCores_Name.Count() > i && List_BuildbotCores_Name.Count() != 0) //index range check
                    {
                        bbArr = Convert.ToString(List_BuildbotCores_Name[i]).Split(' ');

                        // If PC List Does NOT Contain a Buildbot Core Name
                        if (!List_PcCores_Name.Contains(bbArr[0]))
                        {
                            // Name
                            if (List_BuildbotCores_Name.Count() > i && List_BuildbotCores_Name.Count() != 0) // null check
                            {
                                List_BuildbotCores_Name.RemoveAt(i);
                            }
                            // Date
                            if (List_BuildbotCores_Date.Count() > i && List_BuildbotCores_Date.Count() != 0) // null check
                            {
                                List_BuildbotCores_Date.RemoveAt(i);
                            }
                            // Name+Date
                            if (List_BuildbotCores_NameDate.Count() > i && List_BuildbotCores_NameDate.Count() != 0) // null check
                            {
                                List_BuildbotCores_NameDate.RemoveAt(i);
                            }

                            // Buildbot Excluded Name
                            // Check if List already contains Core
                            if (!List_ExcludedCores_Name.Contains(bbArr[0]))
                            {
                                List_ExcludedCores_Name.Add(bbArr[0]);
                            }
                        }
                    }
                } //end loop

                List_BuildbotCores_Name.TrimExcess();
                List_BuildbotCores_Date.TrimExcess();
                List_BuildbotCores_NameDate.TrimExcess();
            }
            catch
            {
                MessageBox.Show("Error: Problem removing Missing Buildbot Cores from list.");
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
                List_UpdatedCores_Name = List_BuildbotCores_Name.Except(List_PcCores_Name).ToList();
            }

            // -------------------------
            // Update Cores
            // -------------------------
            else
            {
                // -------------------------
                // Remove Buildbot Missing Cores
                // -------------------------
                // Remove Cores until both Lists are equal
                RemoveBuildbotMissingCores();

                // -------------------------
                // Remove PC Unknown Cores
                // -------------------------
                // Remove Cores until both Lists are equal
                RemovePCUnknownCores();


                // -------------------------
                // Create and Compare Name & Date Lists
                // -------------------------
                try
                {
                    // Always use Largest List as Loop Counter
                    largestList = Math.Max(List_BuildbotCores_NameDate.Count(), List_PcCores_NameDate.Count());

                    for (int i = 0; i < largestList; i++)
                    {
                        // Buildbot
                        if (List_BuildbotCores_NameDate.Count() > i) //index range check
                        {
                            bbArr = Convert.ToString(List_BuildbotCores_NameDate[i]).Split(' ');
                        }

                        // PC
                        if (List_PcCores_NameDate.Count() > i) //index range check
                        {
                            pcArr = Convert.ToString(List_PcCores_NameDate[i]).Split(' ');
                        }

                        // -------------------------
                        // Compare
                        // -------------------------
                        if (List_PcCores_NameDate.Count() > i) //index range check
                        {
                            // If Buildbot Name+Date List Contains a PC Name
                            //
                            if (List_BuildbotCores_Name.Any(p => p.Contains(pcArr[0])))
                            {
                                if (bbArr != null && pcArr != null) // null check
                                {
                                    // Updated Cores
                                    if (DateTime.Parse(bbArr[1]) > DateTime.Parse(pcArr[1]))
                                    {
                                        // Add Core to Updated
                                        // Name
                                        List_UpdatedCores_Name.Add(bbArr[0]);
                                        // Date
                                        List_UpdatedCores_Date.Add(bbArr[1]);
                                    }

                                    // Excluded Cores
                                    else
                                    {
                                        // Check if Excluded already contains Core
                                        if (!List_ExcludedCores_Name.Contains(bbArr[0]))
                                        {
                                            // Add Core to Excluded
                                            List_ExcludedCores_Name.Add(bbArr[0]);
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
                                List_PcCores_UnknownName.Add(pcArr[0]);
                                // Buildbot Excluded Name
                                List_ExcludedCores_Name.Add(pcArr[0]);
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
                //List_ExcludedCores_Name.Sort();
                List_ExcludedCores_Name.TrimExcess();
                //List_PcCores_UnknownName.Sort();
                List_PcCores_UnknownName.TrimExcess();

                // -------------------------
                // Create Update List
                // -------------------------
                List_UpdatedCores_Name = List_UpdatedCores_Name.Except(List_ExcludedCores_Name).ToList();

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
            if (List_UpdatedCores_Name.Count == 0 
                && List_PcCores_Name.Count != 0 
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
                && List_UpdatedCores_Name.Count == 0)
            {
                MessageBox.Show("No New Cores available.");

                // Prevent Updated Cores from doubling up on next check
                MainWindow.ClearCoresVars();
                MainWindow.ClearLists();
            }

        }

    }
}