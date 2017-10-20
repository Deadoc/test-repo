using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Services.NetworkBattery;

namespace UPSBatteryController.Services
{
    public class BatteryStateEventArgs : EventArgs
    {
        /// <summary>
        /// Уровень заряда
        /// </summary>
        public double Level { get; private set; }

        /// <summary>
        /// Оставшееся время работы батареи
        /// </summary>
        public TimeSpan? RemainingTime { get; private set; }

        /// <summary>
        /// Заряжается ли аккумулятор
        /// </summary>
        public bool IsCharging { get; private set; }

        /// <summary>
        /// Состояние батареи
        /// </summary>
        public BatteryStatus Status { get; private set; }

        public BatteryStateEventArgs(BatteryStatus status, double level, bool isCharging, TimeSpan? remainingTime)
        {
            Status = status;
            Level = level;
            IsCharging = isCharging;
            RemainingTime = remainingTime;
        }
    }
}
