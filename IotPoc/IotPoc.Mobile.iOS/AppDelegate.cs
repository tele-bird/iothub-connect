using Foundation;
using UIKit;

using IotPoc.Mobile.Common;
using Microsoft.Azure.Devices.Client;
using System;
using IotPoc.Common;
using IotPoc.Device.Iot;

namespace IotPoc.Mobile.iOS
{
    public class Application
    {
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, "AppDelegate");
        }
    }

    [Register("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
        UINavigationController navController;
        UITableViewController homeViewController;

        public static AppDelegate Current { get; private set; }

        public IotPoc.Mobile.Common.Mobile MobileAppAsIotDevice { get; private set; }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Current = this;

            // create a new window instance based on the screen size
            window = new UIWindow(UIScreen.MainScreen.Bounds);

            // make the window visible
            window.MakeKeyAndVisible();

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
            MobileAppAsIotDevice.Start();

            // create our nav controller
            navController = new UINavigationController();

            // create our Todo list screen
            homeViewController = new HomeScreen();

            // push the view controller onto the nav controller and show the window
            navController.PushViewController(homeViewController, false);
            window.RootViewController = navController;
            window.MakeKeyAndVisible();

            return true;
        }

        private void MobileAppAsIotDevice_StateChanged(MobileState oldValue, MobileState newValue)
        {
            Console.WriteLine($"mobile changed from {oldValue} to {newValue}");
        }

        private void MobileAppAsIotDevice_DirectMethodResponseSent(string methodName, MethodResponse methodResponse)
        {
            Console.WriteLine($"{methodName} sent {methodResponse.ResultAsJson}");
        }

        private void MobileAppAsIotDevice_DirectMethodInvoked(string methodName, MethodRequest methodRequest)
        {
            Console.WriteLine($"{methodName} received {methodRequest.DataAsJson}");
        }

        private void MobileAppAsIotDevice_EventPublished(string deviceId, string payload, IotDeviceEventPropertyCollection properties)
        {
            Console.WriteLine($"{deviceId} published event with payload: [{payload}] properties: [{properties}]");
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
            Console.WriteLine($"telemetry changed: {newValue}");
        }

        private void MobileAppAsIotDevice_ConsoleStateChanged(string consoleDsn, ConsoleState oldValue, ConsoleState newValue)
        {
            Console.WriteLine($"{consoleDsn} changed from {oldValue} to {newValue}.");
        }
    }
}


