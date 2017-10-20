using GroupAdr.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UPSBatteryController.Providers
{
    public abstract class JsonProviderBase<T>
    {
        protected ILogger _logger = LogFactory.GetLogger();

        /// <summary>
        /// Записать JSON в файл
        /// </summary>
        protected bool Write(string filePath, T data)
        {
            bool success = false;
            try
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                
                using (StreamWriter sw = new StreamWriter(filePath))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, data);
                }

                success = true;
            }
            catch (Exception ex)
            {
                _logger.LogException(Level.Warn, ex, "Не удалось записать JSON");
            }

            return success;
        }

        /// <summary>
        /// Считать JSON с файла
        /// </summary>
        protected T Read(string filePath)
        {
            T result = default(T);
            if (File.Exists(filePath))
            {
                try
                {
                    result = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                }
                catch (Exception ex)
                {
                    _logger.LogException(Level.Warn, ex, "Не удалось считать JSON");
                }
            }

            return result;
        }
    }
}
