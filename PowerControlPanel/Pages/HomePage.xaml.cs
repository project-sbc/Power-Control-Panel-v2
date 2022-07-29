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
        private bool changingTDP = true;

        //system variables
        private bool dragStartedBrightness = false;
        private bool dragStartedVolume = false;

        //display settings
        private bool changingResolution = false;
        private bool changingRefreshRate = false;
        private bool changingScaling = false;

        //AMD gpu clk
        private bool dragStartedGPUCLK = false;
        private bool changingGPUCLK = true;


        public HomePage()
        {
            InitializeComponent();

            initializeTimer();

            setMaxTDP();

            handleVisibility();

            loadTDPValues();
            loadSystemValues();

            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //Add list of resolution refresh to combo box
            displayItemSourceBind();

            loadGPUClock();
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

            updateDisplaySettings();


        }

        private void updateDisplaySettings()
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
                changingResolution=false;
            
            }
            if (cboScaling.Text != GlobalVariables.scaling && !changingScaling)
            {
                changingScaling = true;
                cboScaling.Text = GlobalVariables.scaling;
                changingScaling = false;

            }

        }

        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {
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
                else
                {
                    //SliderThumb is not an object of type Thumb
                }
            }
            else
            {
                //SliderThumb is null
            }
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
            #region tdp updates
            loadTDPValues();

            #endregion tdp updates
            #region system updates
            loadSystemValues();
            #endregion system updates

            updateDisplaySettings();

            loadGPUClock();
        }

        private void loadGPUClock()
        {
            if (!dragStartedGPUCLK && !changingGPUCLK)
            {
                if (GlobalVariables.gpuclk == "Default")
                {
                    if (GPUCLK.Value != 100) { GPUCLK.Value = 100; }

                }
                else
                {
                    if (GPUCLK.Value != Int32.Parse(GlobalVariables.gpuclk))
                    {
                        GPUCLK.Value = Int32.Parse(GlobalVariables.gpuclk);
                    }
                }
            }

        }

        #endregion timer controls

        #region handle visibility

        private void handleVisibility()
        {
            if (Properties.Settings.Default.showTDP)
            { enableControlTDP.IsOn = true; }
            else { enableControlTDP.IsOn = false; }

            if (Properties.Settings.Default.showSystem)
            { enableControlSystem.IsOn = true; }
            else { enableControlSystem.IsOn = false; }
        }


        #endregion handle visibility

        #region TDP controls

        private void TDP1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP1 && !changingTDP)
            {
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
            }
        }
        private void TDP1_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP1 = true;
        }
        private void TDP1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = true;
        }








        private void TDP2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP2 && !changingTDP)
            {
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
            }
        }
        private void TDP2_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
        private void TDP2_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP2 = true;
        }
        private void TDP2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
        private void TDP2_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedTDP2 = true;
        }
        private void TDP2_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void HandleChangingTDP(int tdpPL1, int tdpPL2, bool PL1started)
        {

            if (!changingTDP)
            {
                changingTDP = true;
                GlobalVariables.needTDPRead = true;
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




        void loadTDPValues()
        {
            //If global tdp is not zero meaning it was read within 10 seconds, load those instead of calling a update
            if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
            {
                changingTDP = true;
                updateFromGlobalTDPPL1();
                updateFromGlobalTDPPL2();
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

        private void enableControlGPUCLK_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControlGPUCLK.IsOn)
            {
                GBAMDGPUCLK.Height = 100;
            }
            else
            {
                GBAMDGPUCLK.Height = 40;
                
            }
        }
        

        private void enableControlTDP_Toggled(object sender, RoutedEventArgs e)
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
                loadTDPValues();
            }
            Properties.Settings.Default.Save(); 
        }

        #endregion TDP controls

        #region system controls


        private void Brightness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedBrightness)
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

        private void enableControlSystem_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControlSystem.IsOn)
            {
                GBSystemControls.Height = 150;
            }
            else { GBSystemControls.Height = 40; }
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


        void loadSystemValues()
        {
            if (!dragStartedBrightness && !GlobalVariables.needBrightnessRead) { Brightness.Value = GlobalVariables.brightness; }
            if (!dragStartedVolume && !GlobalVariables.needVolumeRead)
            { Volume.Value = GlobalVariables.volume; }

        }



        #endregion system controls



        private void GPUCLK_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedGPUCLK && !changingGPUCLK)
            {
                HandleChangingGPUCLK((int)GPUCLK.Value);
            }
        }
        private void GPUCLK_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedGPUCLK = false;
            HandleChangingGPUCLK((int)GPUCLK.Value);
        }
        private void GPUCLK_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedGPUCLK = true;
        }
        private void GPUCLK_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedGPUCLK = false;
            HandleChangingGPUCLK((int)GPUCLK.Value);
        }
        private void GPUCLK_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedGPUCLK = false;
            HandleChangingGPUCLK((int)GPUCLK.Value);
        }
        private void GPUCLK_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedGPUCLK = true;
        }

        private void HandleChangingGPUCLK(int gpuclk)
        {
            changingGPUCLK = true;
            PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(gpuclk);
           
            txtsliderAMDGPUCLKDEF.Content = "";
            changingGPUCLK = false;
        }


        private void enableControlDisplay_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControlDisplay.IsOn)
            {
                GBDisplayControls.Height = 200;
            }
            else
            {


                GBDisplayControls.Height = 40;
                
            }
        }

        private void cboResolution_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(GlobalVariables.resolution != cboResolution.SelectedItem && !changingResolution && cboResolution.SelectedItem != "Custom Scaling")
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
    }
}
