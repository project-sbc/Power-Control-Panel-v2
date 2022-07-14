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
        private bool dragStartedVolume = true;

        public HomeSystem()
        {
            InitializeComponent();
            handleVisibility();
        }
        void handleVisibility()
        {
            if (Properties.Settings.Default.showSystem)
            { enableControl.IsOn = true; }
            else { enableControl.IsOn = false; }
        }

        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedBrightness )
            {
                HandleChangingBrightness((short)Brightness.Value);
            }
        }
        private void Brightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((short)Brightness.Value);
        }
        private void Brightness_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedBrightness = true;
        }
        private void Brightness_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((short)Brightness.Value);
        }
        private void Brightness_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness((short)Brightness.Value);
        }
        private void Brightness_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedBrightness = true;
        }
     
        private void enableControl_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControl.IsOn)
            {
                this.Height = 150;
            }
            else { this.Height = 40; }
        }
        void HandleChangingBrightness(short brightness)
        {
            
        }
        void HandleChangingVolume(int birghtness)
        {

        }
        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedVolume)
            {
                HandleChangingVolume((int)Volume.Value);
            }
        }
        private void Volume_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedVolume = false;
            HandleChangingVolume((int)Volume.Value);
        }
        private void Volume_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedVolume = true;
        }
        private void Volume_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedVolume = false;
            HandleChangingVolume((int)Volume.Value);
        }
        private void Volume_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedVolume = false;
            HandleChangingVolume((int)Volume.Value);
        }
        private void Volume_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedVolume = true;
        }



    }
}
