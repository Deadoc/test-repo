using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UPSBatteryController.Models.Settings;
using UPSBatteryController.Providers.Settings;

namespace UPSBatteryController.Providers.Actions
{
    [Export(typeof(IActionsProvider))]
    public class JsonActionsProvider : JsonProviderBase<List<Action>>, IActionsProvider
    {
        #region Fields

        private string _listFilePath = Path.Combine(DefaultSettings.WorkingDirectory, DefaultSettings.ActionsListFileName);
        private ISettingsProvider _settingsService;
        private List<Action> _actions;

        #endregion

        #region Properties

        /// <summary>
        /// Действия
        /// </summary>
        public IEnumerable<Action> Actions { get { return _actions.Select(a => a.Clone()); } }

        #endregion

        [ImportingConstructor]
        public JsonActionsProvider(ISettingsProvider settingsService)
        {
            _settingsService = settingsService;
            
            var listFileDir = Path.GetDirectoryName(_listFilePath);
            Directory.CreateDirectory(listFileDir);

            _actions = Read(_listFilePath);
            if (_actions == null)
                _actions = new List<Action>();
        }

        #region Functions

        /// <summary>
        /// Добавить действие
        /// </summary>
        /// <param name="action"></param>
        public bool Add(Action action)
        {
            bool success = false;

            if (action != null)
            {
                var existingAction = _actions.FirstOrDefault(a => a.Id == action.Id);
                if (existingAction == null)
                {
                    _actions.Add(action.Clone());

                    Write(_listFilePath, _actions);
                    success = true;
                    ActionsListUpdated?.Invoke(this, EventArgs.Empty);
                }
            }

            return success;
        }

        /// <summary>
        /// Удалить действие
        /// </summary>
        /// <param name="action"></param>
        public void Remove(Guid actionId)
        {
            var existingAction = _actions.FirstOrDefault(a => a.Id == actionId);
            if (existingAction != null)
            {
                _actions.Remove(existingAction);

                Write(_listFilePath, _actions);
                ActionsListUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Обновить действие
        /// </summary>
        /// <param name="action"></param>
        public bool Update(Action action)
        {
            bool success = false;

            if (action != null)
            {
                var existingAction = _actions.FirstOrDefault(a => a.Id == action.Id);
                if (existingAction != null)
                {
                    var index = _actions.IndexOf(existingAction);
                    _actions.RemoveAt(index);
                    _actions.Insert(index, action.Clone());

                    Write(_listFilePath, _actions);
                    success = true;
                    ActionsListUpdated?.Invoke(this, EventArgs.Empty);
                }
                    
            }

            return success;
        }

        #endregion

        #region Events

        /// <summary>
        /// Список действий обновлён
        /// </summary>
        public event EventHandler ActionsListUpdated;

        #endregion
    }
}
