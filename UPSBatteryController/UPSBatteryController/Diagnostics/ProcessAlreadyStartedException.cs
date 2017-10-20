using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPSBatteryController.Diagnostics
{
    public class ProcessAlreadyStartedException : Exception
    {
        public ProcessAlreadyStartedException()
            : base(GenerateMessage())
        { }

        private static string GenerateMessage()
        {
            return "There is a process already started with this BatchRunner";
        }
    }
}
