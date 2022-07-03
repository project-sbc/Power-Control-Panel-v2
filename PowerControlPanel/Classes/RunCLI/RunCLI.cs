using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public static class RunCLI
    {

        public static string RunCommand(string arguments, bool readOutput, string processName = "cmd.exe")
        {
            //Runs CLI, if readOutput is true then returns output

            try
            {

                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (readOutput) { startInfo.RedirectStandardOutput = true; } else { startInfo.RedirectStandardOutput = false; }

                startInfo.FileName = processName;
                startInfo.Arguments = "/c " + arguments;
                startInfo.CreateNoWindow = true;    
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
                if (readOutput)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;

                }
                else
                {
                    process.WaitForExit();
                    return "COMPLETE";
                }


            }
            catch (Exception ex)
            {
                return "Error running CLI: " + ex.Message + " " + arguments;
            }


        }



      

        public static string RunHECommand(string arguments, bool readOutput, string processName = "cmd.exe")
        {
            //Runs CLI, if readOutput is true then returns output

            try
            {
                [DllImport("user32.dll")]
                static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

                [DllImport("user32.dll")]
                static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

                [DllImport("user32.dll")]
                static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

                const int GWL_STYLE = -16;
                const long WS_VISIBLE = 0x10000000;


                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.UseShellExecute = false;
                if (readOutput) { startInfo.RedirectStandardOutput = true; } else { startInfo.RedirectStandardOutput = false; }
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = processName;
                startInfo.Arguments =  arguments;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
      
   
                if (readOutput)
                {
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                    return output;

                }
                else
                {
                    process.WaitForExit();
                    return "COMPLETE";
                }


            }
            catch (Exception ex)
            {
                return "Error running CLI: " + ex.Message + " " + arguments;
            }


        }
    }
}
