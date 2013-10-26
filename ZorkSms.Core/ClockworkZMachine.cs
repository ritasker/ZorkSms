using System;
using ZMachine.VM;
using ZMachine.VM.Console.Formatters;

namespace ZorkSms.Core
{
    class ClockworkZMachine : VirtualMachine
    {
        public Window Window { get; private set; }

        protected ClockworkZMachine() {
            Window = new ClockworkWindow(new SimpleFormatter());
        }

        public static ClockworkZMachine Create(byte[] storyBytes)
        {
            var ret = new ClockworkZMachine();
            ret.Load(storyBytes);
            return ret;
        }

        protected override Guid Load(byte[] storyBytes)
        {
            Guid g = base.Load(storyBytes);

            z_io.Screen.UpperWindow = new NullWindow();
            z_io.Screen.LowerWindow = Window;
            z_io.Reset();
            z_io.Keyboard = new ClockworkKeyboard();

            return g;
        }
    }
}
