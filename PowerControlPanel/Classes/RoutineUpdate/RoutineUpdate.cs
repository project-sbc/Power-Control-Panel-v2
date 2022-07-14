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
using System.Threading;

namespace Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate
{
    public class RoutineUpdate
    {
        public static Thread controllerThread;
        public static int sleepTimer = 1000;
        private static int count5 = 0;
        public static void handleRoutineChecks(int counter)
        {
            //Read tdp every 60 seconds
            if (counter == 60) { Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeTDP.ChangeTDP.readTDP()); }
 

            

            if (count5 >= 5)
            {
                checkNetworkInterface();
                checkPowerStatus();
                Classes.ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();
                Classes.ChangeVolume.AudioManager.GetMasterVolume();
                count5 = 0;
            }
            else
            {
                count5++;
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


        public static void createGamePadStateCollectorLoop()
        {
            controllerThread = new Thread(new ThreadStart(gamePadStateCollector));
            controllerThread.IsBackground = true;
            controllerThread.Start();
        }

        public static void gamePadStateCollector()
        {
            while (GlobalVariables.useControllerFastThread)
            {


                if (GlobalVariables.controller.IsConnected)
                {
                    GlobalVariables.gamepadCurrent = GlobalVariables.controller.GetState().Gamepad;
                    Thread.Sleep(sleepTimer);
                    while (GlobalVariables.useControllerFastThread && GlobalVariables.controller.IsConnected)
                    {
                        GlobalVariables.gamepadOld = GlobalVariables.gamepadCurrent;
                        GlobalVariables.gamepadCurrent = GlobalVariables.controller.GetState().Gamepad;
                        Thread.Sleep(sleepTimer);
                    }


                }
                else
                {
                    while (GlobalVariables.controller.IsConnected && GlobalVariables.useControllerFastThread)
                    {
                        Thread.Sleep(5000);
                        GlobalVariables.controller = new Controller(UserIndex.One);
                        if (GlobalVariables.controller.IsConnected == false)
                        {
                            GlobalVariables.controller = new Controller(UserIndex.Two);
                        }
                        if (GlobalVariables.controller.IsConnected == false)
                        {
                            GlobalVariables.controller = new Controller(UserIndex.Three);
                        }
                        if (GlobalVariables.controller.IsConnected == false)
                        {
                            GlobalVariables.controller = new Controller(UserIndex.Four);
                        }


                    }

                }
            }

         
        }

    }
}
