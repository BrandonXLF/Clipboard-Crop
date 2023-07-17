using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ClipboardCrop {
    internal class EqualityConverter : IMultiValueConverter {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            return values.All(val => val.Equals(values[0]));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
