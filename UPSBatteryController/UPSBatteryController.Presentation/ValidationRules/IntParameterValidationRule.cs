using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace UPSBatteryController.Presentation.ValidationRules
{
    /// <summary>
    /// Правило для проверки целочисленных параметров параметров
    /// </summary>
    public class IntParameterValidationRule : ValidationRule
    {
        /// <summary>
        /// Минимум
        /// </summary>
        public Int32 Min { get; set; }

        /// <summary>
        /// Максимум
        /// </summary>
        public Int32 Max { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var result = new ValidationResult(false, "Пожалуйста, заполните поле");

            if (value != null)
            {
                String stringValue = value.ToString();

                if (!String.IsNullOrEmpty(stringValue))
                {
                    Int32 intValue;

                    if (Int32.TryParse(stringValue, out intValue))
                    {
                        if ((intValue < Min) || (intValue > Max))
                        {
                            result = new ValidationResult(false, string.Format("Введите число в интервале: {0} - {1} ", Min, Max));
                        }
                        else
                        {
                            result = new ValidationResult(true, String.Empty);
                        }
                    }
                    else
                        result = new ValidationResult(false, "Введён недопустиный символ");
                }
            }

            return result;
        }
    }
}
