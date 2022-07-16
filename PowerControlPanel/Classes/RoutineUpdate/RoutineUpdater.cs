using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate
{
    public class ParameterHandler
    {
        private string _NetworkStatus;
        private string _PowerStatus;
        private string _BatteryLevel;
        //#1
        public event System.EventHandler NetworkStatusChanged;
        public event System.EventHandler PowerStatusChanged;
        public event System.EventHandler BatteryLevelChanged;
        //#2
        protected virtual void OnNetworkStatusChanged()
        {
            if (NetworkStatusChanged != null) NetworkStatusChanged(this, EventArgs.Empty);
        }

        public string NetworkStatus
        {
            get
            {
                return _NetworkStatus;
            }

            set
            {
                //#3
                _NetworkStatus = value;
                OnNetworkStatusChanged();
            }
        }

        public void checkNetworkInterface()
        {

            //Gets internet status to display on overlay
            NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            bool connectedDevice = false;
            foreach (NetworkInterface networkCard in networkCards)
            {
                if (networkCard.OperationalStatus == OperationalStatus.Up)
                {
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Ethernet) { if (NetworkStatus != "Ethernet") { NetworkStatus = "Ethernet"; }; connectedDevice = true; }
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) { if (NetworkStatus != "Wireless") { NetworkStatus = "Wireless"; }; connectedDevice = true; }
                }


            }
            if (!connectedDevice && NetworkStatus != "Not Connected") { NetworkStatus = "Not Connected"; }
        }



        protected virtual void OnPowerStatusChanged()
        {
            if (PowerStatusChanged != null) PowerStatusChanged(this, EventArgs.Empty);
        }

        public string PowerStatus
        {
            get
            {
                return _PowerStatus;
            }

            set
            {
                //#3
                _PowerStatus = value;
                OnPowerStatusChanged();
            }
        }

        protected virtual void OnBatteryLevelChanged()
        {
            if (BatteryLevelChanged != null) BatteryLevelChanged(this, EventArgs.Empty);
        }

        public string BatteryLevel
        {
            get
            {
                return _BatteryLevel;
            }

            set
            {
                //#3
                _BatteryLevel = value;
                OnPowerStatusChanged();
            }
        }


        public void checkPowerStatus()
        {

            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            string returnValue = "AC";
            foreach (ManagementObject mo in mos.Get())
            {
                returnValue = mo["EstimatedChargeRemaining"].ToString();
            }
            if (returnValue != "AC")
            {
                if (BatteryLevel != returnValue) { BatteryLevel = returnValue; };
                PowerLineStatus Power = SystemParameters.PowerLineStatus;
                if (PowerStatus != Power.ToString()) { PowerStatus = Power.ToString(); };

            }
            else { if (PowerStatus != "AC") { PowerStatus = "AC"; } }


        }


        public void startTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += timerTick;
            timer.Start();
        }
        public void timerTick(object sender, EventArgs e)
        {
            checkNetworkInterface();

        }
    }
}
