using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Services.ExportLifetimeService
{
    [Export(typeof(IExportLifetimeService))]
    public class ExportLifetimeService : IExportLifetimeService
    {
        #region Fields

        /// <summary>
        /// Контексты жизни экспортов
        /// </summary>
        private readonly List<object> _exportLifetimeContexts;

        #endregion

        [ImportingConstructor]
        public ExportLifetimeService()
        {
            _exportLifetimeContexts = new List<object>();
        }

        #region Functions

        /// <summary>
        /// Получить экземпляр
        /// </summary>
        /// <typeparam name="T">Тип экземпляра</typeparam>
        /// <param name="factory">Источник экземпляра</param>
        /// <returns></returns>
        public T GetValue<T>(ExportFactory<T> factory)
        {
            T result = default(T);
            if (factory != null)
            {
                var exportLifetime = factory.CreateExport();
                _exportLifetimeContexts.Add(exportLifetime);
                result = exportLifetime.Value;
            }

            return result;
        }

        /// <summary>
        /// Разрушить экземпляр
        /// </summary>
        /// <typeparam name="T">Тип экземпляра</typeparam>
        /// <param name="instance">Экземпляр</param>
        public void Dispose<T>(T instance)
        {
            if (!EqualityComparer<T>.Default.Equals(instance, default(T)))
            {
                var exportLifetime = _exportLifetimeContexts
                    .OfType<ExportLifetimeContext<T>>()
                    .FirstOrDefault(e => EqualityComparer<T>.Default.Equals(e.Value, instance));
                if (exportLifetime != null)
                {
                    exportLifetime.Dispose();
                    _exportLifetimeContexts.Remove(exportLifetime);
                }
            }
        }

        #endregion
    }
}
