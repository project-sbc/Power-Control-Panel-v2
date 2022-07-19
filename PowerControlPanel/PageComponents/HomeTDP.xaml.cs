﻿using System;
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
using System.Diagnostics;
using System.Windows.Shapes;
using System.Diagnostics;
using Power_Control_Panel.PowerControlPanel.Classes.ChangeTDP;
using Power_Control_Panel.PowerControlPanel.Classes.TaskScheduler;


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

        private DispatcherTimer timer;
        public HomeTDP()
        {
            InitializeComponent();
            handleVisibility();
            initializeTimer();          
        }
        void initializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += timerTick;
            timer.Start();
        }

        void timerTick(object sender, EventArgs e)
        {
            updateFromGlobalTDPPL1();
            updateFromGlobalTDPPL2();

        }


        void handleVisibility()
        {
            if (Properties.Settings.Default.showTDP)
            { enableControl.IsOn = true; }
            else { enableControl.IsOn = false; }
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

                    if (tdpPL1 < tdpPL2) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP2.Value = tdpPL1;
                        tdpPL2= tdpPL1;
                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2));
                    };
                }
                else
                {
                    //If PL2 is less than PL1 drop PL1 down to PL2 new value
                    if (tdpPL1 < tdpPL2) { Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2)); }
                    else
                    {
                        TDP1.Value = tdpPL2;
                        tdpPL1= tdpPL2;
                        Classes.TaskScheduler.TaskScheduler.runTask(() => GlobalVariables.tdp.changeTDP(tdpPL1, tdpPL2));
                    };
                }
                
                changingTDP = false;
            }


        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
         
            loadTDPValues();
          
        }
    
        void loadTDPValues()
        {
            //If global tdp is not zero meaning it was read within 10 seconds, load those instead of calling a update
            if (GlobalVariables.readPL1 > 0 && GlobalVariables.readPL2 > 0)
            {
                updateFromGlobalTDPPL1();
                updateFromGlobalTDPPL2();
            }
       
        }

        void updateFromGlobalTDPPL1()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {
                changingTDP = true;

                try
                {
                    if (!TDP1.IsFocused) { TDP1.Value = Math.Round(GlobalVariables.readPL1, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }

              
                changingTDP = false;
            }
  
        }
        void updateFromGlobalTDPPL2()
        {
            //Make changingTDP boolean true to prevent slider event from updating TDP
            if (GlobalVariables.needTDPRead == false)
            {
                changingTDP = true;
                try
                {
                    if (!TDP2.IsFocused) { TDP2.Value = Math.Round(GlobalVariables.readPL2, 0, MidpointRounding.AwayFromZero); }
                }
                catch { }

                changingTDP = false;
            }

        }

        private void enableControl_Toggled(object sender, RoutedEventArgs e)
        {
            if (enableControl.IsOn)
            {
                this.Height = 150;
            }
            else { 
                this.Height = 40;
                updateFromGlobalTDPPL1();
                updateFromGlobalTDPPL2();
            }
        }

        private void TDP1_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Math.Abs(TDP1.Value - GlobalVariables.readPL1) >2)
            {
                updateFromGlobalTDPPL1();
            }
        }

        private void TDP2_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Math.Abs(TDP2.Value - GlobalVariables.readPL2) > 2)
            {
                updateFromGlobalTDPPL2();
            }
        }
    }
}
