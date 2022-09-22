using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Power_Control_Panel.PowerControlPanel.Classes.RoutineUpdate;
using WindowsInput.Native;
using SharpDX.XInput;
using WindowsInput;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using System.Text.RegularExpressions;
using Power_Control_Panel.PowerControlPanel.Classes;
using System.Diagnostics;
using ControlzEx.Theming;
using System.Windows.Interop;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for OSK.xaml
    /// </summary>
    public partial class OSK : MetroWindow
    {
        

        WindowSinker sinker;
        InputSimulator sim = new InputSimulator();

        private Controller controller;
        private Gamepad gamepadOld;
        private Gamepad gamepadCurrent;

        private const double CircleWidth = 10;
        //private Dictionary<int, Point> TouchPositions = new Dictionary<int, Point>();
        //private Dictionary<int, Ellipse> TouchEllipses = new Dictionary<int, Ellipse>();
        private double LLx = CircleWidth / 2;
        private double LLy = CircleWidth / 2;
        private double ULx = System.Windows.SystemParameters.PrimaryScreenWidth - CircleWidth / 2;
        private double ULy;
        private double windowY;

        private bool ellipseSetup = false;
        private Ellipse ellipseL;
        private Ellipse ellipseR;
        private Point pointL;
        private Point pointR;

        private bool keyAlt = false;
        private bool keyCtrl = false;
        private bool keyCap = false;
        private bool keyShift = false;
        private bool keyWin = false;


        buttonEvents events = new buttonEvents();

        Dictionary<string, VirtualKeyCode> keyLookUp =
       new Dictionary<string, VirtualKeyCode>()
       {
           {"T_A", VirtualKeyCode.VK_A },
           {"T_B", VirtualKeyCode.VK_B },
           {"T_C", VirtualKeyCode.VK_C },
           {"T_D", VirtualKeyCode.VK_D },
           {"T_E", VirtualKeyCode.VK_E },
           {"T_F", VirtualKeyCode.VK_F },
           {"T_G", VirtualKeyCode.VK_G },
           {"T_H", VirtualKeyCode.VK_H },
           {"T_I", VirtualKeyCode.VK_I },
           {"T_J", VirtualKeyCode.VK_J },
           {"T_K", VirtualKeyCode.VK_K },
           {"T_L", VirtualKeyCode.VK_L },
           {"T_M", VirtualKeyCode.VK_M },
           {"T_N", VirtualKeyCode.VK_N },
           {"T_O", VirtualKeyCode.VK_O },
           {"T_P", VirtualKeyCode.VK_P },
           {"T_Q", VirtualKeyCode.VK_Q },
           {"T_R", VirtualKeyCode.VK_R },
           {"T_S", VirtualKeyCode.VK_S },
           {"T_T", VirtualKeyCode.VK_T },
           {"T_U", VirtualKeyCode.VK_U },
           {"T_V", VirtualKeyCode.VK_V },
           {"T_W", VirtualKeyCode.VK_W },
           {"T_X", VirtualKeyCode.VK_X },
           {"T_Y", VirtualKeyCode.VK_Y },
           {"T_Z", VirtualKeyCode.VK_Z },

           {"T_1", VirtualKeyCode.VK_1 },
           {"T_2", VirtualKeyCode.VK_2 },
           {"T_3", VirtualKeyCode.VK_3 },
           {"T_4", VirtualKeyCode.VK_4 },
           {"T_5", VirtualKeyCode.VK_5 },
           {"T_6", VirtualKeyCode.VK_6 },
           {"T_7", VirtualKeyCode.VK_7 },
           {"T_8", VirtualKeyCode.VK_8 },
           {"T_9", VirtualKeyCode.VK_9 },
           {"T_0", VirtualKeyCode.VK_0 },

           {"T_Period", VirtualKeyCode.OEM_PERIOD },
           {"T_Comma", VirtualKeyCode.OEM_COMMA },

           {"T_Period_", VirtualKeyCode.OEM_PERIOD },
           {"T_Comma_", VirtualKeyCode.OEM_COMMA },

        

       };

        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0010;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        // Call this way:

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public OSK()
        {
            InitializeComponent();
            //Make window sinker, always on top, situate bottom of screen
            setUpWindow();

            //Set up  controller events for button presses
            setUpControllerEvents();

            //Set up circles, make the shared ellipses to get ready to add to canvas
            setUpCircles();

            //check if controller is connected 
            setUpController();

            //add circles to canvas if controller connected
            addCirclesCanvas();

            //Setup dispatcher timer
            setupTimer();

            //swap upper and lower case letters
            swapAlphaUpperLower(false);
            IntPtr windowHandle = new WindowInteropHelper(this).Handle;
            SetWindowPos(windowHandle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        #region Controller press events
        void HandlePressLBEvent(object sender, EventArgs a)
        {
            handleButtonPress(pointL, ellipseL);
        }
        void HandlePressRBEvent(object sender, EventArgs a)
        {
            handleButtonPress(pointR, ellipseR);
        }

        void HandlePressBEvent(object sender, EventArgs a)
        {

            Close();
            
        }
        void HandlePressYEvent(object sender, EventArgs a)
        {
            keyboardPress(canvMain.FindName("R_Space"));
        }
        void HandlePressXEvent(object sender, EventArgs a)
        {
            keyboardPress(canvMain.FindName("R_BckSpce"));
        }
        void HandlePressLTEvent(object sender, EventArgs a)
        {

            keyboardPress(canvMain.FindName("R_CAP"));
        }
        void HandlePressRTEvent(object sender, EventArgs a)
        {
            keyboardPress(canvMain.FindName("R_AlphaNum"));
        }
        #endregion controler press events

        #region joystick point math

        private double offset_calculator(short Input)
        {
            //Convert short from joystick to double, divide by largest value, and round to largest nubmer away from zero
            if (Input < 500 && Input > 0)
            {
                return 1;
            }
            else
            {
                return Math.Round(10 * Convert.ToDouble(Input) / 32768, 0, MidpointRounding.AwayFromZero);
            }
        }
        private Point offset_point(Point mp, Double dx, Double dy)
        {
            if ((mp.X + dx >= LLx) && (mp.X + dx <= ULx))
            {
                mp.Offset(dx, 0);
            }
            else
            {
                if (mp.X + dx < LLx)
                {
                    mp.X = LLx;
                }
                else
                {
                    mp.X = ULx;
                }
            }
            if ((mp.Y + dy >= LLy) && (mp.Y + dy <= ULy))
            {
                mp.Offset(0, dy);
            }
            else
            {
                if (mp.Y + dy < LLy)
                {
                    mp.Y = LLy;
                }
                else
                {
                    mp.Y = ULy;
                }
            }
            return mp;
        }

        #endregion joystick point math

        #region setup routines
        private void setUpControllerEvents()
        {

            events.pressBEvent += HandlePressBEvent;
            events.pressXEvent += HandlePressXEvent;
            events.pressYEvent += HandlePressYEvent;
            events.pressLBEvent += HandlePressLBEvent;
            events.pressRBEvent += HandlePressRBEvent;
            events.pressLTEvent += HandlePressLTEvent;
            events.pressRTEvent += HandlePressRTEvent;
        }
        private void setUpWindow()
        {
            //create window sinker for always on top
            sinker = new WindowSinker(this);
            sinker.Sink();

            //more window options
            this.Height = System.Windows.SystemParameters.PrimaryScreenHeight * 0.4;
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Left = 0;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
            ULy = this.Top - CircleWidth / 2;
            windowY = this.Top;

        }
        private void setUpController()
        {
            //set up controller and get first gamepad state
            controller = new Controller(UserIndex.One);
            if (controller != null)
            {
                if (controller.IsConnected)
                {
                    try
                    {
                        gamepadCurrent = controller.GetState().Gamepad;
                    }
                    catch { }
                }
                else { controller = null; }

            }
        }
        private void setupTimer()
        {
            dispatcherTimer.Tick += new EventHandler(timer_Tick);
            //If controller is not null and connected, make timespan 15 ms otherwise make it 3 seconds
            if (controller != null) { if (controller.IsConnected) { dispatcherTimer.Interval = TimeSpan.FromMilliseconds(15); } else { dispatcherTimer.Interval = new TimeSpan(0, 0, 3); } } else { dispatcherTimer.Interval = new TimeSpan(0, 0, 3); }
            dispatcherTimer.Start();
        }
        private void setUpCircles()
        {
            //Set up ellipses as circles to add to canvas
            ellipseL = new Ellipse();
            ellipseL.Stroke = Brushes.Black;
            ellipseL.Fill = Brushes.Red;
            ellipseL.Width = CircleWidth;
            ellipseL.Height = CircleWidth;

            ellipseR = new Ellipse();
            ellipseR.Stroke = Brushes.Black;
            ellipseR.Fill = Brushes.Blue;
            ellipseR.Width = CircleWidth;
            ellipseR.Height = CircleWidth;

        }

        #endregion set up routines

        private void addCirclesCanvas()
        {
            if (controller != null)
            {
                if (controller.IsConnected)
                {
                    if (!canvMain.Children.Contains(ellipseL))
                    {
                        pointL = new Point(this.Width * 0.25, this.Height * 0.5);
                        AddEllipseAt(canvMain, pointL, ellipseL);
                    }
                    if (!canvMain.Children.Contains(ellipseR))
                    {
                        pointR = new Point(this.Width * 0.75, this.Height * 0.5);
                        AddEllipseAt(canvMain, pointR, ellipseR);
                    }


                    ellipseSetup = true;
                }
            }


        }
        private void removeCircles()
        {

            if (canvMain.Children.Contains(ellipseL))
            {
                canvMain.Children.Remove(ellipseL);
            }
            if (canvMain.Children.Contains(ellipseR))
            {
                canvMain.Children.Remove(ellipseR);
            }

        }

        private void AddEllipseAt(Canvas canv, Point pt, Ellipse ellipse)
        {
            Canvas.SetLeft(ellipse, pt.X - (CircleWidth / 2));
            Canvas.SetTop(ellipse, pt.Y - (CircleWidth / 2));

            canv.Children.Add(ellipse);
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            //Check controller, if null start it
            if (controller == null)
            {
                controller = new Controller(UserIndex.One);
                if (controller == null)
                {
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 3);
                }
                else
                { 
                    if (controller.IsConnected)
                    {
                        dispatcherTimer.Interval = TimeSpan.FromMilliseconds(15);
                        addCirclesCanvas();
                        //Use try catch to prevent error when controller disconnects
                        try
                        {
                            gamepadCurrent = controller.GetState().Gamepad;
                        }
                        catch { }
                    }
                    else { dispatcherTimer.Interval = new TimeSpan(0, 0, 3); }
                
                }
            }
            else
            {
                if (!controller.IsConnected) 
                { 
                    controller = null;
                    ellipseSetup = false;
                    removeCircles();
                }
            }


            if (controller != null)
            {
                if (controller.IsConnected)
                {
                    //caputre in try to prevent error if controller disconnected
                    try
                    {
                        //get gamepad states
                        gamepadOld = gamepadCurrent;
                        gamepadCurrent = controller.GetState().Gamepad;



                        //move circles based on joystick movement
                        double dlx = offset_calculator(gamepadCurrent.LeftThumbX);
                        double dly = -1 * offset_calculator(gamepadCurrent.LeftThumbY);
                        double drx = offset_calculator(gamepadCurrent.RightThumbX);
                        double dry = -1 * offset_calculator(gamepadCurrent.RightThumbY);


                        pointL = offset_point(pointL, dlx, dly);
                        Canvas.SetLeft(ellipseL, pointL.X - (CircleWidth / 2));
                        Canvas.SetTop(ellipseL, pointL.Y - (CircleWidth / 2));

                        pointR = offset_point(pointR, drx, dry);
                        Canvas.SetLeft(ellipseR, pointR.X - (CircleWidth / 2));
                        Canvas.SetTop(ellipseR, pointR.Y - (CircleWidth / 2));


                        //check for event firing from button states
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.A) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.A))
                        {
                            events.RaiseEventA();

                        }
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.B) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.B))
                        {
                            events.RaiseEventB();
                        }
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.X) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.X))
                        {
                           events.RaiseEventX();
                        }
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.Y) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.Y))
                        {
                            events.RaiseEventY();
                        }
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                        {
                            events.RaiseEventLB();
                          

                        }
                        if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
                        {
                            events.RaiseEventRB();

                        }

                        if (gamepadOld.RightTrigger == 0 && gamepadCurrent.RightTrigger > 0)
                        {
                            events.RaiseEventRT();

                        }
                        if (gamepadOld.LeftTrigger == 0 && gamepadCurrent.LeftTrigger > 0)
                        {
                            events.RaiseEventLT();

                        }
                    }
                    catch { }





                }

            }



        }


      

        


        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            keyboardPress(sender);
        }

        void swapAlphaUpperLower(bool upper)
        {
            Regex upperCaseRegex = new Regex("[A-Z]");
            Regex lowerCaseRegex = new Regex("[a-z]");

           

            foreach (UIElement elem in AlphaKB.Children) //iterate the main grid
            {

                if (elem is TextBlock) // if button contains only 1 character
                {
                    TextBlock txtblk = elem as TextBlock;
                    if (txtblk.Text.Length == 1)
                    {
                        if (upperCaseRegex.Match(txtblk.Text.ToString()).Success && !upper) // if the char is a letter and uppercase
                        {
                            txtblk.Text = txtblk.Text.ToString().ToLower();
                        }
                        else if (lowerCaseRegex.Match(txtblk.Text.ToString()).Success && upper) // if the char is a letter and lower case
                        {
                            txtblk.Text = txtblk.Text.ToString().ToUpper();
                        }

                    }

                }
            }
        }

        void keyboardPress(object sender)
        {
            TextBlock txtblk;
            string textValue = null;
            string textName;

            if (sender is Rectangle)
            {
                Rectangle rect = (Rectangle)sender;

                sender = canvMain.FindName("T_" + rect.Name.Substring(2, rect.Name.Length - 2));
         
            }

            if (sender is TextBlock)
            {
                txtblk = (TextBlock)sender;
                textValue = txtblk.Text;
                textName = txtblk.Name;
                bool isKeyDown;
                if (textName == "")
                {
                    txtblk = getParentTextBlock(txtblk);
                    textValue = txtblk.Text;
                    textName = txtblk.Name;
                }
                if (textName == "T_HideKeyboard") { this.Close(); }
                else
                {
                    

                    if (textValue != "")
                    {
                        
                        switch (textValue)
                        {
                            case "Esc":
                                sim.Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
                                break;

                            case "Shift":
                                keyShift = !keyShift;
                                isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                                if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.SHIFT); } else { sim.Keyboard.KeyDown(VirtualKeyCode.SHIFT); }
                                toggleRectangleOn(textName);
                                break;
                            case "Alt":
                                keyAlt = !keyAlt;
                                isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.MENU);
                                if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.MENU); } else { sim.Keyboard.KeyDown(VirtualKeyCode.MENU); }
                                toggleRectangleOn(textName);
                                break;
                            case "Ctrl":
                                keyCtrl = !keyCtrl;
                                isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL);
                                if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.CONTROL); } else { sim.Keyboard.KeyDown(VirtualKeyCode.CONTROL); }
                                toggleRectangleOn(textName);
                                break;
                            case "123":
                                AlphaKB.Visibility = Visibility.Hidden;
                                NumberKB.Visibility = Visibility.Visible;
                                txtblk.Text = "ABC";
                                break;
                            case "ABC":
                                AlphaKB.Visibility = Visibility.Visible;
                                NumberKB.Visibility = Visibility.Hidden;
                                txtblk.Text = "123";
                                break;
                            case "!":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_1);
                                break;
                            case "@":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_2);
                                break;
                            case "#":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_3);
                                break;
                            case "$":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_4);
                                break;
                            case "%":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_5);
                                break;
                            case "^":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_6);
                                break;
                            case "&amp;":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_7);
                                break;
                            case "*":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_8);
                                break;
                            case "(":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_9);
                                break;
                            case ")":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.SHIFT, VirtualKeyCode.VK_0);
                                break;
                            case "-":
                                sim.Keyboard.TextEntry("-");
                                break;

                            case "/":
                                sim.Keyboard.TextEntry("/");
                                break;

                            case "+":
                                sim.Keyboard.TextEntry("+");
                                break;
                            case "=":
                                sim.Keyboard.TextEntry("=");
                                break;

                            case "Ctrl+A":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
                                break;
                            case "Ctrl+C":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_C);
                                break;
                            case "Ctrl+X":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_X);
                                break;
                            case "Ctrl+V":
                                sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                                break;

                            case "PrintScreen":
                                sim.Keyboard.KeyPress(VirtualKeyCode.PRINT);
                                break;
                            case "Del":
                                sim.Keyboard.KeyPress(VirtualKeyCode.DELETE);
                                break;

                            default:
                                VirtualKeyCode vkc;
                                vkc = keyLookUp[textName];
                                sim.Keyboard.KeyPress(vkc);
                                //sim.Keyboard.TextEntry(textValue);
                                break;
                        }
                    }
                    else
                    {
                        switch (txtblk.Name)
                        {
                            case "T_HideKeyboard":
                                this.Close();
                                break;
                            case "T_CAP":
                                
                                isKeyDown = Console.CapsLock;
                                if (isKeyDown) { sim.Keyboard.KeyPress(VirtualKeyCode.CAPITAL); swapAlphaUpperLower(false); } else { sim.Keyboard.KeyPress(VirtualKeyCode.CAPITAL); swapAlphaUpperLower(true); }
                                toggleRectangleOn(textName);
                                break;
                            case "T_BckSpce":
                                sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                                break;
                            case "T_Enter":
                                sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
                                break;
                            case "T_Win":
                                sim.Keyboard.KeyPress(VirtualKeyCode.LWIN);
                                break;
                            case "T_Space":
                                sim.Keyboard.KeyPress(VirtualKeyCode.SPACE);
                                break;

                        }
                    }

                    //Clear ctrl alt shift win if used
                    if (textName != "T_Shift" && textName != "T_Alt" && textName != "T_Ctrl" )
                    {
                        isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.SHIFT);
                        if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.SHIFT); toggleRectangleOn("T_Shift"); }

                        isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.MENU);
                        if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.MENU); toggleRectangleOn("T_Alt"); }

                        isKeyDown = sim.InputDeviceState.IsKeyDown(VirtualKeyCode.CONTROL);
                        if (isKeyDown) { sim.Keyboard.KeyUp(VirtualKeyCode.CONTROL); toggleRectangleOn("T_Ctrl"); }

                    }
                }
               

    
            }


        }

        void toggleRectangleOn(string txtblkName)
        {
            TextBlock txtblk = (TextBlock)canvMain.FindName(txtblkName);
            
            Rectangle rect;
            

            if (txtblk != null)
            {
                rect = (Rectangle)canvMain.FindName("R_" + txtblk.Name.Substring(2, txtblk.Name.Length - 2));

                if (rect.Fill == Brushes.WhiteSmoke) { rect.Fill = Brushes.Gray; } else { rect.Fill = Brushes.WhiteSmoke; }

            }

        }
        TextBlock getParentTextBlock(DependencyObject txtblk)
        {

            TextBlock returnTxtblk = null;
            DependencyObject parent;
            if (txtblk != null)
            {
                parent = VisualTreeHelper.GetParent(txtblk);

                if (parent is not TextBlock)
                {
                    parent = VisualTreeHelper.GetParent(parent);
                    if (parent is not TextBlock)
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }
                }
                
                returnTxtblk = (TextBlock)parent;
             
    

            }

           return returnTxtblk;


        }
        
        

        void handleButtonPress(Point point, Ellipse ellipse)
        {
            //Delete ellipse before running inputhittest or it will return the ellipse
            
            canvMain.Children.Remove(ellipse);
            
            IInputElement element = InputHitTest(point);
            object sender = element;

            AddEllipseAt(canvMain, point, ellipse);
         
            keyboardPress(sender);
        }

       
        

        private void Keyboard_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            
            sim.Keyboard.KeyUp(VirtualKeyCode.SHIFT);
            sim.Keyboard.KeyUp(VirtualKeyCode.CONTROL);
            sim.Keyboard.KeyUp(VirtualKeyCode.MENU);
            sim.Keyboard.KeyUp(VirtualKeyCode.LWIN);
            if (Console.CapsLock) { sim.Keyboard.KeyPress(VirtualKeyCode.CAPITAL); }

            dispatcherTimer.Stop();

            //Make null so it can be called again
            MainWindow.osk = null;

            
           

        }


    }

    public class buttonEvents
    {
        public void RaiseEventRT()
        {
            pressRTEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressRTEvent;
        public void RaiseEventLT()
        {
            pressLTEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressLTEvent;

        public void RaiseEventA()
        {
            pressAEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressAEvent;

        public event EventHandler pressXEvent;
        public void RaiseEventX()
        {
            pressXEvent?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler pressYEvent;
        public void RaiseEventY()
        {
            pressYEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressBEvent;
        public void RaiseEventB()
        {
            pressBEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressLBEvent;
        public void RaiseEventLB()
        {
            pressLBEvent?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler pressRBEvent;
        public void RaiseEventRB()
        {
            pressRBEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
