using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Models.Settings
{
    internal static class DefaultSettings
    {
        public const string ConfigurationFileName = "config.json";

        public static readonly string WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UPSBatteryController");

        public static bool ShowNotifications { get { return true; } }

        public static int Port { get { return 1990; } }
        
        public static NetType NetType { get { return NetType.None; } }

        public static string Identifier { get { return "BatteryID"; } }
    }
}
