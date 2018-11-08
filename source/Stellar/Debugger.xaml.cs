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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace Stellar
{
    /// <summary>
    /// Interaction logic for Debugger.xaml
    /// </summary>
    public partial class Debugger : Window
    {
        private static MainWindow mainwindow = ((MainWindow)System.Windows.Application.Current.MainWindow);

        public Debugger()
        {
            InitializeComponent();

            this.MinWidth = 1328;
            this.MinHeight = 622;
            this.MaxWidth = 1328;
            this.MaxHeight = 622;
        }


        // -----------------------------------------------
        // Close All
        // -----------------------------------------------
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        //    // Clear All to prevent Lists doubling up
        //}


        // -----------------------------------------------
        // Test Button
        // -----------------------------------------------
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            ViewModel vm = mainwindow.DataContext as ViewModel;

            // -------------------------
            // Check
            // -------------------------
            if (vm.Download_SelectedItem == "RA+Cores"
            || vm.Download_SelectedItem == "Cores"
            || vm.Download_SelectedItem == "New Cores")
            {
                // -------------------------
                // Clear
                // -------------------------
                // Extra Debugger Clears

                // Clear All to prevent Lists doubling up
                MainWindow.ClearRetroArchVars();
                MainWindow.ClearCoresVars();
                MainWindow.ClearLists();


                // Lists

                // PC Core Name+Date
                if (Queue.List_PcCores_NameDate != null)
                {
                    Queue.List_PcCores_NameDate.Clear();
                    Queue.List_PcCores_NameDate.TrimExcess();
                }

                // Buildbot Cores Name+Date
                if (Queue.List_BuildbotCores_NameDate != null)
                {
                    Queue.List_BuildbotCores_NameDate.Clear();
                    Queue.List_BuildbotCores_NameDate.TrimExcess();
                }

                // Excluded Core Name+Date
                if (Queue.List_ExcludedCores_NameDate != null)
                {
                    Queue.List_ExcludedCores_NameDate.Clear();
                    Queue.List_ExcludedCores_NameDate.TrimExcess();
                }

                // Observable Collections

                // PC Core Name ObservableCollection
                if (Queue.CollectionPcCoresName != null)
                {
                    Queue.CollectionPcCoresName = null;
                }
                // PC Core Date ObservableCollection
                if (Queue.CollectionPcCoresDate != null)
                {
                    Queue.CollectionPcCoresDate = null;
                }
                // PC Core Name+Date ObservableCollection
                if (Queue.Collection_PcCores_NameDate != null)
                {
                    Queue.Collection_PcCores_NameDate = null;
                }
                // PC Core Unknown Name+Date ObservableCollection
                if (Queue.CollectionPcCoresUnknownNameDate != null)
                {
                    Queue.CollectionPcCoresUnknownNameDate = null;
                }

                // Buildbot Core Name ObservableCollection
                if (Queue.Collection_BuildbotCores_Name != null)
                {
                    Queue.Collection_BuildbotCores_Name = null;
                }
                // Buildbot Core Date ObservableCollection
                if (Queue.Collection_BuildbotCores_Date != null)
                {
                    Queue.Collection_BuildbotCores_Date = null;
                }
                // Buildbot Core Name+Date ObservableCollection
                if (Queue.Collection_BuildbotCores_NameDate != null)
                {
                    Queue.Collection_BuildbotCores_NameDate = null;
                }
                // Buildbot Core New Name ObservableCollection
                if (Queue.Collection_BuildbotCores_NewName != null)
                {
                    Queue.Collection_BuildbotCores_NewName = null;
                }

                // Excluded ObservableCollection
                if (Queue.Collection_ExcludedCores_Name != null)
                {
                    Queue.Collection_ExcludedCores_Name = null;
                }
                // Updated ObservableCollection
                if (Queue.Collection_UpdatedCores_Name != null)
                {
                    Queue.Collection_UpdatedCores_Name = null;
                }


                // -------------------------
                // Call SetArchitecture Method
                // -------------------------
                Paths.SetArchitecture(vm);

                // -------------------------
                // If Download Combobox Cores or RA+Cores selected
                // -------------------------
                if (vm.Download_SelectedItem == "RA+Cores"
                    || vm.Download_SelectedItem == "Cores"
                    || vm.Download_SelectedItem == "New Cores")
                {
                    // Parse index-extended
                    Parse.ParseBuildbotCoresIndex(vm);

                    // Get PC Cores
                    Parse.ScanPcCoresDir(vm);

                    // New Cores List - Debugger Only
                    // Subtract PC List from Buildbot List
                    Queue.List_BuildbotCores_NewName = Queue.List_BuildbotCores_Name.Except(Queue.List_PcCores_Name).ToList();

                    // Get Updated Cores
                    Queue.UpdatedCores(vm);

                    // Call Cores Up To Date Method
                    // If All Cores up to date, display message
                    Queue.CoresUpToDateCheck(vm);
                }


                // -------------------------
                // Display
                // -------------------------
                if (Queue.List_UpdatedCores_Name.Count != 0)
                {
                    // Trim List if new
                    Queue.List_UpdatedCores_Name.TrimExcess();

                    // -------------------------
                    // Add List to Obvservable Collection
                    // -------------------------
                    // PC Name
                    Queue.CollectionPcCoresName = new ObservableCollection<string>(Queue.List_PcCores_Name);
                    // PC Date
                    Queue.CollectionPcCoresDate = new ObservableCollection<string>(Queue.List_PcCores_Date);
                    // PC Name+Date
                    Queue.Collection_PcCores_NameDate = new ObservableCollection<string>(Queue.List_PcCores_NameDate);
                    // PC Unknown Name+Date
                    Queue.CollectionPcCoresUnknownNameDate = new ObservableCollection<string>(Queue.List_PcCores_UnknownName);

                    // Buildbot Name
                    Queue.Collection_BuildbotCores_Name = new ObservableCollection<string>(Queue.List_BuildbotCores_Name);
                    // Buildbot Date
                    Queue.Collection_BuildbotCores_Date = new ObservableCollection<string>(Queue.List_BuildbotCores_Date);
                    // Buildbot Name+Date
                    Queue.Collection_BuildbotCores_NameDate = new ObservableCollection<string>(Queue.List_BuildbotCores_NameDate);
                    // Buildbot New Name
                    Queue.Collection_BuildbotCores_NewName = new ObservableCollection<string>(Queue.List_BuildbotCores_NewName);

                    // Excluded
                    Queue.Collection_ExcludedCores_Name = new ObservableCollection<string>(Queue.List_ExcludedCores_Name);
                    // To Update
                    Queue.Collection_UpdatedCores_Name = new ObservableCollection<string>(Queue.List_UpdatedCores_Name);


                    // -------------------------
                    // Display List in ListBox
                    // -------------------------
                    // PC Name
                    listBoxPcName.ItemsSource = Queue.CollectionPcCoresName;
                    // PC Date
                    listBoxPcDate.ItemsSource = Queue.CollectionPcCoresDate;
                    // PC Name+Date
                    listBoxPcNameDate.ItemsSource = Queue.Collection_PcCores_NameDate;
                    // PC Unknown Name Date
                    listBoxPcUnknown.ItemsSource = Queue.CollectionPcCoresUnknownNameDate;

                    // Buildbot Name
                    listBoxBuildbotName.ItemsSource = Queue.Collection_BuildbotCores_Name;
                    // Buildbot Date
                    listBoxBuildbotDate.ItemsSource = Queue.Collection_BuildbotCores_Date;
                    // Buildbot Name+Date
                    listBoxBuildbotNameDate.ItemsSource = Queue.Collection_BuildbotCores_NameDate;
                    // Buildbot New Cores Name
                    listBoxBuildbotNew.ItemsSource = Queue.Collection_BuildbotCores_NewName;

                    // Excluded
                    listBoxExcluded.ItemsSource = Queue.Collection_ExcludedCores_Name;
                    // To Update
                    listBoxUpdate.ItemsSource = Queue.Collection_UpdatedCores_Name;


                    // -------------------------
                    // Display List Count
                    // -------------------------
                    int count = 0;

                    // PC Name
                    count = (from x in Queue.List_PcCores_Name select x).Count();
                    labelPcNameCount.Content = count.ToString();
                    // PC Date
                    count = (from x in Queue.List_PcCores_Date select x).Count();
                    labelPcDateCount.Content = count.ToString();
                    // PC Name+Date
                    count = (from x in Queue.List_PcCores_NameDate select x).Count();
                    labelPcNameDateCount.Content = count.ToString();
                    // PC Unknown Name+Date
                    count = (from x in Queue.List_PcCores_UnknownName select x).Count();
                    labelPcUnknownCount.Content = count.ToString();

                    //// Buildbot Name
                    count = (from x in Queue.List_BuildbotCores_Name select x).Count();
                    labelBuildbotNameCount.Content = count.ToString();
                    // Buildbot Date
                    count = (from x in Queue.List_BuildbotCores_Date select x).Count();
                    labelBuildbotDateCount.Content = count.ToString();
                    // Buildbot Name+Date
                    count = (from x in Queue.List_BuildbotCores_NameDate select x).Count();
                    labelBuildbotNameDateCount.Content = count.ToString();
                    // Buildbot New Name
                    count = (from x in Queue.List_BuildbotCores_NewName select x).Count();
                    labelBuildbotNewCount.Content = count.ToString();

                    // Excluded
                    count = (from x in Queue.List_ExcludedCores_Name select x).Count();
                    labelExcludedCount.Content = count.ToString();
                    // Update
                    count = (from x in Queue.List_UpdatedCores_Name select x).Count();
                    labelUpdateCount.Content = count.ToString();
                }

                //using (FileStream fs = new FileStream(Paths.appDir + "List_PcCores_NameDate.txt", FileMode.Append, FileAccess.Write))
                //using (StreamWriter sw = new StreamWriter(fs))
                //{
                //    for (int x = 0; x < Queue.List_PcCores_NameDate.Count; x++)
                //    {
                //        sw.WriteLine(Queue.List_PcCores_NameDate[x]);
                //    }
                //    sw.Close();
                //}

                //using (FileStream fs = new FileStream(Paths.appDir + "List_BuildbotCores_NameDate.txt", FileMode.Append, FileAccess.Write))
                //using (StreamWriter sw = new StreamWriter(fs))
                //{
                //    for (int x = 0; x < Queue.List_BuildbotCores_NameDate.Count; x++)
                //    {
                //        sw.WriteLine(Queue.List_BuildbotCores_NameDate[x]);
                //    }
                //    sw.Close();
                //}

                // Clear as to not interfere with Check/Update after closing out of Debug
                MainWindow.ClearRetroArchVars();
                MainWindow.ClearCoresVars();
                MainWindow.ClearLists();
            }

            // -------------------------
            // Clarify
            // -------------------------
            else
            {
                MessageBox.Show("For use with RA+Cores, Cores or New Cores menu option only.");
            }
        }


        // -----------------------------------------------
        // Compare Name Button
        // -----------------------------------------------
        private void buttonCompareName_Click(object sender, RoutedEventArgs e)
        {
            //var compareNames = Queue.List_PcCores_Name.Intersect(Queue.List_BuildbotCores_Name);

            //var message = string.Join(Environment.NewLine, compareNames);
            //MessageBox.Show(message);
        }
    }
}
