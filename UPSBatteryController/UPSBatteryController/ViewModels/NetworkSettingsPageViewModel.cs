using UPSBatteryController.Providers.Settings;
using UPSBatteryController.Views;
using USBBatteryController;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;
using UPSBatteryController.Services.ExportLifetimeService;
using UPSBatteryController.Providers.Settings.EventArguments;
using UPSBatteryController.Models.Settings;

namespace UPSBatteryController.ViewModels
{
    [Export]
    public class NetworkSettingsPageViewModel : ViewModel<INetworkSettingsPage>, IDisposable
    {
        #region Fields

        private ClientSettingsViewModel _clientSettingsViewModel;
        private ServerSettingsViewModel _serverSettingsViewModel;

        private IExportLifetimeService _exportLifetimeService;
        private ISettingsProvider _settingsService;
        private ViewModel _mode;
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Режим работы
        /// </summary>
        public ViewModel Mode
        {
            get { return _mode; }
            private set { SetProperty(ref _mode, value); }
        }

        #endregion

        #region Commands

        public ICommand SetNetTypeCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public NetworkSettingsPageViewModel(INetworkSettingsPage view,
            ISettingsProvider settingsService,
            IExportLifetimeService exportLifetimeService,
            ExportFactory<ServerSettingsViewModel> serverSettingsViewModelFactory,
            ExportFactory<ClientSettingsViewModel> clientSettingsViewModelFactory) : base(view)
        {
            _settingsService = settingsService;
            _exportLifetimeService = exportLifetimeService;
            _serverSettingsViewModel = _exportLifetimeService.GetValue(serverSettingsViewModelFactory);
            _clientSettingsViewModel = _exportLifetimeService.GetValue(clientSettingsViewModelFactory);

            SetNetTypeCommand = new DelegateCommand((obj) => SetNetType((NetType)obj));
            _settingsService.SettingsChanged += _settingsServiceSettingsChanged;

            LoadSettings();
            
            ViewCore.PageLeft += (sender, args) => LoadSettings();
        }

        #region Functions

        private void SetNetType(NetType type)
        {
            _settingsService.NetType = type;
            _settingsService.Save();
        }
        
        /// <summary>
        /// Загрузка текущих параметров
        /// </summary>
        private void LoadSettings()
        {
            switch(_settingsService.NetType)
            {
                case NetType.None:
                    Mode = null;
                    break;
                case NetType.Server:
                    Mode = _serverSettingsViewModel;
                    break;
                case NetType.Client:
                    Mode = _clientSettingsViewModel;
                    break;
            }
        }
        
        public void Dispose()
        {
            _exportLifetimeService.Dispose(_serverSettingsViewModel);
            _exportLifetimeService.Dispose(_clientSettingsViewModel);
        }

        #endregion

        #region Event handlers

        private void _settingsServiceSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.NetTypeChanged)
                LoadSettings();
        }

        #endregion
    }
}
