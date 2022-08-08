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
using System.Diagnostics;

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

                //ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();
                //ChangeVolume.AudioManager.GetMasterVolume();
                //ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings();
                //changeCPU.ChangeCPU.readCPUMaxFrequency();
                //changeCPU.ChangeCPU.readActiveCores();

   
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings());
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeVolume.AudioManager.GetMasterVolume());
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeBrightness.WindowsSettingsBrightnessController.getBrightness());

                if (counter > 10) { 
                    
                    Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.readTDP()); 
                    
                    Classes.TaskScheduler.TaskScheduler.runTask(() => changeCPU.ChangeCPU.readActiveCores());
                    Classes.TaskScheduler.TaskScheduler.runTask(() => changeCPU.ChangeCPU.readCPUMaxFrequency());
                    counter = -1;
                }
                
                Thread.Sleep(1000);
         
                counter ++;
            }

        }
        private void runProfileChecker()
        {

            Process[] pList = Process.GetProcesses();

            foreach (Process p in pList)
            {
                

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
