using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace HelpOverlay
{
    public class WindowSizeToRectConverter : MarkupExtension, IMultiValueConverter
    {
        public WindowSizeToRectConverter() { }

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double windowActualHeight = Double.Parse(values[0].ToString());
            double windowActualWidth = Double.Parse(values[1].ToString());
            return new Rect(0, 0, windowActualWidth, windowActualHeight);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
