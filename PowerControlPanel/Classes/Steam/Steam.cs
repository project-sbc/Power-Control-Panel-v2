using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes.Steam
{
    public static class Steam
    {
        public static bool steamRunning()
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
        public static void openSteamBigPicture()
        {
            if (steamRunning())
            {


            }
            else 
            { 
            
            }

        }

    }
}
