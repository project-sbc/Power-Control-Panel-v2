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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Power_Control_Panel.PowerControlPanel.PageComponents
{
    /// <summary>
    /// Interaction logic for HomeCPU.xaml
    /// </summary>
    public partial class HomeCPU : Page
    {
        private bool dragStartedUPC = true;
        private bool dragStartedMCPU = true;
        
        public HomeCPU()
        {
            InitializeComponent();
        }
        private void MCPU_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedMCPU)
            {
                //add handler
            }
        }

        private void MCPU_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedMCPU = false;
            //add handler
        }

        private void MCPU_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedMCPU = true;
        }

        private void MCPU_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedMCPU = false;
            //HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void UPC_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedUPC)
            {
               //UPC handler

            }
        }
        private void UPC_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedUPC = false;
           // HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }

        private void UPC_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedUPC = true;
        }


        private void UPC_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedUPC = false;
            //HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
      
    }
}
