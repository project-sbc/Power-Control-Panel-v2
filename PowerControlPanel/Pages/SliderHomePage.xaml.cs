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
using System.Diagnostics;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class SliderHomePage : Page
    {
        private DispatcherTimer timer = new DispatcherTimer();

        //tdp variables
        private bool dragStartedTDP1 = false;
        private bool dragStartedTDP2 = false;
        private bool dragStartedTDP = false;
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
        private bool enableSystem = Properties.Settings.Default.enableVolume;
        private bool enableDisplay = Properties.Settings.Default.enableDisplay;
        private bool enableCPU = Properties.Settings.Default.enableCPU;
        //profiles
        private bool changingProfiles = false;

        private bool dragStartedMAXCPU = false;
        private bool changingMAXCPU = false;

        private bool dragStartedActiveCores = false;
        private bool changingActiveCores = false;

        public SliderHomePage()
        {



            InitializeComponent();
            //set max cpu core count here

            initializeTimer();

            setMinMaxSliderValues();

            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //Add list of resolution refresh to combo box
            displayItemSourceBind();

            changingProfiles = true;
            cboProfile.ItemsSource = PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.profileListForHomePage();
            changingProfiles = false;

            loadUpdateValues();

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;
        }



        private void setMinMaxSliderValues()
        {
            TDP1.Maximum = Properties.Settings.Default.maxTDP;
            TDP2.Maximum = Properties.Settings.Default.maxTDP;
            TDP1.Minimum = Properties.Settings.Default.minTDP;
            TDP2.Minimum = Properties.Settings.Default.minTDP;
            ActiveCores.Maximum = GlobalVariables.maxCpuCores;
            MAXCPU.Minimum = GlobalVariables.baseCPUSpeed;
            GPUCLK.Maximum = Properties.Settings.Default.maxGPUCLK;
        }

        private void displayItemSourceBind()
        {

            cboRefreshRate.ItemsSource = GlobalVariables.refreshRates;
            cboResolution.ItemsSource = GlobalVariables.resolutions;
            cboFPSLimit.ItemsSource = GlobalVariables.FPSLimits;
            cboScaling.ItemsSource = GlobalVariables.scalings;
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

                    thumb.Width = 32;
                    thumb.Height = 40;
                }
                else { }
            }
            else { }
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

            //max cpu clock updates
            if (!dragStartedMAXCPU && enableCPU && !GlobalVariables.needCPUMaxFreqRead)
            {
                if (GlobalVariables.cpuMaxFrequency == 0)
                {
                    MAXCPU.Value = MAXCPU.Maximum;
                    txtsliderMAXCPU.Visibility = Visibility.Collapsed;
                    txtsliderMAXCPUAuto.Visibility = Visibility.Visible;


                }
                else
                {
                    MAXCPU.Value = GlobalVariables.cpuMaxFrequency;

                    txtsliderMAXCPUAuto.Visibility = Visibility.Collapsed;
                    txtsliderMAXCPU.Visibility = Visibility.Visible;
                }
            }


            //active core updates
            if (!dragStartedActiveCores && enableCPU && !GlobalVariables.needActiveCoreRead)
            {
                ActiveCores.Value = GlobalVariables.cpuActiveCores;
            }

            //profile
            if (!changingProfiles)
            {
                changingProfiles = true;
                cboProfile.Text = GlobalVariables.ActiveProfile;
                changingProfiles = false;

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

            if (PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.rtssRunning())
            {
                cboFPSLimit.Text = GlobalVariables.FPSLimit;

                
                bdFPSLimit.Visibility = Visibility.Visible;
            }
            else
            {
              
                bdFPSLimit.Visibility = Visibility.Collapsed;
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
                    updateFromGlobalTDP();
                    changingTDP = false;
                }

            }

        }



        #endregion timer controls

        #region handle visibility

        private void handleVisibility()
        {
            //handle enabling and showing gpu clock
            if (GlobalVariables.cpuType == "Intel")
            {
                bdGPUCLK.Visibility = Visibility.Collapsed;
            }
          

            if (Properties.Settings.Default.enableCombinedTDP)
            {
                bdTDP1.Visibility = Visibility.Collapsed;
                bdTDP2.Visibility = Visibility.Collapsed;

            }
            else { bdTDP.Visibility = Visibility.Collapsed; }
         
        }


        #endregion handle visibility

        #region toggle group box

 
        #endregion

        #region slider value changed
        private void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
 
        }

        #endregion

        #region slider drag completed
        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;
     
            if (this.IsLoaded)
            {
                //Debug.WriteLine("drag complete event");
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = false;
                        Debug.WriteLine("drag complete event" + TDP1.Value.ToString());
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        break;
                    case "TDP2":
                        dragStartedTDP2 = false;
                        HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
                        break;
                    case "TDP":
                        dragStartedTDP = false;
                        HandleChangingTDP((int)TDP.Value, (int)TDP.Value, true);
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
                    case "MAXCPU":
                        dragStartedMAXCPU = false;
                        HandleChangingMAXCPU((int)MAXCPU.Value);
                        break;
                    case "ActiveCores":
                        dragStartedActiveCores = false;
                        HandleChangingActiveCores(Convert.ToDouble(ActiveCores.Value));
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
                Debug.WriteLine("drag started event");
                switch (sliderName)
                {
                    case "TDP1":
                        dragStartedTDP1 = true;
                        break;
                    case "TDP":
                        dragStartedTDP = true;
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
                    case "MAXCPU":
                        dragStartedMAXCPU = true;
                        break;
                    case "ActiveCores":
                        dragStartedActiveCores = true;
                        break;
                    default:
                        break;
                }

            }

        }


        #endregion

      

        void updateFromGlobalTDP()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {

                try
                {
                    if (!dragStartedTDP & Math.Abs(TDP.Value - GlobalVariables.readPL1) > 0.9)
                    { TDP.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }

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

        private void HandleChangingMAXCPU(int maxcpu)
        {
            if (this.IsLoaded)
            {
                if (dragStartedMAXCPU == false)
                {
                    changingMAXCPU = true;
                    GlobalVariables.needCPUMaxFreqRead = true;
                    int sendMaxCPU = 0;
                    if (maxcpu != MAXCPU.Maximum) { sendMaxCPU = maxcpu; }


                    Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeCPUMaxFrequency(sendMaxCPU));

                    changingMAXCPU = false;
                }

            }



        }

        private void HandleChangingActiveCores(double cores)
        {
            if (this.IsLoaded)
            {
                if (dragStartedActiveCores == false)
                {
                    changingActiveCores = true;
                    GlobalVariables.needActiveCoreRead = true;
                    Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeActiveCores(cores));

                    changingActiveCores = false;
                }

            }



        }


        private void HandleChangingGPUCLK(int gpuclk)
        {
            if (this.IsLoaded)
            {
                changingGPUCLK = true;
                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(gpuclk));
                txtsliderAMDGPUCLK.Visibility = Visibility.Visible;
                txtsliderAMDGPUCLKDEF.Visibility = Visibility.Collapsed;
                changingGPUCLK = false;
            }

        }


        #endregion system controls





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
            if (!changingScaling && cboScaling.SelectedItem.ToString() != "Default")
            {
                changingScaling = true;
                PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayScaling(cboScaling.SelectedItem.ToString());
                changingScaling = false;
            }

        }
        private void cboProfile_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!changingProfiles)
            {
                if (cboProfile.SelectedValue.ToString() != "None")
                {
                    PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.applyProfile(cboProfile.SelectedValue.ToString());
                }
                else
                {
                    PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.applyProfile("None");
                }
            }
        }
        private void cboFPSLimit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.changeLimit(cboFPSLimit.SelectedValue.ToString());
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            handleVisibility();
        }



        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                Debug.WriteLine("value changed event");
                switch (sliderName)
                {
                    case "TDP1":
                        if (!dragStartedTDP1 && !changingTDP)
                        {
                            //Debug.WriteLine(TDP1.Value.ToString());
                            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
                        }
                        break;
                    case "TDP":
                        if (!dragStartedTDP && !changingTDP)
                        {
                            HandleChangingTDP((int)TDP.Value, (int)TDP.Value, true);
                        }
                        break;
                    case "TDP2":
                        if (!dragStartedTDP2 && !changingTDP)
                        {
                            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
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
                    case "MAXCPU":
                        if (!dragStartedMAXCPU && !changingMAXCPU)
                        {
                            HandleChangingMAXCPU((int)MAXCPU.Value);
                        }
                        if (MAXCPU.Value == MAXCPU.Maximum)
                        {
                            txtsliderMAXCPU.Visibility = Visibility.Collapsed;
                            txtsliderMAXCPUAuto.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            txtsliderMAXCPUAuto.Visibility = Visibility.Collapsed;
                            txtsliderMAXCPU.Visibility = Visibility.Visible;
                        }
                        break;
                    case "ActiveCores":
                        if (!dragStartedActiveCores && !changingActiveCores)
                        {
                            HandleChangingActiveCores(Convert.ToDouble(ActiveCores.Value));
                        }
                        break;
                    default:
                        break;
                }

            }
        }
    }
}
