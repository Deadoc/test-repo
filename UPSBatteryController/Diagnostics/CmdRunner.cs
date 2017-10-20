using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPSBatteryController.Diagnostics
{
    public class CmdRunner : AppRunner
    {
        public CmdRunner()
            : base()
        {
            this.AppPath = "cmd";
        }

        public override CmdResult Run(string cmd = "", string workingDir = null, TimeSpan? timeout = null)
        {
            return base.Run("/C " + cmd, workingDir, timeout);
        }

        public override void RunAsync(string cmd = "", string workingDir = null)
        {
            base.RunAsync("/C " + cmd, workingDir);
        }
    }
}
