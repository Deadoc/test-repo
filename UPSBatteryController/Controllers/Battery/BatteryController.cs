using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Services;
using UPSBatteryController.Services.LocalBattery;
using UPSBatteryController.Services.NetworkBattery;
using UPSBatteryController.Providers.Settings;
using UPSBatteryController.Models.Settings;
using UPSBatteryController.Providers.Settings.EventArguments;

namespace UPSBatteryController.Controllers.Battery
{
    [Export(typeof(IBatteryController))]
    public class BatteryController : IBatteryController
    {
        #region Fields
        
        private INetworkBatteryService _networkService;
        private ISettingsProvider _settingsService;

        private IBatteryProvider _localBatteryProvider;
        private IBatteryProvider _networkBatteryProvider;
        private IBatteryProvider _currentProvider;

        #endregion

        #region Properties

        /// <summary>
        /// Текущий поставщик информации о состоянии батареи
        /// </summary>
        public IBatteryProvider CurrentBatteryProvider
        {
            get { return _currentProvider; }
            private set
            {
                if (_currentProvider != value)
                {
                    _currentProvider = value;
                    BatteryProviderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        [ImportingConstructor]
        public BatteryController(ILocalBatteryService batteryService,
            INetworkBatteryService networkService,
            ISettingsProvider settingsService)
        {
            _localBatteryProvider = batteryService;
            _networkBatteryProvider = networkService;
            
            _networkService = networkService;

            _settingsService = settingsService;
            _settingsService.SettingsChanged += _settingsServiceSettingsChanged;

            ChangeBatteryProvider(_settingsService.NetType);
        }

        #region Functions

        /// <summary>
        /// Сменить поставщика информации о батарее
        /// </summary>
        private void ChangeBatteryProvider(NetType netType)
        {
            if(CurrentBatteryProvider != null)
                CurrentBatteryProvider.BatteryChanged -= SendBatteryInfoOnProviderBatteryChanged;

            if (netType == NetType.Client)
                CurrentBatteryProvider = _networkBatteryProvider;
            else
            {
                CurrentBatteryProvider = _localBatteryProvider;

                if(netType == NetType.Server)
                    CurrentBatteryProvider.BatteryChanged += SendBatteryInfoOnProviderBatteryChanged;
            }
        }
        
        #endregion

        #region Event handlers

        private void _settingsServiceSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.NetTypeChanged)
                ChangeBatteryProvider(_settingsService.NetType);
        }

        private void SendBatteryInfoOnProviderBatteryChanged(object sender, BatteryStateEventArgs e)
        { 
            NetBatteryState state = new NetBatteryState(_settingsService.Identifier, e.Status,
                                                        e.Level, e.IsCharging, e.RemainingTime);
            _networkService.SendBatteryState(state);
        }

        #endregion

        #region Events

        public event EventHandler BatteryProviderChanged;

        #endregion
    }
}
