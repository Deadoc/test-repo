using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Services;

namespace UPSBatteryController.Services
{
    public interface IBatteryProvider
    {
        /// <summary>
        /// Уровень батареи
        /// </summary>
        double BatteryLevel { get; }

        /// <summary>
        /// Состояние батарейки
        /// </summary>
        BatteryStatus BatteryStatus { get; }

        /// <summary>
        /// Оставшееся время работы батарейки
        /// </summary>
        TimeSpan? BatteryRemainingTime { get; }

        /// <summary>
        /// Происходит ли зарядка
        /// </summary>
        bool IsBatteryCharging { get; }

        /// <summary>
        /// Уровень заряда изменился
        /// </summary>
        event EventHandler<BatteryStateEventArgs> BatteryChanged;
    }
}
