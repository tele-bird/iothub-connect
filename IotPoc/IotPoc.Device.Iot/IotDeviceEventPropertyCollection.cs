using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Device.Iot
{
    public class IotDeviceEventPropertyCollection : Dictionary<string, string>
    {
        public IotDeviceEventPropertyCollection(params Tuple<string, string>[] properties)
            : base(properties.Length)
        {
            foreach(Tuple<string, string> property in properties)
            {
                Add(property.Item1, property.Item2);
            }
        }

        public override string ToString()
        {
            StringBuilder sbProperties = new StringBuilder("[");
            foreach (string key in Keys)
            {
                sbProperties.Append($" {key}:{this[key]} ");
            }
            sbProperties.Append("]");
            return sbProperties.ToString();
        }
    }
}
