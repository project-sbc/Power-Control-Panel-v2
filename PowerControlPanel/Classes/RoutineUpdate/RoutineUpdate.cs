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
using System.Reflection.Metadata;
using RTSSSharedMemoryNET;

namespace Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate
{
    public class RoutineUpdate
    {
        private Thread routineThread;
        private Thread osdThread;
        private int counter = 0;

      
        private void handleRoutineChecks()
        {
            while (GlobalVariables.useRoutineThread)
            {



   
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings());
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeVolume.AudioManager.GetMasterVolume());
                Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeBrightness.WindowsSettingsBrightnessController.getBrightness());

                if (GlobalVariables.fanControlDevice)
                {
                    Classes.TaskScheduler.TaskScheduler.runTask(() => ChangeFanSpeedOXP.ChangeFanSpeed.readFanSpeed());
                    
                }
                if (counter%2 == 0)
                {
                    //Classes.TaskScheduler.TaskScheduler.runTask(() => PIDandCPUMonitor.PIDCPUMonitor.MonitorCPU());

                }

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
        private void handleOSD()
        {
            Process[] pname = Process.GetProcessesByName("rtss");
            if (pname.Length != 0)
            {
                var osd = new OSD("PowerControlPanel");
                while (GlobalVariables.useRoutineThread)
                {
                    osd.Update(getOsdLine());
                    Thread.Sleep(1000);
                }
            }
    
          

        }

        public void startThread()
        {
            routineThread = new Thread(handleRoutineChecks);
            routineThread.IsBackground = true;
            routineThread.Name = "Dispatch background thread";
            routineThread.Start();
            //osdThread = new Thread(handleOSD);
            //osdThread.IsBackground = true;
            //osdThread.Name = "Dispatch OSD thread";
            //osdThread.Start();
        }
        string getOsdLine()
        {
            var tdps = $"<C0>TDP<S0>S<S><C> <A0>{GlobalVariables.readPL1}<A><A1><S1> W<S><A>";
            var tdpb = $"<C0>TDP<S0>B<S><C> <A0>{GlobalVariables.readPL2}<A><A1><S1> W<S><A>";
            var gpuclk = $"<C0>GPU<S0>CLK<S><C> <A0>{GlobalVariables.gpuclk}<A><A1><S1> W<S><A>";

            var rr = $"<C0>RR<C>  <A0>{GlobalVariables.refreshRate}<A><A1><S1> Hz<S><A>";

            string[] osdArr = { tdps, tdpb };

            if (GlobalVariables.refreshRate != "")
            {
                osdArr.Append(rr);
            }
            if (GlobalVariables.gpuclk != "")
            {
                osdArr.Append(gpuclk);
            }
            return String.Join("\n", osdArr);
        }


    }
}
