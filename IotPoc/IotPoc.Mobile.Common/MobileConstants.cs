using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotPoc.Mobile.Common
{
    public static class MobileConstants
    {
        public static string HostName = "myhub.azure-devices.net";
        public static string DeviceId = "mydevice";
        public static string AccessKey = "mydeviceaccesskey";
        public static TransportType TransportType = TransportType.Mqtt;
    }
}
