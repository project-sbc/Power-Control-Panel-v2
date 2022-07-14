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
using System.Threading;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;
using Power_Control_Panel.PowerControlPanel.Classes.StartUp;
using Power_Control_Panel.PowerControlPanel.Classes;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;
using SharpDX.XInput;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class GlobalVariables
    {
        //TDP global
        public static double readPL1 = 0;
        public static double readPL2 = 0;
        public static double setPL1 = 0;
        public static double setPL2 = 0;
        public static bool needTDPRead = false;
        
        //System global
        public static int batteryPercentage = 0;
        public static string powerStatus = "None";
        public static string internetDevice = "Not Connected";

        //Controller global
        public static Controller? controller = new Controller(UserIndex.One);
        public static Gamepad gamepadOld = controller.GetState().Gamepad;
        public static Gamepad gamepadCurrent = controller.GetState().Gamepad;

        //Shut down boolean to stop threads
        public static bool useControllerFastThread = true;


        //brightness and volume setting
        public static int brightness = 0;
        public static int volume = 0;
    }

    public partial class MainWindow : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;

        public DispatcherTimer inputCheck=new DispatcherTimer();
        public int counter = 0;
        public static Window overlay;
        public static Window osk;
        public MainWindow()
        {
            this.InitializeComponent();

            StartUp.runStartUp();
            
  
            //Run code to set up hamburger menu
            initializeNavigationFrame();

            //Run code to set up dispatch timers, one for inputcheck (i.e. xinput or keyboard prompts) and one for updating TDP values
            initializeDispatchTimersAndBackgroundThread();



        }

       
        void initializeDispatchTimersAndBackgroundThread()
        {
 
            //Set up timespan for timers
            inputCheck.Interval= new TimeSpan(0, 0, 1); 
       
            //Add the event handlers to the timers
            inputCheck.Tick += inputCheck_Tick;
            
            //Start timers
            inputCheck.Start();


            
        }

        void inputCheck_Tick(object sender, EventArgs e)
        {
            //Handle routine object checks like TDP, battery percentage, 
            RoutineUpdate.handleRoutineChecks(counter);
            //Consolidate checks into one timer, reset after 60 ticks/seconds
            if (counter > 60) { counter = 0; }else { counter++; }

         
            //Add overlay open
            if (GlobalVariables.gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.DPadLeft) && GlobalVariables.gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && GlobalVariables.gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
            {
                if (overlay == null) { overlay = new QuickAccessMenu(); overlay.Show(); } else { overlay.Show(); } 
            }
     

        }

      
      
        //Navigation routines
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
                if (menuItem.Label == "Overlay")
                {
                    overlay = new QuickAccessMenu();
                    overlay.Show();

                }
                if (menuItem.Label == "On Screen Keyboard")
                {
                    osk = new OSK();
                    RoutineUpdate.sleepTimer = 10;
                    osk.Show();

                }
                if (menuItem.IsNavigation)
                {
                    this.navigationServiceEx.Navigate(menuItem.NavigationDestination);
                }

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
            GlobalVariables.useControllerFastThread = false;
        }




    }
}