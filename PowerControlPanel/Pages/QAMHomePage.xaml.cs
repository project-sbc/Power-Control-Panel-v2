using System;
using System.Collections.Generic;
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

        public QAMHomePage()
        {
            InitializeComponent();
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
            initializeTimer();
            loadUpdateValues();
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
            labelGPUCLKValue.Content = GlobalVariables.gpuclk;

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
            controlActive = false;
         

            Tile tile = (Tile)sender;
            currentControl = tile.Title;
            GBChangeValue.Visibility = Visibility.Visible;
            clearSlider();
            switch (currentControl)
            {
                case ("TDP Sustain"):
                    generalSlider.Minimum = 5;
                    generalSlider.Maximum = Properties.Settings.Default.maxTDP;
                    generalSlider.Value = GlobalVariables.readPL1;
                    generalSlider.SmallChange = 1;
                    generalSlider.TickFrequency = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconAwesome.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.BoltSolid;
                    iconAwesome.Visibility = Visibility.Visible;
                    break;

                case "TDP Boost":
                    generalSlider.Minimum = 5;
                    generalSlider.Maximum = Properties.Settings.Default.maxTDP;
                    generalSlider.Value = GlobalVariables.readPL2;
                    generalSlider.SmallChange = 1;
                    generalSlider.TickFrequency = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconAwesome.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.BoltSolid;
                    iconAwesome.Visibility = Visibility.Visible;
                    break;
                case "Brightness":
                    generalSlider.Minimum = 1;
                    generalSlider.Maximum = 100;
                    generalSlider.Value = GlobalVariables.brightness;
                    generalSlider.SmallChange = 1;
                    generalSlider.TickFrequency = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconAwesome.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.SunRegular;
                    iconAwesome.Visibility = Visibility.Visible;
                    break;

                case "Volume":
                    generalSlider.Minimum = 1;
                    generalSlider.Maximum = 100;
                    generalSlider.Value = GlobalVariables.volume;
                    generalSlider.SmallChange = 1;
                    generalSlider.TickFrequency = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconAwesome.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.VolumeUpSolid;
                    iconAwesome.Visibility = Visibility.Visible;
                    break;
                case "GPUCLK":
                    generalSlider.Minimum = 300;
                    generalSlider.Maximum = Properties.Settings.Default.maxGPUCLK;
                    if (GlobalVariables.gpuclk == "Default")
                    {
                        labelSliderMessage.Visibility = Visibility.Visible;
                        labelSliderMessage.Content = "Default";
                    }
                    else
                    { 
                        generalSlider.Value = Int32.Parse(GlobalVariables.gpuclk);
                    }
                    
                    
                    generalSlider.SmallChange = 50;
                    generalSlider.LargeChange = 50;
                    generalSlider.TickFrequency = 50;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    //iconAwesome.Kind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.VolumeUpSolid;
                    //iconAwesome.Visibility = Visibility.Visible;
                    break;
                case "Max CPU Freq":
                    generalSlider.Minimum = GlobalVariables.baseCPUSpeed;
                    generalSlider.Maximum = 5000;
                    if (GlobalVariables.cpuMaxFrequency == 0)
                    {
                        generalSlider.Value = generalSlider.Maximum;
                        labelSliderMessage.Visibility = Visibility.Visible;
                        labelSliderMessage.Content = "Auto";
                        labelSliderValue.Visibility = Visibility.Collapsed;
                    }
                    else
                    { generalSlider.Value = GlobalVariables.cpuMaxFrequency; }
                    generalSlider.SmallChange = 100;
                    generalSlider.LargeChange = 100;
                    generalSlider.TickFrequency = 100;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconMaterial.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Memory;
                    iconMaterial.Visibility = Visibility.Visible;
                    break;
                case "Active CPU Cores":
                    generalSlider.Minimum = 1;
                    generalSlider.Maximum = GlobalVariables.maxCpuCores;
                    generalSlider.Value = GlobalVariables.cpuActiveCores;
                    generalSlider.SmallChange = 1;
                    generalSlider.LargeChange = 1;
                    labelSlider.Content = "Change " + currentControl;
                    Task.Delay(100);
                    controlActive = true;
                    iconMaterial.Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.Memory;
                    iconMaterial.Visibility = Visibility.Visible;
                    break;
                default:
                    break;


            }

        }
        private void clearSlider()
        {

            labelSlider.Content = "";
            generalSlider.Minimum = 1;
            generalSlider.Maximum = 100;
            generalSlider.Value = 1;
            generalSlider.TickFrequency = 1;
            labelSliderMessage.Visibility = Visibility.Collapsed;
            labelSliderValue.Visibility = Visibility.Visible;

            iconAwesome.Visibility = Visibility.Collapsed;
            iconMaterial.Visibility = Visibility.Collapsed;

        }


        private void Slider_Loaded(object sender, RoutedEventArgs e)
        {
            var SliderThumb = GetElementFromParent(sender as DependencyObject, "HorizontalThumb"); //Make sure to put the right name for your slider layout options are: ("VerticalThumb", "HorizontalThumb")
            if (SliderThumb != null)
            {
                if (SliderThumb is Thumb thumb)
                {
                    thumb.Width = 35;
                    thumb.Height = 50;
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
            if (controlActive)
            {
                handleChangeValues();
            }
    
        }

        private void Slider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStarted = false;
            handleChangeValues();

        }


        private void Slider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStarted = true;
        }
        private void Slider_TouchDown(object sender, TouchEventArgs e)
        {
            //dragStarted = true;

        }

        private void Slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //dragStarted = false;
            //handleChangeValues();
        }
        private void handleChangeValues()
        {

            if (this.IsLoaded)
            {
                if (!dragStarted & controlActive)
                {
                    switch (currentControl)
                    {
                        case "TDP Sustain":
                            HandleChangingTDP((int)generalSlider.Value, (int)GlobalVariables.readPL2, true);
                            break;
                        case "TDP Boost":
                            HandleChangingTDP((int)GlobalVariables.readPL1, (int)generalSlider.Value, false);
                            break;
                        case "Brightness":
                            HandleChangingBrightness(generalSlider.Value);
                            break;

                        case "Volume":
                            HandleChangingVolume((int)generalSlider.Value);
                            break;
                        case "GPUCLK":
                            HandleChangingGPUCLK((int)generalSlider.Value);
                            labelSliderMessage.Visibility = Visibility.Collapsed;
                            break;
                        case "Max CPU Freq":
                            HandleChangingMAXCPU((int)generalSlider.Value);
                            break;
                        case "Active CPU Cores":
                            HandleChangingActiveCores((int)generalSlider.Value);
                            break;
                        default:
                            break;
                    }
                    dragStarted = false;
                    Task.Delay(500);
                    GBChangeValue.Visibility = Visibility.Collapsed;
                }
                else
                {
                    switch (currentControl)
                    {

                        case "GPUCLK":

                            labelSliderMessage.Visibility = Visibility.Collapsed;
                            break;
                        case "Max CPU Freq":
                            if (generalSlider.Value == generalSlider.Maximum)
                            {
                                labelSliderMessage.Visibility = Visibility.Visible;
                                labelSliderMessage.Content = "Auto";
                                labelSliderValue.Visibility = Visibility.Collapsed;
                            }
                            else
                            {
                                labelSliderMessage.Visibility = Visibility.Collapsed;

                                labelSliderValue.Visibility = Visibility.Visible;
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
         
        }

        private void Slider_TouchUp(object sender, TouchEventArgs e)
        {
            dragStarted = false;
            //handleChangeValues();
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
                int sendMaxCPU = 0;
                if (maxcpu != generalSlider.Maximum) { sendMaxCPU = maxcpu; }


                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeCPUMaxFrequency(sendMaxCPU));
            }



        }

        private void HandleChangingActiveCores(double cores)
        {
            if (this.IsLoaded)
            {
                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.changeCPU.ChangeCPU.changeActiveCores(cores));
            }



        }


        private void HandleChangingGPUCLK(int gpuclk)
        {
            if (this.IsLoaded)
            {

                Classes.TaskScheduler.TaskScheduler.runTask(() => PowerControlPanel.Classes.ChangeGPUCLK.ChangeGPUCLK.changeAMDGPUClock(gpuclk));

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

        private void button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
   
}
