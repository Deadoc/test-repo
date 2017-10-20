using UPSBatteryController.Services.ExportLifetimeService;
using UPSBatteryController.Providers.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UPSBatteryController.Services.LocalBattery;
using UPSBatteryController.Services.NetworkBattery;
using UPSBatteryController.Services;
using UPSBatteryController.Controllers.Battery;

namespace UPSBatteryController.Controllers.Tray
{
    [Export(typeof(ITrayController))]
    [Export(typeof(INotificationController))]
    public class TrayController : INotificationController, ITrayController
    {
        #region Fields

        private NotifyIcon _trayIcon;
        private ISettingsProvider _settingsService;
        private IBatteryController _batteryController;
        private IBatteryProvider _currentProvider;

        #endregion

        [ImportingConstructor]
        public TrayController(ISettingsProvider settingsService,
                              IBatteryController batteryController) {
            _settingsService = settingsService;
            _batteryController = batteryController;

            InitializeTrayIcon();

            _batteryController.BatteryProviderChanged += _batteryControllerBatteryProviderChanged;
            ChangeBatteryProvider(_batteryController.CurrentBatteryProvider);
            
        }

        #region Functions

        /// <summary>
        /// Изменить поставщика иныормации о батарее
        /// </summary>
        private void ChangeBatteryProvider(IBatteryProvider batteryProvider)
        {
            if(batteryProvider == null)
                UpdateBatteryState(BatteryStatus.NoSystemBattery, 0, false, null);
            else
            {
                if (_currentProvider != null)
                    _currentProvider.BatteryChanged -= _batteryProviderBatteryChanged;

                batteryProvider.BatteryChanged += _batteryProviderBatteryChanged;
                _currentProvider = batteryProvider;

                UpdateBatteryState(batteryProvider.BatteryStatus, batteryProvider.BatteryLevel,
                        batteryProvider.IsBatteryCharging, batteryProvider.BatteryRemainingTime);
            }
        }

        /// <summary>
        /// Инициализация иконки в tray
        /// </summary>
        private void InitializeTrayIcon()
        {
            _trayIcon = new NotifyIcon();
            _trayIcon.ContextMenu = CreateContextMenu();
            _trayIcon.BalloonTipIcon = ToolTipIcon.Info;
        }

        /// <summary>
        /// Создать контекстное меню
        /// </summary>
        /// <returns></returns>
        private ContextMenu CreateContextMenu()
        {
            var result = new ContextMenu();

            result.MenuItems.Add("Действия", (sender, args) => 
                OpenActionsRequested?.Invoke(this, EventArgs.Empty));

            result.MenuItems.Add("Настройки сети", (sender, args) => 
                OpenNetworkSettingsRequested?.Invoke(this, EventArgs.Empty));

            result.MenuItems.Add("Выход", (sender, args) => 
                ExitRequested?.Invoke(this, EventArgs.Empty));

            return result;                
        }

        /// <summary>
        /// Отобразить
        /// </summary>
        public void Show()
        {
            _trayIcon.Visible = true;
        }

        /// <summary>
        /// Скрыть
        /// </summary>
        public void Hide()
        {
            _trayIcon.Visible = false;
        }

        /// <summary>
        /// Освободить ресурсы класса
        /// </summary>
        public void Dispose()
        {
            _trayIcon.Dispose();
        }

        /// <summary>
        /// Обновить состояние баттареи
        /// </summary>
        private void UpdateBatteryState(BatteryStatus status, double level, bool isCharging, TimeSpan? remainingTime)
        {
            string iconSource = Icons.Battery_Epsent;
            string text = $"Заряд {level}%" + 
                (isCharging ? ", Заряжается" :
                remainingTime.HasValue ? $", Осталось {remainingTime.Value.Hours} ч {remainingTime.Value.Minutes} мин" : "");

            if (isCharging)
                iconSource = Icons.Battery_Charging;
            else switch (status)
            {
                case BatteryStatus.Full:
                        iconSource = Icons.Battery_100;
                        break;
                case BatteryStatus.High:
                        iconSource = Icons.Battery_75;
                        break;
                case BatteryStatus.Middle:
                        iconSource = Icons.Battery_50;
                        break;
                case BatteryStatus.Low:
                        iconSource = Icons.Battery_25;
                        break;
                case BatteryStatus.Critical:
                        iconSource = Icons.Battery_Low;
                        break;
                case BatteryStatus.NoSystemBattery:
                    {
                        iconSource = Icons.Battery_Epsent;
                        text = "Батарея не обнаружена";
                        break;
                    }
            }

            _trayIcon.Icon = new Icon(iconSource);
            _trayIcon.Text = text;
        }

        #region NotificationController

        /// <summary>
        /// Показать уведомление
        /// </summary>
        /// <param name="text"></param>
        public void ShowNotification(String text)
        {
            _trayIcon.BalloonTipText = text;
            _trayIcon.ShowBalloonTip(0);
        }

        /// <summary>
        /// Показать уведомление
        /// </summary>
        public void ShowNotification(String format, params object[] param)
        {
            if (_settingsService.ShowNotifications)
            {
                _trayIcon.BalloonTipText = string.Format(format, param);
                _trayIcon.ShowBalloonTip(0);
            }

        }
        #endregion

        #endregion


        #region Event handlers

        private void _batteryControllerBatteryProviderChanged(object sender, EventArgs e)
        {
            ChangeBatteryProvider(_batteryController.CurrentBatteryProvider);
        }

        private void _batteryProviderBatteryChanged(object sender, BatteryStateEventArgs e)
        {
            UpdateBatteryState(e.Status, e.Level, e.IsCharging, e.RemainingTime);
        }

        #endregion

        #region Events

        /// <summary>
        /// Запрошено отображение действий
        /// </summary>
        public event EventHandler OpenActionsRequested;

        /// <summary>
        /// Запрошено отображение настроек сети
        /// </summary>
        public event EventHandler OpenNetworkSettingsRequested;

        /// <summary>
        /// Запрошено завершение приложения
        /// </summary>
        public event EventHandler ExitRequested;

        #endregion
    }
}
