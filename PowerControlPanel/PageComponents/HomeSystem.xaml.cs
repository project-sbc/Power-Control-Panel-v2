using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace Power_Control_Panel.PowerControlPanel.PageComponents
{
    /// <summary>
    /// Interaction logic for HomeSystem.xaml
    /// </summary>
    public partial class HomeSystem : Page
    {
        private bool dragStartedBrightness = false;
        private bool dragStartedVolume = false;

        private bool firstTick = true;
        private DispatcherTimer updateTick = new DispatcherTimer();

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
                HandleChangingBrightness(Brightness.Value);
            }
        }
        private void Brightness_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness(Brightness.Value);
        }
        private void Brightness_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedBrightness = true;
        }
        private void Brightness_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness(Brightness.Value);
        }
        private void Brightness_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedBrightness = false;
            HandleChangingBrightness(Brightness.Value);
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
        void HandleChangingBrightness(double brightness)
        {
            GlobalVariables.needBrightnessRead = true;
            Classes.ChangeBrightness.WindowsSettingsBrightnessController.setBrightness((int)brightness);
        }
        void HandleChangingVolume(int volume)
        {
            GlobalVariables.needVolumeRead = true;
            Classes.ChangeVolume.AudioManager.SetMasterVolume((float)volume);
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            intializeTimer();
            loadValues();

        }

        void loadValues()
        {
            //If global tdp is not zero meaning it was read within 10 seconds, load those instead of calling a update
            if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
            {
                updateFromGlobalValues();
            }

        }
        void intializeTimer()
        {
            //Set up auto update tick timer, which syncs with global variable updates
            updateTick.Interval = new TimeSpan(0, 0, 2);
            updateTick.Tick += updateTick_Tick;
            updateTick.Start();
            System.Diagnostics.Debug.WriteLine("");
        }
        void updateTick_Tick(object sender, EventArgs e)
        {
            //Divorce actual routine from tick event so the updateFromGlobalTDP can be called at start up or tick event
            updateFromGlobalValues();
        }
        void updateFromGlobalValues()
        {
            if (!dragStartedBrightness && !GlobalVariables.needBrightnessRead) { Brightness.Value = GlobalVariables.brightness; }
            if (!dragStartedVolume && !GlobalVariables.needVolumeRead) 
            { Volume.Value = GlobalVariables.volume; }
         
        }


    }
}
