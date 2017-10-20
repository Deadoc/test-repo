using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Services.ExportLifetimeService
{
    public interface IExportLifetimeService
    {
        /// <summary>
        /// Получить экземпляр
        /// </summary>
        /// <typeparam name="T">Тип экземпляра</typeparam>
        /// <param name="factory">Источник экземпляра</param>
        /// <returns></returns>
        T GetValue<T>(ExportFactory<T> factory);
        /// <summary>
        /// Разрушить экземпляр
        /// </summary>
        /// <typeparam name="T">Тип экземпляра</typeparam>
        /// <param name="instance">Экземпляр</param>
        void Dispose<T>(T instance);
    }
}
