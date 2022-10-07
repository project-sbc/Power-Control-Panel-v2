using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Power_Control_Panel.PowerControlPanel.Classes.Playnite
{
    public static class Playnite
    {
        public static void playniteToggle()
        {
            if (playniteRunning())
            {
                RunCLI.RunCommand(" --shutdown", false, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite") + "\\Playnite.FullscreenApp.exe",6000,false);

            }
            else
            {

                RunCLI.RunCommand(" --startfullscreen", false, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite") + "\\Playnite.FullscreenApp.exe",6000,false);

            }
        }

        private static bool playniteRunning()
        {
            

            Process[] pname = Process.GetProcessesByName("Playnite.FullscreenApp");
            if (pname.Length != 0)
            {
                return true;

            }
            else
            {
                return false;
            }
                        
        }

    }
}
