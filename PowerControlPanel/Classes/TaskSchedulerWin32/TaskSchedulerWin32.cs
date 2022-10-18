using Microsoft.Win32.TaskScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Power_Control_Panel.PowerControlPanel.Classes.TaskSchedulerWin32
{
    public static class TaskSchedulerWin32
    {
        public static void changeTaskService(string systemAutoStart)
        {
            Microsoft.Win32.TaskScheduler.TaskService ts = new Microsoft.Win32.TaskScheduler.TaskService();
            Microsoft.Win32.TaskScheduler.Task task = ts.GetTask("Power_Control_Panel");
            string BaseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (task == null)
            {
                if (systemAutoStart == "Enable")
                {
                    TaskDefinition td = ts.NewTask();

                    td.RegistrationInfo.Description = "Power Control Panel";
                    td.Triggers.AddNew(TaskTriggerType.Logon);
                    td.Principal.RunLevel = TaskRunLevel.Highest;
                    td.Settings.DisallowStartIfOnBatteries = false;
                    td.Settings.StopIfGoingOnBatteries = false;
                    td.Settings.RunOnlyIfIdle = false;

                    td.Actions.Add(new ExecAction(BaseDir + "\\Power Control Panel.exe"));

                    Microsoft.Win32.TaskScheduler.TaskService.Instance.RootFolder.RegisterTaskDefinition("Power_Control_Panel", td);

                }
            }

            else
            {
                if (systemAutoStart == "Disable")
                {
                    task.RegisterChanges();
                    ts.RootFolder.DeleteTask("Power_Control_Panel");
                }
            }

        }
    }
}
