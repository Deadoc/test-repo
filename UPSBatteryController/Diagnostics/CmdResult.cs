using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UPSBatteryController.Diagnostics
{
    public class CmdResult
    {
        public string StandardOutput { get; set; }

        public string StandardError { get; set; }

        public int? ExitCode { get; set; }

        public static CmdResult FromProcess(Process process)
        {
            return new CmdResult
            {
                ExitCode = process.ExitCode,
                StandardError = process.StandardError.ReadToEnd(),
                StandardOutput = process.StandardOutput.ReadToEnd()
            };
        }
    }
}
