using System;
using UPSBatteryController.Models.Settings;
using UPSBatteryController.Providers.Settings.EventArguments;

namespace UPSBatteryController.Providers.Settings
{
    public interface ISettingsProvider
    {
        /// <summary>
        /// Показывать ли уведомления
        /// </summary>
        bool ShowNotifications { get; set; }

        /// <summary>
        /// Порт обмена данными
        /// </summary>
        int Port { get; set; }

        /// <summary>
        /// Тип распространения данных
        /// </summary>
        NetType NetType { get; set; }

        /// <summary>
        /// Идентификатор серверного режима
        /// </summary>
        string Identifier { get; set; }

        void Revert();
        void Save();

        event EventHandler<SettingsChangedEventArgs> SettingsChanged;
    }
}