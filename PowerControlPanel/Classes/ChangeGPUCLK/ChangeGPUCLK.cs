using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes.ChangeGPUCLK
{
    public static class ChangeGPUCLK
    {
        private static Object objLock = new Object();

        public static void changeAMDGPUClock(int gpuclk)
        {
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;

            if (gpuclk >= 100 && GlobalVariables.cpuType =="AMD")
            {
                lock (objLock)
                {
                    string processRyzenAdj = "";
                    string result = "";
                    string commandArguments = "";


                    try
                    {
                        processRyzenAdj = BaseDir + "\\Resources\\AMD\\RyzenAdj\\ryzenadj.exe";

                        lock (objLock)
                        {
                            if (gpuclk > Properties.Settings.Default.maxGPUCLK) { gpuclk = Properties.Settings.Default.maxGPUCLK; }
                            commandArguments = " --gfx-clk=" + gpuclk.ToString();
                            //StreamWriterLog.startStreamWriter("Read TDP AMD processRyzenAj=" + processRyzenAdj + "; commandarugment=" + commandArguments);

                            result = RunCLI.RunCommand(commandArguments, false, processRyzenAdj);

                            GlobalVariables.gpuclk = gpuclk.ToString();

                        }



                    }
                    catch { }


                }
            }
        }


    }
}
