using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Presentation
{
    public class MessageHookEventArgs : EventArgs
    {
        public IntPtr Hwnd { get; private set; }
        public int Msg { get; private set; }
        public IntPtr WParam { get; private set; }
        public IntPtr LParam { get; private set; }
        public bool Handled { get; set; }

        public MessageHookEventArgs(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            Hwnd = hwnd;
            Msg = msg;
            WParam = wParam;
            LParam = lParam;
        }
    }
}
