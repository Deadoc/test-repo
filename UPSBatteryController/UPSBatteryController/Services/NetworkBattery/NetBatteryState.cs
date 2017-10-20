using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Services.LocalBattery;

namespace UPSBatteryController.Services.NetworkBattery
{
    public class NetBatteryState
    {
        /// <summary>
        /// Идентификатор сервера
        /// </summary>
        public string Identifier { get; private set; }

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

        public NetBatteryState(string identifier, BatteryStatus status, double level, bool isCharging, TimeSpan? remainingTime)
        {
            Identifier = identifier;
            Status = status;
            Level = level;
            IsCharging = isCharging;
            RemainingTime = remainingTime;
        }
    }
}
