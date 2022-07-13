using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes.ChangeBrightness
{
    public class WindowsSettingsBrightnessController
    {
       public static void getBrightness()
        {

            ManagementScope scope;
            SelectQuery query;

            scope = new ManagementScope("root\\WMI");
            query = new SelectQuery("SELECT * FROM WmiMonitorBrightness");

            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (ManagementObject mObj in objectCollection)
                    {
                        Console.WriteLine(mObj.ClassPath);
                        foreach (var item in mObj.Properties)
                        {
                            Console.WriteLine(item.Name + " " + item.Value.ToString());
                            if (item.Name == "CurrentBrightness")
                            { GlobalVariables.brightness = Convert.ToInt32(item.Value); }    //Do something with CurrentBrightness
        }
                    }
                }
            }
        }
    }
}
