using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using MonoTouch.Dialog;
using System.Threading.Tasks;
using IotPoc.Common;

namespace IotPoc.Mobile.iOS
{
    /// <summary>
    /// A UITableViewController that uses MonoTouch.Dialog - displays the list of Tasks
    /// </summary>
    public class HomeScreen : DialogViewController
    {
        // 
        private List<string> _devices;

        // MonoTouch.Dialog individual TaskDetails view (uses /ApplicationLayer/TaskDialog.cs wrapper class)
        ConsoleState _currentConsole;

        public HomeScreen() : base(UITableViewStyle.Plain, null)
        {
            Initialize();
        }

        protected void Initialize()
        {
            string[] ds = { "device1", "device2" };
            _devices = new List<string>(2);
            _devices.AddRange(ds);
            //NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (UIBarButtonSystemItem.Add), false);
            //NavigationItem.RightBarButtonItem.Clicked += (sender, e) => { ShowTaskDetails(new DeviceDto()); };
            RefreshRequested += HomeScreen_RefreshRequested;
        }

        private async void HomeScreen_RefreshRequested(object sender, EventArgs e)
        {
            await PopulateTable();
            ReloadComplete();
        }

        protected void ShowTaskDetails(string deviceDsn)
        {
        }

        public void DeleteTask()
        {
            //if (currentItem.ID >= 0)
            //	AppDelegate.Current.TodoManager.DeleteTask (currentItem.ID);
            NavigationController.PopViewController(true);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            AppDelegate.Current.MobileAppAsIotDevice.ConsoleStateChanged += MobileAppAsIotDevice_ConsoleStateChanged;

            // reload/refresh
            PopulateTable();
        }

        public override void ViewWillDisappear(bool animated)
        {
            AppDelegate.Current.MobileAppAsIotDevice.ConsoleStateChanged -= MobileAppAsIotDevice_ConsoleStateChanged;

            base.ViewWillDisappear(animated);
        }

        protected async Task PopulateTable()
        {
            var rows = from t in _devices select new StringElement(t.ToString());
            var s = new Section();
            s.AddAll(rows);
            Root = new RootElement("Devices") { s };
        }
        public override void Selected(Foundation.NSIndexPath indexPath)
        {
            string device = _devices[indexPath.Row];
            AppDelegate.Current.MobileAppAsIotDevice.RequestDeviceSession(device);
        }
        private void MobileAppAsIotDevice_ConsoleStateChanged(string consoleDsn, ConsoleState oldValue, ConsoleState newValue)
        {
            if(_currentConsole == null)
            {
                _currentConsole = newValue;
                ActivateController(new DetailsScreen(_currentConsole));
            }
        }
        public override Source CreateSizingSource(bool unevenRows)
        {
            return new EditingSource(this);
        }
    }
}