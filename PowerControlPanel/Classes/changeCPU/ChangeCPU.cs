using System;
using System.Collections.Generic;
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


        }



        public static void readActiveCores()
        {


        }
        public static void changeCPUMaxFrequency()
        {


        }

        public static void changeActiveCores(double coreCount)
        {
            double CorePercentage = Math.Round(coreCount/Environment.ProcessorCount,2)*100;
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power == "Online")
            {
                RunCLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMAXCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                RunCLI.RunCommand(" /setacvalueindex scheme_current sub_processor CPMINCORES " + Convert.ToString(CorePercentage), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            else
            {
                RunCLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMAXCORES " + CorePercentage.ToString(), false, "C:\\windows\\system32\\powercfg.exe", 1000);
                RunCLI.RunCommand(" /setdcvalueindex scheme_current sub_processor CPMINCORES " + CorePercentage.ToString(), false, "C:\\windows\\system32\\powercfg.exe", 1000);
            }
            RunCLI.RunCommand("   /S scheme_current", false, "C:\\windows\\system32\\powercfg.exe", 1000);

        }



    }
}
