using System.Collections.Generic;
using ZMachine.VM;

namespace ZorkSms.Core
{
    public class ClockworkKeyboard : Keyboard
    {
        private readonly Queue<string> _messages = new Queue<string>();

        public void EnqueueMessage(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                _messages.Enqueue(message);
            }
        }

        public override char ReadChar(int? time)
        {
            return base.ReadChar(time);
        }

        public override string ReadString(int? time)
        {
            string message = _messages.Dequeue();
            return message;
        }
    }
}
