using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace UPSBatteryController.Presentation.Converters
{
    public class ExecutableFilePathToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ImageSource result = null;
            Icon icon = SystemIcons.Application;

            string filePath = value as string;
            if (filePath != null && File.Exists(filePath)) {
                icon = Icon.ExtractAssociatedIcon(filePath);
            }

            result = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                                  icon.Handle,
                                  System.Windows.Int32Rect.Empty,
                                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            icon.Dispose();

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
