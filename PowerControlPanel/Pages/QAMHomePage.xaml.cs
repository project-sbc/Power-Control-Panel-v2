using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using ControlzEx.Theming;
using MahApps.Metro.Controls;

namespace Power_Control_Panel.PowerControlPanel.Pages
{
    /// <summary>
    /// Interaction logic for QAMHomePage.xaml
    /// </summary>
    public partial class QAMHomePage : Page
    {
        private string currentControl;
        private bool controlActive = false;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool dragTDP = false;
        private bool dragTDP1 = false;
        private bool dragTDP2 = false;
        private bool dragMaxCPU = false;
        private bool dragActiveCores = false;
        private bool dragGPUCLK = false;
        private bool dragVolume = false;
        private bool dragBrightness = false;
        private bool changingRefreshRate = false;
        private bool changingResolution = false;
        private bool changingTDP = false;
        private bool changingScaling = false;
        private bool changingDisplay = false;
        private bool changingProfiles = false;
        private Brush accentBrush = null;
 
        public QAMHomePage()
        {
            InitializeComponent();


            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;

            //get accent color
            accentBrush = Profile_Tile.Background;

            //set combobox sources
            setComboBoxItemSource();

            //set upper and lower limits on sliders
            setMinMaxSliderValues();

            //set values
            loadUpdateValues();


 
            //set intial visibility (to remove disabled stuff)
            hideDisabledItems();


            setViewStyle();


            initializeTimer();
            loadUpdateValues();
            //

        }

        private void setSliderThumbSizes()
        {
            setThumbSize(ActiveCores_Slider);
            setThumbSize(AMDGPUCLK_Slider);
            setThumbSize(Brightness_Slider);
            setThumbSize(MAXCPU_Slider);
            setThumbSize(TDP1_Slider);
            setThumbSize(TDP2_Slider);
            setThumbSize(TDP_Slider);
            setThumbSize(Volume_Slider);
        }
        private void initializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 3);
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
            if (!dragGPUCLK && Properties.Settings.Default.enableGPUCLK)
            {
                if (GlobalVariables.gpuclk == "Default")
                {
                    if (AMDGPUCLK_Slider.Value != 200) { AMDGPUCLK_Slider.Value = 200; }
                    AMDGPUCLK_Label.Content = "Default";
                    GPUCLK_TileLabel.Content = Application.Current.Resources["TileHomePage_GPUCLK"].ToString() + ": Default";
                }
                else
                {
                    if (AMDGPUCLK_Slider.Value != Int32.Parse(GlobalVariables.gpuclk))
                    {
                        AMDGPUCLK_Slider.Value = Int32.Parse(GlobalVariables.gpuclk);
                        GPUCLK_TileLabel.Content = Application.Current.Resources["TileHomePage_GPUCLK"].ToString() + ": " + GlobalVariables.gpuclk + " MHz";
                    }
                }
            }

            //max cpu clock updates
            if (!dragMaxCPU && Properties.Settings.Default.enableCPU && !GlobalVariables.needCPUMaxFreqRead)
            {
                if (GlobalVariables.cpuMaxFrequency == 0)
                {
                    MAXCPU_Slider.Value = MAXCPU_Slider.Maximum;
                    MaxCPU_Label.Content = Application.Current.Resources["Auto"].ToString();
                    MaxCPU_TileLabel.Content = Application.Current.Resources["TileHomePage_MaxCPU"].ToString() + ": " + Application.Current.Resources["Auto"].ToString();
                }
                else
                {
                    MAXCPU_Slider.Value = GlobalVariables.cpuMaxFrequency;
                    MaxCPU_Label.Content = GlobalVariables.cpuMaxFrequency.ToString();
                    MaxCPU_TileLabel.Content = Application.Current.Resources["TileHomePage_MaxCPU"].ToString()+ ": " + GlobalVariables.cpuMaxFrequency.ToString() + " MHz";
                }
            }


            //active core updates
            if (!dragActiveCores && Properties.Settings.Default.enableCPU && !GlobalVariables.needActiveCoreRead)
            {
                ActiveCores_Slider.Value = GlobalVariables.cpuActiveCores;
                ActiveCores_TileLabel.Content = Application.Current.Resources["TileHomePage_ActiveCores"].ToString() + ": " + GlobalVariables.cpuActiveCores ;
            }

            //profile
            if (Profile_Cbo.Text != GlobalVariables.ActiveProfile)
            {
                changingProfiles = true;
                Profile_Cbo.Text = GlobalVariables.ActiveProfile;
                changingProfiles = false;
            }
            Profile_TileLabel.Content = GlobalVariables.ActiveProfile;
            App_TileLabel.Content = GlobalVariables.ActiveApp;

            //display updates
            if (Properties.Settings.Default.enableDisplay)
            {
                ResolutionRefresh_TileLabel.Content = GlobalVariables.resolution + " " + GlobalVariables.refreshRate + " Hz";
                Scaling_TileLabel.Content = GlobalVariables.scaling;
                if (RefreshRate_Cbo.Text != GlobalVariables.refreshRate && !changingRefreshRate)
                {
                    changingRefreshRate = true;
                    RefreshRate_Cbo.Text = GlobalVariables.refreshRate;
                    changingRefreshRate = false;

                }
                if (Resolution_Cbo.Text != GlobalVariables.resolution && !changingResolution && GlobalVariables.resolution != "")
                {
                    changingResolution = true;
                    Resolution_Cbo.Text = GlobalVariables.resolution;
                    changingResolution = false;

                }
                if (Scaling_Cbo.Text != GlobalVariables.scaling && !changingScaling)
                {
                    changingScaling = true;
                    Scaling_Cbo.Text = GlobalVariables.scaling;
                    changingScaling = false;

                }
            }

            if (PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.rtssRunning())
            {
                if (FPSLimit_GroupBorder.Visibility == Visibility.Collapsed)
                {
                    FPSLimit_GroupBorder.Visibility = Visibility.Visible;
                    FPSLimit_Tile.Visibility=Visibility.Visible;
                    FPSLimit_Cbo.Text = GlobalVariables.FPSLimit;
                    FPSLimit_TileLabel.Content = GlobalVariables.FPSLimit;
                    FPSLimit_Border.Visibility = Visibility.Visible;
                    
                }
                else
                {
                    FPSLimit_Cbo.Text = GlobalVariables.FPSLimit;
                    FPSLimit_TileLabel.Content = GlobalVariables.FPSLimit;
                }

            }
            else
            {
                FPSLimit_Tile.Visibility = Visibility.Collapsed;
                FPSLimit_GroupBorder.Visibility = Visibility.Collapsed;
            }


            //system values
            if (Properties.Settings.Default.enableVolume && !GlobalVariables.needVolumeRead && !dragVolume)
            {
                Volume_Slider.Value = GlobalVariables.volume; 
                Volume_TileLabel.Content = GlobalVariables.volume + "%";
            }
            if (Properties.Settings.Default.enableBrightness && !GlobalVariables.needBrightnessRead && !dragBrightness)
            {
                Brightness_Slider.Value = GlobalVariables.brightness;
                Brightness_TileLabel.Content = GlobalVariables.brightness + "%";
            }
//TPD
            if (Properties.Settings.Default.enableTDP)
            {
                if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
                {
                    TDP1_TileLabel.Content = Application.Current.Resources["TileHomePage_TDPSustained"].ToString() + ": " +GlobalVariables.readPL1 + " W";
                    TDP2_TileLabel.Content = Application.Current.Resources["TileHomePage_TDPBoost"].ToString() + ": " + GlobalVariables.readPL2 + " W";
                    changingTDP = true;
                    updateFromGlobalTDPPL1();
                    updateFromGlobalTDPPL2();
                    updateFromGlobalTDP();
                    changingTDP = false;
                }

            }

        }
        void updateFromGlobalTDP()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {

                try
                {
                    if (!dragTDP & Math.Abs(TDP_Slider.Value - GlobalVariables.readPL1) > 0.9)
                    { TDP_Slider.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero); }
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
                    if (!dragTDP1 & Math.Abs(TDP1_Slider.Value - GlobalVariables.readPL1) > 0.9)
                    { TDP1_Slider.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero); }
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
                    if (!dragTDP2 & Math.Abs(TDP2_Slider.Value - GlobalVariables.readPL2) > 0.9)
                    { TDP2_Slider.Value = Math.Round(GlobalVariables.readPL2, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }



            }

        }


        private void hideDisabledItems()
        {
            //set enable/disable
            if (!Properties.Settings.Default.enableDisplay)
            {
                Display_GroupBorder.Visibility = Visibility.Collapsed;
                Display_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableCPU)
            {
                CPU_GroupBorder.Visibility = Visibility.Collapsed;
                CPU_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableVolume)
            {
                Volume_GroupBorder.Visibility = Visibility.Collapsed;
                Volume_Tile.Visibility = Visibility.Collapsed;
            }
            if (!Properties.Settings.Default.enableBrightness)
            {
                Brightness_GroupBorder.Visibility = Visibility.Collapsed;
                Brightness_Tile.Visibility = Visibility.Collapsed;
            }


            if (!Properties.Settings.Default.enableTDP)
            {
                TDP_GroupBorder.Visibility = Visibility.Collapsed;
                TDP_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableGPUCLK)
            {
                AMD_GroupBorder.Visibility = Visibility.Collapsed;
                AMD_Tile.Visibility = Visibility.Collapsed;
            }

            if (!PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.rtssRunning())
            {
                FPSLimit_Border.Visibility = Visibility.Collapsed;
                FPSLimit_Tile.Visibility = Visibility.Collapsed;
            }

            if (Properties.Settings.Default.enableCombinedTDP)
            {
                TDP1_Border.Visibility = Visibility.Collapsed;
                TDP2_Border.Visibility = Visibility.Collapsed;

            }
            else
            {
                TDP_Border.Visibility = Visibility.Collapsed;

            }


            //hide stuff that is cpu specific
            if (GlobalVariables.cpuType == "Intel")
            {
                AMD_Tile.Visibility = Visibility.Collapsed;
                AMD_Tile.Visibility = Visibility.Collapsed;
            }

            //havent added intel specific stuff, will fix in the future
            Intel_Tile.Visibility = Visibility.Collapsed;

            //hide game launcher tiles that aren't configured
            if (Properties.Settings.Default.directoryPlaynite == "")
            {
                PlayNite_Tile.Visibility = Visibility.Collapsed;
            }
            if (Properties.Settings.Default.directorySteam == "")
            {
                Steam_Tile.Visibility = Visibility.Collapsed;
            }

        }

        private void setMinMaxSliderValues()
        {
            TDP1_Slider.Maximum = Properties.Settings.Default.maxTDP;
            TDP2_Slider.Maximum = Properties.Settings.Default.maxTDP;
            TDP_Slider.Maximum = Properties.Settings.Default.maxTDP;
            TDP1_Slider.Minimum = Properties.Settings.Default.minTDP;
            TDP2_Slider.Minimum = Properties.Settings.Default.minTDP;
            TDP_Slider.Minimum = Properties.Settings.Default.minTDP;
            ActiveCores_Slider.Maximum = GlobalVariables.maxCpuCores;
            MAXCPU_Slider.Minimum = GlobalVariables.baseCPUSpeed;
            AMDGPUCLK_Slider.Maximum = Properties.Settings.Default.maxGPUCLK;
        }
        private void setComboBoxItemSource()
        {
            FPSLimit_Cbo.ItemsSource = GlobalVariables.FPSLimits;
            Scaling_Cbo.ItemsSource = GlobalVariables.scalings;
            Resolution_Cbo.ItemsSource = GlobalVariables.resolutions;
            RefreshRate_Cbo.ItemsSource = GlobalVariables.refreshRates;
            Profile_Cbo.ItemsSource = PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.profileListForHomePage();

            changingResolution = true;
            Resolution_Cbo.SelectedIndex = 0;
            changingResolution = false;
        }

        private void setViewStyle()
        {
            if (this.Title == "QAMHomePage")
            {
                //set by view style
                if (Properties.Settings.Default.homePageTypeQAM == "Tile")
                {
                    removeGroupSliderBoxAndBorder();
                }
                if (Properties.Settings.Default.homePageTypeQAM == "Slider")
                {
                    removeGroupSliderBoxAndBorder();
                    wrapPanel.Visibility = Visibility.Collapsed;
                }
                if (Properties.Settings.Default.homePageTypeQAM == "Group Slider")
                {
                    wrapPanel.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                removeGroupSliderBoxAndBorder();
                rdViewChange.Height = new GridLength(0);
                borderChangeViewStyle.Visibility = Visibility.Collapsed;
            }

        }

        private void resetView()
        {
            AMD_BoxHeader.Visibility = Visibility.Visible;
            Brightness_BoxHeader.Visibility = Visibility.Visible;
            CPU_BoxHeader.Visibility = Visibility.Visible;
            Display_BoxHeader.Visibility = Visibility.Visible;
            FPSLimit_BoxHeader.Visibility = Visibility.Visible;
            Profile_BoxHeader.Visibility = Visibility.Visible;
            TDP_BoxHeader.Visibility = Visibility.Visible;
            Volume_BoxHeader.Visibility = Visibility.Visible;

            wrapPanel.Visibility = Visibility.Visible;

            TDP_GroupBorder.BorderThickness = new Thickness(3);
            CPU_GroupBorder.BorderThickness = new Thickness(3);
            Display_GroupBorder.BorderThickness = new Thickness(3);
            Volume_GroupBorder.BorderThickness = new Thickness(3);
            Brightness_GroupBorder.BorderThickness = new Thickness(3);
            Profile_GroupBorder.BorderThickness = new Thickness(3);
            AMD_GroupBorder.BorderThickness = new Thickness(3);
            FPSLimit_GroupBorder.BorderThickness = new Thickness(3);



        }
        private void removeGroupSliderBoxAndBorder()
        {
            //get rid of border and box headers
            TDP_GroupBorder.BorderThickness = new Thickness(0);
            CPU_GroupBorder.BorderThickness = new Thickness(0);
            Display_GroupBorder.BorderThickness = new Thickness(0);
            Volume_GroupBorder.BorderThickness = new Thickness(0);
            Brightness_GroupBorder.BorderThickness = new Thickness(0);
            Profile_GroupBorder.BorderThickness = new Thickness(0);
            AMD_GroupBorder.BorderThickness = new Thickness(0);
            FPSLimit_GroupBorder.BorderThickness = new Thickness(0);


            AMD_BoxHeader.Visibility = Visibility.Collapsed;
            Brightness_BoxHeader.Visibility = Visibility.Collapsed;
            CPU_BoxHeader.Visibility = Visibility.Collapsed;
            Display_BoxHeader.Visibility = Visibility.Collapsed;
            FPSLimit_BoxHeader.Visibility = Visibility.Collapsed;
            Profile_BoxHeader.Visibility = Visibility.Collapsed;
            TDP_BoxHeader.Visibility = Visibility.Collapsed;
            Volume_BoxHeader.Visibility = Visibility.Collapsed;
        }

        private void setInitialVisibility()
        {

            //hide tile and sliders if setting is disabled


            //hide just sliders if you don't want it showing
            if (!Properties.Settings.Default.showTDP && Properties.Settings.Default.enableTDP)
            {
                handleToggleControls("TDP_Toggle", false);
                TDP_Toggle.IsOn = false;
            }


            if (!Properties.Settings.Default.showDisplay && Properties.Settings.Default.enableDisplay)
            {
                handleToggleControls("Display_Toggle", false);
                Display_Toggle.IsOn = false;
            }

            if (!Properties.Settings.Default.showCPU && Properties.Settings.Default.enableCPU)
            {
                handleToggleControls("CPU_Toggle", false);
                CPU_Toggle.IsOn = false;
            }

            if (!Properties.Settings.Default.showVolume && Properties.Settings.Default.enableVolume)
            {
                handleToggleControls("Volume_Toggle", false);
                Volume_Toggle.IsOn = false;
            }

            if (!Properties.Settings.Default.showBrightness && Properties.Settings.Default.enableBrightness)
            {
                handleToggleControls("Brightness_Toggle", false);
                Brightness_Toggle.IsOn = false;
            }

           
            

            if (!Properties.Settings.Default.showGPUCLK && Properties.Settings.Default.enableGPUCLK)
            {
                handleToggleControls("AMD_Toggle", false);
                AMD_Toggle.IsOn = false;
            }





        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //set height of scrollviewer
            //sliderScrollViewer.Height = this.ActualHeight - wrapPanel.ActualHeight;
            //set thumb size
            setSliderThumbSizes();

            setInitialVisibility();

  
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            //handles all tile click events
            Tile tile = (Tile)sender;
            string tileName = tile.Name;
           
            switch (tileName)
            {
                case "TDP_Tile":
                    TDP_Toggle.IsOn = !TDP_Toggle.IsOn;
                    break;
                case "Volume_Tile":
                    Volume_Toggle.IsOn = !Volume_Toggle.IsOn;
                    break;
                case "Brightness_Tile":
                    Brightness_Toggle.IsOn= !Brightness_Toggle.IsOn;
                    break;
                case "Display_Tile":
                    Display_Toggle.IsOn =  !Display_Toggle.IsOn;
                    break;
                case "CPU_Tile":
                    CPU_Toggle.IsOn = !CPU_Toggle.IsOn;
                    break;
                case "AMD_Tile":
                    AMD_Toggle.IsOn = !AMD_Toggle.IsOn;
                    break;
                case "Profile_Tile":
                    Profile_Toggle.IsOn = !Profile_Toggle.IsOn;
                    break;
                case "FPSLimit_Tile":
                    FPSLimit_Toggle.IsOn = !FPSLimit_Toggle.IsOn;
                    break;
                case "Steam_Tile":
                    PowerControlPanel.Classes.Steam.Steam.openSteamBigPicture();
                    break;
                case "PlayNite_Tile":
                    PowerControlPanel.Classes.Playnite.Playnite.playniteToggle();
                    break;
                default:
                    break;


            }

        }

     


        private DependencyObject GetElementFromParent(DependencyObject parent, string childname)
        {
            //Internet routine for finding thumb of slider
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


        private void Toggle_Toggled(object sender, RoutedEventArgs e)
        {
            
            if (this.IsLoaded)
            {
                ToggleSwitch toggleSwitch = (ToggleSwitch)sender;

                string toggleName = toggleSwitch.Name;

                handleToggleControls(toggleName, toggleSwitch.IsOn);
            }


  
                

        }
        private void handleToggleControls(string toggleName, bool toggleSwitch)
        {
            if (toggleSwitch)
            {
                switch (toggleName)
                {
                    case "TDP_Toggle":
                        if (Properties.Settings.Default.enableCombinedTDP)
                        {
                            TDP_Border.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            TDP1_Border.Visibility = Visibility.Visible;
                            TDP2_Border.Visibility = Visibility.Visible;
                        }
                        TDP_Tile.Background = accentBrush;
                        Properties.Settings.Default.showTDP = true;
                        break;
                    case "AMD_Toggle":
                        AMDGPUCLK_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showGPUCLK = true;
                        AMD_Tile.Background = accentBrush;
                        break;
                    case "CPU_Toggle":
                        MaxCPU_Border.Visibility = Visibility.Visible;
                        ActiveCores_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showCPU = true;
                        CPU_Tile.Background = accentBrush;
                        break;
                    case "Display_Toggle":
                        Resolution_Border.Visibility = Visibility.Visible;
                        RefreshRate_Border.Visibility = Visibility.Visible;
                        Scaling_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showDisplay = true;
                        Display_Tile.Background = accentBrush;
                        break;
                    case "Volume_Toggle":
                        Volume_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showVolume = true;
                        Volume_Tile.Background = accentBrush;
                        break;
                    case "Brightness_Toggle":
                        Brightness_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showBrightness = true;
                        Brightness_Tile.Background = accentBrush;
                        break;
                    case "FPSLimit_Toggle":
                        FPSLimit_Border.Visibility = Visibility.Visible;
                        FPSLimit_Tile.Background = accentBrush;
                        break;
                    case "Profile_Toggle":
                        Profile_Border.Visibility = Visibility.Visible;
                        Profile_Tile.Background = accentBrush;
                        break;
                    default:
                        break;



                }
            }
            else
            {
                switch (toggleName)
                {
                    case "TDP_Toggle":
                        if (Properties.Settings.Default.enableCombinedTDP)
                        {
                            TDP_Border.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            TDP1_Border.Visibility = Visibility.Collapsed;
                            TDP2_Border.Visibility = Visibility.Collapsed;
                        }
                        Properties.Settings.Default.showTDP = false;
                        TDP_Tile.Background = Brushes.Gray;
                        break;
                    case "AMD_Toggle":
                        AMDGPUCLK_Border.Visibility = Visibility.Collapsed;
                        AMD_Tile.Background = Brushes.Gray;
                        Properties.Settings.Default.showGPUCLK = false;
                        break;
                    case "CPU_Toggle":
                        MaxCPU_Border.Visibility = Visibility.Collapsed;
                        ActiveCores_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showCPU = false;
                        CPU_Tile.Background = Brushes.Gray;
                        break;
                    case "Display_Toggle":
                        Resolution_Border.Visibility = Visibility.Collapsed;
                        RefreshRate_Border.Visibility = Visibility.Collapsed;
                        Scaling_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showDisplay = false;
                        Display_Tile.Background = Brushes.Gray;
                        break;
                    case "Volume_Toggle":
                        Volume_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showVolume = false;
                        Volume_Tile.Background = Brushes.Gray;
                        break;
                    case "Brightness_Toggle":
                        Brightness_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showBrightness = false;
                        Brightness_Tile.Background = Brushes.Gray;
                        break;
                    case "FPSLimit_Toggle":
                        FPSLimit_Border.Visibility = Visibility.Collapsed;
                        FPSLimit_Tile.Background = Brushes.Gray;
                        break;
                    case "Profile_Toggle":
                        Profile_Border.Visibility = Visibility.Collapsed;
                        Profile_Tile.Background = Brushes.Gray;
                        break;
                    default:
                        break;



                }

            }
            Properties.Settings.Default.Save();

        }
 

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (Properties.Settings.Default.homePageTypeQAM)
            {
                case "Slider":
                    Properties.Settings.Default.homePageTypeQAM = "Group Slider";
                    break;
                case "Group Slider":
                    Properties.Settings.Default.homePageTypeQAM = "Tile";
                    break;
                case "Tile":
                    Properties.Settings.Default.homePageTypeQAM = "Slider";
                    break;
                default:
                    break;

            }

            Properties.Settings.Default.Save();
            resetView();
            setViewStyle();
        }




        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.IsLoaded)
            {
                Slider slider = (Slider)sender;
                string sliderName = slider.Name;
                //if new value = old value, dont update it, no point
                if (e.NewValue != e.OldValue)
                { 

                    handleChangeValues(sliderName, false, false, e.NewValue);
                }
              
            }

        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Slider slider = (Slider)sender;
            string sliderName = slider.Name;
            double sliderValue = slider.Value;
            handleChangeValues(sliderName, false, true, sliderValue);

        }


        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Slider slider = (Slider)sender;
            string sliderName = slider.Name;
            double sliderValue = slider.Value;
            handleChangeValues(sliderName, true, false, sliderValue);
        }
        private void handleChangeValues(string sliderName, bool dragStarted, bool dragCompleted, double sliderValue)
        {
            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP_Slider":
                        if (dragStarted) { dragTDP = true; }
                        else
                        {
                            if (dragCompleted) { dragTDP = false; }
                            if (!dragTDP)
                            {
                                HandleChangingTDP((int)sliderValue, (int)sliderValue, true);
                            }
                        }

                        break;
                    case "TDP1_Slider":
                        if (dragStarted) { dragTDP1 = true; }
                        else
                        {
                            if (dragCompleted) { dragTDP1 = false; }
                            if (!dragTDP1)
                            {
                                HandleChangingTDP((int)sliderValue, (int)TDP2_Slider.Value, true);
                            }
                        }
                        break;
                    case "TDP2_Slider":
                        if (dragStarted) { dragTDP2 = true; }
                        else
                        {
                            if (dragCompleted) { dragTDP2 = false; }
                            if (!dragTDP2)
                            {
                                 HandleChangingTDP((int)TDP1_Slider.Value, (int)sliderValue, false);
                            }

                        }
                        break;
                    case "AMDGPUCLK_Slider":
                        if (dragStarted) { dragGPUCLK = true; }
                        else
                        {
                            AMDGPUCLK_Label.Content = sliderValue.ToString();
                            if (dragCompleted) { dragGPUCLK = false; }
                            if (!dragGPUCLK)
                            {
                                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock((int)sliderValue));
                                
                            }

                        }
                        break;
                    case "Volume_Slider":
                        if (dragStarted) { dragVolume = true; }
                        else
                        {
                            if (dragCompleted) { dragVolume = false; }
                            if (!dragVolume)
                            {
                                GlobalVariables.needVolumeRead = true;
                                Classes.TaskScheduler.TaskScheduler.runTask(() => Classes.ChangeVolume.AudioManager.SetMasterVolume((float)sliderValue));
                            }
                        }
                        break;
                    case "Brightness_Slider":
                        if (dragStarted) { dragBrightness = true; }
                        else
                        {
                            if (dragCompleted) { dragBrightness = false; }
                            if (!dragBrightness)
                            {
                                GlobalVariables.needBrightnessRead = true;
                                Classes.TaskScheduler.TaskScheduler.runTask(() => Classes.ChangeBrightness.WindowsSettingsBrightnessController.setBrightness((int)sliderValue));
                            }
                           
                        }
                        break;
                    case "MAXCPU_Slider":

                        if (dragStarted) { dragMaxCPU = true; }
                        else
                        {
                            if (dragCompleted) { dragMaxCPU = false; }
                            if (!dragMaxCPU)
                            {
                                GlobalVariables.needCPUMaxFreqRead = true;
                                int sendMaxCPU = 0;
                                if ((int)sliderValue != MAXCPU_Slider.Maximum)
                                {
                                    //update label because maxcpu is special
                                    MaxCPU_Label.Content = sliderValue.ToString();
                                    sendMaxCPU = (int)sliderValue;
                                }
                                else
                                {
                                    MaxCPU_Label.Content = "Auto";
                                }
                                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeCPUMaxFrequency(sendMaxCPU));
                            }
 
                        }


                        break;

                    case "ActiveCores_Slider":
                        if (dragStarted) { dragActiveCores = true; }
                        else
                        {
                            if (dragCompleted) { dragActiveCores = false; }
                            if (!dragActiveCores)
                            {
                                GlobalVariables.needActiveCoreRead = true;
                                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeActiveCores((int)sliderValue));
                            }
                        
                        }
                        break;
                    default:
                        break;
                }
            }


        }
        private void HandleChangingTDP(int tdpPL1, int tdpPL2, bool PL1started)
        {

            GlobalVariables.needTDPRead = true;
            Thread.Sleep(150);
            if (PL1started)
            {
                //If PL1 is greater than PL2 then PL2 needs to be set to the PL1 value

                if (tdpPL1 < tdpPL2) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2)); }
                else
                {
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
                    tdpPL1 = tdpPL2;
                    Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2));
                };
            }


        }


        private void HandleChangingGPUCLK(int gpuclk)
        {
            if (this.IsLoaded)
            {

                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(gpuclk));

            }

        }


        private void Resolution_Cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalVariables.resolution != Resolution_Cbo.SelectedItem && !changingResolution && Resolution_Cbo.SelectedItem != "Custom Scaling")
            { PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayResolution(Resolution_Cbo.SelectedItem.ToString()); }
        }

        private void RefreshRate_Cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GlobalVariables.refreshRate != RefreshRate_Cbo.SelectedItem && !changingRefreshRate)
            { PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayRefreshRate(RefreshRate_Cbo.SelectedItem.ToString()); }
        }

        private void Scaling_Cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!changingScaling && Scaling_Cbo.SelectedItem.ToString() != "Default")
            {
                changingScaling = true;
                PowerControlPanel.Classes.ChangeDisplaySettings.ChangeDisplaySettings.SetDisplayScaling(Scaling_Cbo.SelectedItem.ToString());
                changingScaling = false;
            }

        }
        private void Profile_Cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!changingProfiles)
            {
                if (Profile_Cbo.SelectedValue.ToString() != "None")
                {
                    PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.applyProfile(Profile_Cbo.SelectedValue.ToString());
                }
                else
                {
                    PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.applyProfile("None");
                }
            }
        }
        private void FPSLimit_Cbo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.changeLimit(FPSLimit_Cbo.SelectedValue.ToString());
        }

        private void setThumbSize(Slider slider)
        {
            //set thumb size, internet routine
            var SliderThumb = GetElementFromParent(slider as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {

                if (SliderThumb is Thumb thumb)
                {

                    thumb.Width = 25;
                    thumb.Height = 35;
                }
                else { }
            }
            else { }
        }
    }

 

}
