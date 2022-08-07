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
using MahApps.Metro.Controls;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using ControlzEx.Theming;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();

        //tdp variables
        private bool dragStartedTDP1 = false;
        private bool dragStartedTDP2 = false;
        private bool changingTDP = false;

        //system variables
        private bool dragStartedBrightness = false;
        private bool dragStartedVolume = false;

        //display settings
        private bool changingResolution = false;
        private bool changingRefreshRate = false;
        private bool changingScaling = false;

        //AMD gpu clk
        private bool dragStartedGPUCLK = false;
        private bool changingGPUCLK = false;

        //enabled booleans
        private bool enableTDP = Properties.Settings.Default.enableTDP;
        private bool enableGPUCLK = Properties.Settings.Default.enableGPUCLK;
        private bool enableSystem = Properties.Settings.Default.enableSystem;
        private bool enableDisplay = Properties.Settings.Default.enableDisplay;

        public HomePage()
        {
           
            

            InitializeComponent();
            //set max cpu core count here
            MAXCPU.Maximum = Environment.ProcessorCount;
            initializeTimer();

            setMaxTDP();

            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //Add list of resolution refresh to combo box
            displayItemSourceBind();

            loadUpdateValues();
        }



        private void setMaxTDP()
        {
            TDP1.Maximum = Properties.Settings.Default.maxTDP;
            TDP2.Maximum = Properties.Settings.Default.maxTDP;
        }

        private void displayItemSourceBind()
        {

            cboRefreshRate.ItemsSource = GlobalVariables.refreshRates;
            cboResolution.ItemsSource = GlobalVariables.resolutions;

            changingResolution = true;
            cboResolution.SelectedIndex = 0;
            changingResolution = false;
        }

      


        #region slider loaded change thumb
        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            var SliderThumb = GetElementFromParent(sender as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {
                if (SliderThumb is Thumb thumb)
                {
                    thumb.Width = 20;
                    thumb.Height = 25;
                }
                else { }
            }
            else  { }
        }
        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {

            //Use element parent for thumb size control on slider
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement childframeworkelement && childframeworkelement.Name == childname)
                    return child;

                var FindRes = GetElementFromParent(child, childname);
                if (FindRes != null)
                    return FindRes;
            }
            return null;
        }
    
   

        #endregion


        #region timer controls

        private void initializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += timerTick;
            timer.Start();

        }
        private void timerTick(object sender, EventArgs e)
        {

            loadUpdateValues();
        }

        private void loadUpdateValues()
        {
            //GPU clock updates
            if (!dragStartedGPUCLK && enableGPUCLK)
            {
                if (GlobalVariables.gpuclk == "Default")
                {
                    if (GPUCLK.Value != 200) { GPUCLK.Value = 200; }

                }
                else
                {
                    if (GPUCLK.Value != Int32.Parse(GlobalVariables.gpuclk))
                    {
                        GPUCLK.Value = Int32.Parse(GlobalVariables.gpuclk);
                    }
                }
            }


            //display updates
            if (enableDisplay)
            {
                if (cboRefreshRate.Text != GlobalVariables.refreshRate && !changingRefreshRate)
                {
                    changingRefreshRate = true;
                    cboRefreshRate.Text = GlobalVariables.refreshRate;
                    changingRefreshRate = false;

                }
                if (cboResolution.Text != GlobalVariables.resolution && !changingResolution && GlobalVariables.resolution != "")
                {
                    changingResolution = true;
                    cboResolution.Text = GlobalVariables.resolution;
                    changingResolution = false;

                }
                if (cboScaling.Text != GlobalVariables.scaling && !changingScaling)
                {
                    changingScaling = true;
                    cboScaling.Text = GlobalVariables.scaling;
                    changingScaling = false;

                }
            }
  
            //system values
            if (enableSystem)
            {
                if (!dragStartedBrightness && !GlobalVariables.needBrightnessRead) { Brightness.Value = GlobalVariables.brightness; }
                if (!dragStartedVolume && !GlobalVariables.needVolumeRead)
                { Volume.Value = GlobalVariables.volume; }
            }

            //TPD
            if (enableTDP)
            {
                if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
                {
                    changingTDP = true;
                    updateFromGlobalTDPPL1();
                    updateFromGlobalTDPPL2();
                    changingTDP = false;
                }

            }

        }

 

        #endregion timer controls

        #region handle visibility

        private void handleVisibility()
        {
            //handle enabling and showing gpu clock
            if (!Properties.Settings.Default.enableGPUCLK ^ GlobalVariables.cpuType == "Intel")
            {
                GBAMDGPUCLK.Visibility = Visibility.Collapsed;
                GBAMDGPUCLK.Margin = new Thickness(0, 0, 0, 0);

            }
            else
            {
                if (Properties.Settings.Default.showGPUCLK)
                { enableControlGPUCLK.IsOn = true; }
                else
                { enableControlGPUCLK.IsOn = false; GBAMDGPUCLK.Height = 40; enableControlGPUCLK.IsOn = false; }
            }


            if (!Properties.Settings.Default.enableTDP)
            {
                GBTDPControls.Visibility = Visibility.Collapsed;
                GBTDPControls.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                if (Properties.Settings.Default.showTDP)
                { enableControlTDP.IsOn = true; }
                else { enableControlTDP.IsOn = false; GBTDPControls.Height = 40; enableControlTDP.IsOn = false; }
            }


            if (!Properties.Settings.Default.enableCPU)
            {
                GBCPUControls.Visibility = Visibility.Collapsed;
                GBCPUControls.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                if (Properties.Settings.Default.showTDP)
                { enableControlCPU.IsOn = true; }
                else { enableControlCPU.IsOn = false; GBCPUControls.Height = 40; enableControlCPU.IsOn = false; }
            }


            if (!Properties.Settings.Default.enableSystem)
            {
                GBSystemControls.Visibility = Visibility.Collapsed;
                GBSystemControls.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                if (Properties.Settings.Default.showSystem)
                { enableControlSystem.IsOn = true; }
                else { enableControlSystem.IsOn = false; GBSystemControls.Height = 40; enableControlSystem.IsOn = false; }
            }

            if (!Properties.Settings.Default.enableDisplay)
            {
                GBDisplayControls.Visibility = Visibility.Collapsed;
                GBDisplayControls.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                if (Properties.Settings.Default.showDisplay)
                { enableControlDisplay.IsOn = true; }
                else { enableControlDisplay.IsOn = false; GBDisplayControls.Height = 40; enableControlDisplay.IsOn = false; }
            }



        }


        #endregion handle visibility

        #region toggle group box
        private void enableControlGPUCLK_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableControlGPUCLK.IsOn)
                {
                    GBAMDGPUCLK.Height = 100;
                    Properties.Settings.Default.showGPUCLK = true;
                }
                else
                {
                    GBAMDGPUCLK.Height = 40;
                    Properties.Settings.Default.showGPUCLK = false;
                }
                Properties.Settings.Default.Save();
            }


        }


        private void enableControlTDP_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableControlTDP.IsOn)
                {
                    GBTDPControls.Height = 150;
                    Properties.Settings.Default.showTDP = true;
                }
                else
                {
                    GBTDPControls.Height = 40;
                    Properties.Settings.Default.showTDP = false;
                }
                Properties.Settings.Default.Save();
            }
        }

        
        private void enableControlCPU_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableControlCPU.IsOn)
                {
                    GBCPUControls.Height = 150;
                    Properties.Settings.Default.showCPU = true;
                }
                else
                {
                    GBCPUControls.Height = 40;
                    Properties.Settings.Default.showCPU = false;
                }
                Properties.Settings.Default.Save();
            }
        }
        private void enableControlSystem_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableControlSystem.IsOn)
                {
                    GBSystemControls.Height = 150;
                    Properties.Settings.Default.showSystem = true;
                }
                else { GBSystemControls.Height = 40; Properties.Settings.Default.showSystem = false; }
                Properties.Settings.Default.Save();
            }
        }
        private void enableControlDisplay_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                if (enableControlDisplay.IsOn)
                {
                    GBDisplayControls.Height = 200;
                    Properties.Settings.Default.showDisplay = true;
                }
                else
                {
                    GBDisplayControls.Height = 40;
                    Properties.Settings.Default.showDisplay = false;
                }
                Properties.Settings.Default.Save();
            }



        }

        #endregion

        #region slider value changed
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        if (!dragStartedTDP1 && !changingTDP)
                        {
                            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        }
                        break;
                    case "TDP2":
                        if (!dragStartedTDP2 && !changingTDP)
                        {
                            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        }
                        break;
                    case "Brightness":
                        if (!dragStartedBrightness)
                        {
                            HandleChangingBrightness(Brightness.Value);
                        }
                        break;

                    case "Volume":
                        if (!dragStartedVolume)
                        {
                            HandleChangingVolume((int)Volume.Value);
                        }
                        break;
                    case "GPUCLK":
                        if (!dragStartedGPUCLK && !changingGPUCLK)
                        {
                            HandleChangingGPUCLK((int)GPUCLK.Value);
                        }
                        break;
                    default:
                        break;
                }

            }
        }
      
        #endregion

        #region slider drag completed
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "TDP2":
                        dragStartedTDP2 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "Brightness":
                        dragStartedBrightness = false;
                        HandleChangingBrightness(Brightness.Value);
                        break;

                    case "Volume":
                        dragStartedVolume = false;
                        HandleChangingVolume((int)Volume.Value);
                        break;
                    case "GPUCLK":
                        dragStartedGPUCLK = false;
                        HandleChangingGPUCLK((int)GPUCLK.Value);
                        break;
                    default:
                        break;
                }

            }

        }
     
     
        #endregion

        #region slider drag started
        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = true;
                        break;
                    case "TDP2":
                        dragStartedTDP2 = true;
                        break;
                    case "Brightness":
                        dragStartedBrightness = true;
                        break;
                    case "Volume":
                        dragStartedVolume = true;
                        break;
                    case "GPUCLK":
                        dragStartedGPUCLK = true;
                        break;
                    default:
                        break;
                }

            }

        }

  
        #endregion

        #region slider mouse left up
        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "TDP2":
                        dragStartedTDP2 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "Brightness":
                        dragStartedBrightness = false;
                        HandleChangingBrightness(Brightness.Value);
                        break;
                    case "Volume":
                        dragStartedVolume = false;
                        HandleChangingVolume((int)Volume.Value);
                        break;
                    case "GPUCLK":
                        dragStartedGPUCLK = false;
                        HandleChangingGPUCLK((int)GPUCLK.Value);
                        break;
                    default:
                        break;
                }

            }
        }

        private void TDP1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider_MouseLeftButtonUp(sender, e);

        }
        private void TDP2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider_MouseLeftButtonUp(sender, e);
        }
        private void Brightness_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider_MouseLeftButtonUp(sender, e);
        }
        private void Volume_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider_MouseLeftButtonUp(sender, e);
        }
        private void GPUCLK_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider_MouseLeftButtonUp(sender, e);
        }


        #endregion

        #region slider touchup

        private void Slider_TouchUp(object sender, TouchEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "TDP2":
                        dragStartedTDP2 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "Brightness":
                        dragStartedBrightness = false;
                        HandleChangingBrightness(Brightness.Value);
                        break;
                    case "Volume":
                        dragStartedVolume = false;
                        HandleChangingVolume((int)Volume.Value);
                        break;
                    case "GPUCLK":
                        dragStartedGPUCLK = false;
                        HandleChangingGPUCLK((int)GPUCLK.Value);
                        break;
                    default:
                        break;
                }

            }
        }
        private void TDP1_TouchUp(object sender, TouchEventArgs e)
        {
            Slider_TouchUp(sender, e);
        }


        private void TDP2_TouchUp(object sender, TouchEventArgs e)
        {
            Slider_TouchUp(sender, e);
        }
        private void GPUCLK_TouchUp(object sender, TouchEventArgs e)
        {
            Slider_TouchUp(sender, e);
        }

        private void Brightness_TouchUp(object sender, TouchEventArgs e)
        {
            Slider_TouchUp(sender, e);
        }
        private void Volume_TouchUp(object sender, TouchEventArgs e)
        {
            Slider_TouchUp(sender, e);
        }

        #endregion

        #region slider touchdown
        private void Slider_TouchDown(object sender, TouchEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = true;
                        break;
                    case "TDP2":
                        dragStartedTDP2 = true;
                        break;
                    case "Brightness":
                        dragStartedBrightness = true;
                        break;
                    case "Volume":
                        dragStartedVolume = true;
                        break;
                    case "GPUCLK":
                        dragStartedGPUCLK = true;
                        break;
                    default:
                        break;
                }

            }
        }
        private void TDP1_TouchDown(object sender, TouchEventArgs e)
        {
            Slider_TouchDown(sender, e);
        }
        private void TDP2_TouchDown(object sender, TouchEventArgs e)
        {
            Slider_TouchDown(sender, e);
        }
        private void Brightness_TouchDown(object sender, TouchEventArgs e)
        {
            Slider_TouchDown(sender, e);
        }
        private void Volume_TouchDown(object sender, TouchEventArgs e)
        {
            Slider_TouchDown(sender, e);
        }
        private void GPUCLK_TouchDown(object sender, TouchEventArgs e)
        {
            Slider_TouchDown(sender, e);
        }


        #endregion








        private void HandleChangingTDP(int tdpPL1, int tdpPL2, bool PL1started)
        {
            //6800U MessageBox.Show("can change tdp:" + !changingTDP);
            if (!changingTDP)
            {
                changingTDP = true;
                GlobalVariables.needTDPRead = true;
                Thread.Sleep(150);
                if (PL1started)
                {
                    //If PL1 is greater than PL2 then PL2 needs to be set to the PL1 value

                    if (tdpPL1 < tdpPL2) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP2.Value = tdpPL1;
                        tdpPL2 = tdpPL1;
                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2));
                    };
                }
                else
                {
                    //If PL2 is less than PL1 drop PL1 down to PL2 new value
                    if (tdpPL1 < tdpPL2) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP1.Value = tdpPL2;
                        tdpPL1 = tdpPL2;
                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2));
                    };
                }

                changingTDP = false;
            }


        }





        void updateFromGlobalTDPPL1()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {

                try
                {
                    if (!dragStartedTDP1 & Math.Abs(TDP1.Value - GlobalVariables.readPL1) > 0.9)
                    { TDP1.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }

            }

        }
        void updateFromGlobalTDPPL2()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {


                try
                {
                    if (!dragStartedTDP2 & Math.Abs(TDP2.Value - GlobalVariables.readPL2) > 0.9)
                    { TDP2.Value = Math.Round(GlobalVariables.readPL2, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }



            }

        }


        #region system controls








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










        #endregion system controls


        #region GPU Clock Slider





        private void HandleChangingGPUCLK(int gpuclk)
        {
            if (this.IsLoaded)
            {
                changingGPUCLK = true;
                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(gpuclk));

                txtsliderAMDGPUCLKDEF.Content = "";
                changingGPUCLK = false;
            }

        }

        #endregion GPU Clock Slider



        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalVariables.resolution != cboResolution.SelectedItem && !changingResolution && cboResolution.SelectedItem != "Custom Scaling")
            { PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayResolution(cboResolution.SelectedItem.ToString()); }
        }

        private void cboRefreshRate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalVariables.refreshRate != cboRefreshRate.SelectedItem && !changingRefreshRate)
            { PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayRefreshRate(cboRefreshRate.SelectedItem.ToString()); }
        }

        private void cboScaling_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!changingScaling && cboScaling.SelectedValue.ToString() != "Default")
            {
                changingScaling = true;
                PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayScaling(cboScaling.SelectedValue.ToString());
                changingScaling = false;
            }

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            handleVisibility();
        }

  
    }
}
