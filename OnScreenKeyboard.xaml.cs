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
using System.Windows.Navigation;
using System.ComponentModel;
using MahApps.Metro.Controls;
using System.Text.RegularExpressions;
using TCD.System.TouchInjection;
using WindowsInput.Native;
using SharpDX.XInput;
using WindowsInput;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for OnScreenKeyboard.xaml
    /// </summary>
    public partial class OnScreenKeyboard : MetroWindow
    {
        #region Public Properties
        private Controller controller;
        private Gamepad gamepad;
        private const double CircleWidth = 14;
        private const double RectangleWidth = 90;
        private const double CharFontSize = 65;
        private Dictionary<int, Point> TouchPositions;
        private Dictionary<int, Ellipse> TouchEllipses;
        private double LLx = CircleWidth / 2;
        private double LLy = CircleWidth / 2;
        private double ULx = System.Windows.SystemParameters.PrimaryScreenWidth - CircleWidth / 2;
        private double ULy;
        private double windowY;
        private bool LTouch = false;
        private bool RTouch = false;
        private PointerTouchInfo[] contacts = new PointerTouchInfo[1];
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        WindowSinker sinker;
        InputSimulator inputSim = new InputSimulator();


        private bool _showNumericKeyboard;
        public bool ShowNumericKeyboard

        {
            get { return _showNumericKeyboard; }
            set { _showNumericKeyboard = value; this.OnPropertyChanged("ShowNumericKeyboard"); }
        }



        #endregion

        #region Constructor

 

        public OnScreenKeyboard()
        {
            InitializeComponent();
            setUpWindow();
            setUpCircles();
            setUpController();
            setUpDispatchTimer();
        }

        void setUpCircles()
        {
            TouchPositions = new Dictionary<int, Point>();
            TouchEllipses = new Dictionary<int, Ellipse>();
            Point startL = new Point(this.Width * 0.25, this.Height * 0.5);

            Point startR = new Point(this.Width * 0.75, this.Height * 0.5);

            Ellipse el = AddEllipseAt(canvMain, startL, Brushes.Red);
            Ellipse el2 = AddEllipseAt(canvMain, startR, Brushes.Blue);
            TouchPositions.Add(1, startL);
            TouchPositions.Add(2, startR);
            TouchEllipses.Add(1, el);
            TouchEllipses.Add(2, el2);


        }
        void setUpDispatchTimer()
        {
      
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Start();    
            

        }
        void setUpController()
        {
            controller = new Controller(UserIndex.One);
            if (controller.IsConnected == false)
            {
                controller = new Controller(UserIndex.Two);
            }
            if (controller.IsConnected == false)
            {
                controller = new Controller(UserIndex.Three);
            }
            if (controller.IsConnected == false)
            {
                controller = new Controller(UserIndex.Four);
            }
            if (controller.IsConnected == false)
            {
                //set up dispatch timer to check for xinput controller every 5 seconds
                dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
            }
            else
            {
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(8);
            }

        }
        void setUpWindow()
        {
            sinker = new WindowSinker(this);
            sinker.Sink();

            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;


            this.Left = 0;
            this.Top = System.Windows.SystemParameters.PrimaryScreenHeight - this.Height;
            ULy = this.Top - CircleWidth / 2;
            windowY = this.Top;

        }
        private double offset_calculator(short Input)
        {
            //Convert short from joystick to double, divide by largest value, and round to largest nubmer away from zero
            if (Input < 3277 && Input > 0)
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

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            
            if (controller.IsConnected)
            {
                gamepad = controller.GetState().Gamepad;
                double dlx = offset_calculator(gamepad.LeftThumbX);
                double dly = -1 * offset_calculator(gamepad.LeftThumbY);
                double drx = offset_calculator(gamepad.RightThumbX);
                double dry = -1 * offset_calculator(gamepad.RightThumbY);



                System.Windows.Point mp = TouchPositions[1];
                //Use offset point function to make sure 
                mp = offset_point(mp, dlx, dly);
                TouchPositions[1] = mp;
                Canvas.SetLeft(TouchEllipses[1], mp.X - (CircleWidth / 2));
                Canvas.SetTop(TouchEllipses[1], mp.Y - (CircleWidth / 2));

                if (gamepad.LeftTrigger > 0)
                {
                    contacts[0] = MakePointerTouchInfo(PointerInputType.TOUCH, PointerFlags.NONE, (int)mp.X,
                                (int)mp.Y, 1, 1, "Start");
                    LTouch = true;
                    TouchInjector.InjectTouchInput(1, contacts);
                }
                if (gamepad.LeftTrigger == 0 & LTouch)
                {
                    contacts[0] = MakePointerTouchInfo(PointerInputType.TOUCH, PointerFlags.NONE, (int)mp.X,
            (int)mp.Y, 1, 1, "End");
                    LTouch = false;
                    TouchInjector.InjectTouchInput(1, contacts);
                }
                // LeftMouseClick((int)mp.X, (int)(mp.Y + windowY));

                System.Windows.Point mp2 = TouchPositions[2];
                mp2 = offset_point(mp2, drx, dry);
                TouchPositions[2] = mp2;
                Canvas.SetLeft(TouchEllipses[2], mp2.X - (CircleWidth / 2));
                Canvas.SetTop(TouchEllipses[2], mp2.Y - (CircleWidth / 2));


                if (gamepad.RightTrigger > 0)
                {
                    contacts[0] = MakePointerTouchInfo(PointerInputType.TOUCH, PointerFlags.NONE, (int)mp2.X,
                                (int)mp2.Y, 1, 1, "Start");
                    RTouch = true;
                    TouchInjector.InjectTouchInput(1, contacts);
                }
                if (gamepad.RightTrigger == 0 & RTouch)
                {
                    contacts[0] = MakePointerTouchInfo(PointerInputType.TOUCH, PointerFlags.NONE, (int)mp2.X,
            (int)mp2.Y, 1, 1, "End");
                    RTouch = false;
                    TouchInjector.InjectTouchInput(1, contacts);
                }
            }
            else { 
                setUpController(); 
                
            }
            



        }



        private Ellipse AddEllipseAt(Canvas canv, Point pt, Brush brush)
        {
            Ellipse el = new Ellipse();
            el.Stroke = Brushes.Black;
            el.Fill = brush;

            el.Width = CircleWidth;
            el.Height = CircleWidth;

            Canvas.SetLeft(el, pt.X - (CircleWidth / 2));
            Canvas.SetTop(el, pt.Y - (CircleWidth / 2));

            canv.Children.Add(el);

            return el;
        }

        #endregion

        #region Callbacks

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                switch (button.CommandParameter.ToString())
                {
                    case "HideKeyboard":
                        this.Hide();
                        break;
                    case "LSHIFT":
                        Regex upperCaseRegex = new Regex("[A-Z]");
                        Regex lowerCaseRegex = new Regex("[a-z]");
                        Button btn;
                        foreach (UIElement elem in AlfaKeyboard.Children) //iterate the main grid
                        {
                            Grid grid = elem as Grid;
                            if (grid != null)
                            {
                                foreach (UIElement uiElement in grid.Children)  //iterate the single rows
                                {
                                    btn = uiElement as Button;
                                    if (btn != null) // if button contains only 1 character
                                    {
                                        if (btn.Content.ToString().Length == 1)
                                        {
                                            if (upperCaseRegex.Match(btn.Content.ToString()).Success) // if the char is a letter and uppercase
                                                btn.Content = btn.Content.ToString().ToLower();
                                            else if (lowerCaseRegex.Match(button.Content.ToString()).Success) // if the char is a letter and lower case
                                                btn.Content = btn.Content.ToString().ToUpper();
                                        }

                                    }
                                }
                            }
                        }
                        break;

                    case "ALT":
                    case "CTRL":
                        break;

                    case "RETURN":
                        this.DialogResult = true;
                        break;

                    case "BACK":
                        
                        break;

                    default:
                        inputSim.Keyboard.KeyPress(VirtualKeyCode.VK_E);
                        break;
                }
            }
        }

        #endregion

        private PointerTouchInfo MakePointerTouchInfo(PointerInputType pointer, PointerFlags click, int x, int y,
            int radius, uint id, string type, uint orientation = 90, uint pressure = 32000)
        {
            var contact = new PointerTouchInfo
            {
                PointerInfo = { pointerType = pointer },
                TouchFlags = TouchFlags.NONE,
                Orientation = orientation,
                Pressure = pressure
            };

            if (type == "Start")
                contact.PointerInfo.PointerFlags = PointerFlags.DOWN | PointerFlags.INRANGE | PointerFlags.INCONTACT;
            else if (type == "Move")
                contact.PointerInfo.PointerFlags = PointerFlags.UPDATE | PointerFlags.INRANGE | PointerFlags.INCONTACT;
            else if (type == "End")
                contact.PointerInfo.PointerFlags = PointerFlags.UP;
            else if (type == "EndToHover")
                contact.PointerInfo.PointerFlags = PointerFlags.UP | PointerFlags.INRANGE;
            else if (type == "Hover")
                contact.PointerInfo.PointerFlags = PointerFlags.UPDATE | PointerFlags.INRANGE;
            else if (type == "EndFromHover")
                contact.PointerInfo.PointerFlags = PointerFlags.UPDATE;

            contact.PointerInfo.PointerFlags |= click;

            contact.TouchMasks = TouchMask.CONTACTAREA | TouchMask.ORIENTATION | TouchMask.PRESSURE;
            contact.PointerInfo.PtPixelLocation.X = x;
            contact.PointerInfo.PtPixelLocation.Y = y;
            contact.PointerInfo.PointerId = id;
            contact.ContactArea.left = x - radius;
            contact.ContactArea.right = x + radius;
            contact.ContactArea.top = y - radius;
            contact.ContactArea.bottom = y + radius;
            return contact;
        }

        #region INotifyPropertyChanged members

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
