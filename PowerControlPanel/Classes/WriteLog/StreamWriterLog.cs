using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public static class StreamWriterLog
    {
        
        public static string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
        public static Object objLock = new Object();
        public static void startStreamWriter(string newLog)
        {
            try
            {
                lock (objLock)
                {
                    if (!File.Exists(BaseDir + "\\PowerControlPanel\\Logs\\application_log.txt")) { createLogFile(); }
                    using (StreamWriter w = File.AppendText(BaseDir + "\\PowerControlPanel\\Logs\\application_log.txt"))
                    {
                        Log(newLog, w);

                    }
                   
                }
            }

            catch (Exception ex)
            {
                
            }

         
        }
        public static void Log(string logMessage, TextWriter w)
        {
            try
            {
                w.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} {logMessage}");
                w.Flush();
            }
            catch { }
        }
        public static void createLogFile()
        {
            try
            {
                if (!Directory.Exists(BaseDir + "\\PowerControlPanel\\Logs")) { System.IO.Directory.CreateDirectory(BaseDir + "\\PowerControlPanel\\Logs"); }
                if (!File.Exists(BaseDir + "\\PowerControlPanel\\Logs\\application_log.txt")) { File.CreateText(BaseDir + "\\PowerControlPanel\\Logs\\application_log.txt"); }
            }
            catch { }
         
        }

    }
}
