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
        // Toggle Window
        bool toggleWindow = true;

        // ----------------------------------------------------------------------------------------------
        // METHODS 
        // ----------------------------------------------------------------------------------------------

        // -----------------------------------------------
        // Window Defaults
        // -----------------------------------------------

        public Checklist()
        {
            InitializeComponent();

            this.MinWidth = 360;
            this.MinHeight = 470;
            this.MaxWidth = 1137;
            this.MaxHeight = 470;

            // Trim List if new
            Queue.List_CoresToUpdate_Name.TrimExcess();

            // Add Updated Cores List to List Box
            Queue.Collection_CoresToUpdate_Name = new ObservableCollection<string>(Queue.List_CoresToUpdate_Name);

            // Add PC Cores Name+Date to List Box
            Queue.Collection_PcCores_NameDate = new ObservableCollection<string>(Queue.List_PcCores_NameDate);

            // Add Buildbot Cores Name+Date to List Box 
            Queue.Collection_BuildbotCores_NameDate = new ObservableCollection<string>(Queue.List_BuildbotCores_NameDate);


            // Add to List View
            listViewCoresToUpdate.ItemsSource = Queue.Collection_CoresToUpdate_Name;
            // Select All
            listViewCoresToUpdate.SelectAll();

            // Load the Name+Date Lists
            // List Box Pc Cores Name+Date (Advanced)
            listBoxPcCoresNameDate.ItemsSource = Queue.Collection_PcCores_NameDate;
            listBoxPcCoresNameDate.SelectedIndex = -1; // Deselect All at Initialize

            // List Box Buildbot Cores Name+Date (Advanced)
            listBoxBuildbotCoresNameDate.ItemsSource = Queue.Collection_BuildbotCores_NameDate;
            listBoxBuildbotCoresNameDate.SelectedIndex = -1; // Deselect All at Initialize
        }

        /// <summary>
        ///     Window Close
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        /// <summary>
        ///     Rejected List
        /// </summary>
        private void listViewUpdatedCores_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If Unchecked
            // For each item in (unchecked), Add to Rejected List
            foreach (string item in e.RemovedItems)
            {
                Queue.List_RejectedCores_Name.Add(item);
            }
            // If Checked
            // For each item in (checked), Remove from Rejected List
            foreach (string item in e.AddedItems)
            {
                Queue.List_RejectedCores_Name.Remove(item);
                Queue.List_RejectedCores_Name.TrimExcess();
            }

            // Save List to Settings - DISABLED FOR NOW
            //Settings.Default["excludedCores"] = listViewUpdatedCores.Items;
            //Settings.Default.Save();
            //Settings.Default.Reload();
        }

        // ----------------------------------------------------------------------------------------------
        // CONTROLS
        // ----------------------------------------------------------------------------------------------

        /// <summary>
        ///     Select All Button
        /// </summary>
        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            listViewCoresToUpdate.SelectAll();
        }

        /// <summary>
        ///     Deselect All Button
        /// </summary>
        private void buttonDeselectAll_Click(object sender, RoutedEventArgs e)
        {
            listViewCoresToUpdate.SelectedIndex = -1;
        }

        /// <summary>
        ///     Advanced Button
        /// </summary>
        private void buttonAdvanced_Click(object sender, RoutedEventArgs e)
        {
            // -------------------------
            // Toggle Window Advanced
            // -------------------------
            if (toggleWindow == true)
            {
                this.Width = 1137;
                this.Left = this.Left - 407;
                this.Title = "Checklist Advanced";
                toggleWindow = false;
            }

            // -------------------------
            // Toggle Window Basic
            // -------------------------
            else if (toggleWindow == false)
            {
                this.Width = 360;
                this.Left = this.Left + 407;
                this.Title = "Checklist";
                toggleWindow = true;
            }

        }

        /// <summary>
        ///     OK Button
        /// </summary>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}