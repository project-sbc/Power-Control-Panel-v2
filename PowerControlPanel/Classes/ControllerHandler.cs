using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SharpDX.XInput;

namespace Power_Control_Panel.PowerControlPanel.Classes
{
    public class ControllerHandler
    {
        public Thread controllerThread;
        public Controller controller;
        private Gamepad gamepadCurrent;
        private  Gamepad gamepadOld;

        public buttonEvents events = new buttonEvents();

        public void createGamePadStateCollectorLoop()
        {
            controllerThread = new Thread(new ThreadStart(gamePadStateCollector));
            controllerThread.IsBackground = true;
            controllerThread.Start();
        }

        public void gamePadStateCollector()
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
                    try
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
                    catch
                    {

                    }



                }


            }


        }


    }

    public class buttonEvents
    {
        public event EventHandler pressAEvent;
        public void RaiseEventA()
        {
            pressAEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }


        public event EventHandler pressXEvent;
        public void RaiseEventX()
        {
            pressXEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }

        public event EventHandler pressYEvent;
        public void RaiseEventY()
        {
            pressYEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }
        public event EventHandler pressBEvent;
        public void RaiseEventB()
        {
            pressBEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }
        public event EventHandler pressLBEvent;
        public void RaiseEventLB()
        {
            pressLBEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }
        public event EventHandler pressRBEvent;
        public void RaiseEventRB()
        {
            pressRBEvent?.Invoke(typeof(buttonEvents), EventArgs.Empty);
        }
    }

}
