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
        private Thread routineThread;

        private int counter = 0;

      
        private void handleRoutineChecks()
        {
            while (GlobalVariables.useRoutineThread)
            {

                ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();
                ChangeVolume.AudioManager.GetMasterVolume();
                ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings();
                if (counter > 10) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.readTDP()); counter = -1; }
                
                Thread.Sleep(1000);
         
                counter ++;
            }

        }



        public void startThread()
        {
            routineThread = new Thread(handleRoutineChecks);
            routineThread.IsBackground = true;
            routineThread.Name = "Dispatch background thread";
            routineThread.Start();

        }


    }
}
