using System;
using System.IO;
using Android.App;
using IotPoc.Common;
using IotPoc.Mobile.Common;
using Microsoft.Azure.Devices.Client;

namespace IotPoc.Mobile.Droid
{
    [Application]
    public class IotDevicesApp : Application
    {
        public static IotDevicesApp Current { get; private set; }

        public string[] DeviceDsns = { "device1", "device2" };

        public Common.Mobile MobileAppAsIotDevice { get; private set; }

        public IotDevicesApp(IntPtr handle, global::Android.Runtime.JniHandleOwnership transfer)
            : base(handle, transfer)
        {
            Current = this;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            MobileAppAsIotDevice = new Common.Mobile(
                MobileConstants.HostName, 
                MobileConstants.DeviceId, 
                MobileConstants.AccessKey, 
                MobileConstants.TransportType);
            // subscribe to IotDevice events:
            MobileAppAsIotDevice.DeviceDisposed += MobileAppAsIotDevice_DeviceDisposed;
            MobileAppAsIotDevice.DeviceStarted += MobileAppAsIotDevice_DeviceStarted;
            MobileAppAsIotDevice.DeviceStopped += MobileAppAsIotDevice_DeviceStopped;
            MobileAppAsIotDevice.DirectMethodInvoked += MobileAppAsIotDevice_DirectMethodInvoked;
            MobileAppAsIotDevice.DirectMethodResponseSent += MobileAppAsIotDevice_DirectMethodResponseSent;
            MobileAppAsIotDevice.EventPublished += MobileAppAsIotDevice_EventPublished;
            // subscribe to Mobile events:
            MobileAppAsIotDevice.ConsoleStateChanged += MobileAppAsIotDevice_ConsoleStateChanged;
            MobileAppAsIotDevice.ConsoleTelemetryChanged += MobileAppAsIotDevice_ConsoleTelemetryChanged;
            MobileAppAsIotDevice.StateChanged += MobileAppAsIotDevice_StateChanged;
            MobileAppAsIotDevice.Start().Wait();
        }

        private void MobileAppAsIotDevice_StateChanged(MobileState oldValue, MobileState newValue)
        {
            Console.WriteLine($"mobile changed from {oldValue} to {newValue}");
        }

        private void MobileAppAsIotDevice_EventPublished(string deviceId, string payload, Device.Iot.IotDeviceEventPropertyCollection properties)
        {
            Console.WriteLine($"{deviceId} published event with payload: [{payload}] properties: [{properties}]");
        }

        private void MobileAppAsIotDevice_DirectMethodResponseSent(string methodName, MethodResponse methodResponse)
        {
            Utils.WriteToConsole(methodResponse.Status == (int)System.Net.HttpStatusCode.OK, $"{methodName} {Utils.GenerateText(methodResponse.Status == (int)System.Net.HttpStatusCode.OK)} with response: {methodResponse.ResultAsJson}");
        }

        private void MobileAppAsIotDevice_DirectMethodInvoked(string methodName, MethodRequest methodRequest)
        {
            Console.WriteLine($"{Environment.NewLine}{methodName} invoked with payload: {methodRequest.DataAsJson}");
        }

        private void MobileAppAsIotDevice_DeviceStopped(string deviceId, string hostName)
        {
            Console.WriteLine($"{deviceId} stopped on host: {hostName}");
        }

        private void MobileAppAsIotDevice_DeviceStarted(string deviceId, string hostName)
        {
            Console.WriteLine($"{deviceId} started on host: {hostName}");
        }

        private void MobileAppAsIotDevice_DeviceDisposed(string deviceId)
        {
            Console.WriteLine($"{deviceId} was disposed");
        }

        private void MobileAppAsIotDevice_ConsoleTelemetryChanged(ConsoleTelemetry oldValue, ConsoleTelemetry newValue)
        {
            Console.WriteLine($"telemetry from {newValue.Id} changed to: {newValue}");
        }

        private void MobileAppAsIotDevice_ConsoleStateChanged(string consoleDsn, ConsoleState oldValue, ConsoleState newValue)
        {
            Console.WriteLine($"{consoleDsn} state changed to: {newValue}");
        }
    }
}

