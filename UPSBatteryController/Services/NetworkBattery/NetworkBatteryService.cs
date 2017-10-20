using GroupAdr.Library.AsyncEvents;
using GroupAdr.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UPSBatteryController.Models.Settings;
using UPSBatteryController.Providers.Settings.EventArguments;
using UPSBatteryController.Services.LocalBattery;
using UPSBatteryController.Providers.Settings;

namespace UPSBatteryController.Services.NetworkBattery
{
    [Export(typeof(INetworkBatteryService))]
    public class NetworkBatteryService : INetworkBatteryService
    {
        #region Constants

        private int TimeOutSec = 10;

        #endregion

        #region Fields

        private ILogger _logger = LogFactory.GetLogger();
        private ISettingsProvider _settingsService;
        private IAsyncEventSource _eventSource;
        private UdpClient _udpClient;
        private int _secondsLeft;

        #endregion

        #region Properties

        /// <summary>
        /// Уровень батареи
        /// </summary>
        public double BatteryLevel { get; private set; }

        /// <summary>
        /// Состояние батарейки
        /// </summary>
        public BatteryStatus BatteryStatus { get; private set; }

        /// <summary>
        /// Оставшееся время работы батарейки
        /// </summary>
        public TimeSpan? BatteryRemainingTime { get; private set; }

        /// <summary>
        /// Происходит ли зарядка
        /// </summary>
        public bool IsBatteryCharging { get; private set; }

        #endregion

        [ImportingConstructor]
        public NetworkBatteryService(ISettingsProvider settingsService,
                                     IAsyncEventSource eventSource)
        {
            _settingsService = settingsService;
            _settingsService.SettingsChanged += SettingsServiceSettingsChanged;

            if (_settingsService.NetType == NetType.Client)
                StartListening();

            _eventSource = eventSource;
            _eventSource.Tick += _eventSourceTick;
        }

        #region Functions

        public void SendBatteryState(NetBatteryState batteryState)
        {
            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Broadcast, _settingsService.Port);

                byte[] send_buffer = ToJson(batteryState);

                if (send_buffer != null)
                    sock.SendTo(send_buffer, endPoint);
            }catch(Exception ex)
            {
                _logger.LogException(Level.Warn, ex, "Не удалось отправить UDP пакет");
            }
        }

        private byte[] ToJson(NetBatteryState state)
        {
            byte[] buffer = null;
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;

                var stream = new MemoryStream();
                using (StreamWriter sw = new StreamWriter(stream))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, state);
                    writer.Flush();
                    buffer = new byte[stream.Length];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, buffer.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(Level.Warn, ex, "Не удалось преобразовать BatteryState в Json");
            }

            return buffer;
        }

        private NetBatteryState FromJson(byte[] data)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;

            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return serializer.Deserialize(reader, typeof(NetBatteryState)) as NetBatteryState;
        }

        private void StartListening()
        {
            try
            {
                var broadcastAddress = new IPEndPoint(IPAddress.Any, _settingsService.Port);
                _udpClient = new UdpClient();
                _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                _udpClient.Client.Bind(broadcastAddress);
                _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), broadcastAddress);
            }catch(Exception ex)
            {
                _logger.LogException(Level.Warn, ex, "Не удалось начать прослушивание");
            }
        }

        private void StopListening()
        {
            if (_udpClient != null)
            {
                _udpClient.Close();
                _udpClient = null;
            }
        }

        private void UpdateBatteryStatus(BatteryStatus batteryStatus, double batteryLevel, TimeSpan? batteryRemainingTime, bool isBatteryCharging)
        {
            _secondsLeft = 0;

            BatteryLevel = batteryLevel;
            BatteryRemainingTime = batteryRemainingTime;
            IsBatteryCharging = isBatteryCharging;
            BatteryStatus = batteryStatus;

            BatteryChanged?.Invoke(this, new BatteryStateEventArgs(batteryStatus, batteryLevel, isBatteryCharging, batteryRemainingTime));
        }

        #endregion

        #region Event handlers

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var broadcastAddress = (IPEndPoint)result.AsyncState;
                byte[] data = _udpClient.EndReceive(result, ref broadcastAddress);
                NetBatteryState batteryState = FromJson(data);

                if (batteryState != null && _settingsService.Identifier == batteryState.Identifier)
                {
                    UpdateBatteryStatus(batteryState.Status, batteryState.Level, 
                                        batteryState.RemainingTime, batteryState.IsCharging);
                }

                _udpClient.BeginReceive(new AsyncCallback(ReceiveCallback), broadcastAddress);
            }
            catch(Exception ex)
            {
                _logger.LogException(Level.Warn, ex, "Не удалось перезапустить получение UDP пакетов");
            }
        }

        private void SettingsServiceSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if(e.NetTypeChanged)
            {
                if (_settingsService.NetType == NetType.Client)
                    StartListening();
                else
                    StopListening();
            }

            if(e.PortChanged && _settingsService.NetType == NetType.Client)
            {
                StopListening();
                StartListening();
            }
        }

        private void _eventSourceTick(object sender, EventArgs e)
        {
            ++_secondsLeft;
            if(_secondsLeft >= TimeOutSec)
            {
                _secondsLeft = 0;
                UpdateBatteryStatus(BatteryStatus.NoSystemBattery, 0, null, false);
            }
        }

        #endregion

        #region Events

        public event EventHandler<BatteryStateEventArgs> BatteryChanged;

        #endregion
    }
}
