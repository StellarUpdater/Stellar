using System;
using System.IO;

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
    public partial class Log
    {
        public static void WriteLog()
        {
            // Write Log Append
            if (Configure.logEnable == true) // Only if Log is Enabled through Config Checkbox
            {
                using (FileStream fs = new FileStream(/*pass data*/Configure.logPath + "stellar.log", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(DateTime.Now);
                    sw.WriteLine("--------------------------------------\r\n");

                    // Append List
                    for (int x = 0; x < Queue.ListUpdatedCoresName.Count; x++)
                    {
                        sw.WriteLine(Queue.ListUpdatedCoresName[x]);
                    }

                    sw.WriteLine("\r\n");

                    // Close Log
                    sw.Close();
                }
            }
        }
    }
}
