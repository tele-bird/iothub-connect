using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public class ConsoleSessionNotification
    {
        public string OldMobileSession { get; set; }

        public string NewMobileSession { get; set; }

        public ConsoleState ConsoleState { get; set; }
    }
}
