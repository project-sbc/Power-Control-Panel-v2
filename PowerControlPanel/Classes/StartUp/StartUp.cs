using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;
using Microsoft.Win32;

namespace Power_Control_Panel.PowerControlPanel.Classes.StartUp
{
    public class StartUp
    {

        public static void runStartUp()
        {
            TaskScheduler.TaskScheduler.startScheduler();
            GlobalVariables.tdp.readTDP();
            if (GlobalVariables.readPL1 > 4000) { Properties.Settings.Default.IntelMMIOMSR = "MSR"; Properties.Settings.Default.Save(); GlobalVariables.tdp.readTDP(); }

            ChangeDisplaySettings.ChangeDisplaySettings.generateDisplayResolutionAndRateList();
            ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings();
            //ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();

            //Modify settings for CPU specific like gpu clock and intel power bal
            configureSettings();


        }

        private static void configureSettings()
        {
            string processorName = "";
            string cpuType = "";
            object processorNameRegistry = Registry.GetValue("HKEY_LOCAL_MACHINE\\hardware\\description\\system\\centralprocessor\\0", "ProcessorNameString", null);

            if (processorNameRegistry != null)
            {
                //If not null, find intel or AMD string and clarify type. For Intel determine MCHBAR for rw.exe
                processorName = processorNameRegistry.ToString();
                if (processorName.IndexOf("Intel") >= 0) { cpuType = "Intel"; }
                if (processorName.IndexOf("AMD") >= 0) { cpuType = "AMD"; }
                GlobalVariables.cpuType = cpuType;

            }

            if (cpuType == "Intel")
            {
                //Properties.Settings.Default.enableGPUCLK = false;

            }
            if (cpuType == "AMD")
            {
                Properties.Settings.Default.enableIntelPB = false;

            }

            Properties.Settings.Default.Save();

           
        }
    }
}
