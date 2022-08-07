using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using MenuItem = Power_Control_Panel.PowerControlPanel.Classes.ViewModels.MenuItem;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Threading;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;
using Power_Control_Panel.PowerControlPanel.Classes.StartUp;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;
using SharpDX.XInput;
using ControlzEx.Theming;
using System.IO;
using System.Management;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class GlobalVariables
    {

        //Processor global
        public static string cpuType = "";

        //TDP global
        public static double readPL1 = 0;
        public static double readPL2 = 0;
        public static double setPL1 = 0;
        public static double setPL2 = 0;
        public static bool needTDPRead = false;

        //AMD GPU CLOCK
        public static string gpuclk = "Default";

        //Shut down boolean to stop threads
        public static bool useRoutineThread = true;


        //brightness and volume setting
        public static int brightness = 0;
        public static int volume = 0;
        public static bool needVolumeRead = false;
        public static bool needBrightnessRead = false;

        //cpu settings
        public static int cpuMaxFrequency = 0;
        public static int cpuActiveCores = 0;
        public static int maxCpuCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
        public static int baseCPUSpeed = 1000;




        //Profile and app settings
        public static string ActiveProfile = "None";
        public static string DefaultProfile = "None";
        public static string ActiveApp = "None";
        public static string powerStatus = "";

        //display settings
        public static string resolution = "";
        public static string refreshRate = "";
        public static string scaling = "Default";

        public static List<string> resolutions = new List<string>();
        public static List<string> refreshRates = new List<string>();
        //TDP change class
        public static PowerControlPanel.Classes.ChangeTDP.ChangeTDP tdp = new PowerControlPanel.Classes.ChangeTDP.ChangeTDP();

        //Routine update class
        public static RoutineUpdate routineUpdate = new RoutineUpdate();

        public static string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "\\PowerControlPanel\\ProfileData\\Profiles.xml";
    }
    

    public partial class MainWindow : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;
        private DispatcherTimer timer = new DispatcherTimer();
        
        private Controller controller;
        private Gamepad gamepad;

        private string theme = Properties.Settings.Default.systemTheme;

        public static Window overlay;
        public static Window osk;

        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {

            StartUp.runStartUp();

            this.InitializeComponent();

            

       

            GlobalVariables.routineUpdate.startThread();

            //Run code to set up hamburger menu
            initializeNavigationFrame();

            initializeTimer();


            //set theme
            setTheme();

            //test code here
            

        }

        private void setUpNotifyIcon()
        {
            notifyIcon.Click += notifyIcon_Click;
            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(AppDomain.CurrentDomain.BaseDirectory + "\\Power Control Panel.exe");


            if (String.Equals("C:\\Windows\\System32", Directory.GetCurrentDirectory(), StringComparison.OrdinalIgnoreCase))
            {

                
                this.ShowInTaskbar = false;
                notifyIcon.Visible = true;
            
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }

            

        }
      
        private void notifyIcon_Click(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
            notifyIcon.Visible = false;
            this.ShowInTaskbar = true;

        }

        private void setTheme()
        {
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }
        private void initializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timerTick;
            timer.Start();

        }
        private void timerTick(object sender, EventArgs e)
        {
            //Controller input handler
            controller = new Controller(UserIndex.One);
            if (controller != null)
            { 
                if (controller.IsConnected)
                {
                    gamepad = controller.GetState().Gamepad;

                    if (gamepad.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && gamepad.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                    {
                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadRight))
                        {
                            handleOpenCloseQAM();

                        }
                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.DPadDown))
                        {
                            handleOpenCloseOSK();

                        }
                    }

                }
            }


            //Theme manager
            if (theme != Properties.Settings.Default.systemTheme)
            {
                setTheme();
                theme = Properties.Settings.Default.systemTheme;
            }



            //Profile or power status change updater
            string Power = SystemParameters.PowerLineStatus.ToString();
            if (Power != GlobalVariables.powerStatus & GlobalVariables.powerStatus != "" & GlobalVariables.ActiveProfile != "None")
            {
                MessageBox.Show("run change profile here");

            }

        }
        private void OSKEvent(object sender, EventArgs e)
        {
            handleOpenCloseOSK();

        }

        private void QAMEvent(object sender, EventArgs e)
        {

            handleOpenCloseQAM();
        }

        //Navigation routines
        #region navigation
        void initializeNavigationFrame()
        {
            navigationServiceEx = new NavigationServiceEx();
            navigationServiceEx.Navigated += this.NavigationServiceEx_OnNavigated;
            HamburgerMenuControl.Content = this.navigationServiceEx.Frame;
            // Navigate to the home page.
            this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {

            if (e.InvokedItem is MenuItem menuItem)
            {
                if (menuItem.Label == "Quick Access Menu")
                {

                    handleOpenCloseQAM();
                }
                if (menuItem.Label == "On Screen Keyboard")
                {
                    handleOpenCloseOSK();

                }
                if (menuItem.IsNavigation)
                {
                    this.navigationServiceEx.Navigate(menuItem.NavigationDestination);
                }

            }
        }

        private void handleOpenCloseQAM()
        {
            if (overlay == null)
            {
                overlay = new QuickAccessMenu();
                overlay.Show();
            }
            else
            {
                overlay.Close();
            }

        }
        private void handleOpenCloseOSK()
        {
            if (osk == null)
            {
                osk = new OSK();
                osk.Show();
            }
            else
            {
                osk.Close();
            }

        }
        private void NavigationServiceEx_OnNavigated(object sender, NavigationEventArgs e)
        {
            // select the menu item
            this.HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedItemProperty,
                this.HamburgerMenuControl.Items
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));
            this.HamburgerMenuControl.SetCurrentValue(HamburgerMenu.SelectedOptionsItemProperty,
                this.HamburgerMenuControl
                    .OptionsItems
                    .OfType<MenuItem>()
                    .FirstOrDefault(x => x.NavigationDestination == e.Uri));

            // or when using the NavigationType on menu item
            // this.HamburgerMenuControl.SelectedItem = this.HamburgerMenuControl
            //                                              .Items
            //                                              .OfType<MenuItem>()
            //                                              .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());
            // this.HamburgerMenuControl.SelectedOptionsItem = this.HamburgerMenuControl
            //                                                     .OptionsItems
            //                                                     .OfType<MenuItem>()
            //                                                     .FirstOrDefault(x => x.NavigationType == e.Content?.GetType());

            // update back button
            this.GoBackButton.SetCurrentValue(VisibilityProperty, this.navigationServiceEx.CanGoBack ? Visibility.Visible : Visibility.Collapsed);
        }

        private void GoBack_OnClick(object sender, RoutedEventArgs e)
        {
            this.navigationServiceEx.GoBack();

        }

        //End navigation routines

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Close overlay when main window is closed
            if (overlay != null) { overlay.Close(); }
            if (osk != null) { osk.Close(); }
            
            // Dispose of thread to allow program to close properly
            PowerControlPanel.Classes.TaskScheduler.TaskScheduler.closeScheduler();
            //Throw boolean to end controller checking thread
            GlobalVariables.useRoutineThread = false;
        }


        #endregion navigation


        private void MetroWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                notifyIcon.Visible = true;
                this.ShowInTaskbar = false;
      

            }
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            setUpNotifyIcon();
        }
    }
}