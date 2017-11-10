using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Providers.Actions
{
    public class Action
    {
        #region Constants

        public const string ShutdownProgramm = "shutdown.exe";
        public const string ShutdownArguments = "/s /t 0";
        public const string ShutdownWorkingDir = @"C:\Windows\System32";

        #endregion

        #region Properties

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

        /// <summary>
        /// Программа
        /// </summary>
        public string Programm { get; set; }

        /// <summary>
        /// Аргументы
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// Рабочая папка
        /// </summary>
        public string WorkingDir { get; set; }

        #endregion

        #region Functions

        public Action Clone()
        {
            return new Action()
            {
                Id = Id,
                BatteryLevel = BatteryLevel,
                Type = Type,
                Programm = Programm,
                Arguments = Arguments,
                WorkingDir = WorkingDir
            };
        }

        public static Action Create()
        {
            return new Action()
            {
                Id = Guid.NewGuid(),
                BatteryLevel = 25,
                Type = ActionType.Shutdown,
                Programm = ShutdownProgramm,
                Arguments = ShutdownArguments,
                WorkingDir = ShutdownWorkingDir
            };
        }

        #endregion
    }
}
