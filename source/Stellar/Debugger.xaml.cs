using System.Collections.ObjectModel;
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
        //    MainWindow.ClearAll();
        //}


        // -----------------------------------------------
        // Test Button
        // -----------------------------------------------
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            // -------------------------
            // Check
            // -------------------------
            if ((string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores"
            || (string)mainwindow.comboBoxDownload.SelectedItem == "Cores")
            {
                // -------------------------
                // Clear
                // -------------------------
                // Extra Debugger Clears

                // Clear All to prevent Lists doubling up
                //MainWindow.ClearAll();
                //MainWindow.ClearNameDates();
                MainWindow.ClearRetroArchVars();
                MainWindow.ClearCoresVars();
                MainWindow.ClearLists();


                // Lists

                // PC Core Name+Date
                if (Queue.ListPcCoresNameDate != null)
                {
                    Queue.ListPcCoresNameDate.Clear();
                    Queue.ListPcCoresNameDate.TrimExcess();
                }

                // Buildbot Cores Name+Date
                if (Queue.ListBuildbotCoresNameDate != null)
                {
                    Queue.ListBuildbotCoresNameDate.Clear();
                    Queue.ListBuildbotCoresNameDate.TrimExcess();
                }

                // Excluded Core Name+Date
                if (Queue.ListExcludedCoresNameDate != null)
                {
                    Queue.ListExcludedCoresNameDate.Clear();
                    Queue.ListExcludedCoresNameDate.TrimExcess();
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
                if (Queue.CollectionPcCoresNameDate != null)
                {
                    Queue.CollectionPcCoresNameDate = null;
                }
                // PC Core Unknown Name+Date ObservableCollection
                if (Queue.CollectionPcCoresUnknownNameDate != null)
                {
                    Queue.CollectionPcCoresUnknownNameDate = null;
                }

                // Buildbot Core Name ObservableCollection
                if (Queue.CollectionBuildbotCoresName != null)
                {
                    Queue.CollectionBuildbotCoresName = null;
                }
                // Buildbot Core Date ObservableCollection
                if (Queue.CollectionBuildbotCoresDate != null)
                {
                    Queue.CollectionBuildbotCoresDate = null;
                }
                // Buildbot Core Name+Date ObservableCollection
                if (Queue.CollectionBuildbotCoresNameDate != null)
                {
                    Queue.CollectionBuildbotCoresNameDate = null;
                }
                // Buildbot Core New Name ObservableCollection
                if (Queue.CollectionBuildbotCoresNewName != null)
                {
                    Queue.CollectionBuildbotCoresNewName = null;
                }

                // Excluded ObservableCollection
                if (Queue.CollectionExcludedCoresName != null)
                {
                    Queue.CollectionExcludedCoresName = null;
                }
                // Updated ObservableCollection
                if (Queue.CollectionUpdatedCoresName != null)
                {
                    Queue.CollectionUpdatedCoresName = null;
                }


                // -------------------------
                // Call SetArchitecture Method
                // -------------------------
                Paths.SetArchitecture(mainwindow);

                // -------------------------
                // If Download Combobox Cores or RA+Cores selected
                // -------------------------
                if ((string)mainwindow.comboBoxDownload.SelectedItem == "RA+Cores"
                    || (string)mainwindow.comboBoxDownload.SelectedItem == "Cores"
                    || (string)mainwindow.comboBoxDownload.SelectedItem == "New Cores")
                {
                    // Parse index-extended
                    Parse.ParseBuildbotCoresIndex(mainwindow);

                    // Get PC Cores
                    Parse.ScanPcCoresDir(mainwindow);

                    // New Cores List - Debugger Only
                    // Subtract PC List from Buildbot List
                    Queue.ListBuildbotCoresNewName = Queue.ListBuildbotCoresName.Except(Queue.ListPcCoresName).ToList();

                    // Get Updated Cores
                    Queue.UpdatedCores(mainwindow);

                    // Call Cores Up To Date Method
                    // If All Cores up to date, display message
                    Queue.CoresUpToDateCheck(mainwindow);
                }


                // -------------------------
                // Display
                // -------------------------
                if (Queue.ListUpdatedCoresName.Count != 0)
                {
                    // Trim List if new
                    Queue.ListUpdatedCoresName.TrimExcess();

                    // -------------------------
                    // Add List to Obvservable Collection
                    // -------------------------
                    // PC Name
                    Queue.CollectionPcCoresName = new ObservableCollection<string>(Queue.ListPcCoresName);
                    // PC Date
                    Queue.CollectionPcCoresDate = new ObservableCollection<string>(Queue.ListPcCoresDate);
                    // PC Name+Date
                    Queue.CollectionPcCoresNameDate = new ObservableCollection<string>(Queue.ListPcCoresNameDate);
                    // PC Unknown Name+Date
                    Queue.CollectionPcCoresUnknownNameDate = new ObservableCollection<string>(Queue.ListPcCoresUnknownNameDate);

                    // Buildbot Name
                    Queue.CollectionBuildbotCoresName = new ObservableCollection<string>(Queue.ListBuildbotCoresName);
                    // Buildbot Date
                    Queue.CollectionBuildbotCoresDate = new ObservableCollection<string>(Queue.ListBuildbotCoresDate);
                    // Buildbot Name+Date
                    Queue.CollectionBuildbotCoresNameDate = new ObservableCollection<string>(Queue.ListBuildbotCoresNameDate);
                    // Buildbot New Name
                    Queue.CollectionBuildbotCoresNewName = new ObservableCollection<string>(Queue.ListBuildbotCoresNewName);

                    // Excluded
                    Queue.CollectionExcludedCoresName = new ObservableCollection<string>(Queue.ListExcludedCoresName);
                    // To Update
                    Queue.CollectionUpdatedCoresName = new ObservableCollection<string>(Queue.ListUpdatedCoresName);


                    // -------------------------
                    // Display List in ListBox
                    // -------------------------
                    // PC Name
                    listBoxPcName.ItemsSource = Queue.CollectionPcCoresName;
                    // PC Date
                    listBoxPcDate.ItemsSource = Queue.CollectionPcCoresDate;
                    // PC Name+Date
                    listBoxPcNameDate.ItemsSource = Queue.CollectionPcCoresNameDate;
                    // PC Unknown Name Date
                    listBoxPcUnknown.ItemsSource = Queue.CollectionPcCoresUnknownNameDate;

                    // Buildbot Name
                    listBoxBuildbotName.ItemsSource = Queue.CollectionBuildbotCoresName;
                    // Buildbot Date
                    listBoxBuildbotDate.ItemsSource = Queue.CollectionBuildbotCoresDate;
                    // Buildbot Name+Date
                    listBoxBuildbotNameDate.ItemsSource = Queue.CollectionBuildbotCoresNameDate;
                    // Buildbot New Cores Name
                    listBoxBuildbotNew.ItemsSource = Queue.CollectionBuildbotCoresNewName;

                    // Excluded
                    listBoxExcluded.ItemsSource = Queue.CollectionExcludedCoresName;
                    // To Update
                    listBoxUpdate.ItemsSource = Queue.CollectionUpdatedCoresName;


                    // -------------------------
                    // Display List Count
                    // -------------------------
                    int count = 0;

                    // PC Name
                    count = (from x in Queue.ListPcCoresName select x).Count();
                    labelPcNameCount.Content = count.ToString();
                    // PC Date
                    count = (from x in Queue.ListPcCoresDate select x).Count();
                    labelPcDateCount.Content = count.ToString();
                    // PC Name+Date
                    count = (from x in Queue.ListPcCoresNameDate select x).Count();
                    labelPcNameDateCount.Content = count.ToString();
                    // PC Unknown Name+Date
                    count = (from x in Queue.ListPcCoresUnknownNameDate select x).Count();
                    labelPcUnknownCount.Content = count.ToString();

                    //// Buildbot Name
                    count = (from x in Queue.ListBuildbotCoresName select x).Count();
                    labelBuildbotNameCount.Content = count.ToString();
                    // Buildbot Date
                    count = (from x in Queue.ListBuildbotCoresDate select x).Count();
                    labelBuildbotDateCount.Content = count.ToString();
                    // Buildbot Name+Date
                    count = (from x in Queue.ListBuildbotCoresNameDate select x).Count();
                    labelBuildbotNameDateCount.Content = count.ToString();
                    // Buildbot New Name
                    count = (from x in Queue.ListBuildbotCoresNewName select x).Count();
                    labelBuildbotNewCount.Content = count.ToString();

                    // Excluded
                    count = (from x in Queue.ListExcludedCoresName select x).Count();
                    labelExcludedCount.Content = count.ToString();
                    // Update
                    count = (from x in Queue.ListUpdatedCoresName select x).Count();
                    labelUpdateCount.Content = count.ToString();
                }

                // Clear All to prevent Lists doubling up
                //MainWindow.ClearAll();
                //MainWindow.ClearNameDates();

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
                System.Windows.MessageBox.Show("For use with RA+Cores or Cores menu option only.");
            }
        }


    }
}
