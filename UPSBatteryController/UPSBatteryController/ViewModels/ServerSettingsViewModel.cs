using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using UPSBatteryController.Providers.Settings.EventArguments;
using UPSBatteryController.Providers.Settings;
using UPSBatteryController.Views;

namespace UPSBatteryController.ViewModels
{
    [Export]
    public class ServerSettingsViewModel : ViewModel<IServerSettingsView>, IDisposable
    {
        #region Fields

        private string _identifier;
        private int _port;
        private ISettingsProvider _settingsService;

        #endregion

        #region Properties

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Identifier
        {
            get { return _identifier; }
            set
            {
                if(SetProperty(ref _identifier, value))
                {
                    _settingsService.Identifier = value;
                    _settingsService.Save();
                }
            }
        }

        /// <summary>
        /// Порт
        /// </summary>
        public int Port
        {
            get { return _port; }
            set
            {
                if (SetProperty(ref _port, value))
                {
                    _settingsService.Port = value;
                    _settingsService.Save();
                }
            }
        }

        #endregion

        [ImportingConstructor]
        public ServerSettingsViewModel(IServerSettingsView view,
            ISettingsProvider settingsService) : base(view)
        {
            _settingsService = settingsService;
            _settingsService.SettingsChanged += _settingsServiceSettingsChanged;

            LoadSettings();
        }

        #region Functions

        /// <summary>
        /// Загрузка текущих параметров
        /// </summary>
        private void LoadSettings()
        {
            Identifier = _settingsService.Identifier;
            Port = _settingsService.Port;
        }

        public void Dispose()
        {
            _settingsService.SettingsChanged -= _settingsServiceSettingsChanged;
        }

        #endregion

        #region Event handlers

        private void _settingsServiceSettingsChanged(object sender, SettingsChangedEventArgs e)
        {
            if (e.PortChanged)
                Port = _settingsService.Port;

            if (e.IdentifierChanged)
                Identifier = _settingsService.Identifier;
        }

        #endregion
    }
}
