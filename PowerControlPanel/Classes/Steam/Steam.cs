using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes.Steam
{
    public static class Steam
    {
        public static bool steamRunning()
        {
            Process[] pname = Process.GetProcessesByName("Steam");
            if (pname.Length != 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        public static void openSteamBigPicture()
        {
            if (Properties.Settings.Default.directorySteam != "")
            {
                if (steamRunning())
                {
                    RunCLI.RunCommand(" \"steam://open/bigpicture\"", false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, false);

                }
                else
                {
                    RunCLI.RunCommand(" -bigpicture", false, Properties.Settings.Default.directorySteam + "\\Steam.exe", 6000, false);
                }
            }

  

        }
        public static void setSteamDirectory()
        {
            if (Properties.Settings.Default.directorySteam == "")
            {
                if (File.Exists(Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"),"Steam") + "\\Steam.exe"))
                {
                    Properties.Settings.Default.directorySteam = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"),"Steam");
                }
            }

        }
    }
}
