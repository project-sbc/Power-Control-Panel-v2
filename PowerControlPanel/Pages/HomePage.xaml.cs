using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Power_Control_Panel.PowerControlPanel.PageComponents;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
      
        public HomePage()
        {
            InitializeComponent();
            arrangeStackPanel();
            

            
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

            if (Properties.Settings.Default.enableSystem) { page[i] = new HomeSystem(); i = i + 1; }
            if (Properties.Settings.Default.enableTDP)  {page[i] = new HomeTDP(); i = i + 1; }
            if (Properties.Settings.Default.enableCPU) { page[i] = new HomeCPU(); i = i + 1; }

            i = 0;
            while (page[i] is not null)
            {
                frame[i].Navigate(page[i]);
                i=i + 1;
            }

        }
 
    }
}
