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

        public Action Clone()
        {
            return new Action()
            {
                Id = Id
            };
        }
    }
}
