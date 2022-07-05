using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Power_Control_Panel.PowerControlPanel.Classes.BatteryInfo
{
    public class BatteryInfo
    {
        public static void readBatteryValue()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            string returnValue = "AC";
            foreach (ManagementObject mo in mos.Get())
            {
                returnValue = mo["EstimatedChargeRemaining"].ToString();
            }
            if (returnValue != "AC") { GlobalVariables.batteryPercentage = returnValue; } else { GlobalVariables.powerStatus = "AC"; }
            
            

        }

    }
}
