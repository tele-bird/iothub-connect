using IotPoc.Common;
using IotPoc.Device.Iot;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IotPoc.Mobile.Common
{
    public class Mobile : IotDevice
    {
        private MobileState _state;
        private ConsoleState _consoleState;
        private ConsoleTelemetry _consoleTelemetry;

        public MobileState State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (!value.Equals(_state))
                {
                    MobileState oldValue = _state;
                    _state = value;
                    StateChanged?.Invoke(oldValue, value);
                }
            }
        }

        public string ConsoleSession { get; private set; }

        public ConsoleState ConsoleState
        {
            get
            {
                return _consoleState;
            }
            set
            {
                if((value != null) && !value.Id.Equals(ConsoleSession))
                {
                    throw new Exception($"Unexpected scenario: DSN of the new Console state doesn't match the Console session DSN.");
                }

                if (((value != null) && !value.Equals(_consoleState)) || ((value == null) && (_consoleState != null)))
                {
                    ConsoleState oldValue = _consoleState;
                    _consoleState = value;
                    ConsoleStateChanged?.Invoke(ConsoleSession, oldValue, value);
                }
            }
        }

        public ConsoleTelemetry ConsoleTelemetry
        {
            get
            {
                return _consoleTelemetry;
            }
            private set
            {
                if ((value != null) && !value.Id.Equals(ConsoleSession))
                {
                    throw new Exception($"Unexpected scenario: DSN of the new Console telemetry doesn't match the Console session DSN.");
                }

                if (((value != null) && !value.Equals(_consoleTelemetry)) || ((value == null) && (_consoleTelemetry != null)))
                {
                    ConsoleTelemetry oldValue = _consoleTelemetry;
                    _consoleTelemetry = value;
                    ConsoleTelemetryChanged?.Invoke(oldValue, value);
                }
            }
        }

        public delegate void StateChangedHandler(MobileState oldValue, MobileState newValue);
        public delegate void ConsoleStateChangedHandler(string consoleDsn, ConsoleState oldValue, ConsoleState newValue);
        public delegate void ConsoleTelemetryChangedHandler(ConsoleTelemetry oldValue, ConsoleTelemetry newValue);

        public event StateChangedHandler StateChanged;
        public event ConsoleStateChangedHandler ConsoleStateChanged;
        public event ConsoleTelemetryChangedHandler ConsoleTelemetryChanged;

        public Mobile(string hostName, string deviceId, string deviceAccessKey, TransportType transportType)
            : base(hostName, deviceId, deviceAccessKey, transportType)
        {
            _state = new MobileState
            {
                Id = deviceId,
            };
            _consoleState = null;
            _consoleTelemetry = null;
        }

        #region C2D Direct Methods

        protected override IotDeviceMethod[] GetNamedMethods()
        {
            return new[]
                {
                    //new IotDeviceMethod
                    //{
                    //    Name = Constants.NamedMethodGetMobile,
                    //    Callback = Get,
                    //    Context = null
                    //},
                    //new IotDeviceMethod
                    //{
                    //    Name = Constants.NamedMethodSetMobile,
                    //    Callback = Set,
                    //    Context = null
                    //},
                    new IotDeviceMethod
                    {
                        Name = Constants.NamedMethodSetMobileConsoleState,
                        Callback = SetConsoleState,
                        Context = null
                    },
                    new IotDeviceMethod
                    {
                        Name = Constants.NamedMethodSetMobileConsoleTelemetry,
                        Callback = SetConsoleTelemetry,
                        Context = null
                    },
                    new IotDeviceMethod
                    {
                        Name = Constants.NamedMethodSetMobileConsoleSession,
                        Callback = SetConsoleSession,
                        Context = null
                    },
                    new IotDeviceMethod
                    {
                        Name = Constants.NamedMethodUnsetMobileConsoleSession,
                        Callback = UnsetConsoleSession,
                        Context = null
                    }
                };
        }

        //private Task<MethodResponse> Get(MethodRequest methodRequest, object userContext)
        //{
        //    RequestReceived?.Invoke(methodRequest.Name, methodRequest);
        //    Task<MethodResponse> task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Dto)), (int)HttpStatusCode.OK));
        //    ResponseSent?.Invoke(methodRequest.Name, task.Result);
        //    return task;
        //}

        //private Task<MethodResponse> Set(MethodRequest methodRequest, object userContext)
        //{
        //    RequestReceived?.Invoke(methodRequest.Name, methodRequest);
        //    string json = Encoding.UTF8.GetString(methodRequest.Data);
        //    Dto = JsonConvert.DeserializeObject<MobileDto>(json);

        //    // Acknowlege the direct method call with a 200 success message
        //    string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Executed direct method: {methodRequest.Name}" });
        //    Task<MethodResponse> task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.OK));
        //    ResponseSent?.Invoke(methodRequest.Name, task.Result);
        //    return task;
        //}

        private Task<MethodResponse> SetConsoleState(MethodRequest methodRequest, object userContext)
        {
            FireDirectMethodInvoked(methodRequest.Name, methodRequest);
            string json = Encoding.UTF8.GetString(methodRequest.Data);
            var consoleState = JsonConvert.DeserializeObject<ConsoleState>(json);

            Task<MethodResponse> task = null;
            if(ConsoleSession != null && ConsoleSession.Equals(consoleState.Id))
            {
                ConsoleState = consoleState;

                // Acknowlege the direct method call with a 200 success message
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Executed direct method: {methodRequest.Name}" });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.OK));
            }

            FireDirectMethodResponseSent(methodRequest.Name, task.Result);
            return task;
        }

        private Task<MethodResponse> SetConsoleTelemetry(MethodRequest methodRequest, object userContext)
        {
            FireDirectMethodInvoked(methodRequest.Name, methodRequest);
            string json = Encoding.UTF8.GetString(methodRequest.Data);
            var consoleTelemetry = JsonConvert.DeserializeObject<ConsoleTelemetry>(json);

            Task<MethodResponse> task = null;
            if (ConsoleSession != null && ConsoleSession.Equals(consoleTelemetry.Id))
            {
                ConsoleTelemetry = consoleTelemetry;

                // Acknowlege the direct method call with a 200 success message
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Executed direct method: {methodRequest.Name}" });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.OK));
            }

            FireDirectMethodResponseSent(methodRequest.Name, task.Result);
            return task;
        }

        private Task<MethodResponse> SetConsoleSession(MethodRequest methodRequest, object userContext)
        {
            FireDirectMethodInvoked(methodRequest.Name, methodRequest);
            string json = Encoding.UTF8.GetString(methodRequest.Data);
            var consoleState = JsonConvert.DeserializeObject<ConsoleState>(json);

            Task<MethodResponse> task = null;
            if(ConsoleSession == null)
            {
                ConsoleSession = consoleState.Id;
                ConsoleState = consoleState;

                // Acknowlege the direct method call with a 200 success message
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Executed direct method: {methodRequest.Name}" });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.OK));
            }
            else
            {
                // return 409 Conflict:
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"A Console session already exists for {ConsoleSession}." });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.Conflict));
            }

            FireDirectMethodResponseSent(methodRequest.Name, task.Result);
            return task;
        }

        private Task<MethodResponse> UnsetConsoleSession(MethodRequest methodRequest, object userContext)
        {
            FireDirectMethodInvoked(methodRequest.Name, methodRequest);
            string json = Encoding.UTF8.GetString(methodRequest.Data);
            var consoleDsn = JsonConvert.DeserializeObject<string>(json);

            // if the existing MobileSession matches the value provided, then unset it:
            Task<MethodResponse> task = null;
            if (ConsoleSession != null && ConsoleSession.Equals(consoleDsn))
            {
                ConsoleSession = null;
                ConsoleState = null;

                // Acknowlege the direct method call with a 200 success message
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Executed direct method: {methodRequest.Name}" });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.OK));
            }
            else
            {
                // return 409 Conflict:
                string jsonResponse = JsonConvert.SerializeObject(new DirectMethodResponse() { Result = $"Console session did not match (consoleDsn: {consoleDsn} current value: {ConsoleSession})." });
                task = Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(jsonResponse), (int)HttpStatusCode.Conflict));
            }

            FireDirectMethodResponseSent(methodRequest.Name, task.Result);
            return task;
        }

        #endregion

        #region D2C Messages

        /// <summary>
        /// A one-way D2C Message to request that a session be started with a Device.
        /// If successful, Func invokes direct method: SetConsoleSession() to initialize the MobileSession property.
        /// </summary>
        /// <param name="deviceDsn">The id of the Device with which to request a session.</param>
        /// <returns></returns>
        public async Task RequestDeviceSession(string deviceDsn)
        {
            if(ConsoleSession != null)
            {
                throw new Exception($"A session already exists with {ConsoleSession}.");
            }

            IotDeviceEventPropertyCollection properties = new IotDeviceEventPropertyCollection(new Tuple<string, string>(Constants.D2CMessageTypePropertyKey, DeviceEventType.MobileRequestDeviceSession.ToString()));
            await PublishEvent(JsonConvert.SerializeObject(deviceDsn), properties);
        }

        /// <summary>
        /// A one-way D2C Message to request that a sesion be ended with a Device.
        /// If successful, Func invokes direct method: SetDeviceession() to de-initialize the MobileSession property.
        /// </summary>
        /// <param name="deviceDsn">The id of the Device with which to end a session.</param>
        /// <returns></returns>
        public async Task EndDeviceSession(string deviceDsn)
        {
            IotDeviceEventPropertyCollection properties = new IotDeviceEventPropertyCollection(new Tuple<string, string>(Constants.D2CMessageTypePropertyKey, DeviceEventType.MobileEndDeviceSession.ToString()));
            await PublishEvent(JsonConvert.SerializeObject(deviceDsn), properties);
        }

        /// <summary>
        /// A one-way D2C Message to request that a command be executed on a Device.
        /// If successful, Func invokes direct method: SetDeviceSession() when the Device's state changes.
        /// </summary>
        /// <param name="consoleState"></param>
        /// <returns></returns>
        public async Task RequestDeviceCommand(ConsoleState consoleState)
        {
            IotDeviceEventPropertyCollection properties = new IotDeviceEventPropertyCollection(new Tuple<string, string>(Constants.D2CMessageTypePropertyKey, DeviceEventType.MobileRequestDeviceCommand.ToString()));
            await PublishEvent(JsonConvert.SerializeObject(consoleState), properties);
        }

        public async Task SetConsoleTelemetryInterval(int telemetryInterval)
        {
            ConsoleState consoleState = new ConsoleState(ConsoleState);
            consoleState.TelemetryInterval = telemetryInterval;
            await RequestDeviceCommand(consoleState);
        }

        public async Task SetConsoleTelemetryColor(ConsoleColor consoleColor)
        {
            ConsoleState consoleState = new ConsoleState(ConsoleState);
            consoleState.ConsoleColor = consoleColor;
            await RequestDeviceCommand(consoleState);
        }

        public async Task SetConsoleTelemetryState(ConsoleTelemetryState consoleTelemetryState)
        {
            ConsoleState consoleState = new ConsoleState(ConsoleState);
            consoleState.TelemetryState = consoleTelemetryState;
            await RequestDeviceCommand(consoleState);
        }

        #endregion
    }
}
