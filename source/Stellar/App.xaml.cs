using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
// Disable XML Comment warnings
#pragma warning disable 1591
#pragma warning disable 1587
#pragma warning disable 1570

namespace Stellar
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //private static MainWindow mainwindow = ((MainWindow)System.Windows.Application.Current.MainWindow);
        //ViewModel vm = MainWindow.DataContext as ViewModel;
        


        // -----------------------------------------------------------------
        // Run CLI Arguments
        // -----------------------------------------------------------------
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    //ViewModel vm = MainWindow.DataContext as ViewModel;

        //    // Arguments passed from CMD
        //    var args = e.Args;

        //    string message = string.Join("\n", args);

        //    // Only launch CMD if arguments not empty
        //    if (args != null &&
        //        args.Length != 0)
        //    {
        //        // -------------------------
        //        // Download
        //        // -------------------------
        //        if (args.Contains("-download cores"))
        //        {
        //            vm.Download_SelectedItem = "Cores";
        //        }

        //        // -------------------------
        //        // Architecture
        //        // -------------------------
        //        if (args.Contains("-arch 64-bit"))
        //        {
        //            vm.Architecture_SelectedItem = "64-bit";
        //        }

        //        // -------------------------
        //        // Server
        //        // -------------------------
        //        if (args.Contains("-server buildbot"))
        //        {
        //            vm.Server_SelectedItem = "buildbot";
        //        }

        //        // -------------------------
        //        // Update
        //        // -------------------------
        //        mainwindow.Update();



                //MessageBox.Show(message,
                //                "Error",
                //                MessageBoxButton.OK,
                //                MessageBoxImage.Information);

        //    }
        //}



    

    }
}
