using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;

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
       
        }
    }
}
