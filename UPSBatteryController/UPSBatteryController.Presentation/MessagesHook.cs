using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Presentation
{
    public class MessagesHook
    {
        public static IntPtr HookThis(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var handler = MessageHook;
            if (handler != null)
            {
                var args = new MessageHookEventArgs(hwnd, msg, wParam, lParam);
                handler.Invoke(hwnd, args);
                handled = args.Handled;
            }
            return IntPtr.Zero;
        }

        public static event EventHandler<MessageHookEventArgs> MessageHook;
    }
}
