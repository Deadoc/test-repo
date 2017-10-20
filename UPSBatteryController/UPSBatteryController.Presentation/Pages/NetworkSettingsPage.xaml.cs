using UPSBatteryController.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UPSBatteryController.Presentation.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    [Export(typeof(INetworkSettingsPage)), PartCreationPolicyAttribute(CreationPolicy.NonShared)]
    public partial class NetworkSettingsPage : INetworkSettingsPage
    {
        public NetworkSettingsPage()
        {
            InitializeComponent();
            Unloaded += (sender, args) => InvokePageLeft();
        }
        
        public event EventHandler PageLeft;
        private void InvokePageLeft()
        {
            var handler = PageLeft;
            if(handler != null)
                handler.Invoke(this, EventArgs.Empty);
        }
    }
}
