using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public class ConsoleState
    {
        private static readonly string DISCONNECTED = "disconnected";
        private static readonly string TO_STRING_DELIMITER = ",";

        public ConsoleState()
        {
        }

        public ConsoleState(ConsoleState original)
        {
            Id = original.Id;
            Connected = original.Connected;
            TelemetryState = original.TelemetryState;
            TelemetryInterval = original.TelemetryInterval;
            ConsoleColor = original.ConsoleColor;
        }

        public string Id { get; set; }

        [JsonIgnore]
        public bool Connected { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ConsoleTelemetryState TelemetryState { get; set; }

        public int? TelemetryInterval { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ConsoleColor? ConsoleColor { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);// $"{Id} ({(!Connected ? DISCONNECTED : TelemetryState.ToString() + TO_STRING_DELIMITER + TelemetryInterval.ToString() + TO_STRING_DELIMITER + ConsoleColor.ToString())})";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + Connected.GetHashCode() + TelemetryState.GetHashCode() + TelemetryInterval.GetHashCode() + ConsoleColor.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is ConsoleState)
            {
                ConsoleState other = (ConsoleState)obj;
                result = Id.Equals(other.Id)
                    && Connected.Equals(other.Connected)
                    && TelemetryState.Equals(other.TelemetryState)
                    && TelemetryInterval.Equals(other.TelemetryInterval)
                    && ConsoleColor.Equals(other.ConsoleColor);
            }
            return result;
        }
    }
}
