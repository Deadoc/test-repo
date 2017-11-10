using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Providers.Actions
{
    public interface IActionsProvider
    {
        /// <summary>
        /// Действия
        /// </summary>
        IEnumerable<Action> Actions { get; }

        /// <summary>
        /// Добавить действие
        /// </summary>
        /// <param name="action"></param>
        bool Add(Action action);

        /// <summary>
        /// Удалить действие
        /// </summary>
        /// <param name="actionId"></param>
        void Remove(Guid actionId);

        /// <summary>
        /// Обновить действие
        /// </summary>
        /// <param name="action"></param>
        bool Update(Action action);

        /// <summary>
        /// Список действий обновлён
        /// </summary>
        event EventHandler ActionsListUpdated;
    }
}
