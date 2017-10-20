using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Services
{
    public enum BatteryStatus
    {
        NoSystemBattery,
        Full,
        High,
        Middle,
        Low,
        Critical
    }
}
