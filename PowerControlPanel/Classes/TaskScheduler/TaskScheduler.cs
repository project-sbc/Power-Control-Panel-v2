using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows;

namespace Power_Control_Panel.PowerControlPanel.Classes.TDPTaskScheduler
{
    public class TDPTaskScheduler
    {
        public static SecretNest.TaskSchedulers.SequentialScheduler scheduler;
        public static Thread taskScheduler;
        public static TaskFactory taskFactory = new TaskFactory(scheduler);

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
   
            var result = taskFactory.StartNew(action);
       
            
        }

        public static void closeScheduler()
        {
            scheduler.Dispose();

        }
    }
}
