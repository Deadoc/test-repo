using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Services;

namespace UPSBatteryController.Controllers.Battery
{
    public interface IBatteryController
    {
        /// <summary>
        /// Текущий поставщик информации о состоянии батареи
        /// </summary>
        IBatteryProvider CurrentBatteryProvider { get; }
        
        /// <summary>
        /// Изменился поставщик батареи
        /// </summary>
        event EventHandler BatteryProviderChanged;
    }
}
