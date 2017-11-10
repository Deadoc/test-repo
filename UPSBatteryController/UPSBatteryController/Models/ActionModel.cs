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
        private bool _haveChanges;
        private Providers.Actions.Action _action;
        private string _programm;
        private string _arguments;
        private string _workingDir;

        #endregion

        #region Properties

        /// <summary>
        /// Программа
        /// </summary>
        public string Programm
        {
            get { return _programm; }
            set
            {
                if (SetProperty(ref _programm, value))
                    HaveChanges = value != _action.Programm;
            }
        }

        /// <summary>
        /// Аргументы
        /// </summary>
        public string Arguments
        {
            get { return _arguments; }
            set
            {
                if (SetProperty(ref _arguments, value))
                    HaveChanges = value != _action.Arguments;
            }
        }

        /// <summary>
        /// Рабочая папка
        /// </summary>
        public string WorkingDir
        {
            get { return _workingDir; }
            set
            {
                if (SetProperty(ref _workingDir, value))
                    HaveChanges = value != _action.WorkingDir;
            }
        }

        /// <summary>
        /// Имеются ли изменения
        /// </summary>
        public bool HaveChanges
        {
            get { return _haveChanges; }
            set { SetProperty(ref _haveChanges, value);}
        }

        /// <summary>
        /// Уровень заряда батареи
        /// </summary>
        public int BatteryLevel
        {
            get { return _batteryLevel; }
            set
            {
                if (SetProperty(ref _batteryLevel, value))
                    HaveChanges = value != _action.BatteryLevel;
            }
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
            set
            {
                if (SetProperty(ref _type, value))
                {
                    HaveChanges = value != _action.Type;
                    ApplyType(value);
                }
            }
        }

        #endregion

        public ActionModel(Providers.Actions.Action action)
        {
            _action = action;
            Id = action.Id;
            _batteryLevel = action.BatteryLevel;
            _type = action.Type;
            _programm = action.Programm;
            _arguments = action.Arguments;
            _workingDir = action.WorkingDir;
        }

        #region Functions

        private void ApplyType(ActionType type)
        {
            switch (type)
            {
                case ActionType.Shutdown:
                    Programm = Providers.Actions.Action.ShutdownProgramm;
                    Arguments = Providers.Actions.Action.ShutdownArguments;
                    WorkingDir = Providers.Actions.Action.ShutdownWorkingDir;
                    break;
                case ActionType.Custom:
                    Programm = string.Empty;
                    Arguments = string.Empty;
                    WorkingDir = string.Empty;
                    break;
            }
        }

        #endregion
    }
}
