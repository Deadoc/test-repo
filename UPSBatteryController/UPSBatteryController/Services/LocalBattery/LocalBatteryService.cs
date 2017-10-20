using GroupAdr.Library.AsyncEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPSBatteryController.Services.NetworkBattery;
using UPSBatteryController.Providers.Settings;

namespace UPSBatteryController.Services.LocalBattery
{
    [Export(typeof(ILocalBatteryService))]
    public class LocalBatteryService : ILocalBatteryService
    {
        #region Fields
        
        private IAsyncEventSource _eventSource;
        private INetworkBatteryService _networkService;

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
        public LocalBatteryService(IAsyncEventSource eventSource,
            INetworkBatteryService networkService)
        {
            _eventSource = eventSource;
            _networkService = networkService;

            _eventSource.Tick += _eventSourceTick;
        }

        #region Functions
        
        /// <summary>
        /// Обновить информацию о состоянии батарейки
        /// </summary>
        private void UpdateBatteryStatus()
        {
            var batteryLevel = Math.Floor(SystemInformation.PowerStatus.BatteryLifePercent * 100);
            TimeSpan? batteryRemainingTime = null;
            var isBatteryCharging = false;
            var batteryStatus = BatteryStatus.NoSystemBattery;

            if (SystemInformation.PowerStatus.PowerLineStatus != PowerLineStatus.Unknown &&
                ((SystemInformation.PowerStatus.BatteryChargeStatus & BatteryChargeStatus.NoSystemBattery) == 0) &&
                batteryLevel > 0)
            {
                if(SystemInformation.PowerStatus.BatteryLifeRemaining > 0)
                    batteryRemainingTime = TimeSpan.FromSeconds(SystemInformation.PowerStatus.BatteryLifeRemaining);

                isBatteryCharging = SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Online;
                
                batteryStatus = batteryLevel > 75 ? BatteryStatus.Full :
                                batteryLevel > 50 ? BatteryStatus.High : 
                                batteryLevel > 25 ? BatteryStatus.Middle : 
                                batteryLevel > 10 ? BatteryStatus.Low :
                                batteryLevel > 0 ? BatteryStatus.Critical : BatteryStatus.NoSystemBattery;
            }

            UpdateBatteryStatus(batteryStatus, batteryLevel, batteryRemainingTime, isBatteryCharging);
        }

        private void UpdateBatteryStatus(BatteryStatus batteryStatus, double batteryLevel, TimeSpan? batteryRemainingTime, bool isBatteryCharging)
        {
            BatteryLevel = batteryLevel;
            BatteryRemainingTime = batteryRemainingTime;
            IsBatteryCharging = isBatteryCharging;
            BatteryStatus = batteryStatus;

            BatteryChanged?.Invoke(this, new BatteryStateEventArgs(batteryStatus, batteryLevel, isBatteryCharging, batteryRemainingTime));
        }

        #endregion

        #region Event handlers
        
        private void _eventSourceTick(object sender, EventArgs e)
        {
            try
            {
                UpdateBatteryStatus();
            }
            catch (Exception ex)
            {
                BatteryStatus = BatteryStatus.NoSystemBattery;
            }
        }

        #endregion

        #region Events

        public event EventHandler<BatteryStateEventArgs> BatteryChanged;

        #endregion
    }
}
