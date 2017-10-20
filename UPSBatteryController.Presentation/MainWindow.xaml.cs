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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UPSBatteryController.Presentation
{
    /// <summary>
    /// Interaction logic for AppsListWindow.xaml
    /// </summary>
    [Export(typeof(IMainWindow)), PartCreationPolicyAttribute(CreationPolicy.NonShared)]
    public partial class MainWindow : IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void NavigateTo(object page)
        {
            mainFrame.Navigate(page);
        }

        public void NavigateBack()
        {
            if (mainFrame.NavigationService.CanGoBack)
                mainFrame.NavigationService.GoBack();
        }

        public void NavigateForward()
        {
            if (mainFrame.NavigationService.CanGoForward)
                mainFrame.NavigationService.GoForward();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            ((HwndSource)PresentationSource.FromVisual(this)).AddHook(MessagesHook.HookThis);
        }
    }
}
