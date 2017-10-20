using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace UPSBatteryController.Presentation.Behaviors
{
    public class ClosePopupBehavior : Behavior<Popup>
    {
        protected override void OnAttached()
        {
            MessagesHook.MessageHook += OnMessageHook;
        }

        private void OnMessageHook(object sender, MessageHookEventArgs e)
        {
            const int WM_LBUTTONDOWN = 0x0201;
            const int WM_RBUTTONDOWN = 0x0204;
            const int WM_MOUSEWHEEL = 0x020A;
            const int WM_MBUTTONDOWN = 0x0207;
            const int WM_NCLBUTTONDOWN = 0x00A1;
            const int WM_NCMBUTTONDOWN = 0x00A7;
            const int WM_NCRBUTTONDOWN = 0x00A4;
            if (e.Msg == WM_LBUTTONDOWN ||
                e.Msg == WM_RBUTTONDOWN ||
                e.Msg == WM_MBUTTONDOWN ||
                e.Msg == WM_MOUSEWHEEL ||
                e.Msg == WM_NCLBUTTONDOWN ||
                e.Msg == WM_NCMBUTTONDOWN ||
                e.Msg == WM_NCRBUTTONDOWN)
            {
                if (!AssociatedObject.IsMouseDirectlyOver)
                {
                    AssociatedObject.IsOpen = false;
                }
            }
        }
    }
}
