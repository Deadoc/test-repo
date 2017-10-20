using UPSBatteryController.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;
using UPSBatteryController.Services.ExportLifetimeService;
using USBBatteryController;
using System.Reflection;

namespace UPSBatteryController.ViewModels
{
    [Export]
    public class MainWindowViewModel : ViewModel<IMainWindow>, IDisposable
    {
        #region Fields

        /// <summary>
        /// Страница настроек
        /// </summary>
        private NetworkSettingsPageViewModel _networkSettingsPage;

        /// <summary>
        /// Страница действий
        /// </summary>
        private ActionsPageViewModel _actionsPage;

        private IExportLifetimeService _exportLifetimeService;

        #endregion

        #region Properties

        /// <summary>
        /// Версия ПО
        /// </summary>
        public string Version { get { return VersionFormatter.FormatVersion(Assembly.GetEntryAssembly().GetName().Version); } }

        /// <summary>
        /// Страница сетевых настроек
        /// </summary>
        public ViewModel NetworkSettingsPage { get { return _networkSettingsPage; } }

        /// <summary>
        /// Страница действий
        /// </summary>
        public ViewModel ActionsPage { get { return _actionsPage; } }

        #endregion

        #region Commands



        #endregion

        [ImportingConstructor]
        public MainWindowViewModel(IMainWindow view,
            IExportLifetimeService exportLifetimeService,
            ExportFactory<NetworkSettingsPageViewModel> networkSettingsPageFactory,
            ExportFactory<ActionsPageViewModel> actionsPageFactory) : base(view)
        {
            _exportLifetimeService = exportLifetimeService;
            _networkSettingsPage = _exportLifetimeService.GetValue(networkSettingsPageFactory);
            _actionsPage = _exportLifetimeService.GetValue(actionsPageFactory);
        }

        #region Functions
        
        /// <summary>
        /// Перейти на страницу страницу
        /// </summary>
        /// <param name="page">Страница</param>
        public void NavigateTo(ViewModel page)
        {
            ViewCore.NavigateTo(page.View);
        }

        /// <summary>
        /// Вернуться на предыдущую страицу
        /// </summary>
        public void NavigateBack()
        {
            ViewCore.NavigateBack();
        }

        /// <summary>
        /// Перейти на следующую страницу
        /// </summary>
        public void NavigateForward()
        {
            ViewCore.NavigateForward();
        }

        /// <summary>
        /// Закрыть окно
        /// </summary>
        public void Close()
        {
            ViewCore.Close();
        }

        /// <summary>
        /// Отобразить окно
        /// </summary>
        public void Show()
        {
            ViewCore.Show();
        }

        /// <summary>
        /// Сделать окно активным
        /// </summary>
        public void Activate()
        {
            ViewCore.Activate();
        }

        public void Dispose()
        {
            _exportLifetimeService.Dispose(_networkSettingsPage);
            _exportLifetimeService.Dispose(_actionsPage);
        }

        #endregion

        #region Event handlers



        #endregion

        #region Events

        /// <summary>
        /// Окно было закрыто
        /// </summary>
        public event EventHandler Closed {
            add { ViewCore.Closed += value; }
            remove { ViewCore.Closed -= value; }
        }

        #endregion
    }
}
