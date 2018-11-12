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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;

namespace Stellar
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private void OnPropertyChanged(string prop)
        {
            //PropertyChangedEventHandler handler = PropertyChanged;
            //handler(this, new PropertyChangedEventArgs(name));

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(prop));
            }
        }

        // --------------------------------------------------
        // Main
        // --------------------------------------------------
        public ViewModel()
        {
        }

        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        // Main Window
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        // --------------------------------------------------
        // Location - TextBox
        // --------------------------------------------------
        // Text
        private string _Location_Text;
        public string Location_Text
        {
            get { return _Location_Text; }
            set
            {
                if (_Location_Text == value)
                {
                    return;
                }

                _Location_Text = value;
                OnPropertyChanged("Location_Text");
            }
        }

        // Controls Enable
        private bool _Location_IsEnabled = true;
        public bool Location_IsEnabled
        {
            get { return _Location_IsEnabled; }
            set
            {
                if (_Location_IsEnabled == value)
                {
                    return;
                }

                _Location_IsEnabled = value;
                OnPropertyChanged("Location_IsEnabled");
            }
        }


        // --------------------------------------------------
        // DownloadURL - TextBox
        // --------------------------------------------------
        // Text
        private string _DownloadURL_Text;
        public string DownloadURL_Text
        {
            get { return _DownloadURL_Text; }
            set
            {
                if (_DownloadURL_Text == value)
                {
                    return;
                }

                _DownloadURL_Text = value;
                OnPropertyChanged("DownloadURL_Text");
            }
        }


        // --------------------------------------------------
        // Server
        // --------------------------------------------------
        // Item Source
        private List<string> _Server_Items = new List<string>()
        {
            "buildbot",
            "raw"
        };
        public List<string> Server_Items
        {
            get { return _Server_Items; }
            set
            {
                _Server_Items = value;
                OnPropertyChanged("Server_Items");
            }
        }

        // Selected Index
        private int _Server_SelectedIndex { get; set; }
        public int Server_SelectedIndex
        {
            get { return _Server_SelectedIndex; }
            set
            {
                if (_Server_SelectedIndex == value)
                {
                    return;
                }

                _Server_SelectedIndex = value;
                OnPropertyChanged("Server_SelectedIndex");
            }
        }

        // Selected Item
        private string _Server_SelectedItem { get; set; }
        public string Server_SelectedItem
        {
            get { return _Server_SelectedItem; }
            set
            {
                if (_Server_SelectedItem == value)
                {
                    return;
                }

                _Server_SelectedItem = value;
                OnPropertyChanged("Server_SelectedItem");
            }
        }

        // Controls Enable
        private bool _Server_IsEnabled = true;
        public bool Server_IsEnabled
        {
            get { return _Server_IsEnabled; }
            set
            {
                if (_Server_IsEnabled == value)
                {
                    return;
                }

                _Server_IsEnabled = value;
                OnPropertyChanged("Server_IsEnabled");
            }
        }


        // --------------------------------------------------
        // ProgressInfo - TextBox
        // --------------------------------------------------
        // Text
        private string _ProgressInfo_Text;
        public string ProgressInfo_Text
        {
            get { return _ProgressInfo_Text; }
            set
            {
                if (_ProgressInfo_Text == value)
                {
                    return;
                }

                _ProgressInfo_Text = value;
                OnPropertyChanged("ProgressInfo_Text");
            }
        }

        // --------------------------------------------------
        // Progress Bar
        // --------------------------------------------------

        private double _currentProgress;
        public double CurrentProgress_Value
        {
            get { return _currentProgress; }
            set
            {
                _currentProgress = value;
                OnPropertyChanged("CurrentProgress_Value");
            }
        }


        // --------------------------------------------------
        // Download
        // --------------------------------------------------
        // Item Source
        private List<string> _Download_Items = new List<string>()
        {
            "New Install",
            "Upgrade",
            "RetroArch",
            "RA+Cores",
            "Cores",
            "New Cores",
            "Redist",
            "Stellar"
        };
        public List<string> Download_Items
        {
            get { return _Download_Items; }
            set
            {
                _Download_Items = value;
                OnPropertyChanged("Download_Items");
            }
        }

        // Selected Index
        private int _Download_SelectedIndex { get; set; }
        public int Download_SelectedIndex
        {
            get { return _Download_SelectedIndex; }
            set
            {
                if (_Download_SelectedIndex == value)
                {
                    return;
                }

                _Download_SelectedIndex = value;
                OnPropertyChanged("Download_SelectedIndex");
            }
        }

        // Selected Item
        private string _Download_SelectedItem { get; set; }
        public string Download_SelectedItem
        {
            get { return _Download_SelectedItem; }
            set
            {
                if (_Download_SelectedItem == value)
                {
                    return;
                }

                _Download_SelectedItem = value;
                OnPropertyChanged("Download_SelectedItem");
            }
        }

        // Controls Enable
        private bool _Download_IsEnabled = true;
        public bool Download_IsEnabled
        {
            get { return _Download_IsEnabled; }
            set
            {
                if (_Download_IsEnabled == value)
                {
                    return;
                }

                _Download_IsEnabled = value;
                OnPropertyChanged("Download_IsEnabled");
            }
        }


        // --------------------------------------------------
        // Architecture
        // --------------------------------------------------
        // Item Source
        private List<string> _Architecture_Items = new List<string>()
        {
            "32-bit",
            "64-bit"
        };
        public List<string> Architecture_Items
        {
            get { return _Architecture_Items; }
            set
            {
                _Architecture_Items = value;
                OnPropertyChanged("Architecture_Items");
            }
        }

        // Selected Index
        private int _Architecture_SelectedIndex { get; set; }
        public int Architecture_SelectedIndex
        {
            get { return _Architecture_SelectedIndex; }
            set
            {
                if (_Architecture_SelectedIndex == value)
                {
                    return;
                }

                _Architecture_SelectedIndex = value;
                OnPropertyChanged("Architecture_SelectedIndex");
            }
        }

        // Selected Item
        private string _Architecture_SelectedItem { get; set; }
        public string Architecture_SelectedItem
        {
            get { return _Architecture_SelectedItem; }
            set
            {
                if (_Architecture_SelectedItem == value)
                {
                    return;
                }

                _Architecture_SelectedItem = value;
                OnPropertyChanged("Architecture_SelectedItem");
            }
        }

        // Controls Enable
        private bool _Architecture_IsEnabled = true;
        public bool Architecture_IsEnabled
        {
            get { return _Architecture_IsEnabled; }
            set
            {
                if (_Architecture_IsEnabled == value)
                {
                    return;
                }

                _Architecture_IsEnabled = value;
                OnPropertyChanged("Architecture_IsEnabled");
            }
        }

        // --------------------------------------------------
        // Update - TextBox
        // --------------------------------------------------
        // Text
        private string _Update_Text;
        public string Update_Text
        {
            get { return _Update_Text; }
            set
            {
                if (_Update_Text == value)
                {
                    return;
                }

                _Update_Text = value;
                OnPropertyChanged("Update_Text");
            }
        }


        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------
        // Configure Window
        // ----------------------------------------------------------------------------------------------------
        // ----------------------------------------------------------------------------------------------------

        // --------------------------------------------------
        // 7-Zip - TextBox
        // --------------------------------------------------
        // Text
        private string _SevenZipPath_Text;
        public string SevenZipPath_Text
        {
            get { return _SevenZipPath_Text; }
            set
            {
                if (_SevenZipPath_Text == value)
                {
                    return;
                }

                _SevenZipPath_Text = value;
                OnPropertyChanged("SevenZipPath_Text");
            }
        }


        // --------------------------------------------------
        // WinRAR - TextBox
        // --------------------------------------------------
        // Text
        private string _WinRARPath_Text;
        public string WinRARPath_Text
        {
            get { return _WinRARPath_Text; }
            set
            {
                if (_WinRARPath_Text == value)
                {
                    return;
                }

                _WinRARPath_Text = value;
                OnPropertyChanged("WinRARPath_Text");
            }
        }


        // --------------------------------------------------
        // LogPath - TextBox
        // --------------------------------------------------
        // Text
        private string _LogPath_Text;
        public string LogPath_Text
        {
            get { return _LogPath_Text; }
            set
            {
                if (_LogPath_Text == value)
                {
                    return;
                }

                _LogPath_Text = value;
                OnPropertyChanged("LogPath_Text");
            }
        }

        // -------------------------
        // LogPath - CheckBox
        // -------------------------
        // Checked
        private bool _LogPath_IsChecked;
        public bool LogPath_IsChecked
        {
            get { return _LogPath_IsChecked; }
            set
            {
                if (_LogPath_IsChecked == value) return;

                _LogPath_IsChecked = value;
            }
        }
        // Enabled
        private bool _LogPath_IsEnabled = true;
        public bool LogPath_IsEnabled
        {
            get { return _LogPath_IsEnabled; }
            set
            {
                if (_LogPath_IsEnabled == value)
                {
                    return;
                }

                _LogPath_IsEnabled = value;
                OnPropertyChanged("LogPath_IsEnabled");
            }
        }


        // --------------------------------------------------
        // Theme
        // --------------------------------------------------
        // Item Source
        private List<string> _Theme_Items = new List<string>()
        {
            "Milky Way",
            "Spiral Galaxy",
            "Flaming Star",
            "Spiral Nebula",
            "Dark Galaxy",
            "Lagoon",
            "Solar Flare",
            "Dark Nebula",
            "Star Dust",
            "Chaos"
        };
        public List<string> Theme_Items
        {
            get { return _Theme_Items; }
            set
            {
                _Theme_Items = value;
                OnPropertyChanged("Theme_Items");
            }
        }

        // Selected Index
        private int _Theme_SelectedIndex { get; set; }
        public int Theme_SelectedIndex
        {
            get { return _Theme_SelectedIndex; }
            set
            {
                if (_Theme_SelectedIndex == value)
                {
                    return;
                }

                _Theme_SelectedIndex = value;
                OnPropertyChanged("Theme_SelectedIndex");
            }
        }

        // Selected Item
        private string _Theme_SelectedItem { get; set; }
        public string Theme_SelectedItem
        {
            get { return _Theme_SelectedItem; }
            set
            {
                if (_Theme_SelectedItem == value)
                {
                    return;
                }

                _Theme_SelectedItem = value;
                OnPropertyChanged("Theme_SelectedItem");
            }
        }

        // Controls Enable
        private bool _Theme_IsEnabled = true;
        public bool Theme_IsEnabled
        {
            get { return _Theme_IsEnabled; }
            set
            {
                if (_Theme_IsEnabled == value)
                {
                    return;
                }

                _Theme_IsEnabled = value;
                OnPropertyChanged("Theme_IsEnabled");
            }
        }


        // -------------------------
        // Update Auto Check - Toggle
        // -------------------------
        // Checked
        private bool _UpdateAutoCheck_IsChecked;
        public bool UpdateAutoCheck_IsChecked
        {
            get { return _UpdateAutoCheck_IsChecked; }
            set
            {
                if (_UpdateAutoCheck_IsChecked == value) return;

                _UpdateAutoCheck_IsChecked = value;
            }
        }
        // Enabled
        private bool _UpdateAutoCheck_IsEnabled = true;
        public bool UpdateAutoCheck_IsEnabled
        {
            get { return _UpdateAutoCheck_IsEnabled; }
            set
            {
                if (_UpdateAutoCheck_IsEnabled == value)
                {
                    return;
                }

                _UpdateAutoCheck_IsEnabled = value;
                OnPropertyChanged("UpdateAutoCheck_IsEnabled");
            }
        }


        // --------------------------------------------------
        // Update Auto Check - TextBlock
        // --------------------------------------------------
        // Text
        private string _UpdateAutoCheck_Text;
        public string UpdateAutoCheck_Text
        {
            get { return _UpdateAutoCheck_Text; }
            set
            {
                if (_UpdateAutoCheck_Text == value)
                {
                    return;
                }

                _UpdateAutoCheck_Text = value;
                OnPropertyChanged("UpdateAutoCheck_Text");
            }
        }


    }
}
