﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace UPSBatteryController.Views
{
    public interface INetworkSettingsPage : IView
    {
        event EventHandler PageLeft;
    }
}
