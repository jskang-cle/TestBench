using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
using HelixToolkit.SharpDX.Core.Cameras;
using HelixToolkit.Wpf.SharpDX;

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

        HelixViewPort.InputBindings.Add(new KeyBinding() { Command = ViewportCommands.Reset, Key = Key.R });
    }

    private void UpdatePointGeometry()
    {
        if (PointNormals is null || PointNormals.Length == 0)
        {
            PointGeometry.ClearAllGeometryData();
            return;
        }

        PointGeometry.Positions = new(PointNormals.Select(pn => new Vector3(pn.x, pn.y, pn.z)));

        UpdatePointColor();
        UpdateCameraTarget();
    }

    private void UpdatePointColor()
    {
        if (PointNormals is null || PointNormals.Length == 0)
        {
            return;
        }

        if (PointColors?.Length == PointNormals.Length)
        {
            PointGeometry.Colors = new(PointColors);
            PointGeometryModel.Color = Colors.White;
        }
        else if (PointColor.HasValue)
        {
            PointGeometry.Colors = null;
            PointGeometryModel.Color = PointColor.Value;
        }
        else
        {
            PointGeometry.Colors = null;
            PointGeometryModel.Color = Colors.White;
        }
    }

    private void UpdateCameraTarget()
    {
        Vector3D newTargetPoint;

        if (!AutoCenterTargetPoint)
        {
            newTargetPoint = this.TargetPoint;
        }
        else if (this.PointNormals != null)
        {
            double zAvg = PointNormals.Where(pn => pn.z != 0).Average(pn => pn.z);
            newTargetPoint = new Vector3D(0, 0, zAvg);
        }
        else
        {
            return;
        }

        Debug.WriteLine(newTargetPoint);

        Vector3D lookDirection = new(0, 0, -CameraDistance);
        Point3D position = (Point3D)(newTargetPoint - lookDirection);

        DefaultCamera.LookDirection = lookDirection;
        DefaultCamera.Position = position;

        HelixViewPort.Reset();
    }
}
