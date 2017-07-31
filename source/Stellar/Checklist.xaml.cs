using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
    /// <summary>
    /// Interaction logic for Checklist.xaml
    /// </summary>
    public partial class Checklist : Window
    {
        // Checklist Window's (public) Excluded Cores List
        public static List<string> ListExcludedCores = new List<string>();

        // Toggle Window
        bool toggleWindow = true;

        // ----------------------------------------------------------------------------------------------
        // METHODS 
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Window Defaults
        // -----------------------------------------------

        // Pass Data from Main Window to Checklist Window
        ObservableCollection<string> CollectionUpdatedCoresName;
        ObservableCollection<string> CollectionPcCoresNameDate;
        ObservableCollection<string> CollectionBuildbotNameDate;

        public Checklist(ObservableCollection<string> CollectionUpdatedCoresName, ObservableCollection<string> CollectionPcCoresNameDate, ObservableCollection<string> CollectionBuildbotNameDate) // Pass both Updated Cores Name & PC Name+Date Observable Collection to Checklist Window
        {
            InitializeComponent();

            // Pass Data
            this.CollectionUpdatedCoresName = CollectionUpdatedCoresName;
            this.CollectionPcCoresNameDate = CollectionPcCoresNameDate;
            this.CollectionBuildbotNameDate = CollectionBuildbotNameDate;


            // Add to List View
            //listBoxUpdatedCores.ItemsSource = CollectionUpdatedCoresName; //List Box
            listViewUpdatedCores.ItemsSource = CollectionUpdatedCoresName;
            // Select All
            listViewUpdatedCores.SelectAll();


            // Load the Name+Date Lists #################
            // List Box Pc Cores Name+Date (Advanced)
            listBoxPcCoresNameDate.ItemsSource = CollectionPcCoresNameDate;
            listBoxPcCoresNameDate.SelectedIndex = -1; // Deselect All at Initialize

            // List Box Buildbot Cores Name+Date (Advanced)
            listBoxBuildbotCoresNameDate.ItemsSource = CollectionBuildbotNameDate;
            listBoxBuildbotCoresNameDate.SelectedIndex = -1; // Deselect All at Initialize


            // Load Excluded Cores ListView Checkboxes from Settigns - NOT WORKING
            //if (ListExcludedCores.Count != 0) //if list is not empty (will crash otherwise)
            //{
            //    //listViewUpdatedCores = Properties.Settings.Default.excludedCores.Cast<string>().ToList();
            //}

            // Clear List to prevent doubling up
            //CollectionUpdatedCoresName.Clear();
        }

        // -----------------------------------------------
        // Window Close
        // -----------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Clear Lists to prevent doubling up
            // Once Main List has been cleared, you can't get it back
            CollectionPcCoresNameDate.Clear();
            CollectionBuildbotNameDate.Clear();
        }

        // -----------------------------------------------
        // Excluded List
        // -----------------------------------------------
        private void listViewUpdatedCores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If Unchecked
            // For each item in (unchecked), Add to Excluded List
            foreach (string item in e.RemovedItems)
            {
                ListExcludedCores.Add(item);
            }
            // If Checked
            // For each item in (checked), Remove from Excluded List
            foreach (string item in e.AddedItems)
            {
                ListExcludedCores.Remove(item);
                ListExcludedCores.TrimExcess();
            }

            // Save List to Settings - DISABLED FOR NOW
            //Settings.Default["excludedCores"] = listViewUpdatedCores.Items;
            //Settings.Default.Save();
            //Settings.Default.Reload();
        }

        // ----------------------------------------------------------------------------------------------
        // CONTROLS
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Select All Button
        // -----------------------------------------------
        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            listViewUpdatedCores.SelectAll();
        }

        // -----------------------------------------------
        // Deselect All Button
        // -----------------------------------------------
        private void buttonDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            listViewUpdatedCores.SelectedIndex = -1;
        }

        // -----------------------------------------------
        // Advanced Button
        // -----------------------------------------------
        private void buttonAdvanced_Click(object sender, RoutedEventArgs e)
        {
            // -------------------------
            // Toggle Window Advanced
            // -------------------------
            if (toggleWindow == true)
            {
                // set window size
                this.Width = 1117;

                // Reposition Window
                this.Left = this.Left - 387;

                // set window title
                this.Title = "Checklist Advanced";

                // set the toggle to false
                toggleWindow = false;
            }

            // -------------------------
            // Toggle Window Basic
            // -------------------------
            else if (toggleWindow == false)
            {
                // set window size
                this.Width = 340;

                // Reposition Window
                this.Left = this.Left + 387;

                // set window title
                this.Title = "Checklist";

                // set the toggle to true
                toggleWindow = true;
            }

        }

    }
}