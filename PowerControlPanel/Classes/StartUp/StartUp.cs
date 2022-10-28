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
using System.IO;

namespace Power_Control_Panel.PowerControlPanel.Classes.StartUp
{
    public class StartUp
    {

        public static void runStartUp()
        {
            if (Properties.Settings.Default.upgradeSettingsRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.upgradeSettingsRequired = false;
                Properties.Settings.Default.Save();

                if (Properties.Settings.Default.systemAutoStart == "Enable")
                {
                    PowerControlPanel.Classes.TaskSchedulerWin32.TaskSchedulerWin32.changeTaskService("Enable");
                }
            }

            //if first run of app, then make the profile
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\PowerControlPanel\\ProfileData\\Profiles.xml")==false)
            {
                File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\PowerControlPanel\\ProfileData\\Profiles_Template.xml", AppDomain.CurrentDomain.BaseDirectory + "\\PowerControlPanel\\ProfileData\\Profiles.xml");
            }

            //set game launcher directories
            PowerControlPanel.Classes.Steam.Steam.setSteamDirectory();
            PowerControlPanel.Classes.Playnite.Playnite.setPlayniteDirectory();


            //start dedicated task scheduler for background tdp, cpu, etc changes
            TaskScheduler.TaskScheduler.startScheduler();
            //read tdp
            GlobalVariables.tdp.readTDP();

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;

          
            //set max cpu cores
            GlobalVariables.maxCpuCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
            //get manufacturer info (for OXP fan control)
            GlobalVariables.manufacturer = PowerControlPanel.Classes.MotherboardInfo.MotherboardInfo.Manufacturer.ToUpper();
            GlobalVariables.product = PowerControlPanel.Classes.MotherboardInfo.MotherboardInfo.Product.ToUpper();


            //generate lists for combo boxes across the app such as display resolutions, refresh rates and fps limits (RTSS)
            ChangeDisplaySettings.ChangeDisplaySettings.generateDisplayResolutionAndRateList();
            ChangeDisplaySettings.ChangeDisplaySettings.getCurrentDisplaySettings();
            ChangeFPSLimit.ChangeFPSLimit.setFPSLimits();
            ChangeFanSpeedOXP.ChangeFanSpeed.generateFanControlModeList();

            //read current cpu max frequency and active cores.        
            changeCPU.ChangeCPU.readCPUMaxFrequency();
            changeCPU.ChangeCPU.readActiveCores();

            //Modify settings for CPU specific like gpu clock and intel power bal
            configureSettings();


            //adjust base clock speed
            int baseClockSpeed = new ManagementObjectSearcher("select MaxClockSpeed from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["MaxClockSpeed"].ToString()));
            double roundClockSpeed = Math.Round(Convert.ToDouble(baseClockSpeed) / 100, 0) * 100;
            GlobalVariables.baseCPUSpeed = (int)roundClockSpeed;
            GlobalVariables.powerStatus = SystemParameters.PowerLineStatus.ToString();
            PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.applyProfile("Default");

            //unhide core parking from power plan
            RunCLI.RunCommand(" -attributes SUB_PROCESSOR CPMAXCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            RunCLI.RunCommand(" -attributes SUB_PROCESSOR CPMINCORES -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            RunCLI.RunCommand(" -attributes SUB_PROCESSOR PROCFREQMAX -ATTRIB_HIDE", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            
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
                    //PIDandCPUMonitor.PIDCPUMonitor.MonitorCPU();
                    
                }
            }

             //set language resource
            switch (Properties.Settings.Default.Language)
            {
                default:
                case "English":
                    GlobalVariables.languageDict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.xaml", UriKind.RelativeOrAbsolute);
                    break;
              
                case "中文":
                    GlobalVariables.languageDict.Source = new Uri("PowerControlPanel/Classes/StartUp/Resources/StringResources.zh-Hans.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }
            Application.Current.Resources.MergedDictionaries.Add(GlobalVariables.languageDict);



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
