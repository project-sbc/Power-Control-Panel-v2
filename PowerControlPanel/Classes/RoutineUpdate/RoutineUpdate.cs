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
using System.Windows.Threading;

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

                Classes.ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();
                Classes.ChangeVolume.AudioManager.GetMasterVolume();
                count5 = 0;
            }
            else
            {
                count5++;
            }
        }

        public static DispatcherTimer timer = new DispatcherTimer();

        public static void startTimer()
        {


        }


    }
}
