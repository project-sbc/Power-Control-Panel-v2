using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public static class RunCLI
    {

        public static string RunCommand(string arguments, bool readOutput, string processName = "cmd.exe",  int waitExit=6000, bool runasadmin=true)
        {
            //Runs CLI, if readOutput is true then returns output

            try
            {

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (readOutput) { startInfo.RedirectStandardOutput = true; } else { startInfo.RedirectStandardOutput = false; }

                startInfo.FileName = processName;
                //startInfo.Arguments = "/c " + arguments;
                startInfo.Arguments = arguments;
                startInfo.CreateNoWindow = true;
                if (runasadmin) { startInfo.Verb = "runas"; }
                startInfo.RedirectStandardError = readOutput;
                startInfo.RedirectStandardOutput = readOutput;
                process.EnableRaisingEvents = true;
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit(7000);
                if (readOutput)
                {
                    //int Errorlevel = process.ExitCode;
                    
                    string output = process.StandardOutput.ReadToEnd();
                    process.Close();
                    return output;

                }
                else
                {
                    process.Close();
                    return "COMPLETE";
                }


            }
            catch (Exception ex)
            {
                string error = "Error running CLI: " + ex.Message + " " + arguments; 
                
                return error;
                
            }


        }


    }
}
