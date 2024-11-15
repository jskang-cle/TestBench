using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

using SharpDX;

namespace WpfCore.Controls;

public partial class PointCloudView
{

    #region Dependency Properties

    public bool AutoCenterTargetPoint
    {
        get => (bool)GetValue(AutoCenterTargetPointProperty);
        set => SetValue(AutoCenterTargetPointProperty, value);
    }

    public static readonly DependencyProperty AutoCenterTargetPointProperty = DependencyProperty.Register(
        "AutoCenterTargetPoint", typeof(bool), typeof(PointCloudView), 
        new PropertyMetadata(true, CameraParamChanged));

    public Vector3D TargetPoint
    {
        get => (Vector3D)GetValue(TargetPointProperty);
        set => SetValue(TargetPointProperty, value);
    }

    public static readonly DependencyProperty TargetPointProperty = DependencyProperty.Register(
        "TargetPoint", typeof(Vector3D), typeof(PointCloudView), 
        new PropertyMetadata(default(Vector3D), CameraParamChanged));

    public double CameraDistance
    {
        get => (double)GetValue(CameraDistanceProperty);
        set => SetValue(CameraDistanceProperty, value);
    }

    public static readonly DependencyProperty CameraDistanceProperty = DependencyProperty.Register(
        "CameraDistance", typeof(double), typeof(PointCloudView), 
        new PropertyMetadata(1000d, CameraParamChanged));

    public PointNormal[]? PointNormals
    {
        get => (PointNormal[])GetValue(PointNormalsProperty);
        set => SetValue(PointNormalsProperty, value);
    }

    public static readonly DependencyProperty PointNormalsProperty = DependencyProperty.Register(
        "PointNormals", typeof(PointNormal[]), typeof(PointCloudView), 
        new PropertyMetadata(default(PointNormal[]), PointDataChanged));

    public Color4[]? PointColors
    {
        get => (Color4[])GetValue(PointColorsProperty);
        set => SetValue(PointColorsProperty, value);
    }

    public static readonly DependencyProperty PointColorsProperty = DependencyProperty.Register(
        "PointColors", typeof(Color4[]), typeof(PointCloudView), 
        new PropertyMetadata(default(Color4[]),PointDataChanged));

    public System.Windows.Media.Color? PointColor
    {
        get => (System.Windows.Media.Color?)GetValue(PointColorProperty);
        set => SetValue(PointColorProperty, value);
    }

    public static readonly DependencyProperty PointColorProperty = DependencyProperty.Register(
        "PointColor", typeof(System.Windows.Media.Color?), typeof(PointCloudView), 
        new PropertyMetadata(System.Windows.Media.Colors.White, PointColorChanged));

    public double PointSize
    {
        get => (double)GetValue(PointSizeProperty);
        set => SetValue(PointSizeProperty, value);
    }

    public static readonly DependencyProperty PointSizeProperty = DependencyProperty.Register(
        "PointSize", typeof(double), typeof(PointCloudView),
        new PropertyMetadata(0.5), v => (double)v > 0);

    #endregion Dependency Properties

    #region Dependency Property Changed Callbacks

    private static void CameraParamChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not PointCloudView pointCloudView)
        {
            return;
        }

        pointCloudView.UpdateCameraTarget();
    }

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

}
