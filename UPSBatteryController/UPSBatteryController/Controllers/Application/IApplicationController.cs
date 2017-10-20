using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Controllers.Application
{
    public interface IApplicationController
    {
        void Run();
        void Shutdown();
        event EventHandler ExitRequested;
    }
}
