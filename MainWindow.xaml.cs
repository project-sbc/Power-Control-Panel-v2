using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;

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
using System.Windows.Input;
using System.Diagnostics;
using System.Text;
using System.Data;
using Power_Control_Panel.PowerControlPanel.Classes.ManageXML;
using AutoUpdaterDotNET;

using System.Net;
using System.Windows;
using System.Windows.Forms;
//using RTSSSharedMemoryNET;


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
        public static int maxCpuCores = 1;
        public static int baseCPUSpeed = 1000;
        public static bool needCPUMaxFreqRead = false;
        public static bool needActiveCoreRead = false;
        //RTSS fps limit
        public static string FPSLimit = "Unlocked";

        //Profile and app settings
        public static string ActiveProfile = "None";
        public static string ActiveApp = "None";
        public static string powerStatus = "";
        public static bool updateProfileAppTable = false;
        

        //display settings
        public static string resolution = "";
        public static string refreshRate = "";
        public static string scaling = "Default";

        public static List<string> resolutions = new List<string>();
        public static List<string> refreshRates = new List<string>();
        public static List<string> scalings = new List<string>();
        public static List<string> FPSLimits = new List<string>();
        public static List<string> FanModes = new List<string>();
        //TDP change class
        public static PowerControlPanel.Classes.ChangeTDP.ChangeTDP tdp = new PowerControlPanel.Classes.ChangeTDP.ChangeTDP();

        //Routine update class
        public static RoutineUpdate routineUpdate = new RoutineUpdate();

        public static string xmlFile = AppDomain.CurrentDomain.BaseDirectory + "\\PowerControlPanel\\ProfileData\\Profiles.xml";

        //Motherboard info
        public static string manufacturer = "";
        public static string product = "";

        //fan controls
        public static bool fanControlDevice = false;
        public static bool fanControlEnable = false;
        public static string fanControlMode = "Hardware";
        public static int fanRangeBase = 100;
        public static int fanSpeed = 0;

        //cpu values
        public static double cpuTemp = 0;

        //


        //language pack
        public static ResourceDictionary languageDict = new ResourceDictionary();


        //close window to update from settings update button
        public static bool closeForUpdate = false;
    }
    

    public partial class MainWindow : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;
        private DispatcherTimer timer = new DispatcherTimer();
        private int timercounter = 0;

        public Controller controller;
        public Gamepad gamepad;

        private string theme = Properties.Settings.Default.systemTheme;

        public static Window overlay;
        public static Window osk;

        //make date time variable to check if system went to sleep.The datetime should always be less than 10 seconds. when its greater than it knows the computer went to sleep
        private DateTime timeSleepCheck;

        //notify icon for task tray icon when minimized
        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();

        public MainWindow()
        {
            
            StartUp.runStartUp();

            this.InitializeComponent();

            GlobalVariables.routineUpdate.startThread();

            //Run code to set up hamburger menu
            initializeNavigationFrame();

            initializeTimer();

            //set time for sleep check, this is updated every couple seconds. If computer goes to sleep the delta time between now and last update will change drastically
            timeSleepCheck = DateTime.Now;

            //set theme
            setTheme();

            //force touch due to wpf bug 
            _ = Tablet.TabletDevices;

            //test code here
            //RunCLI.RunCommand(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile + ""));
            //System.Windows.Forms.MessageBox.Show(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "\\Playnite\\Playnite.FullscreenApp.exe --start"));
           
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
            //set theme, is called at load and when theme is changed in settings. IMPORTANT TO KEEP AS SEPARATE ROUTINE
            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }
        private void initializeTimer()
        {
            timer.Interval = TimeSpan.FromMilliseconds(15);
            timer.Tick += timerTick;
            timer.Start();
        }
        private void getController()
        {
            int controllerNum = 1;
            //get controller used, loop controller number if less than 5, so if controller is connected make num = 5 to get out of while loop
            while (controllerNum <5)
            {
                switch (controllerNum)
                {
                    default:
                        break;
                    case 1:
                        controller = new Controller(UserIndex.One);
                        break;
                    case 2:
                        controller = new Controller(UserIndex.Two);
                        break;
                    case 3:
                        controller = new Controller(UserIndex.Three);
                        break;
                    case 4:
                        controller = new Controller(UserIndex.Four);
                        break;
                }           
                if (controller == null )
                {
                    controllerNum++;
                }
                else
                {
                    if (controller.IsConnected)
                    {
                        controllerNum = 5;
                    }
                    else
                    {
                        controllerNum++;
                    }
                }
            }
        }

        Dictionary<string, GamepadButtonFlags> buttonLookUp =
    new Dictionary<string, GamepadButtonFlags>()
    {
           {"A", GamepadButtonFlags.A },
           {"B", GamepadButtonFlags.B },
           {"DPadUp", GamepadButtonFlags.DPadUp },
           {"DPadDown", GamepadButtonFlags.DPadDown },
           {"DPadLeft", GamepadButtonFlags.DPadLeft },
           {"DPadRight", GamepadButtonFlags.DPadRight},
           {"LB", GamepadButtonFlags.LeftShoulder },
           {"RB", GamepadButtonFlags.RightShoulder },
           {"L3", GamepadButtonFlags.LeftThumb },
           {"R3", GamepadButtonFlags.RightThumb },
           {"X", GamepadButtonFlags.X },
           {"Y", GamepadButtonFlags.Y},
           {"Start", GamepadButtonFlags.Start },
           {"Back", GamepadButtonFlags.Back },
     };

        private bool ButtonComboPress(Gamepad gp,string BC)
        {
            //make default false
            bool result = false;

            //split string into array
            

            if (BC.IndexOf("+") > 0)
            {
                string[] strBC = BC.Split('+');

                //gamepad flag array
                GamepadButtonFlags[] ButtonCombo = new GamepadButtonFlags[strBC.Length];
                int intCount = 0;

                foreach (var bc in strBC)
                {
                    //for each string lookup in the dictionary for the gamepad flag
                    ButtonCombo[intCount] = buttonLookUp[bc];
                    intCount++;
                }

                //set bool to true
                bool oldGamepadPress = true;
                bool GamepadPress = true;
                foreach (GamepadButtonFlags button in ButtonCombo)
                {
                    //the press scenario is defined when the old gamepad state doesn't have all buttons pressed but the new state does. This will turn the bool false if any of the buttons arent pressed
                    if (!gamepad.Buttons.HasFlag(button))
                    {
                        oldGamepadPress = false;
                    }
                    if (!gp.Buttons.HasFlag(button))
                    {
                        GamepadPress = false;
                    }
                }
                //button press is true when current gamepadpress is true and the old gamepad state is false
                if (!oldGamepadPress && GamepadPress)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }



            }
  


            return result;

        }

        private void timerTick(object sender, EventArgs e)
        {

            //Controller input handler
            if (controller == null) 
            { 
                getController(); 
                if (controller.IsConnected == false)
                {
                    getController();
                }
            }
            else if (!controller.IsConnected)
            {
                getController();
            }


            if (controller != null)
            { 
                if (controller.IsConnected)
                {
                    Gamepad currentGamepad = controller.GetState().Gamepad;

                    if (ButtonComboPress(currentGamepad,Properties.Settings.Default.qamButtonCombo))
                    {
                        handleOpenCloseQAM();
                    }
                    if (ButtonComboPress(currentGamepad, Properties.Settings.Default.oskButtonCombo))
                    {
                        handleOpenCloseOSK();
                        
                    }
                    if (ButtonComboPress(currentGamepad, Properties.Settings.Default.fsrButtonCombo))
                    {
                        PowerControlPanel.Classes.EnableFSR.EnableFSR.enableDisableFSR();
                    }
                    if (ButtonComboPress(currentGamepad, Properties.Settings.Default.gameLauncherButtonCombo))
                    {
                        if (Properties.Settings.Default.gameLauncher == "Steam")
                        {
                            PowerControlPanel.Classes.Steam.Steam.openSteamBigPicture();
                        }
                        if (Properties.Settings.Default.gameLauncher == "PlayNite")
                        {
                            PowerControlPanel.Classes.Playnite.Playnite.playniteToggle();
                        }
               
                    }

                    //set currentgamepad snapshot to global gamepad for comparison
                    gamepad = currentGamepad;
                    if (1==0)
                    {

                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.B))
                        {
                           
                            //GlobalVariables.tdp.changeTDP((int)GlobalVariables.setPL1 + 1, (int)GlobalVariables.setPL2 + 1);
                        }
                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.A))
                        {
                            GlobalVariables.tdp.changeTDP((int)GlobalVariables.setPL1 - 1, (int)GlobalVariables.setPL2 - 1);
                        }
                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.Y))
                        {
                            PowerControlPanel.Classes.ChangeBrightness.WindowsSettingsBrightnessController.setBrightness(GlobalVariables.brightness + 10);
                        }
                        if (gamepad.Buttons.HasFlag(GamepadButtonFlags.X))
                        {
                            PowerControlPanel.Classes.ChangeBrightness.WindowsSettingsBrightnessController.setBrightness(GlobalVariables.brightness - 10);
                        }
                    }

                }

            }
            if (timercounter > 60)
            {
                if (GlobalVariables.closeForUpdate)
                { this.Close(); }


                //Theme manager, set theme if changed in settings
                if (theme != Properties.Settings.Default.systemTheme)
                {
                    setTheme();
                    theme = Properties.Settings.Default.systemTheme;
                }

                //call routine to check if profile needs to be changed because of app running or power change
                PowerControlPanel.Classes.TaskScheduler.TaskScheduler.runTask(() => handleRoutineProfileChecker());
                
                timercounter = 0;


            }
            else { timercounter++; }



        }

        private void handleRoutineProfileChecker()
        {

            //------------------------------------ Profile change----------------------------------


            //Profile or power status change updater
            string Power = SystemParameters.PowerLineStatus.ToString();

            string profileCase = "";
            string profile = "";
            string exe = "";
            string setProfile = "";
            string setApp = "";
            string lookUpProfileByActiveApp = ManageXML_Apps.lookupProfileByAppExe(GlobalVariables.ActiveApp);
            //look up all profiles to apps in XML and put in table
            DataTable dtApps = ManageXML_Apps.appListProfileExe();

            foreach (DataRow dr in dtApps.Rows)
            {
                profile = dr[0].ToString();
                exe = dr[1].ToString();

                Process[] pname = Process.GetProcessesByName(exe);
                if (pname.Length != 0 && exe != "")
                {
                    setProfile = profile;
                    setApp = exe;
                    if (exe == GlobalVariables.ActiveApp)
                    {
                        //if exe name matches current active app then break so that app is always the one picked
                        break;
                    }

                }
            }


            //big split, is there an active app already?
            if (GlobalVariables.ActiveApp != "None")
            {
                //if there is active app

                //if active app profile changed in settings while running it
                if (profileCase == "" && setApp == GlobalVariables.ActiveApp && setProfile != GlobalVariables.ActiveProfile)
                { profileCase = "Apply Profile"; }

                //if power changed but set app and active app are same, reapply profile
                if (profileCase == "" && setApp == GlobalVariables.ActiveApp && Power != GlobalVariables.powerStatus && GlobalVariables.ActiveApp != "None")
                { profileCase = "Reapply Profile"; }

                //if active app closes and no new app opens
                if (profileCase == "" && setApp == "" && GlobalVariables.ActiveProfile != "Default")
                { profileCase = "Remove Profile"; }

                //if active app closes and new app is detected
                if (profileCase == "" && setApp != "" && setApp != GlobalVariables.ActiveApp)
                { profileCase = "Apply Profile"; }


            }
            else
            {
                //if there no active app 

                //if there was a detected app with an associated profile (setApp and setProfile arent null anymore), apply it
                if (profileCase == "" && setApp != "" && setProfile != "")
                { profileCase = "Apply Profile"; }


                //if there is an active profile and the power changed, then reapply profile
                if (profileCase == "" && GlobalVariables.ActiveProfile != "None" && Power != GlobalVariables.powerStatus)
                { profileCase = "Reapply Profile"; }


            }

            TimeSpan timeDifference = DateTime.Now.Subtract(timeSleepCheck);

            if (timeDifference.Seconds > 15)
            {
                if (profileCase == "" && GlobalVariables.ActiveProfile != "None")
                {
                    profileCase = "Reapply Profile";
                }
                if (profileCase == "" && GlobalVariables.ActiveProfile == "None")
                {
                    profileCase = "Default Profile";
                }
                
            }
            //set sleep check clock
            timeSleepCheck = DateTime.Now;
            

            GlobalVariables.powerStatus = Power;
            //scenarios
            //program opens,  key indicator is  




            switch (profileCase)
            {
                default:
                    break;
                case "Default Profile":
                    ManageXML_Profiles.applyProfile("Default");
                    break;
                case "Reapply Profile":
                    ManageXML_Profiles.applyProfile(GlobalVariables.ActiveProfile);
                    break;
                case "Apply Profile":
                    GlobalVariables.ActiveApp = setApp;
                    ManageXML_Profiles.applyProfile(setProfile);

                    break;
                case "Remove Profile":
                    //if no default profile exists, active profile of none is applied
                    GlobalVariables.ActiveApp = "None";
                    ManageXML_Profiles.applyProfile("Default");
                    break;
            }


        }

        private void OSKEvent()
        {
            handleOpenCloseOSK();
            

        }

        private void QAMEvent()
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
                
                if (menuItem.Label == "Quick Access Menu" || menuItem.Label == "接触选单")
                {

                    handleOpenCloseQAM();
           


                }
                if (menuItem.Label == "On Screen Keyboard" || menuItem.Label == "视窗键盘")
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