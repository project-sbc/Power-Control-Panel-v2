using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XInput;
using Power_Control_Panel.PowerControlPanel.Classes;
using System.Net.NetworkInformation;
using System.Windows;
using System.Management;

namespace Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate
{
    public class RoutineUpdate
    {
        public static void handleRoutineChecks(int counter)
        {
            //Read tdp every 60 seconds
            if (counter == 60) { Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeTDP.ChangeTDP.readTDP()); }
 

            decimal dec5Counter = counter / 5;

            if (dec5Counter == decimal.Round(dec5Counter, 0))
            {
                checkNetworkInterface();
                checkPowerStatus();
            }
        }


        public static void  checkNetworkInterface()
        {

            //Gets internet status to display on overlay
            NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            bool connectedDevice = false;
            foreach (NetworkInterface networkCard in networkCards)
            {
                if (networkCard.OperationalStatus == OperationalStatus.Up)
                {
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Ethernet) { GlobalVariables.internetDevice = "Ethernet"; connectedDevice = true; }
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) { GlobalVariables.internetDevice = "Wireless"; connectedDevice = true; }
                }
                

            }
            if (!connectedDevice) { GlobalVariables.internetDevice = "Not Connected"; }
        }

        public static void checkPowerStatus()
        {

            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            string returnValue = "AC";
            foreach (ManagementObject mo in mos.Get())
            {
                returnValue = mo["EstimatedChargeRemaining"].ToString();
            }
            if (returnValue != "AC") 
            { GlobalVariables.batteryPercentage = Int16.Parse(returnValue);
                PowerLineStatus Power = SystemParameters.PowerLineStatus;
                GlobalVariables.powerStatus = Power.ToString();

            } else { GlobalVariables.powerStatus = "AC"; }


        }
    }
}
