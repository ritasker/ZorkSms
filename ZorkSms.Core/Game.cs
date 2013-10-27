using System;

namespace ZorkSms.Core
{
    public class Game
    {
        private ClockworkZMachine _virtualMachine;

        public event EventHandler<PrintCompletedEventArgs> PrintCompleted;

        protected Game(ClockworkZMachine virtualMachine)
        {
            _virtualMachine = virtualMachine;
            _virtualMachine.Window.PrintCompleted += OnPrintCompleted;
        }

        protected Game(ClockworkZMachine virtualMachine, string message) : this(virtualMachine)
        {
            _virtualMachine.Process(message);
        }

        public static Game CreateNew(byte[] data, IEnumerable<string> commands)
        {
            return new Game(ClockworkZMachine.Create(data, commands));
        }

        public static Game Restore(BinaryReader reader, string message)
        {
            return new Game(ClockworkZMachine.Load(reader), message);
        }

        public void Process(string message)
        {
            _virtualMachine.Process(message);
        }

        public void Start()
        {
            _virtualMachine.Start();
        }

        private void OnPrintCompleted(object sender, PrintCompletedEventArgs args)
        {
            if (PrintCompleted != null) PrintCompleted(this, args);
        }

        public void Save(BinaryWriter writer)
        {
            _virtualMachine.Save(writer);
        }
    }
}
