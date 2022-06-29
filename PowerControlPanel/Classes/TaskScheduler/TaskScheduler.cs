using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Power_Control_Panel.PowerControlPanel.Classes.TDPTaskScheduler
{
    public class TDPTaskScheduler
    {
        public static SecretNest.TaskSchedulers.SequentialScheduler scheduler = new SecretNest.TaskSchedulers.SequentialScheduler();
        public static Thread taskScheduler;

        public static void startScheduler()
        {
            scheduler = new SecretNest.TaskSchedulers.SequentialScheduler(true);

            taskScheduler = new Thread(ThreadTDPHandler);
            taskScheduler.Start();
        }

        public static void ThreadTDPHandler()
        {
            

            scheduler.Run(); //This will block this thread until the scheduler disposed.
        }
        public static void runTask(Action action)
        {
            var taskFactory = new TaskFactory(scheduler);
            var result = taskFactory.StartNew(action);
            Debug.WriteLine("Ran action ");
        }

    }
}
