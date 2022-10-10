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
        private bool dragStarted = false;


        private Brush accentBrush = null;
        
        public QAMHomePage()
        {
            InitializeComponent();

            
            //apply theme
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;

            //set combobox sources
            setComboBoxItemSource();

            //set upper and lower limits on sliders
            setMinMaxSliderValues();

            //set visibility
            setInitialVisibility();

            //
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
            Resolution_Cbo.ItemsSource= GlobalVariables.resolutions;
            RefreshRate_Cbo.ItemsSource =GlobalVariables.refreshRates;
            Profile_Cbo.ItemsSource = PowerControlPanel.Classes.ManageXML.ManageXML_Profiles.profileListForHomePage();
        }

        private void setInitialVisibility()
        {
            //get accentbrush
            accentBrush = Profile_Tile.Background;
            //hide tile and sliders if setting is disabled
            if (!Properties.Settings.Default.enableDisplay)
            {
                RefreshRate_Border.Visibility = Visibility.Collapsed;
                Resolution_Border.Visibility = Visibility.Collapsed;
                Scaling_Border.Visibility = Visibility.Collapsed;
                Display_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableCPU)
            {
                MaxCPU_Border.Visibility = Visibility.Collapsed;
                ActiveCores_Border.Visibility = Visibility.Collapsed;
                CPU_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableVolume)
            {
                Volume_Border.Visibility = Visibility.Collapsed;
                Volume_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableBrightness)
            {
                Brightness_Tile.Visibility = Visibility.Collapsed;
                Brightness_Border.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableTDP)
            {
                TDP1_Border.Visibility = Visibility.Collapsed;
                TDP2_Border.Visibility = Visibility.Collapsed;
                TDP_Border.Visibility = Visibility.Collapsed;
                TDP_Tile.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.enableGPUCLK)
            {
                AMDGPUCLK_Border.Visibility = Visibility.Collapsed;
                AMD_Tile.Visibility = Visibility.Collapsed;
            }

            //hide just sliders if you don't want it showing
            if (!Properties.Settings.Default.showDisplay)
            {
                RefreshRate_Border.Visibility = Visibility.Collapsed;
                Resolution_Border.Visibility = Visibility.Collapsed;
                Scaling_Border.Visibility = Visibility.Collapsed;
                Display_Tile.Background = Brushes.Gray;
            }

            if (!Properties.Settings.Default.showCPU)
            {
                MaxCPU_Border.Visibility = Visibility.Collapsed;
                ActiveCores_Border.Visibility = Visibility.Collapsed;
                CPU_Tile.Background = Brushes.Gray;
            }

            if (!Properties.Settings.Default.showVolume)
            {
                Volume_Border.Visibility = Visibility.Collapsed;
                Volume_Tile.Background = Brushes.Gray;
            }

            if (!Properties.Settings.Default.showBrightness)
            {
                Brightness_Border.Visibility = Visibility.Collapsed;
                Brightness_Tile.Background = Brushes.Gray;
            }

            if (!Properties.Settings.Default.showTDP)
            {
                TDP1_Border.Visibility = Visibility.Collapsed;
                TDP2_Border.Visibility = Visibility.Collapsed;
                TDP_Border.Visibility = Visibility.Collapsed;
                TDP_Tile.Background = Brushes.Gray;
            }
            if (Properties.Settings.Default.enableCombineTDP=="Enable")
            {
                TDP1_Border.Visibility = Visibility.Collapsed;
                TDP2_Border.Visibility = Visibility.Collapsed;
            }
            else
            {
                TDP_Border.Visibility = Visibility.Collapsed;
            }

            if (!Properties.Settings.Default.showGPUCLK)
            {
                AMDGPUCLK_Border.Visibility = Visibility.Collapsed;
                AMD_Tile.Background = Brushes.Gray;
            }

            if (!PowerControlPanel.Classes.ChangeFPSLimit.ChangeFPSLimit.rtssRunning())
            {
                FPSLimit_Border.Visibility = Visibility.Collapsed;
                FPSLimit_Tile.Visibility = Visibility.Collapsed;
            }
     

            //hide stuff that is cpu specific
            if (GlobalVariables.cpuType == "Intel")
            {
                AMD_Tile.Visibility = Visibility.Collapsed;
                AMD_Tile.Visibility = Visibility.Collapsed;
            }
            if (GlobalVariables.cpuType == "AMD")
            {
                Intel_Tile.Visibility = Visibility.Collapsed;
                //place holder for future controls
            }



        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //set height of scrollviewer
            sliderScrollViewer.Height = this.ActualHeight - wrapPanel.ActualHeight;

        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            //handles all tile click events
            Tile tile = (Tile)sender;
            string tileName = tile.Name;
            Brush backgroundBrush = accentBrush;
            switch (tileName)
            {
                case "TDP_Tile":
                    if (Properties.Settings.Default.enableCombineTDP == "Enable")
                    {
                        if (TDP_Border.Visibility == Visibility.Collapsed)  
                        {
                            TDP_Slider.Value = GlobalVariables.readPL1;
                            TDP_Border.Visibility = Visibility.Visible;
                            Properties.Settings.Default.showTDP = true;
                            TDP_Tile.Background = backgroundBrush;
                        }
                        else 
                        { 
                            TDP_Border.Visibility = Visibility.Collapsed; 
                            Properties.Settings.Default.showTDP = false;
                            TDP_Tile.Background = Brushes.Gray;
                        }
                    }
                    else
                    {
                        if (TDP1_Border.Visibility == Visibility.Collapsed) 
                        {
                            TDP1_Slider.Value = GlobalVariables.readPL1;
                            TDP2_Slider.Value = GlobalVariables.readPL2;
                            TDP1_Border.Visibility = Visibility.Visible;
                            TDP2_Border.Visibility = Visibility.Visible;
                            Properties.Settings.Default.showTDP = true;
                            TDP_Tile.Background = backgroundBrush;
                        }
                        else 
                        { 
                            TDP1_Border.Visibility = Visibility.Collapsed;
                            TDP2_Border.Visibility = Visibility.Collapsed;
                            Properties.Settings.Default.showTDP = false;
                            TDP_Tile.Background = Brushes.Gray;
                        }
                    }
                    break;
                case "Volume_Tile":
                    if (Volume_Border.Visibility == Visibility.Collapsed)
                    {
                        VolumeSlider.Value = GlobalVariables.volume;
                        Volume_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showVolume = true;
                        Volume_Tile.Background = backgroundBrush;
                    }
                    else
                    {
                        Volume_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showVolume = false;
                        Volume_Tile.Background = Brushes.Gray;  
                    }
                    break;
                case "Brightness_Tile":
                    if (Brightness_Border.Visibility == Visibility.Collapsed)
                    {
                        BrightnessSlider.Value = GlobalVariables.volume;
                        Brightness_Border.Visibility = Visibility.Visible;
                        Properties.Settings.Default.showBrightness = true;
                        Brightness_Tile.Background = backgroundBrush;
                    }
                    else
                    {
                        Brightness_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showBrightness = false;
                        Brightness_Tile.Background = Brushes.Gray;
                    }
                    break;
                case "Display_Tile":
                    if (Resolution_Border.Visibility == Visibility.Collapsed)
                    {
                        Resolution_Cbo.Text = GlobalVariables.resolution;
                        RefreshRate_Cbo.Text = GlobalVariables.refreshRate;
                        Scaling_Cbo.Text = GlobalVariables.scaling;
                        Resolution_Border.Visibility = Visibility.Visible;
                        RefreshRate_Border.Visibility = Visibility.Visible;
                        Scaling_Border.Visibility= Visibility.Visible;

                        Properties.Settings.Default.showDisplay = true;
                        Display_Tile.Background = backgroundBrush;
                    }
                    else
                    {
                        Resolution_Border.Visibility = Visibility.Collapsed;
                        RefreshRate_Border.Visibility = Visibility.Collapsed;
                        Scaling_Border.Visibility = Visibility.Collapsed;
                        Properties.Settings.Default.showDisplay = false;
                        Display_Tile.Background= Brushes.Gray;
                    }
                    break;
                default:
                    break;


            }
            Properties.Settings.Default.Save();
        }

    

        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            //set thumb size, internet routine
            var SliderThumb = GetElementFromParent(sender as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
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

    }

}
