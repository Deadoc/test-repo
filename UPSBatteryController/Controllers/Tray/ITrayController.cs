using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Controllers.Tray
{
    public interface ITrayController : IDisposable
    {
        /// <summary>
        /// Отобразить
        /// </summary>
        void Show();

        /// <summary>
        /// Скрыть
        /// </summary>
        void Hide();

        /// <summary>
        /// Запрошено отображение действий
        /// </summary>
        event EventHandler OpenActionsRequested;

        /// <summary>
        /// Запрошено отображение окна приложения
        /// </summary>
        event EventHandler OpenNetworkSettingsRequested;

        /// <summary>
        /// Запрошено завершение приложения
        /// </summary>
        event EventHandler ExitRequested;
    }
}
