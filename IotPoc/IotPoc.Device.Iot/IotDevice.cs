using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IotPoc.Device.Iot
{
    public abstract class IotDevice : IDisposable
    {
        public string DeviceId { get; private set; }

        public string HostName { get; private set; }

        public TransportType TransportType { get; private set; }

        private DeviceClient _deviceClient;

        private bool _started = false;

        private bool _initialized = false;

        private bool _disposed = false;

        // The device connection string to authenticate the device with your IoT hub.
        private readonly string ConnectionStringFormat = "HostName={0};DeviceId={1};SharedAccessKey={2}";

        public delegate void DeviceStartedHandler(string deviceId, string hostName);
        public delegate void DeviceStoppedHandler(string deviceId, string hostName);
        public delegate void EventPublishedHandler(string deviceId, string payload, IotDeviceEventPropertyCollection properties);
        public delegate void DeviceDisposedHandler(string deviceId);
        public delegate void DirectMethodInvokedHandler(string methodName, MethodRequest methodRequest);
        public delegate void DirectMethodResponseSentHandler(string methodName, MethodResponse methodResponse);

        public event DeviceStartedHandler DeviceStarted;
        public event DeviceStoppedHandler DeviceStopped;
        public event EventPublishedHandler EventPublished;
        public event DeviceDisposedHandler DeviceDisposed;
        public event DirectMethodInvokedHandler DirectMethodInvoked;
        public event DirectMethodResponseSentHandler DirectMethodResponseSent;

        protected IotDevice(string hostName, string deviceId, string deviceAccessKey, TransportType transportType)
        {
            DeviceId = deviceId;
            HostName = hostName;
            TransportType = transportType;
            string connectionString = string.Format(ConnectionStringFormat, HostName, deviceId, deviceAccessKey);
            //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            _deviceClient = DeviceClient.CreateFromConnectionString(connectionString, TransportType);
        }

        public async Task Start()
        {
            try
            {
                if (_started)
                {
                    throw new Exception("Device already started.");
                }
                await _deviceClient.OpenAsync();
                _started = true;
                DeviceStarted?.Invoke(DeviceId, HostName);
                if (!_initialized)
                {
                    IotDeviceMethod[] methods = GetNamedMethods();
                    foreach (IotDeviceMethod method in methods)
                    {
                        _deviceClient.SetMethodHandlerAsync(method.Name, method.Callback, method.Context).Wait();
                    }
                    _initialized = true;
                }
            }
            catch (Exception exc)
            {
                int x = 0;
            }
        }

        public async Task Stop()
        {
            if (!_started)
            {
                throw new Exception("Device already stopped.");
            }
            await _deviceClient.CloseAsync();
            _started = false;
            DeviceStopped?.Invoke(DeviceId, HostName);
        }

        protected abstract IotDeviceMethod[] GetNamedMethods();

        protected async Task PublishEvent(string payload, IotDeviceEventPropertyCollection properties)
        {
            try
            {
                Message message = new Message(Encoding.ASCII.GetBytes(payload));
                if (properties != null)
                {
                    foreach (string key in properties.Keys)
                    {
                        message.Properties.Add(key, properties[key]);
                    }
                }

                await _deviceClient.SendEventAsync(message);
                EventPublished?.Invoke(DeviceId, payload, properties);
            }
            catch (Exception exc)
            {
                int x = 0;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_deviceClient != null)
                    {
                        if (_started)
                        {
                            Stop().Wait();
                        }
                        _deviceClient.Dispose();
                        DeviceDisposed?.Invoke(DeviceId);
                    }
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void FireDirectMethodInvoked(string methodName, MethodRequest methodRequest)
        {
            DirectMethodInvoked?.Invoke(methodName, methodRequest);
        }

        protected void FireDirectMethodResponseSent(string methodName, MethodResponse methodResponse)
        {
            DirectMethodResponseSent?.Invoke(methodName, methodResponse);
        }
    }
}
