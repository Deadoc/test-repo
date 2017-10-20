using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPSBatteryController.Diagnostics
{
    public class AppNotFoundException : Exception
    {
        public AppNotFoundException(IEnumerable<string> appPossibleLocations)
            : base(GenerateMessage(appPossibleLocations))
        { }

        public AppNotFoundException()
            : base(GenerateMessage())
        { }

        private static string GenerateMessage(IEnumerable<string> appPossibleLocations = null)
        {
            var res = new StringBuilder("Application not found on the computer. Please verify it's install location.");

            if (appPossibleLocations != null)
            {
                res.AppendLine(" Searched locations: ");
                foreach (var l in appPossibleLocations)
                {
                    res.AppendLine(l);
                }
            }
            return res.ToString();
        }
    }
}
