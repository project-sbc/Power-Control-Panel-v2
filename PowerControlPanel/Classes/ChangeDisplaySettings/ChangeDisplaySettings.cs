using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes.ChangeDisplaySettings
{
    public static class ChangeDisplaySettings
    {
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        private static Object objLock = new Object();
        public static void getCurrentDisplaySettings()
        {


            string commandArguments = " /S";
            string result = QResCLIResult(commandArguments);
            try
            {
                               
                if (result != null)
                {
                    int findStartStringResolution = result.IndexOf("Kjersem.")+8;
                    int findEndStringResolution = result.IndexOf(",") -1;
                    string displayResolution = result.Substring(findStartStringResolution, 1+findEndStringResolution - findStartStringResolution).Trim();

                    //If list contains the resolution then display it, else dont change it - scaling was changed which caused resolution to be non standard
                    if (GlobalVariables.resolutions.Contains(displayResolution))
                    {
                        GlobalVariables.resolution = displayResolution;
                    }

                    int findStartStringRate = result.IndexOf("@") + 1;
                    int findEndStringRate = result.IndexOf("Hz") - 2;

                    string displayRate = result.Substring(findStartStringRate, 1 + findEndStringRate - findStartStringRate).Trim();

                    GlobalVariables.refreshRate = displayRate;
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Get current display settings: " + ex.Message + " Result: " + result;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);

            }





        }
        private static string SetDPICLIResult(string commandArguments)
        {
            string result = "";
            string processSDPI = BaseDir + "\\Resources\\SetDPI\\SetDPI.exe";

            try
            {
                lock (objLock)
                {
                    result = RunCLI.RunCommand(commandArguments, true, processSDPI);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  set DPI CLI: " + ex.Message + " Result: " + result;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
            }

            return result;

        }
        private static string QResCLIResult(string commandArguments)
        {
            string result = "";
            string processQRes = BaseDir + "\\Resources\\QRes\\QRes.exe";

            try
            {
                lock (objLock)
                {
                    Thread.Sleep(100);
                    result = RunCLI.RunCommand(commandArguments, true, processQRes,1000);
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Get current display settings: " + ex.Message + " Result: " + result;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);
            }

            return result;

        }

        public static void generateDisplayResolutionAndRateList()
        {
            GlobalVariables.resolutions.Clear();
            GlobalVariables.refreshRates.Clear();
            string commandArguments = " /L";
            string result = QResCLIResult(commandArguments);

            GlobalVariables.resolutions.Add("Custom Scaling");

            string resolution;
            string refreshrate;

            using (StringReader reader = new StringReader(result))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.IndexOf("@") > 0)
                    {
                        resolution = line.Substring(0, line.IndexOf(",")).Trim();
                        refreshrate = line.Substring(line.IndexOf("@") + 1, 4).Trim();
                        if (!GlobalVariables.resolutions.Contains(resolution)) { GlobalVariables.resolutions.Add(resolution); }
                        if (!GlobalVariables.refreshRates.Contains(refreshrate)) { GlobalVariables.refreshRates.Add(refreshrate); }
                    }
                }
            }

            GlobalVariables.scalings.Clear();
            GlobalVariables.scalings.Add("Default");
            GlobalVariables.scalings.Add("100");
            GlobalVariables.scalings.Add("125");
            GlobalVariables.scalings.Add("150");
            GlobalVariables.scalings.Add("175");
            GlobalVariables.scalings.Add("200");
            GlobalVariables.scalings.Add("225");
        }


        public static void SetDisplayResolution(string resolution)
        {
            string resolutionX = resolution.Substring(0, resolution.IndexOf("x"));
            string resolutionY = resolution.Substring(resolution.IndexOf("x")+1, resolution.Length - (1+resolution.IndexOf("x")));
            string commandArguments = " /X " + resolutionX + " /Y " + resolutionY;
            string result = "";

            try
            {
                result = QResCLIResult(commandArguments);
                Thread.Sleep(200);
                getCurrentDisplaySettings();
            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Set display resolution: " + ex.Message + " Result: " + result;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);

            }





        }

        public static void SetDisplayScaling(string scaling)
        {
            if (scaling != "Default")
            {
                string commandArguments = " " + scaling;
                string result = "";

                try
                {
                    result = SetDPICLIResult(commandArguments);
                    Thread.Sleep(200);
                    GlobalVariables.scaling = scaling;
                }
                catch (Exception ex)
                {
                    string errorMsg = "Error: ChangeDisplaySettings.cs:  Set display resolution: " + ex.Message + " Result: " + result;
                    StreamWriterLog.startStreamWriter(errorMsg);
                    MessageBox.Show(errorMsg);

                }


            }




        }
        public static void SetDisplayRefreshRate(string refresh)
        {
     
            string commandArguments = " /R " + refresh;
            string result = "";
            try
            {
                result = QResCLIResult(commandArguments);
                Thread.Sleep(200);
                getCurrentDisplaySettings();

            }
            catch (Exception ex)
            {
                string errorMsg = "Error: ChangeDisplaySettings.cs:  Set refresh rate: " + ex.Message + " Result: " + result;
                StreamWriterLog.startStreamWriter(errorMsg);
                MessageBox.Show(errorMsg);

            }





        }
    }
}
