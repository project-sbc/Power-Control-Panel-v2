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
using System.Net.NetworkInformation;
using System.Management;
using ControlzEx.Theming;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    

    public partial class QuickAccessMenu : MetroWindow
    {
        private NavigationServiceEx navigationServiceEx;
        //WindowSinker sinker;

        DispatcherTimer timer = new DispatcherTimer();  
        
        public QuickAccessMenu()
        {
            this.InitializeComponent();

            initializeTimer();

            //Run code to set up hamburger menu
            initializeNavigationFrame();

            initializeWindow();

            updateValues();

            ThemeManager.Current.ChangeTheme(this, Properties.Settings.Default.systemTheme);
        }

        void initializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 3);
            timer.Tick += updateValuesTick;
            timer.Start();
        }

       void initializeWindow()
        {
            //sinker = new WindowSinker(this);
            //sinker.Sink();
            InitializeComponent();
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
                       
        }



        #region hamburger navigation
        //Navigation routines
        void initializeNavigationFrame()
        {
            navigationServiceEx = new NavigationServiceEx();
            navigationServiceEx.Navigated += this.NavigationServiceEx_OnNavigated;
            HamburgerMenuControl.Content = this.navigationServiceEx.Frame;

            this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/QAMHomePage.xaml", UriKind.RelativeOrAbsolute));
            // Navigate to the home page.
            //if (Properties.Settings.Default.homePageTypeQAM == "Slider")
            //{
            //this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/sliderHomePage.xaml", UriKind.RelativeOrAbsolute));
            //}
            //if (Properties.Settings.Default.homePageTypeQAM == "Grouped Slider")
            //{
            // this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/HomePage.xaml", UriKind.RelativeOrAbsolute));
            //}
            //if (Properties.Settings.Default.homePageTypeQAM == "Tile")
            //{
            //   this.Loaded += (sender, args) => this.navigationServiceEx.Navigate(new Uri("PowerControlPanel/Pages/TileHomePage.xaml", UriKind.RelativeOrAbsolute));
            //}
        }

        private void HamburgerMenuControl_OnItemInvoked(object sender, HamburgerMenuItemInvokedEventArgs e)
        {

            if (e.InvokedItem is MenuItem menuItem)
            {
                if (menuItem.Label == "Hide" ^ menuItem.Label == "隐藏")
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


        #endregion hamburger navigation
        //End navigation routines

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timer.Stop();
            MainWindow.overlay = null;
            
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //double width =(Convert.ToDouble(Properties.Settings.Default.sizeQAM) / 100);
            //this.Width = width * System.Windows.SystemParameters.PrimaryScreenWidth;
            this.Left = System.Windows.SystemParameters.PrimaryScreenWidth - this.Width;
            this.Top = 0;

            //make height total height - 10%
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight - ((System.Windows.SystemParameters.PrimaryScreenHeight) / 15);

        }




        public void checkNetworkInterface()
        {

            //Gets internet status to display on overlay
            NetworkInterface[] networkCards = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            bool connectedDevice = false;
            foreach (NetworkInterface networkCard in networkCards)
            {
                if (networkCard.OperationalStatus == OperationalStatus.Up)
                {
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Ethernet) { txtblkInternet.Text = "\uE839"; ; connectedDevice = true; }
                    if (networkCard.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) { txtblkInternet.Text = "\uE701"; ; connectedDevice = true; }
                }


            }
            if (!connectedDevice) { txtblkInternet.Text = "\uF384"; }

           
        }

        void checkPowerStatus()
        {
            int batterylevel = -1;
            ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_Battery");
            string powerStatus = "AC";
            foreach (ManagementObject mo in mos.Get())
            {
                powerStatus = mo["EstimatedChargeRemaining"].ToString();
            }
            if (powerStatus != "AC")
            {
                batterylevel = Int16.Parse(powerStatus);
                PowerLineStatus Power = SystemParameters.PowerLineStatus;
                powerStatus = Power.ToString();

            }
            else { powerStatus = "AC"; }


            switch (powerStatus)
            {
                case "AC":
                    txtblkPower.Text = "";
                    break;
                case "Online":

                    if (batterylevel < 10 && batterylevel >= 0) { txtblkPower.Text = "\uE85A"; }
                    if (batterylevel < 20 && batterylevel >= 10) { txtblkPower.Text = "\uE85B"; }
                    if (batterylevel < 30 && batterylevel >= 20) { txtblkPower.Text = "\uE85C"; }
                    if (batterylevel < 40 && batterylevel >= 30) { txtblkPower.Text = "\uE85D"; }
                    if (batterylevel < 50 && batterylevel >= 40) { txtblkPower.Text = "\uE85E"; }
                    if (batterylevel < 60 && batterylevel >= 50) { txtblkPower.Text = "\uE85F"; }
                    if (batterylevel < 70 && batterylevel >= 60) { txtblkPower.Text = "\uE860"; }
                    if (batterylevel < 80 && batterylevel >= 70) { txtblkPower.Text = "\uE861"; }
                    if (batterylevel < 90 && batterylevel >= 80) { txtblkPower.Text = "\uE862"; }
                    if (batterylevel <= 100 && batterylevel >= 90) { txtblkPower.Text = "\uE83E"; }
                    txtblkBatteryPercentage.Text = batterylevel.ToString() + "%";
                    break;
                case "Offline":
                    if (batterylevel < 10 && batterylevel >= 0) { txtblkPower.Text = "\uE850"; }
                    if (batterylevel < 20 && batterylevel >= 10) { txtblkPower.Text = "\uE851"; }
                    if (batterylevel < 30 && batterylevel >= 20) { txtblkPower.Text = "\uE852"; }
                    if (batterylevel < 40 && batterylevel >= 30) { txtblkPower.Text = "\uE853"; }
                    if (batterylevel < 50 && batterylevel >= 40) { txtblkPower.Text = "\uE854"; }
                    if (batterylevel < 60 && batterylevel >= 50) { txtblkPower.Text = "\uE855"; }
                    if (batterylevel < 70 && batterylevel >= 60) { txtblkPower.Text = "\uE856"; }
                    if (batterylevel < 80 && batterylevel >= 70) { txtblkPower.Text = "\uE857"; }
                    if (batterylevel < 90 && batterylevel >= 80) { txtblkPower.Text = "\uE858"; }
                    if (batterylevel < 100 && batterylevel >= 90) { txtblkPower.Text = "\uE859"; }
                    txtblkBatteryPercentage.Text = batterylevel.ToString() + "%";
                    break;
                default:
                    break;
            }

        }


        void updateValuesTick(object sender, EventArgs e)
        {
            updateValues();

        }

        void updateValues()
        {
            checkNetworkInterface();
            checkPowerStatus();

            txtblkDateTime.Text = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");

           
            //game pad stuff here
            //if (GlobalVariables.controller is null) { txtblkGamepad.Text = ""; } else { if (GlobalVariables.controller.IsConnected) { txtblkGamepad.Text = "\uE7FC"; } else { txtblkGamepad.Text = ""; } }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}