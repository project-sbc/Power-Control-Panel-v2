using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public class StreamWriterLog
    {
        
        public static void startStreamWriter(string newLog)
        {
            using (StreamWriter w = File.AppendText("PowerControlPanel/Logs/application_log.txt"))
            {
               Log(newLog,w);
            }

         
        }
        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine($"{DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()} {logMessage}");
            w.Flush();
        }

    }
}
