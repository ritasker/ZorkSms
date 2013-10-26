using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZorkSms.Core
{
    class Game
    {
        private ClockworkZMachine _virtualMachine;

        public Game()
        {
            
        }

        public void Start(string path)
        {
            byte[] storyBytes;

            using (var stream = new FileStream(path, FileMode.Open))
            {
                storyBytes = new byte[stream.Length];
                stream.Read(storyBytes, 0, (int)stream.Length);
                stream.Close();
            }

            _virtualMachine = ClockworkZMachine.Create(storyBytes);
            _virtualMachine.Start();
        }
    }
}
