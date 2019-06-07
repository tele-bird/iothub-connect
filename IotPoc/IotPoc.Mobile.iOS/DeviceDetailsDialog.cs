using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using IotPoc.Common;
using IotPoc.Mobile.Common;
using MonoTouch.Dialog;
using UIKit;

namespace IotPoc.Mobile.iOS
{
    public class DeviceDetailsDialog
    {
        [Skip]
        private ConsoleState _console = null;

        public DeviceDetailsDialog(ConsoleState console)
        {
            _console = console;
            Temperature = "0";
            Humidity = "0";
            Timestamp = DateTime.MinValue.ToString();
        }

        [Section("Controls")]
        [Caption("Telemetry")]
        public ConsoleTelemetryState State
        {
            get
            {
                return _console.TelemetryState;
            }
            set
            {
                _console.TelemetryState = value;
            }
        }

        [Caption("Interval")]
        public ConsoleTelemetryInterval TelemetryInterval
        {
            get
            {
                return _console.TelemetryInterval.HasValue
                    ? (ConsoleTelemetryInterval)Enum.Parse(typeof(ConsoleTelemetryInterval), _console.TelemetryInterval.Value.ToString())
                    : ConsoleTelemetryInterval.One;
            }
            set
            {
                _console.TelemetryInterval = Convert.ToInt32(value);
            }
        }

        [Caption("Color")]
        public ConsoleColor ConsoleColor
        {
            get
            {
                return _console.ConsoleColor.HasValue ? _console.ConsoleColor.Value : ConsoleColor.Gray;
            }
            set
            {
                _console.ConsoleColor = value;
            }
        }

        [Section("Telemetry")]
        [Caption("Temperature")]
        [Entry(Placeholder = "?")]
        public string Temperature { get; set; }

        [Caption("Humidity")]
        [Entry(Placeholder = "?")]
        public string Humidity { get; set; }

        [Caption("Timestamp")]
        [Entry(Placeholder = "?")]
        public string Timestamp { get; set; }

        internal bool TryUpdate(ConsoleState input, out ConsoleState output)
        {
            output = null;
            if (!_console.Equals(input))
            {
                output = new ConsoleState(_console);
            }
            return output != null;
        }
    }
}