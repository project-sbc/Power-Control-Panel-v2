using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using Power_Control_Panel.PowerControlPanel.Classes.TDPTaskScheduler;


namespace Power_Control_Panel.PowerControlPanel.PageComponents
{
    /// <summary>
    /// Interaction logic for HomeTDP.xaml
    /// </summary>

    public partial class HomeTDP : Page
    {
        private bool dragStartedTDP1 = true;
        private bool dragStartedTDP2 = true;
        private bool changingTDP = false;
        private bool firstTick = true;
        private DispatcherTimer updateTick = new DispatcherTimer();
        public HomeTDP()
        {
            InitializeComponent();

        }

        private void updateTDP()
        {
            TDPTaskScheduler.runTask(() => ChangeTDP.readTDP());
        }
        private void TDP1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP1 && !changingTDP)
            {
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
            }
        }
        private void TDP1_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP1 = true;
        }
        private void TDP1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void TDP1_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedTDP1 = true;
        }

        private void TDP2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!dragStartedTDP2 && !changingTDP)
            {
                HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
            }
        }
        private void TDP2_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
        private void TDP2_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            dragStartedTDP2 = true;
        }
        private void TDP2_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, false);
        }
        private void TDP2_TouchDown(object sender, TouchEventArgs e)
        {
            dragStartedTDP2 = true;
        }
        private void TDP2_TouchUp(object sender, TouchEventArgs e)
        {
            dragStartedTDP2 = false;
            HandleChangingTDP((int)TDP1.Value, (int)TDP2.Value, true);
        }
        private void HandleChangingTDP(int tdpPL1, int tdpPL2, bool PL1started)
        {
     
            if (!changingTDP)
            {
                changingTDP = true;
                GlobalVariables.needTDPRead = true;
                if (PL1started)
                {
                    //If PL1 is greater than PL2 then PL2 needs to be set to the PL1 value

                    if (tdpPL1 < tdpPL2) { TDPTaskScheduler.runTask(() => ChangeTDP.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP2.Value = tdpPL1;
                        tdpPL2= tdpPL1; 
                        TDPTaskScheduler.runTask(() => ChangeTDP.changeTDP(tdpPL1, tdpPL2));
                    };
                }
                else
                {
                    //If PL2 is less than PL1 drop PL1 down to PL2 new value
                    if (tdpPL1 < tdpPL2) { TDPTaskScheduler.runTask(() => ChangeTDP.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP1.Value = tdpPL2;
                        tdpPL1= tdpPL2;
                        TDPTaskScheduler.runTask(() => ChangeTDP.changeTDP(tdpPL1, tdpPL2));
                    };
                }
                TDPTaskScheduler.runTask(() => ChangeTDP.readTDP());
                changingTDP = false;
            }


        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            intializeTimer();
            loadTDPValues();
          
        }
    
        void loadTDPValues()
        {
            //If global tdp is not zero meaning it was read within 10 seconds, load those instead of calling a update
            if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
            {
                updateFromGlobalTDP();
            }
       
        }
        void intializeTimer()
        {
            //Set up auto update tick timer, which syncs with global variable updates
            updateTick.Interval = new TimeSpan(0, 0, 2);
            updateTick.Tick += updateTick_Tick;
            updateTick.Start();
            System.Diagnostics.Debug.WriteLine("");
        }
        void updateTick_Tick(object sender, EventArgs e)
        {
            //Divorce actual routine from tick event so the updateFromGlobalTDP can be called at start up or tick event
            updateFromGlobalTDP();
        }
        void updateFromGlobalTDP()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {
                changingTDP = true;
                TDP1.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero);
                TDP2.Value = Math.Round(GlobalVariables.readPL2, 0, MidpointRounding.AwayFromZero);
                changingTDP = false;
            }

        }


    }
}
