using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Applications;
using UPSBatteryController.Providers.Actions;
using UPSBatteryController.Views;

namespace UPSBatteryController.ViewModels
{
    [Export]
    public class ActionsPageViewModel : ViewModel<IActionsPage>
    {
        #region Fields

        private IActionsProvider _actionsProvider;

        #endregion

        [ImportingConstructor]
        public ActionsPageViewModel(IActionsPage view,
            IActionsProvider actionsProvider) : base(view)
        {
        }
    }
}
