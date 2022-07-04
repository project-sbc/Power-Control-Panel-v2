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
    public partial class HomeSystem : Page
    {
        private bool dragStartedBrightness = true;
     
    
        public HomeSystem()
        {
            InitializeComponent();
        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedBrightness )
            {
                HandleChangingBrightness((int)Brightness.Value);
            }
        }
        private void Brightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((int)Brightness.Value);
        }
        private void Brightness_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedBrightness = true;
        }
        private void Brightness_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((int)Brightness.Value);
        }
        private void Brightness_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((int)Brightness.Value);
        }
        private void Brightness_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedBrightness = true;
        }
     
        private void enableControl_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControl.IsOn)
            {
                this.Height = 100;
            }
            else { this.Height = 40; }
        }
        void HandleChangingBrightness(int birghtness)
        {

        }
    }
}
