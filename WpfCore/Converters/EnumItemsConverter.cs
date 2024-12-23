using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfCore.Converters;

[ValueConversion(typeof(object), typeof(List<string>))]
public class EnumItemsConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Type enumType;
        if (value is Type t)
        {
            enumType = t;
        }
        else if (value is Enum e)
        {
            enumType = e.GetType();
        }
        else
        {
            return null;
        }

        var names = Enum.GetValues(enumType);
        return names;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}