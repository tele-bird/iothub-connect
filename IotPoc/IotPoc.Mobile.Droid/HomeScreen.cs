using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using IotPoc.Mobile.Droid.ApplicationLayer;

namespace IotPoc.Mobile.Droid.Screens
{
    /// <summary>
    /// Main ListView screen displays a list of tasks, plus an [Add] button
    /// </summary>
    [Activity(Label = "Devices", MainLauncher = true, Icon = "@mipmap/ic_launcher", AlwaysRetainTaskState = true)]
    public class HomeScreen : Activity
    {
        DeviceListAdapter _deviceListAdapter;
        IList<string> _devicesList;
        Button _addTaskButton;
        ListView _devicesListView;

        public HomeScreen()
            : base()
        {

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // set our layout to be the home screen
            SetContentView(Resource.Layout.HomeScreen);

            //Find our controls
            _devicesListView = FindViewById<ListView>(Resource.Id.TaskList);
            //addTaskButton = FindViewById<Button> (Resource.Id.AddButton);

            // wire up add task button handler
            if (_addTaskButton != null)
            {
                _addTaskButton.Click += (sender, e) => {
                    StartActivity(typeof(DeviceDetailsScreen));
                };
            }

            // wire up task click handler
            if (_devicesListView != null)
            {
                _devicesListView.ItemClick += (object sender, AdapterView.ItemClickEventArgs e) => {
                    var deviceDetailsScreenIntent = new Intent(this, typeof(DeviceDetailsScreen));
                    deviceDetailsScreenIntent.PutExtra("DeviceDsn", _devicesList[e.Position]);
                    StartActivity(deviceDetailsScreenIntent);
                };
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            _devicesList = new List<string>(IotDevicesApp.Current.DeviceDsns);

            // create our adapter
            _deviceListAdapter = new DeviceListAdapter(this, _devicesList);

            //Hook up our adapter to our ListView
            _devicesListView.Adapter = _deviceListAdapter;
        }

        protected override void OnPause()
        {
            base.OnPause();
        }
    }
}