using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace HelpOverlay.Converters
{
    public class PathToNameConverter : MarkupExtension, IValueConverter
    {
        public PathToNameConverter() { }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<TypeIndexAssociation> path = (List<TypeIndexAssociation>)value;

            if (path.Count > 0)
            {
                return path[path.Count - 1].ElementType.ToString();
            }

            return "Element";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
