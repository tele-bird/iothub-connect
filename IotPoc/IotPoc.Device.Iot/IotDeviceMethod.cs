using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Device.Iot
{
    public class IotDeviceMethod
    {
        public string Name { get; set; }

        public MethodCallback Callback { get; set; }

        public object Context { get; set; }
    }
}
