using System;
using System.Collections.Generic;
using ZMachine.VM;

namespace ZorkSms.Core
{
    public class ClockworkWindow : Window
    {
        public event EventHandler<PrintCompletedEventArgs> PrintCompleted;

        private readonly IList<string> _messageBuffer = new List<string>();

        public ClockworkWindow(Formatter formatter) : base(formatter){
            formatter.EndOutput = message =>
            {
                if (string.Equals(message, ">", StringComparison.OrdinalIgnoreCase))
                {
                    // Printing to screen has finished. Send response message.
                    SendResponseMessage();
                }

                // Buffer print message until finished.
                _messageBuffer.Add(message);
            };
        }

        private void SendResponseMessage()
        {
            _messageBuffer.Clear();

            if (PrintCompleted != null) return;
        }
    }
}
