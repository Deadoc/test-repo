using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Providers.Actions
{
    public class Action
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Уровень заряда батареи
        /// </summary>
        public int BatteryLevel { get; set; }
        
        /// <summary>
        /// Тип действия
        /// </summary>
        public ActionType Type { get; set; }

        public Action Clone()
        {
            return new Action()
            {
                Id = Id,
                BatteryLevel = BatteryLevel,
                Type = Type
            };
        }

        public static Action Create()
        {
            return new Action()
            {
                Id = Guid.NewGuid(),
                BatteryLevel = 25,
                Type = ActionType.Shutdown
            };
        }
    }
}
