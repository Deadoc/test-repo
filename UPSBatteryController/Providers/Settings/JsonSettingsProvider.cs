using UPSBatteryController.Providers.Settings.EventArguments;
using GroupAdr.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Models.Settings;
using UPSBatteryController.Providers;

namespace UPSBatteryController.Providers.Settings
{
    [Export(typeof(ISettingsProvider))]
    public class JsonSettingsProvider : JsonProviderBase<AppSettings>, ISettingsProvider
    {
        #region Constants


        
        #endregion

        #region Fields
        
        private AppSettings _settings;
        private TimeSpan _updateCheckInterval;

        #endregion

        #region Properties

        /// <summary>
        /// Показывать ли уведомления
        /// </summary>
        public bool ShowNotifications { get; set; }

        /// <summary>
        /// Порт обмена данными
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Тип распространения данных
        /// </summary>
        public NetType NetType { get; set; }

        /// <summary>
        /// Идентификатор серверного режима
        /// </summary>
        public string Identifier { get; set; }

        #endregion

        [ImportingConstructor]
        public JsonSettingsProvider()
        {
            _settings = new AppSettings();
            Revert();
        }

        #region Functions

        /// <summary>
        /// Откатить изменения
        /// </summary>
        public void Revert()
        {
            ReadConfig();

            ShowNotifications = _settings.ShowNotifications;
            Port = _settings.Port;
            NetType = _settings.NetType;
            Identifier = _settings.Identifier;
        }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public void Save()
        {
            if (AnyChanges())
            {
                bool showNotificationsChanged = ShowNotifications != _settings.ShowNotifications;
                _settings.ShowNotifications = ShowNotifications;

                bool portChanged = Port != _settings.Port;
                _settings.Port = Port;

                bool netTypeChanged = NetType != _settings.NetType;
                _settings.NetType = NetType;

                bool identifierChanged = Identifier != _settings.Identifier;
                _settings.Identifier = Identifier;

                WriteConfig();
                InvokeSettingsChanged(new SettingsChangedEventArgs(showNotificationsChanged, portChanged, netTypeChanged,
                    identifierChanged));
            }
        }

        /// <summary>
        /// Есть ли какие-нибудь изменения
        /// </summary>
        /// <returns></returns>
        private bool AnyChanges()
        {
            return ShowNotifications != _settings.ShowNotifications ||
                   Port != _settings.Port ||
                   NetType != _settings.NetType ||
                   Identifier != _settings.Identifier;
        }

        /// <summary>
        /// Записать настройки в файл
        /// </summary>
        private void WriteConfig()
        {
            var configFilePath = Path.Combine(DefaultSettings.WorkingDirectory, DefaultSettings.ConfigurationFileName);
            var configFileDir = Path.GetDirectoryName(configFilePath);
            Directory.CreateDirectory(configFileDir);

            if (!Write(configFilePath, _settings))
                _logger.LogEvent(Level.Info, "Настройки не сохранены");
        }

        /// <summary>
        /// Считать настройки с файла
        /// </summary>
        private void ReadConfig()
        {
            var filePath = Path.Combine(DefaultSettings.WorkingDirectory, DefaultSettings.ConfigurationFileName);

            _settings = Read(filePath);
            
            if(_settings == null)
            {
                _settings = new AppSettings()
                {
                    ShowNotifications = DefaultSettings.ShowNotifications,
                    Port = DefaultSettings.Port,
                    NetType = DefaultSettings.NetType,
                    Identifier = DefaultSettings.Identifier
                };
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Настройки изменены
        /// </summary>
        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;
        private void InvokeSettingsChanged(SettingsChangedEventArgs args)
        {
            var handle = SettingsChanged;
            if (handle != null)
                handle.Invoke(this, args);
        }

        #endregion
    }
}
