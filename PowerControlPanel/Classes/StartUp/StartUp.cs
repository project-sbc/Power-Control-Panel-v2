using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;

namespace Power_Control_Panel.PowerControlPanel.Classes.StartUp
{
    public class StartUp
    {

        public static void runStartUp()
        {
            TaskScheduler.TaskScheduler.startScheduler();
            ChangeTDP.ChangeTDP.readTDP();
        }
    }
}
