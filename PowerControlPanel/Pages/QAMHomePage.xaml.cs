using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public QAMHomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
            initializeTimer();
            loadUpdateValues();
        }


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


            //max cpu clock updates
            if (GlobalVariables.cpuMaxFrequency == 0)
            {
                labelMAXCPUValue.Content = "Auto";
            }
            else { labelMAXCPUValue.Content = GlobalVariables.cpuMaxFrequency + " MHz"; }



            //active core updates
            labelActiveCoresValue.Content = GlobalVariables.cpuActiveCores;


            //display updates


            //system values
            labelBrightnessValue.Content = GlobalVariables.brightness + " %";
            labelVolumeValue.Content = GlobalVariables.volume + " %";

            //TPD
            labelTDPBoostValue.Content = GlobalVariables.readPL2.ToString() + " W";
            labelTDPSustainValue.Content = GlobalVariables.readPL1.ToString()+ " W";
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile tile = (Tile)sender;
            currentControl = tile.Title;
            GBChangeValue.Visibility = Visibility.Visible;
            controlActive = false;
            switch (currentControl)
            {
                case ("TDP Sustain"):
                    generalSlider.Minimum = 5;
                    generalSlider.Maximum = Properties.Settings.Default.maxTDP;
                    generalSlider.Value = GlobalVariables.readPL1;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;

                case "TDP Boost":
                    generalSlider.Minimum = 5;
                    generalSlider.Maximum = Properties.Settings.Default.maxTDP;
                    generalSlider.Value = GlobalVariables.readPL2;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;
                case "Brightness":
                    generalSlider.Minimum = 1;
                    generalSlider.Maximum = 100;
                    generalSlider.Value = GlobalVariables.brightness;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;

                case "Volume":
                    generalSlider.Minimum = 1;
                    generalSlider.Maximum = 100;
                    generalSlider.Value = GlobalVariables.volume;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;
                case "GPUCLK":
                    generalSlider.Minimum = 300;
                    generalSlider.Maximum = Properties.Settings.Default.maxGPUCLK;
                    //generalSlider.Value = GlobalVariables.gpuclk;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;
                case "Max CPU Freq":
                    generalSlider.Minimum = GlobalVariables.baseCPUSpeed;
                    generalSlider.Maximum = 5000;
                    if (GlobalVariables.cpuMaxFrequency == 0)
                    {
                        generalSlider.Value = generalSlider.Maximum;
                    }
                    else
                    { generalSlider.Value = GlobalVariables.cpuMaxFrequency; }
                    generalSlider.SmallChange = 100;
                    generalSlider.LargeChange = 100;
                    labelSlider.Content = currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    break;
                case "Active CPU Cores":

                    break;
                default:
                    break;


            }

        }
        private void clearSlider()
        {
            dpSlider.Visibility = Visibility.Collapsed;
            labelSlider.Content = "";
            generalSlider.Minimum = 1;
            generalSlider.Maximum = 100;
            generalSlider.Value = 1;
            labelSliderMessage.Visibility = Visibility.Collapsed;



        }

        private void configureSlider(string labelContent)
        {


            dpSlider.Visibility = Visibility.Visible;
        }
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

        
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;

         

            if (this.IsLoaded)
            {
                switch (currentControl)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }

        }


        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }
        }


        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }
        }


        private void Slider_TouchUp(object sender, TouchEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }
        }

        private void Slider_TouchDown(object sender, TouchEventArgs e)
        {
            Slider slider = sender as Slider;

            string sliderName = slider.Name;

            if (this.IsLoaded)
            {
                switch (sliderName)
                {
                    case "TDP Sustain":

                        break;
                    case "TDP Boost":

                        break;
                    case "Brightness":

                        break;

                    case "Volume":

                        break;
                    case "GPUCLK":

                        break;
                    case "Max CPU Freq":

                        break;
                    case "Active CPU Cores":

                        break;
                    default:
                        break;
                }

            }

        }

        private void enableGroup_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.IsLoaded)
            {
                ToggleSwitch toggle = sender as ToggleSwitch;
                GroupBox groupBox = null;
                if (toggle.Name == "enablePowerControl")
                {
                    groupBox = GBSystemControls;

                }

                if (groupBox != null)
                {
                    if (toggle.IsOn) { groupBox.Height = double.NaN; }
                    else { groupBox.Height = 40; }

                }
            }


        }
     
    }
}
