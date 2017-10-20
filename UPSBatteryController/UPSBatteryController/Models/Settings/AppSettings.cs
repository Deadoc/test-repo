using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Models.Settings
{
    public class AppSettings
    {
        /// <summary>
        /// Показывать ли уведомления
        /// </summary>
        public bool ShowNotifications { get; set; }

        /// <summary>
        /// Порт отправки и получения данных
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Тип распространения данных
        /// </summary>
        public NetType NetType { get; set; }

        /// <summary>
        /// Идентификатор
        /// </summary>
        public string Identifier { get; set; }
    }
}
