using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using HelixToolkit.Wpf.SharpDX;

using SharpDX;

namespace WpfCore;

/// <summary>
/// Interaction logic for MultiPointCloudDemo.xaml
/// </summary>
public partial class MultiPointCloudDemo : Window
{
    public MultiPointCloudDemoViewModel ViewModel { get; }

    public MultiPointCloudDemo()
    {
        DataContext = ViewModel = new();
        InitializeComponent();

        Vector3 position = new(0, 0, 200);
        Vector3 target = ViewModel.SourceAligned!.Bound.Center;
        Vector3 lookDirection = target - position;
        ViewPort.DefaultCamera.Position = new(position.X, position.Y, position.Z);
        ViewPort.DefaultCamera.LookDirection = new(lookDirection.X, lookDirection.Y, lookDirection.Z);
        ViewPort.Reset();
    }
}
