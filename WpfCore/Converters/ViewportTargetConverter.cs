using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Media3D;

namespace WpfCore.Converters;
internal class ViewportTargetConverter : MarkupExtension, IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Vector3D target = new();

        foreach (var value in values)
        {
            if (value is Point3D p3d)
            {
                target += new Vector3D(p3d.X, p3d.Y, p3d.Z);
            }

            if (value is Vector3D vec)
            {
                target += vec;
            }
        }

        return $"[{target.X:0.000},{target.Y:0.000},{target.Z:0.000}]";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}
