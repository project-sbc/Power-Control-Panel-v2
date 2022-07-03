using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Power_Control_Panel.PowerControlPanel.Classes.TDPTaskScheduler;

namespace Power_Control_Panel.PowerControlPanel.Classes.StartUp
{
    public class StartUp
    {

        public static void runStartUp()
        {
            TDPTaskScheduler.TDPTaskScheduler.startScheduler();
            ChangeTDP.ChangeTDP.readTDP();
        }
    }
}
