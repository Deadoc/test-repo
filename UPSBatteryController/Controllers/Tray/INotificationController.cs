using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Controllers.Tray
{
    public interface INotificationController
    {
        /// <summary>
        /// Показать уведомление
        /// </summary>
        void ShowNotification(String text);

        /// <summary>
        /// Показать уведомление
        /// </summary>
        void ShowNotification(String format, params object[] param);
    }
}
