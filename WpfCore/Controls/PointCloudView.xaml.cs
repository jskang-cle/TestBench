using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using HelixToolkit.SharpDX.Core;

using SharpDX;

namespace WpfCore.Controls;
/// <summary>
/// Interaction logic for PointCloudView.xaml
/// </summary>
public partial class PointCloudView : UserControl
{
    public PointCloudView()
    {
        InitializeComponent();
    }

    #region Dependency Properties

    public Vector3D TargetPoint
    {
        get => (Vector3D)GetValue(TargetPointProperty);
        set => SetValue(TargetPointProperty, value);
    }

    public static readonly DependencyProperty TargetPointProperty = DependencyProperty.Register(
        "TargetPoint", typeof(Vector3D), typeof(PointCloudView), new PropertyMetadata(default(Vector3D)));

    public PointNormal[]? PointNormals
    {
        get => (PointNormal[])GetValue(PointNormalsProperty);
        set => SetValue(PointNormalsProperty, value);
    }

    public static readonly DependencyProperty PointNormalsProperty = DependencyProperty.Register(
        "PointNormals", typeof(PointNormal[]), typeof(PointCloudView), new PropertyMetadata(default(PointNormal[]), PointDataChanged));

    public Vector3[]? PointColors
    {
        get => (Vector3[])GetValue(PointColorsProperty);
        set => SetValue(PointColorsProperty, value);
    }

    public static readonly DependencyProperty PointColorsProperty = DependencyProperty.Register(
        "PointColors", typeof(Vector3[]), typeof(PointCloudView), new PropertyMetadata(default(Vector3[]), PointDataChanged));

    public System.Windows.Media.Color? PointColor
    {
        get => (System.Windows.Media.Color?)GetValue(PointColorProperty);
        set => SetValue(PointColorProperty, value);
    }

    public static readonly DependencyProperty PointColorProperty = DependencyProperty.Register(
        "PointColor", typeof(System.Windows.Media.Color?), typeof(PointCloudView), new PropertyMetadata(default(System.Windows.Media.Color?), PointColorChanged));

    public double PointSize
    {
        get => (double)GetValue(PointSizeProperty);
        set => SetValue(PointSizeProperty, value);
    }

    public static readonly DependencyProperty PointSizeProperty = DependencyProperty.Register(
        "PointSize", typeof(double), typeof(PointCloudView), new PropertyMetadata(1.0), v => (double)v > 0);

    #endregion Dependency Properties

    #region Dependency Property Changed Callbacks

    private static void PointDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCloudView pointCloudView)
        {
            return;
        }

        pointCloudView.UpdatePointGeometry();
    }

    private static void PointColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCloudView pointCloudView)
        {
            return;
        }

        pointCloudView.UpdatePointColor();
    }

    #endregion Dependency Property Changed Callbacks

    private void UpdatePointGeometry()
    {
        if (PointNormals is null || PointNormals.Length == 0)
        {
            PointGeometry.ClearAllGeometryData();
            return;
        }

        PointGeometry.Positions = new(PointNormals.AsParallel().Select(pn => new Vector3(pn.x, pn.y, pn.z)));
        UpdatePointColor();
    }

    private void UpdatePointColor()
    {
        if (PointNormals is null || PointNormals.Length == 0)
        {
            return;
        }

        if (PointColors?.Length == PointNormals.Length)
        {
            Color4Collection colors = new(PointColors.AsParallel().Select(c => new Color4(c.X, c.Y, c.Z, 1.0f)));
            PointGeometry.Colors = colors;
            PointGeometryModel.Color = Colors.White;
        }
        else if (PointColor.HasValue)
        {
            PointGeometry.Colors?.Clear();
            PointGeometryModel.Color = PointColor.Value;
        }
        else
        {
            PointGeometry.Colors = null;
            PointGeometryModel.Color = Colors.White;
        }
    }
}
