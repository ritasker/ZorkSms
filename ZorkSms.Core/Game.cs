using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZorkSms.Core
{
    public class Game
    {
        private ClockworkZMachine _virtualMachine;

        public event EventHandler<PrintCompletedEventArgs> PrintCompleted;

        protected Game(byte[] data)
        {
            _virtualMachine = ClockworkZMachine.Create(data);
            _virtualMachine.Window.PrintCompleted += OnPrintCompleted;
        }

        protected Game(byte[] data, string message) : this(data)
        {
            _virtualMachine.Process(message);
        }

        public static Game CreateNew(byte[] data)
        {
            return new Game(data);
        }

        public static Game Restore(byte[] data, string message)
        {
            return new Game(data, message);
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

        public byte[] Save()
        {
            return _virtualMachine.Save();
        }
    }
}
