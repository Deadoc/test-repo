using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
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

        #endregion

        [ImportingConstructor]
        public ActionsPageViewModel(IActionsPage view,
            IActionsProvider actionsProvider) : base(view)
        {
            _actionsProvider = actionsProvider;

            _actions = new ObservableCollection<ActionModel>(_actionsProvider.Actions.Select(a => new ActionModel(a)));
            Actions = new ReadOnlyObservableCollection<ActionModel>(_actions);

            AddActionCommand = new DelegateCommand(AddAction);
            RemoveActionCommand = new DelegateCommand((obj) => RemoveAction(obj as Providers.Actions.Action));

            _actions.Add(new ActionModel(Providers.Actions.Action.Create()));
        }

        #region Functions

        private void AddAction()
        {

        }

        private void RemoveAction(Providers.Actions.Action action)
        {

        }

        #endregion

        #region Event handlers

        #endregion
    }
}
