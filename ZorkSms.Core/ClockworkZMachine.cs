using System;
using System.Collections.Generic;
using System.IO;
using ZMachine.v3;
using ZMachine.VM;
using ZMachine.VM.Console.Formatters;

namespace ZorkSms.Core
{
    public class ClockworkZMachine : VirtualMachine
    {
        public ClockworkWindow Window { get; private set; }

        protected ClockworkZMachine() {
            Window = new ClockworkWindow(new SimpleFormatter());
        }

        public static ClockworkZMachine Create(byte[] storyBytes, IEnumerable<string> commands)
        {
            var ret = new ClockworkZMachine();
            ret.Load(storyBytes);

            foreach (var command in commands) ret.Process(command);

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

        public void Process(string message)
        {
            var keyboard = (ClockworkKeyboard)z_io.Keyboard;
            keyboard.EnqueueMessage(message);
        }

        public void Save(BinaryWriter writer)
        {
            z_processor.Save(writer);
        }

        public static ClockworkZMachine Load(BinaryReader reader)
        {
            var virtualMachine = new ClockworkZMachine();
            virtualMachine.z_processor = ClockworkZProcessor.Load(reader);

            virtualMachine.z_io = virtualMachine.z_processor.IO;
            virtualMachine.z_io.Screen = new ZMachine.v3.V3Screen(virtualMachine.z_processor as ZProcessor);

            virtualMachine.z_io.Screen.UpperWindow = new NullWindow();
            virtualMachine.z_io.Screen.LowerWindow = virtualMachine.Window;
            virtualMachine.z_io.Reset();
            virtualMachine.z_io.Keyboard = new ClockworkKeyboard();

            return virtualMachine;
        }
    }
}
