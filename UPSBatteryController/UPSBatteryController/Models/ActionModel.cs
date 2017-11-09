using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Waf.Foundation;
using UPSBatteryController.Providers.Actions;

namespace UPSBatteryController.Models
{
    public class ActionModel : Model
    {
        #region Fields

        private int _batteryLevel;
        private ActionType _type;

        #endregion

        #region Properties

        /// <summary>
        /// Уровень заряда батареи
        /// </summary>
        public int BatteryLevel
        {
            get { return _batteryLevel; }
            set { SetProperty(ref _batteryLevel, value); }
        }

        /// <summary>
        /// Идентификатор действия
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Тип действия
        /// </summary>
        public ActionType Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        #endregion

        public ActionModel(Providers.Actions.Action action)
        {
            Id = action.Id;
            BatteryLevel = action.BatteryLevel;
            Type = action.Type;
        }
    }
}
