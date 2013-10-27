using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMachine.v3;

namespace ZorkSms.Core
{
    class ClockworkZProcessor : ZProcessor
    {
        public ClockworkZProcessor()
        {
            OnSave = HandleSave;
            OnRestore = HandleRestore;
        }

        private void HandleSave()
        {
        }

        private void HandleRestore()
        {
        }
    }
}
