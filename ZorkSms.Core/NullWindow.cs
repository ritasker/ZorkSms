using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZMachine.VM;

namespace ZorkSms.Core
{
    public class NullWindow : Window
    {
        public NullWindow() : this(new NullFormatter()) { }

        private NullWindow(Formatter formatter) : base(formatter)
        {

        }
    }
}
