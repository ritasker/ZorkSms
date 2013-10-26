using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ZorkSms.Core
{
    public class PrintCompletedEventArgs
    {
        public ReadOnlyCollection<string> Lines { get; private set; }

        public PrintCompletedEventArgs(IEnumerable<string> lines)
        {
            if (lines == null || !lines.Any()) Lines = new List<string>().AsReadOnly();

            Lines = new List<string>(lines).AsReadOnly();
        }
    }
}
