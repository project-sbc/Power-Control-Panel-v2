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
            if (Properties.Settings.Default.enableTDP) { page[i] = new OverlayTDP(); i = i + 1; }
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
