using UPSBatteryController.Controllers.Tray;
using UPSBatteryController.Services.ExportLifetimeService;
using UPSBatteryController.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Controllers.Battery;
using GroupAdr.Library.AsyncEvents;

namespace UPSBatteryController.Controllers.Application
{
    /// <summary>
    /// Контроллер приложения
    /// </summary>
    [Export(typeof(IApplicationController))]
    public class ApplicationController : IApplicationController
    {
        #region Fields

        private IExportLifetimeService _exportLifetimeService;
        private ExportFactory<MainWindowViewModel> _mainWindowFactory;
        private MainWindowViewModel _mainWindow;
        private ITrayController _trayController;
        private IAsyncEventSource _eventSource;
        private IBatteryController _batteryController;

        #endregion

        [ImportingConstructor]
        public ApplicationController(IExportLifetimeService exportLifetimeService,
            ITrayController trayController,
            IAsyncEventSource eventSource,
            IBatteryController batteryController,
            ExportFactory<MainWindowViewModel> mainWindowFactory)
        {
            _eventSource = eventSource;
            _trayController = trayController;
            _batteryController = batteryController;
            _exportLifetimeService = exportLifetimeService;
            _mainWindowFactory = mainWindowFactory;

            _trayController.OpenNetworkSettingsRequested += OnOpenNetworkSettingsRequested;
            _trayController.OpenActionsRequested += OnOpenActionsRequested;
        }

        #region Functions

        public void Run()
        {
            _eventSource.Start();
            _trayController.Show();
        }

        public void Shutdown()
        {
            _eventSource.Stop();

            if (_mainWindow != null)
                _mainWindow.Close();

            _trayController.Hide();
        }

        /// <summary>
        /// Открыть окно приложения, если оно закрыто
        /// </summary>
        private void ShowWindow()
        {
            if (_mainWindow == null)
            {
                _mainWindow = _exportLifetimeService.GetValue(_mainWindowFactory);
                _mainWindow.Closed += (sender, args) =>
                {
                    _exportLifetimeService.Dispose(_mainWindow);
                    _mainWindow = null;
                };
                _mainWindow.Show();
            }
            else
                _mainWindow.Activate();
        }
        
        #endregion

        #region Events

        /// <summary>
        /// Запрос завершения приложения
        /// </summary>
        public event EventHandler ExitRequested {
            add { _trayController.ExitRequested += value; }
            remove { _trayController.ExitRequested -= value; }
        }

        #endregion

        #region Event handlers

        private void OnOpenNetworkSettingsRequested(object sender, EventArgs e)
        {
            ShowWindow();
            _mainWindow.NavigateTo(_mainWindow.NetworkSettingsPage);
        }

        private void OnOpenActionsRequested(object sender, EventArgs e)
        {
            ShowWindow();
            _mainWindow.NavigateTo(_mainWindow.ActionsPage);
        }

        #endregion
    }
}
