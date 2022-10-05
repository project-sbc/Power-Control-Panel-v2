using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibreHardwareMonitor.Hardware;
namespace Power_Control_Panel.PowerControlPanel.Classes.PIDandCPUMonitor
{
    public static class PIDCPUMonitor
    {
        public class UpdateVisitor : IVisitor
        {
            public void VisitComputer(IComputer computer)
            {
                computer.Traverse(this);
            }
            public void VisitHardware(IHardware hardware)
            {
                hardware.Update();
                foreach (IHardware subHardware in hardware.SubHardware) subHardware.Accept(this);
            }
            public void VisitSensor(ISensor sensor) { }
            public void VisitParameter(IParameter parameter) { }
        }

        public static Computer computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = false,
            IsMemoryEnabled = false,
            IsMotherboardEnabled = false,
            IsControllerEnabled = false,
            IsNetworkEnabled = false,
            IsStorageEnabled = false
        };
        public static void MonitorCPU()
        {
          

            
            computer.Open();
            foreach (IHardware hardware in computer.Hardware)
            {
                computer.Accept(new UpdateVisitor());
                Console.WriteLine("Hardware: {0}", hardware.Name);



                foreach (ISensor sensor in hardware.Sensors)
                {
                    string sense = sensor.SensorType.ToString().ToLower() + " " + sensor.Name.ToLower();

                    if (sense.Contains("temp"))
                    {
                        GlobalVariables.cpuTemp = Convert.ToDouble(sensor.Value);
                    }


                }



            }

            computer.Close();
        }

    }
}
