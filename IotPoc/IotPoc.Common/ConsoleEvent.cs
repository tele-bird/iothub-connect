using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public class ConsoleEvent
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public DeviceEventType Type { get; set; }

        public string Payload { get; set; }

        public override string ToString()
        {
            return $"{Id} {Timestamp} {Type} {Payload}";
        }
    }

    public enum DeviceEventType
    {
        ConsoleSession = 0,
        ConsoleState = 1,
        ConsoleTelemetry = 2,
        MobileRequestDeviceSession = 3,
        MobileEndDeviceSession = 4,
        MobileRequestDeviceCommand = 5
    }
}
