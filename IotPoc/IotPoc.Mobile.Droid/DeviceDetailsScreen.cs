using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using IotPoc.Common;
using IotPoc.Mobile.Common;

namespace IotPoc.Mobile.Droid.Screens
{
    /// <summary>
    /// View/edit a Task
    /// </summary>
    [Activity(Label = "")]
    public class DeviceDetailsScreen : Activity
    {
        private const string UnknownValueText = "?";

        private string _deviceDsn = null;
        private ConsoleState _consoleState = null;
        //private Button _telemetryButton;
        private Switch _telemetrySwitch;
        private Spinner _intervalSpinner;
        private Spinner _colorSpinner;
        //private Button _saveButton;
        private TextView _temperatureTextView;
        private TextView _humidityTextView;
        private TextView _timestampTextView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            _deviceDsn = Intent.GetStringExtra("DeviceDsn");

            // set our layout to be the home screen
            SetContentView(Resource.Layout.TaskDetails);
            _telemetrySwitch = FindViewById<Switch>(Resource.Id.TelemetrySwitch);
            _intervalSpinner = FindViewById<Spinner>(Resource.Id.IntervalSpinner);
            _colorSpinner = FindViewById<Spinner>(Resource.Id.ColorSpinner);
            _temperatureTextView = FindViewById<TextView>(Resource.Id.TelemetryTemperatureText);
            _humidityTextView = FindViewById<TextView>(Resource.Id.TelemetryHumidityText);
            _timestampTextView = FindViewById<TextView>(Resource.Id.TelemetryTimestampText);

            var intervalSpinnerItems = new List<string>();
            foreach (ConsoleTelemetryInterval interval in Enum.GetValues(typeof(ConsoleTelemetryInterval)))
            {
                intervalSpinnerItems.Add(interval.ToString());
            }
            var intervalSpinnerAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, intervalSpinnerItems);
            intervalSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _intervalSpinner.Adapter = intervalSpinnerAdapter;

            var colorSpinnerItems = new List<string>();
            foreach (ConsoleColor consoleColor in Enum.GetValues(typeof(ConsoleColor)))
            {
                colorSpinnerItems.Add(consoleColor.ToString());
            }
            var colorSpinnerAdapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, colorSpinnerItems);
            intervalSpinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            _colorSpinner.Adapter = colorSpinnerAdapter;

            _temperatureTextView.Enabled = false;
            _humidityTextView.Enabled = false;
            _timestampTextView.Enabled = false;

            //_saveButton = FindViewById<Button>(Resource.Id.SaveButton);

            //// find all our controls
            //_telemetryButton = FindViewById<Button>(Resource.Id.TelemetryButton);
            //_telemetryButton.Enabled = !string.IsNullOrEmpty(_deviceId);

            //// button clicks 
            //_telemetryButton.Click += (sender, e) => 
            //{
            //    var deviceTelemetryScreenIntent = new Intent(this, typeof(DeviceTelemetryScreen));
            //    deviceTelemetryScreenIntent.PutExtra("DeviceId", _deviceId);
            //    StartActivity(deviceTelemetryScreenIntent);
            //};
            //_saveButton.Click += async (sender, e) =>
            //{
            //    _device.Id = _deviceId;
            //    _device.State = _telemetrySwitch.Checked ? DeviceState.Running : DeviceState.Idle;
            //    _device.TelemetryInterval = _intervalSpinner.SelectedItemPosition + 1;
            //    //_device.ConsoleColor = (ConsoleColor)_colorSpinner.SelectedItemPosition + 1;
            //    ConsoleColor consoleColor;
            //    if (Enum.TryParse<ConsoleColor>(_colorSpinner.SelectedItem.ToString(), out consoleColor))
            //    {
            //        _device.ConsoleColor = consoleColor;
            //    }
            //    await IotDevicesApp.Current.DeviceManager.SaveDevice(_device);
            //    Finish();
            //};

            // wire up event handlers last, to avoid premature invocations:
            _telemetrySwitch.CheckedChange += TelemetrySwitch_CheckedChange;
            _intervalSpinner.ItemSelected += IntervalSpinner_ItemSelected;
            _colorSpinner.ItemSelected += ColorSpinner_ItemSelected;
        }

        private async void ColorSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_consoleState != null)
            {
                _consoleState.ConsoleColor = (ConsoleColor)e.Position;
                //ConsoleColor consoleColor;
                //if (Enum.TryParse<ConsoleColor>(_colorSpinner.SelectedItem.ToString(), out consoleColor))
                //{
                //    _device.ConsoleColor = consoleColor;
                //}
                await IotDevicesApp.Current.MobileAppAsIotDevice.RequestDeviceCommand(_consoleState);
            }
        }

        private async void IntervalSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (_consoleState != null)
            {
                _consoleState.TelemetryInterval = e.Position + 1;
                await IotDevicesApp.Current.MobileAppAsIotDevice.RequestDeviceCommand(_consoleState);
            }
        }

        private async void TelemetrySwitch_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            if (_consoleState != null)
            {
                _consoleState.TelemetryState = e.IsChecked ? ConsoleTelemetryState.On : ConsoleTelemetryState.Off;
                await IotDevicesApp.Current.MobileAppAsIotDevice.RequestDeviceCommand(_consoleState);
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            if(string.IsNullOrEmpty(_deviceDsn))
            {
                throw new Exception($"Unexpected scenario: _deviceDsn is null or empty.");
            }

            await IotDevicesApp.Current.MobileAppAsIotDevice.RequestDeviceSession(_deviceDsn);
            IotDevicesApp.Current.MobileAppAsIotDevice.ConsoleStateChanged += MobileAppAsIotDevice_ConsoleStateChanged;
            IotDevicesApp.Current.MobileAppAsIotDevice.ConsoleTelemetryChanged += MobileAppAsIotDevice_ConsoleTelemetryChanged;
            Window.SetTitle($"Device: {_deviceDsn}");
            _temperatureTextView.Text = UnknownValueText;
            _humidityTextView.Text = UnknownValueText;
            _timestampTextView.Text = UnknownValueText;
        }

        private void MobileAppAsIotDevice_ConsoleStateChanged(string consoleDsn, ConsoleState oldValue, ConsoleState newValue)
        {
            _consoleState = newValue;
            RunOnUiThread(() =>
            {
                if(_consoleState == null)
                {
                    _telemetrySwitch.Checked = false;
                    _intervalSpinner.SetSelection(0);
                    _colorSpinner.SetSelection(0);
                    _temperatureTextView.Text = "?";
                    _humidityTextView.Text = "?";
                }
                else
                {
                    _telemetrySwitch.Checked = _consoleState.TelemetryState == ConsoleTelemetryState.On;
                    _intervalSpinner.SetSelection(GetPosition(_consoleState.TelemetryInterval));
                    _colorSpinner.SetSelection(_consoleState.ConsoleColor.HasValue ? Convert.ToInt32((short)_consoleState.ConsoleColor.Value) : 0);
                }
            });
        }

        private void MobileAppAsIotDevice_ConsoleTelemetryChanged(ConsoleTelemetry oldValue, ConsoleTelemetry newValue)
        {
            if(newValue.Id != _consoleState.Id)
            {
                throw new Exception($"Unexpected scenario: received event for deviceDsn: {newValue.Id}");
            }
            RunOnUiThread(() =>
            {
                _temperatureTextView.Text = newValue.Temperature.ToString();
                _humidityTextView.Text = newValue.Humidity.ToString();
                _timestampTextView.Text = newValue.Timestamp.ToString();
            });
        }

        private static int GetPosition(int? telemetryInterval)
        {
            int result = 0;
            if (telemetryInterval.HasValue)
            {
                result = telemetryInterval.Value - 1;
            }
            return result;
        }

        protected async override void OnPause()
        {
            _consoleState = null;
            base.OnPause();
            await IotDevicesApp.Current.MobileAppAsIotDevice.EndDeviceSession(_deviceDsn);
        }
    }
}