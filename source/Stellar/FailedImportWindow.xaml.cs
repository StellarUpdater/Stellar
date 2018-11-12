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

using System.Windows;
using System.Windows.Documents;

namespace Stellar
{
    /// <summary>
    /// Interaction logic for FailedImportWindow.xaml
    /// </summary>
    public partial class FailedImportWindow : Window
    {
        public FailedImportWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        ///    Window Loaded
        /// </summary>
        public void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Paragraph p = new Paragraph();
            rtbFailedImport.Document = new FlowDocument(p);

            rtbFailedImport.BeginChange();
            p.Inlines.Add(new Run("Please set the following and re-save your profile.\n\n"));
            p.Inlines.Add(new Run(Configure.failedImportMessage));
            rtbFailedImport.EndChange();

            // Clear
            Configure.failedImportMessage = string.Empty;
        }
    }
}
