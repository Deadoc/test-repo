using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Providers.Settings.EventArguments
{
    public class SettingsChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Изменился флаг получения уведомлений
        /// </summary>
        public bool ShowNotificationsChanged { get; private set; }

        /// <summary>
        /// Изменился порт обмена данными
        /// </summary>
        public bool PortChanged { get; private set; }

        /// <summary>
        /// Изменился тип обмена данными
        /// </summary>
        public bool NetTypeChanged { get; private set; }

        /// <summary>
        /// Изменился идентификатор
        /// </summary>
        public bool IdentifierChanged { get; private set; }

        public SettingsChangedEventArgs(bool showNotificationsChanged, bool portChanged, bool netTypeChanged,
            bool identifierChanged)
        {
            ShowNotificationsChanged = showNotificationsChanged;
            PortChanged = portChanged;
            NetTypeChanged = netTypeChanged;
            IdentifierChanged = identifierChanged;
        }
    }
}
