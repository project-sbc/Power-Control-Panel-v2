using MahApps.Metro.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Power_Control_Panel.PowerControlPanel.Classes.Navigation;
using MenuItem = Power_Control_Panel.PowerControlPanel.Classes.VMQAM.MI;
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

  

    public partial class QuickAccessMenu : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;
        WindowSinker sinker;
        public DispatcherTimer updateTimer = new DispatcherTimer();
        public QuickAccessMenu()
        {
            this.InitializeComponent();

            //Run code to set up hamburger menu
            initializeNavigationFrame();

            initializeWindow();



            updateValues();



            //Set up timespan for timers
            updateTimer.Interval = new TimeSpan(0, 0, 5);

            //Add the event handlers to the timers
            updateTimer.Tick += tick_updateValues;

            //Start timers
            updateTimer.Start();
        }

       void initializeWindow()
        {
            sinker = new WindowSinker(this);
            sinker.Sink();
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;

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
                if (menuItem.Label == "Hide")
                {
                    this.Close();

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
            MainWindow.overlay = null;
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = 0;

            //make height total height - 10%
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - ((System.Windows.SystemParameters.PrimaryScreenHeight) / 15);

        }

        void tick_updateValues(object sender, EventArgs e)
        {
            updateValues();
        }
        void updateValues()
        {
            switch (GlobalVariables.internetDevice)
            {
                case "Not Connected":
                    txtblkInternet.Text = "\uF384";
                    break;
                case "Wireless":
                    txtblkInternet.Text = "\uE701";
                    break;
                case "Ethernet":
                    txtblkInternet.Text = "\uE839";
                    break;
                default:
                    txtblkInternet.Text = "";
                    break;
            }

            switch (GlobalVariables.powerStatus)
            {
                case "AC":
                    txtblkPower.Text = "";
                    break;
                case "Online":
                    if (GlobalVariables.batteryPercentage < 10 && GlobalVariables.batteryPercentage >= 0) { txtblkPower.Text = "\uE85A"; }
                    if (GlobalVariables.batteryPercentage < 20 && GlobalVariables.batteryPercentage >= 10) { txtblkPower.Text = "\uE85B"; }
                    if (GlobalVariables.batteryPercentage < 30 && GlobalVariables.batteryPercentage >= 20) { txtblkPower.Text = "\uE85C"; }
                    if (GlobalVariables.batteryPercentage < 40 && GlobalVariables.batteryPercentage >= 30) { txtblkPower.Text = "\uE85D"; }
                    if (GlobalVariables.batteryPercentage < 50 && GlobalVariables.batteryPercentage >= 40) { txtblkPower.Text = "\uE85E"; }
                    if (GlobalVariables.batteryPercentage < 60 && GlobalVariables.batteryPercentage >= 50) { txtblkPower.Text = "\uE85F"; }
                    if (GlobalVariables.batteryPercentage < 70 && GlobalVariables.batteryPercentage >= 60) { txtblkPower.Text = "\uE860"; }
                    if (GlobalVariables.batteryPercentage < 80 && GlobalVariables.batteryPercentage >= 70) { txtblkPower.Text = "\uE861"; }
                    if (GlobalVariables.batteryPercentage < 90 && GlobalVariables.batteryPercentage >= 80) { txtblkPower.Text = "\uE862"; }
                    if (GlobalVariables.batteryPercentage <= 100 && GlobalVariables.batteryPercentage >= 90) { txtblkPower.Text = "\uE83E"; }
                    txtblkBatteryPercentage.Text = GlobalVariables.batteryPercentage.ToString() + "%";
                    break;
                case "Offline":
                    if (GlobalVariables.batteryPercentage < 10 && GlobalVariables.batteryPercentage >= 0) { txtblkPower.Text = "\uE850"; }
                    if (GlobalVariables.batteryPercentage < 20 && GlobalVariables.batteryPercentage >= 10) { txtblkPower.Text = "\uE851"; }
                    if (GlobalVariables.batteryPercentage < 30 && GlobalVariables.batteryPercentage >= 20) { txtblkPower.Text = "\uE852"; }
                    if (GlobalVariables.batteryPercentage < 40 && GlobalVariables.batteryPercentage >= 30) { txtblkPower.Text = "\uE853"; }
                    if (GlobalVariables.batteryPercentage < 50 && GlobalVariables.batteryPercentage >= 40) { txtblkPower.Text = "\uE854"; }
                    if (GlobalVariables.batteryPercentage < 60 && GlobalVariables.batteryPercentage >= 50) { txtblkPower.Text = "\uE855"; }
                    if (GlobalVariables.batteryPercentage < 70 && GlobalVariables.batteryPercentage >= 60) { txtblkPower.Text = "\uE856"; }
                    if (GlobalVariables.batteryPercentage < 80 && GlobalVariables.batteryPercentage >= 70) { txtblkPower.Text = "\uE857"; }
                    if (GlobalVariables.batteryPercentage < 90 && GlobalVariables.batteryPercentage >= 80) { txtblkPower.Text = "\uE858"; }
                    if (GlobalVariables.batteryPercentage < 100 && GlobalVariables.batteryPercentage >= 90) { txtblkPower.Text = "\uE859"; }
                    txtblkBatteryPercentage.Text = GlobalVariables.batteryPercentage.ToString() + "%";
                    break;
                default:
                    break;
            }

            if (GlobalVariables.controller is null) { txtblkGamepad.Text = ""; } else { if (GlobalVariables.controller.IsConnected) { txtblkGamepad.Text = "\uE7FC"; } else { txtblkGamepad.Text = ""; } }
        }
    }
}