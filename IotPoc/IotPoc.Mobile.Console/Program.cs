using IotPoc.Common;
using IotPoc.Device.Iot;
using IotPoc.Mobile.Common;
using Microsoft.Azure.Devices.Client;
using System;
using System.Net;

namespace IotPoc.Mobile.Console
{
    class Program
    {
        private static Common.Mobile _mobile = null;

        static void Main(string[] args)
        {
            try
            {
                using (_mobile = new Common.Mobile(
                    MobileConstants.HostName, 
                    MobileConstants.DeviceId, 
                    MobileConstants.AccessKey,
                    MobileConstants.TransportType))
                {
                    _mobile.ConsoleStateChanged += Mobile_ConsoleStateChanged;
                    _mobile.ConsoleTelemetryChanged += Mobile_ConsoleTelemetryChanged;
                    _mobile.DeviceDisposed += Mobile_DeviceDisposed;
                    _mobile.DeviceStarted += Mobile_DeviceStarted;
                    _mobile.DeviceStopped += Mobile_DeviceStopped;
                    _mobile.EventPublished += Mobile_EventPublished;
                    _mobile.DirectMethodInvoked += Mobile_DirectMethodInvoked;
                    _mobile.DirectMethodResponseSent += Mobile_DirectMethodResponseSent;
                    _mobile.StateChanged += Mobile_StateChanged;
                    _mobile.Start().Wait();
                    System.Console.WriteLine("Press <enter> to close.");
                    System.Console.ReadLine();
                    //string deviceDsn = "temp";
                    //while (!deviceDsn.Equals("exit"))
                    //{
                        //System.Console.WriteLine("Enter DSN of device to control, or \"exit\" to quit:");
                        //deviceDsn = System.Console.ReadLine();
                        //if (!deviceDsn.Equals("exit"))
                        //{
                        //    _mobile.RequestDeviceSession(deviceDsn).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.SetConsoleTelemetryInterval(3).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.SetConsoleTelemetryColor(ConsoleColor.Blue).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.SetConsoleTelemetryState(ConsoleTelemetryState.On).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.SetConsoleTelemetryInterval(1).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.SetConsoleTelemetryState(ConsoleTelemetryState.Off).Wait();
                        //    System.Console.ReadLine();
                        //    _mobile.EndDeviceSession(deviceDsn).Wait();
                        //}
                    //}
                }
            }
            catch (Exception exc)
            {
                int x = 0;
            }
        }

        private static void Mobile_StateChanged(IotPoc.Common.MobileState oldValue, IotPoc.Common.MobileState newValue)
        {
            System.Console.WriteLine($"mobile changed from {oldValue} to {newValue}");
        }

        private static void Mobile_DirectMethodResponseSent(string methodName, MethodResponse methodResponse)
        {
            Utils.WriteToConsole(methodResponse.Status == (int)HttpStatusCode.OK, $"{methodName} {Utils.GenerateText(methodResponse.Status == (int)HttpStatusCode.OK)} with response: {methodResponse.ResultAsJson}");
        }

        private static void Mobile_DirectMethodInvoked(string methodName, MethodRequest methodRequest)
        {
            System.Console.WriteLine($"{Environment.NewLine}{methodName} invoked with payload: {methodRequest.DataAsJson}");
        }

        private static void Mobile_EventPublished(string deviceId, string payload, IotDeviceEventPropertyCollection properties)
        {
            System.Console.WriteLine($"{deviceId} published event with payload: [{payload}] properties: [{properties}]");
        }

        private static void Mobile_DeviceStopped(string deviceId, string hostName)
        {
            System.Console.WriteLine($"{deviceId} stopped on host: {hostName}");
        }

        private static void Mobile_DeviceStarted(string deviceId, string hostName)
        {
            System.Console.WriteLine($"{deviceId} started on host: {hostName}");
            UpdateConsoleTitle();
        }

        private static void Mobile_DeviceDisposed(string deviceId)
        {
            System.Console.WriteLine($"{deviceId} was disposed");
        }

        private static void Mobile_ConsoleTelemetryChanged(ConsoleTelemetry oldValue, ConsoleTelemetry newValue)
        {
            UpdateConsoleTitle();
        }

        private static void Mobile_ConsoleStateChanged(string consoleDsn, IotPoc.Common.ConsoleState oldValue, IotPoc.Common.ConsoleState newValue)
        {
            UpdateConsoleTitle();
        }

        private static void UpdateConsoleTitle()
        {
            if(_mobile.ConsoleSession == null)
            {
                System.Console.Title = "(No device session)";
            }
            else
            {
                System.Console.Title = $"Device: {_mobile.ConsoleState} Telemetry: {Utils.GenerateText(_mobile.ConsoleTelemetry)}";
            }
        }
    }
}
