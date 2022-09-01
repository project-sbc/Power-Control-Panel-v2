using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;
using Power_Control_Panel.PowerControlPanel.Classes;
using Microsoft.Win32;
using System.Management;
using System.Windows.Input;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes.StartUp
{
    public class StartUp
    {

        public static void runStartUp()
        {
            TaskScheduler.TaskScheduler.startScheduler();
            GlobalVariables.tdp.readTDP();

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;

            //set max cpu cores
            GlobalVariables.maxCpuCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
            GlobalVariables.manufacturer = PowerControlPanel.Classes.MotherboardInfo.MotherboardInfo.Manufacturer.ToUpper();
            GlobalVariables.product = PowerControlPanel.Classes.MotherboardInfo.MotherboardInfo.Product.ToUpper();

            ChangeDisplaySettings.ChangeDisplaySettings.generateDisplayResolutionAndRateList();
            ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings();
            //ChangeBrightness.WindowsSettingsBrightnessController.getBrightness();
            changeCPU.ChangeCPU.readCPUMaxFrequency();
            changeCPU.ChangeCPU.readActiveCores();
            //Modify settings for CPU specific like gpu clock and intel power bal
            configureSettings();


            //adjust base clock speed
            int baseClockSpeed = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["MaxClockSpeed"].ToString()));
            double roundClockSpeed = Math.Round(Convert.ToDouble(baseClockSpeed) / 100, 0) * 100;
            GlobalVariables.baseCPUSpeed = (int)roundClockSpeed;

            //unhide core parking from power plan
            RunCLI.RunCommand(" -attributes SUB_PROCESSOR CPMAXCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            RunCLI.RunCommand(" -attributes SUB_PROCESSOR CPMINCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            //check if device is one netbook one x player for fan control capability
            if (GlobalVariables.manufacturer.Contains("ONE") && GlobalVariables.manufacturer.Contains("NETBOOK"))
            {
                if (GlobalVariables.product.Contains("ONE") && GlobalVariables.product.Contains("X") && GlobalVariables.product.Contains("PLAYER"))
                {
                    GlobalVariables.fanControlDevice = true;
                    if (GlobalVariables.cpuType == "Intel") { GlobalVariables.fanRangeBase = 255; }
                    if (GlobalVariables.cpuType == "AMD") { GlobalVariables.fanRangeBase = 100; }
                    ChangeFanSpeedOXP.ChangeFanSpeed.readSoftwareFanControl();
                    ChangeFanSpeedOXP.ChangeFanSpeed.readFanSpeed();
                    PIDandCPUMonitor.PIDCPUMonitor.MonitorCPU();
                    
                }
            }

            string language = Properties.Settings.Default.Language;
            

            ResourceDictionary dict = new ResourceDictionary();
            switch (language.ToLower())
            {
                default:
                case "een":
                    dict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.xaml", UriKind.RelativeOrAbsolute);
                    break;
              
                case "en":
                    dict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.zh-Hans.xaml", UriKind.Relative);
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Add(dict);



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
                Properties.Settings.Default.enableGPUCLK = false;

            }
            if (cpuType == "AMD")
            {
                Properties.Settings.Default.enableIntelPB = false;

            }

            Properties.Settings.Default.Save();

           
        }
    }
}
