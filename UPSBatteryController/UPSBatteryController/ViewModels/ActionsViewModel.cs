using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Forms;
using System.Windows.Input;
using UPSBatteryController.Models;
using UPSBatteryController.Providers.Actions;
using UPSBatteryController.Views;

namespace UPSBatteryController.ViewModels
{
    [Export]
    public class ActionsPageViewModel : ViewModel<IActionsPage>
    {
        #region Fields

        private IActionsProvider _actionsProvider;
        private ObservableCollection<ActionModel> _actions;

        #endregion

        #region Properties

        public ReadOnlyObservableCollection<ActionModel> Actions { get; private set; }

        #endregion

        #region Commands

        /// <summary>
        /// Добавить действие
        /// </summary>
        public ICommand AddActionCommand { get; private set; }

        /// <summary>
        /// Удалить действие
        /// </summary>
        public ICommand RemoveActionCommand { get; private set; }

        /// <summary>
        /// Сохранить изменения
        /// </summary>
        public ICommand SaveActionCommand { get; private set; }

        /// <summary>
        /// Выбрать программу
        /// </summary>
        public ICommand SelectProgrammCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public ActionsPageViewModel(IActionsPage view,
            IActionsProvider actionsProvider) : base(view)
        {
            _actionsProvider = actionsProvider;

            _actions = new ObservableCollection<ActionModel>(_actionsProvider.Actions.Select(a => new ActionModel(a)));
            Actions = new ReadOnlyObservableCollection<ActionModel>(_actions);

            AddActionCommand = new DelegateCommand(AddAction);
            RemoveActionCommand = new DelegateCommand((obj) => RemoveAction(obj as ActionModel));
            SaveActionCommand = new DelegateCommand((obj) => SaveAction(obj as ActionModel));
            SelectProgrammCommand = new DelegateCommand((obj) => SelectProgramm(obj as ActionModel));
        }

        #region Functions

        private void AddAction()
        {
            var action = Providers.Actions.Action.Create();
            _actions.Add(new ActionModel(action));
            _actionsProvider.Add(action);
        }

        private void RemoveAction(ActionModel action)
        {
            _actionsProvider.Remove(action.Id);
            _actions.Remove(action);
        }

        private void SaveAction(ActionModel action)
        {

        }

        private void SelectProgramm(ActionModel action)
        {
            OpenFileDialog ofDialog = new OpenFileDialog();

            if(ofDialog.ShowDialog() == DialogResult.OK)
                action.Programm = ofDialog.FileName;
        }

        #endregion

        #region Event handlers

        #endregion
    }
}
