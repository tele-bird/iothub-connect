using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Common
{
    public class ConsoleTelemetry
    {
        public string Id { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public string Timestamp { get; set; }

        public override string ToString()
        {
            return $"{Temperature} {Humidity} {Timestamp}";
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() + Temperature.GetHashCode() + Humidity.GetHashCode() + Timestamp.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is ConsoleTelemetry)
            {
                ConsoleTelemetry other = (ConsoleTelemetry)obj;
                result = Id.Equals(other.Id)
                    && Temperature.Equals(other.Temperature)
                    && Humidity.Equals(other.Humidity)
                    && Timestamp.Equals(other.Timestamp);
            }
            return result;
        }
    }
}
