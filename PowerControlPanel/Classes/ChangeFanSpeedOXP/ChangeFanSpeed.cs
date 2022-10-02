using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Threading;

namespace Power_Control_Panel.PowerControlPanel.Classes.ChangeFanSpeedOXP
{
    public static class ChangeFanSpeed
    {
        public static void readSoftwareFanControl()
        {
            string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
            string argument = " -winring0 -r 0x4A";

            string result = "";

            result = RunCLI.RunCommand(argument, true, processEC, 2000);

            if (result != null)
            {
                if (result.Contains("0x00"))
                {
                    GlobalVariables.fanControlEnable = false;
                    GlobalVariables.fanControlMode = "Hardware";
                }
                else 
                { 
                    GlobalVariables.fanControlEnable = true;
                    GlobalVariables.fanControlMode = "Manual";

                }


            }


        }
        public static void readFanSpeed()
        {
            try
            {
                string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
                string argument = " -winring0 -r 0x4B";

                string result = "";

                result = RunCLI.RunCommand(argument, true, processEC, 2000);

                if (result != null)
                {
                    result = result.Replace("\r", "").Trim();
                    result = result.Replace("\n", "").ToUpper();
                    int decValue = Convert.ToInt32(result, 16);

                    double fanPercentage = Math.Round(100 * ((double)decValue / (double)GlobalVariables.fanRangeBase), 0);
                    GlobalVariables.fanSpeed = (int)fanPercentage;
                }
            }
            catch { 
            
            }

        }
        public static void generateFanControlModeList()
        {

            GlobalVariables.FanModes.Add("Hardware");
            GlobalVariables.FanModes.Add("Manual");
      

        }
        public static void enableSoftwareFanControl()
        {
            string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
            string argument = " -winring0 -w 0x4A 0x01";

            string result = "";

            result = RunCLI.RunCommand(argument, false, processEC, 2000);
            Thread.Sleep(400);
            readSoftwareFanControl();
        }
        public static void disableSoftwareFanControl()
        {
            string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
            string argument = " -winring0 -w 0x4A 0x00";

            string result = "";

            result = RunCLI.RunCommand(argument, false, processEC, 2000);
            readSoftwareFanControl();
        }
        public static void setFanSpeed(int fanSpeed)
        {
            if (GlobalVariables.fanControlEnable)
            {
                string processEC = AppDomain.CurrentDomain.BaseDirectory + "\\Resources\\EC\\EC.exe";
                string hexValue = "";
                if (GlobalVariables.fanRangeBase == 100)
                {
                    hexValue = fanSpeed.ToString("X");
                }
                else
                {
                    double normalizedFanSpeed = Math.Round(((double)fanSpeed / 100) * 255,0);
                    int fanspeedInt = (int)normalizedFanSpeed;
                    hexValue = fanspeedInt.ToString("X");
                }
                
                string argument = " -winring0 -w 0x4B " + hexValue;



                string result = "";

                result = RunCLI.RunCommand(argument, false, processEC, 2000);
                readFanSpeed();
            }

        }

      
    }

  



}
