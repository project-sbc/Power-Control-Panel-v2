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
    /// Interaction logic for HomeSystem.xaml
    /// </summary>
    public partial class HomePowerBalance : Page
    {
        private bool dragStartedCPU = true;
        private bool dragStartedGPU = true;

        public HomePowerBalance()
        {
            InitializeComponent();
            handleVisibility();
        }
        void handleVisibility()
        {
            if (Properties.Settings.Default.showIntlPB)
            { enableControl.IsOn = true; }
            else { enableControl.IsOn = false; }
        }

        private void CPU_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedCPU)
            {
                HandleChangingCPU((int)CPU.Value);
            }
        }
        private void CPU_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedCPU = false;
            HandleChangingCPU((int)CPU.Value);
        }
        private void CPU_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedCPU = true;
        }
        private void CPU_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedCPU = false;
            HandleChangingCPU((int)CPU.Value);
        }
        private void CPU_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedCPU = false;
            HandleChangingCPU((int)CPU.Value);
        }
        private void CPU_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedCPU = true;
        }

        private void enableControl_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControl.IsOn)
            {
                this.Height = 150;
            }
            else { this.Height = 40; }
        }
        void HandleChangingCPU(int intCPU)
        {

        }




        //-------------------
        private void GPU_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedGPU)
            {
                HandleChangingGPU((int)GPU.Value);
            }
        }
        private void GPU_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedGPU = false;
            HandleChangingGPU((int)GPU.Value);
        }
        private void GPU_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedGPU = true;
        }
        private void GPU_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedGPU = false;
            HandleChangingGPU((int)GPU.Value);
        }
        private void GPU_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedGPU = false;
            HandleChangingGPU((int)CPU.Value);
        }
        private void GPU_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedGPU = true;
        }

    
        void HandleChangingGPU(int intGPU)
        {

        }
    }
}
