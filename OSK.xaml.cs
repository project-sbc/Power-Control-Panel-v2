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

using WindowsInput.Native;
using SharpDX.XInput;
using WindowsInput;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;

namespace Power_Control_Panel
{
    /// <summary>
    /// Interaction logic for OSK.xaml
    /// </summary>
    public partial class OSK : MetroWindow
    {


        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        WindowSinker sinker;
        InputSimulator inputSim = new InputSimulator();

        private Controller controller;
        private Gamepad gamepad;
        private const double CircleWidth = 10;
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

        private Dictionary<string, VirtualKeyCode> keyPressDictionary = new Dictionary<string, VirtualKeyCode>();


        void populateDictionary()
        {
            keyPressDictionary.Add("A", VirtualKeyCode.VK_A);
            keyPressDictionary.Add("B", VirtualKeyCode.VK_B);
            keyPressDictionary.Add("C", VirtualKeyCode.VK_C);
            keyPressDictionary.Add("D", VirtualKeyCode.VK_D);
            keyPressDictionary.Add("E", VirtualKeyCode.VK_E);
            keyPressDictionary.Add("F", VirtualKeyCode.VK_F);
            keyPressDictionary.Add("G", VirtualKeyCode.VK_G);
            keyPressDictionary.Add("H", VirtualKeyCode.VK_H);
            keyPressDictionary.Add("I", VirtualKeyCode.VK_I);
            keyPressDictionary.Add("J", VirtualKeyCode.VK_J);
            keyPressDictionary.Add("K", VirtualKeyCode.VK_K);
            keyPressDictionary.Add("L", VirtualKeyCode.VK_L);
            keyPressDictionary.Add("M", VirtualKeyCode.VK_M);
            keyPressDictionary.Add("N", VirtualKeyCode.VK_N);
            keyPressDictionary.Add("O", VirtualKeyCode.VK_O);
            keyPressDictionary.Add("P", VirtualKeyCode.VK_P);
            keyPressDictionary.Add("Q", VirtualKeyCode.VK_Q);
            keyPressDictionary.Add("R", VirtualKeyCode.VK_R);
            keyPressDictionary.Add("S", VirtualKeyCode.VK_S);
            keyPressDictionary.Add("T", VirtualKeyCode.VK_T);
            keyPressDictionary.Add("U", VirtualKeyCode.VK_U);
            keyPressDictionary.Add("V", VirtualKeyCode.VK_V);
            keyPressDictionary.Add("W", VirtualKeyCode.VK_W);
            keyPressDictionary.Add("X", VirtualKeyCode.VK_X);
            keyPressDictionary.Add("Y", VirtualKeyCode.VK_Y);
            keyPressDictionary.Add("Z", VirtualKeyCode.VK_Z);




        }

        public OSK()
        {
            InitializeComponent();


            InitializeComponent();
            setUpWindow();
            setUpCircles();

            setUpController();
            setUpDispatchTimer();
        }
        
        private void button_Click(object sender, RoutedEventArgs e)
        {
            keyboardPress(sender);
        }


        void keyboardPress(object sender)
        {
            string lookUpValue = null;
            if (sender is Rectangle)
            {
                Rectangle rect = (Rectangle)sender;
                lookUpValue = rect.Name.Substring(2, rect.Name.Length - 2);
            }

            if (sender is TextBlock)
            {
                TextBlock txtblk = (TextBlock)sender;
                lookUpValue = txtblk.Name.Substring(2, txtblk.Name.Length - 2);
            }

            if (lookUpValue != null)
            {
                var sim = new InputSimulator();
                sim.Keyboard.TextEntry(lookUpValue);
            }
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
                dispatcherTimer.Interval = TimeSpan.FromMilliseconds(4);
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

        void handleButtonPress(Point point, int ellipse)
        {
            //Delete ellipse before running inputhittest or it will return the ellipse
            Brush ellipseColor = TouchEllipses[ellipse].Fill;

            canvMain.Children.Remove(TouchEllipses[ellipse]);
            
            IInputElement element = InputHitTest(point);
            object sender = element;

            Ellipse el = AddEllipseAt(canvMain, point, ellipseColor);
            TouchEllipses[ellipse] = el;
            keyboardPress(sender);
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

                if (gamepad.LeftTrigger > 0 & !LTouch)
                {
                    handleButtonPress(mp,1);
                    LTouch = true;

              
               
                }
                if (gamepad.LeftTrigger == 0 & LTouch)
                {
                    LTouch = false;
                }


                System.Windows.Point mp2 = TouchPositions[2];
                mp2 = offset_point(mp2, drx, dry);
                TouchPositions[2] = mp2;
                Canvas.SetLeft(TouchEllipses[2], mp2.X - (CircleWidth / 2));
                Canvas.SetTop(TouchEllipses[2], mp2.Y - (CircleWidth / 2));


                if (gamepad.RightTrigger > 0 & !RTouch)
                {
                    handleButtonPress(mp2, 2);
                    RTouch = true;
                }
                if (gamepad.RightTrigger == 0 & RTouch)
                {
                    RTouch = false;
                }
            }
            else
            {
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


        private void rectangle_Click(object sender, RoutedEventArgs e)
        {
            string lookUpValue;
            if (sender is Rectangle)
            {
                Rectangle rect = (Rectangle)sender;
                lookUpValue = rect.Name.Substring(2, rect.Name.Length - 2);

                VirtualKeyCode vkc;
                keyPressDictionary.TryGetValue(lookUpValue, out vkc);
                var sim = new InputSimulator();
                sim.Keyboard.KeyPress(vkc);

            }

        }

        private void keyboard_Press(object sender)
        {
            System.Windows.IInputElement ele = InputHitTest(new Point(500, 250));
            if (ele != null)
            {
                if (ele.GetType() == typeof(Rectangle))
                {
                    MessageBox.Show("rec");
                }
            }
        }

        private void Keyboard_Loaded(object sender, RoutedEventArgs e)
        {

           
        }

        private void H_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Hi");
        }
    }
}
