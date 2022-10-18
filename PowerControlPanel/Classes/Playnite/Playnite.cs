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
            if (Properties.Settings.Default.directoryPlaynite != "")
            {
                if (playniteRunning())
                {
                    RunCLI.RunCommand(" --shutdown", false, Properties.Settings.Default.directoryPlaynite + "\\Playnite.FullscreenApp.exe", 6000, false);
                }
                else
                {
                    RunCLI.RunCommand(" --startfullscreen", false, Properties.Settings.Default.directoryPlaynite + "\\Playnite.FullscreenApp.exe", 6000, false);
                }
            }
           
          
        }
        public static void setPlayniteDirectory()
        {
            if (Properties.Settings.Default.directoryPlaynite == "")
            {
                if (File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite") + "\\Playnite.FullscreenApp.exe"))
                {
                    Properties.Settings.Default.directoryPlaynite = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Playnite");
                }
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
