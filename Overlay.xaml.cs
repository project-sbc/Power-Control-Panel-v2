using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using Power_Control_Panel.PowerControlPanel.PageComponents;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : MetroWindow
    {
        WindowSinker sinker;
        public DispatcherTimer updateTimer = new DispatcherTimer();
        public Overlay()
        {
            InitializeComponent();

            sinker = new WindowSinker(this);
            sinker.Sink();
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

            arrangeStackPanel();


            updateValues();



            //Set up timespan for timers
            updateTimer.Interval = new TimeSpan(0, 0, 5);

            //Add the event handlers to the timers
            updateTimer.Tick += tick_updateValues;

            //Start timers
            updateTimer.Start();
        }

        void tick_updateValues(object sender, EventArgs e)
        {
            updateValues();
        }
        void updateValues()
        {
            switch (GlobalVariables.internetDevice)
            {
                case "Not Connected":
                    txtblkInternet.Text = "\uF384";
                    break;
                case "Wireless":
                    txtblkInternet.Text = "\uE701";
                    break;
                case "Ethernet":
                    txtblkInternet.Text = "\uE839";
                    break;
                default:
                    txtblkInternet.Text = "";
                    break;
            }

            switch (GlobalVariables.powerStatus)
            {
                case "AC":
                    txtblkPower.Text = "";
                    break;
                case "Online":
                    if (GlobalVariables.batteryPercentage < 10 && GlobalVariables.batteryPercentage >= 0) { txtblkPower.Text = "\uE85A"; }
                    if (GlobalVariables.batteryPercentage < 20 && GlobalVariables.batteryPercentage >= 10) { txtblkPower.Text = "\uE85B"; }
                    if (GlobalVariables.batteryPercentage < 30 && GlobalVariables.batteryPercentage >= 20) { txtblkPower.Text = "\uE85C"; }
                    if (GlobalVariables.batteryPercentage < 40 && GlobalVariables.batteryPercentage >= 30) { txtblkPower.Text = "\uE85D"; }
                    if (GlobalVariables.batteryPercentage < 50 && GlobalVariables.batteryPercentage >= 40) { txtblkPower.Text = "\uE85E"; }
                    if (GlobalVariables.batteryPercentage < 60 && GlobalVariables.batteryPercentage >= 50) { txtblkPower.Text = "\uE85F"; }
                    if (GlobalVariables.batteryPercentage < 70 && GlobalVariables.batteryPercentage >= 60) { txtblkPower.Text = "\uE860"; }
                    if (GlobalVariables.batteryPercentage < 80 && GlobalVariables.batteryPercentage >= 70) { txtblkPower.Text = "\uE861"; }
                    if (GlobalVariables.batteryPercentage < 90 && GlobalVariables.batteryPercentage >= 80) { txtblkPower.Text = "\uE862"; }
                    if (GlobalVariables.batteryPercentage <= 100 && GlobalVariables.batteryPercentage >= 90) { txtblkPower.Text = "\uE83E"; }
                    txtblkBatteryPercentage.Text = GlobalVariables.batteryPercentage.ToString() + "%";   
                    break;
                case "Offline":
                    if (GlobalVariables.batteryPercentage < 10 && GlobalVariables.batteryPercentage >= 0) { txtblkPower.Text = "\uE850"; }
                    if (GlobalVariables.batteryPercentage < 20 && GlobalVariables.batteryPercentage >= 10) { txtblkPower.Text = "\uE851"; }
                    if (GlobalVariables.batteryPercentage < 30 && GlobalVariables.batteryPercentage >= 20) { txtblkPower.Text = "\uE852"; }
                    if (GlobalVariables.batteryPercentage < 40 && GlobalVariables.batteryPercentage >= 30) { txtblkPower.Text = "\uE853"; }
                    if (GlobalVariables.batteryPercentage < 50 && GlobalVariables.batteryPercentage >= 40) { txtblkPower.Text = "\uE854"; }
                    if (GlobalVariables.batteryPercentage < 60 && GlobalVariables.batteryPercentage >= 50) { txtblkPower.Text = "\uE855"; }
                    if (GlobalVariables.batteryPercentage < 70 && GlobalVariables.batteryPercentage >= 60) { txtblkPower.Text = "\uE856"; }
                    if (GlobalVariables.batteryPercentage < 80 && GlobalVariables.batteryPercentage >= 70) { txtblkPower.Text = "\uE857"; }
                    if (GlobalVariables.batteryPercentage < 90 && GlobalVariables.batteryPercentage >= 80) { txtblkPower.Text = "\uE858"; }
                    if (GlobalVariables.batteryPercentage < 100 && GlobalVariables.batteryPercentage >= 90) { txtblkPower.Text = "\uE859"; }
                    txtblkBatteryPercentage.Text = GlobalVariables.batteryPercentage.ToString() + "%";
                    break;
                default:
                    break;
            }

            if (GlobalVariables.controller is null) { txtblkGamepad.Text = ""; } else { if (GlobalVariables.controller.IsConnected) { txtblkGamepad.Text = "\uE7FC"; } else { txtblkGamepad.Text = ""; } }
        }

        private void HideOverlay_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - 380;
            this.Top = 0;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        void arrangeStackPanel()
        {
            Page[] page;
            Frame[] frame = new Frame[3];
            frame[0] = frame0;
            frame[1] = frame1;
            frame[2] = frame2;
            page = new Page[3];
            int i = 0;

            //if (Properties.Settings.Default.enableSystem) { page[i] = new HomeSystem(); i = i + 1; }
            if (Properties.Settings.Default.enableTDP) { page[i] = new HomeTDP(); i = i + 1; }
            //if (Properties.Settings.Default.enableCPU) { page[i] = new HomeCPU(); i = i + 1; }

            i = 0;
            while (page[i] is not null)
            {
                frame[i].Navigate(page[i]);
                i = i + 1;
            }

        }
    }
}
