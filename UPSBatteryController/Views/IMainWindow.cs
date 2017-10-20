using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace UPSBatteryController.Views
{
    public interface IMainWindow : IView
    {
        void Show();
        void Close();
        void NavigateTo(object page);
        void NavigateBack();
        void NavigateForward();
        bool Activate();

        event EventHandler Closed;
    }
}
