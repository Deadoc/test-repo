using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Services.NetworkBattery
{
    public interface INetworkBatteryService : IBatteryProvider
    {
        /// <summary>
        /// Отправить состояние батареи
        /// </summary>
        /// <param name="batteryState"></param>
        void SendBatteryState(NetBatteryState batteryState);
    }
}
