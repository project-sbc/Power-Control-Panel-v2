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
            Page tdpPage = new HomeTDP();
            Page cpuPage = new HomeCPU();
            tdpFrame.Navigate(tdpPage);
            cpuFrame.Navigate(cpuPage);

            
        }
     
 
    }
}
