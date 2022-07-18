using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public static class ControllerHandler
    {
        public static Thread controllerThread;
        public static Controller controller;
        private static Gamepad gamepadCurrent;
        private static Gamepad gamepadOld;

 

        public static void createGamePadStateCollectorLoop()
        {
            controllerThread = new Thread(new ThreadStart(gamePadStateCollector));
            controllerThread.IsBackground = true;
            controllerThread.Start();
        }

        public static void gamePadStateCollector()
        {
            while (GlobalVariables.useControllerFastThread)
            {

                if (controller == null)
                {
                    while (controller == null && GlobalVariables.useControllerFastThread)
                    {
                        controller = new Controller(UserIndex.One);
                        if (controller == null) { Task.Delay(4000); }
                    }

                }
                if (controller != null)
                {
                    if (controller.IsConnected)
                    {
                        gamepadCurrent = controller.GetState().Gamepad;
                        Task.Delay(15);
                        while (GlobalVariables.useControllerFastThread && controller.IsConnected)
                        {
                            gamepadOld = gamepadCurrent;
                            gamepadCurrent = controller.GetState().Gamepad;
                            
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.A) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.A))
                            {
                                pressA.RaiseEvent();
                            }
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.B) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.B))
                            {
                                pressB.RaiseEvent();
                            }
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.X) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.X))
                            {
                                pressX.RaiseEvent();
                            }
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.Y) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.Y))
                            {
                                pressY.RaiseEvent();
                            }
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder))
                            {
                                pressLB.RaiseEvent();
                            }
                            if (!gamepadOld.Buttons.HasFlag(GamepadButtonFlags.RightShoulder) && gamepadCurrent.Buttons.HasFlag(GamepadButtonFlags.RightShoulder))
                            {
                                pressRB.RaiseEvent();
                            }
                        }


                    }
                    else
                    {
                        while (!controller.IsConnected && GlobalVariables.useControllerFastThread)
                        {
                            Task.Delay(3000);
                            controller = new Controller(UserIndex.One);
                        }

                    }

                }


            }


        }


    }

    public static class pressA
    {
        public static event EventHandler pressAEvent;
        public static void RaiseEvent()
        {
            pressAEvent?.Invoke(typeof(pressA), EventArgs.Empty);
        }
    }

    public static class pressX
    {
        public static event EventHandler pressXEvent;
        public static void RaiseEvent()
        {
            pressXEvent?.Invoke(typeof(pressX), EventArgs.Empty);
        }
    }
    public static class pressY
    {
        public static event EventHandler pressYEvent;
        public static void RaiseEvent()
        {
            pressYEvent?.Invoke(typeof(pressA), EventArgs.Empty);
        }
    }
    public static class pressB
    {
        public static event EventHandler pressBEvent;
        public static void RaiseEvent()
        {
            pressBEvent?.Invoke(typeof(pressA), EventArgs.Empty);
        }
    }

    public static class pressLB
    {
        public static event EventHandler pressLBEvent;
        public static void RaiseEvent()
        {
            pressLBEvent?.Invoke(typeof(pressA), EventArgs.Empty);
        }
    }
    public static class pressRB
    {
        public static event EventHandler pressRBEvent;
        public static void RaiseEvent()
        {
            pressRBEvent?.Invoke(typeof(pressA), EventArgs.Empty);
        }
    }
}
