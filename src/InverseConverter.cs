using System;
using System.Globalization;
using System.Windows.Data;

namespace ClipboardCrop {
    [ValueConversion(typeof(double), typeof(double))]
    internal class InverseConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return -(double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}