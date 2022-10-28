using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes.changeCPU
{
    public static class ChangeCPU
    {
        public static void readCPUMaxFrequency()
        {


            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = RunCLI.RunCommand(" -Q SCHEME_CURRENT sub_processor PROCFREQMAX", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string cpuFreq = "";
            int intCpuFreq = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    cpuFreq = resultArray[resultArray.Length - 2];
                    cpuFreq = cpuFreq.Replace("\r", "");
                    cpuFreq = cpuFreq.Replace(" ", "");
                    cpuFreq = cpuFreq.Substring(cpuFreq.IndexOf(":")+1,10).Trim();
                    intCpuFreq= Convert.ToInt32(cpuFreq, 16);
                }
                else
                {
                    cpuFreq = resultArray[resultArray.Length - 1];
                    cpuFreq = cpuFreq.Replace("\r", "");
                    cpuFreq = cpuFreq.Replace(" ", "");
                    cpuFreq = cpuFreq.Substring(cpuFreq.IndexOf(":") + 1, 10).Trim();
                    intCpuFreq = Convert.ToInt32(cpuFreq, 16);

                }
                GlobalVariables.cpuMaxFrequency = intCpuFreq;
            }

       
        }



        public static void readActiveCores()
        {


            string Power = SystemParameters.PowerLineStatus.ToString();
            string result = RunCLI.RunCommand(" -Q SCHEME_CURRENT sub_processor CPMAXCORES", true, "C:\\windows\\system32\\powercfg.exe", 1000).Trim();
            string maxCores = "";
            int intMaxCores = 0;
            string[] resultArray = result.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);


            if (resultArray.Length > 2)
            {
                if (Power == "Online")
                {
                    maxCores = resultArray[resultArray.Length - 2];
                    maxCores = maxCores.Replace("\r", "");
                    maxCores = maxCores.Replace(" ", "");
                    maxCores = maxCores.Substring(maxCores.IndexOf(":") + 1, 10).Trim();
                    intMaxCores = Convert.ToInt32(maxCores, 16);
                }
                else
                {
                    maxCores = resultArray[resultArray.Length - 1];
                    maxCores = maxCores.Replace("\r", "");
                    maxCores = maxCores.Replace(" ", "");
                    maxCores = maxCores.Substring(maxCores.IndexOf(":") + 1, 10).Trim();
                    intMaxCores = Convert.ToInt32(maxCores, 16);

                }
                double calculateCores =(intMaxCores * GlobalVariables.maxCpuCores) / 100;
                calculateCores = Math.Round(calculateCores, 0);
                
                GlobalVariables.cpuActiveCores = (int)calculateCores;
            }


        }
        public static void changeCPUMaxFrequency(int maxCPU)
        {
            
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                RunCLI.RunCommand(" /setacvalueindex scheme_current sub_processor PROCFREQMAX " + Convert.ToString(maxCPU), false, "C:\\windows\\system32\\powercfg.exe", 1000);
             
            }
            else
            {
                RunCLI.RunCommand(" /setdcvalueindex scheme_current sub_processor PROCFREQMAX " + Convert.ToString(maxCPU), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            
            }
            RunCLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);

            readCPUMaxFrequency();
            GlobalVariables.needCPUMaxFreqRead = false;

        }

        public static void changeActiveCores(double coreCount)
        {
            double CorePercentage = Math.Round(coreCount/GlobalVariables.maxCpuCores,2)*100;
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                RunCLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMAXCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                RunCLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMINCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            else
            {
                RunCLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMAXCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                RunCLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMINCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            RunCLI.RunCommand(" /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);
            readActiveCores();
            GlobalVariables.needActiveCoreRead = false;
        }



    }
}
