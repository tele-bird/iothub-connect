using IotPoc.Common;
using MonoTouch.Dialog;
using UIKit;

namespace IotPoc.Mobile.iOS
{
    public class DetailsScreen : DialogViewController
    {
        private ConsoleState _console;
        private DeviceDetailsDialog _dialog;
        private BindingContext _context;


        public DetailsScreen(ConsoleState console)
            : base(UITableViewStyle.Plain, null, true)
        {
            _console = console;
            _dialog = new DeviceDetailsDialog(console);
            _context = new BindingContext(this, _dialog, $"Device: {_console.Id}");
            Root = _context.Root;
            AppDelegate.Current.MobileAppAsIotDevice.ConsoleStateChanged += MobileAppAsIotDevice_ConsoleStateChanged;
            AppDelegate.Current.MobileAppAsIotDevice.ConsoleTelemetryChanged += MobileAppAsIotDevice_ConsoleTelemetryChanged;
        }

        public override async void ViewWillAppear(bool animated)
        {
            _context.Fetch();
            ConsoleState newConsole;
            if (_dialog.TryUpdate(_console, out newConsole))
            {
                await AppDelegate.Current.MobileAppAsIotDevice.RequestDeviceCommand(newConsole);
            }
            base.ViewWillAppear(animated);
            //if (IsMovingToParentViewController)
            //{
            //    AppDelegate.Current.DeviceListener.DeviceEventOccurred += DeviceListener_DeviceEventOccurred;
            //    await AppDelegate.Current.DeviceListener.Start(_console.Id);
            //}
        }

        private void MobileAppAsIotDevice_ConsoleStateChanged(string consoleDsn, ConsoleState oldValue, ConsoleState newValue)
        {
            _console = newValue;
        }

        public override async void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            //if (IsMovingFromParentViewController)
            //{
            //    await AppDelegate.Current.DeviceListener.Stop();
            //    AppDelegate.Current.DeviceListener.DeviceEventOccurred -= DeviceListener_DeviceEventOccurred;
            //}
        }

        private void MobileAppAsIotDevice_ConsoleTelemetryChanged(ConsoleTelemetry oldValue, ConsoleTelemetry newValue)
        {
            InvokeOnMainThread(() =>
            {
                if (newValue.Id == _console.Id)
                {
                    ((EntryElement)Root[1].Elements[0]).Value = newValue.Temperature.ToString();
                    ((EntryElement)Root[1].Elements[1]).Value = newValue.Humidity.ToString();
                    ((EntryElement)Root[1].Elements[2]).Value = newValue.Timestamp.ToString();
                    //((EntryElement)Root[1].Elements[3]).Value = newValue.ToString();
                }
            });
        }
    }
}