using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;

using SharpDX;
using SharpDX.WIC;

using WpfCore.Loader;

namespace WpfCore;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private PointNormal[]? _pointNormals1;

    [ObservableProperty]
    private Color4[]? _pointColors1;

    public MainWindowViewModel()
    {

    }

    [RelayCommand]
    public void OpenFile()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = ModelLoaderManager.GetFileFilter()
        };

        if (dialog.ShowDialog() == true)
        {
            var (pointNormals, pointColors) = ModelLoaderManager.LoadFile(dialog.FileName);
            PointNormals1 = pointNormals;
            PointColors1 = pointColors;
        }
    }

    [RelayCommand]
    public void Exit()
    {
        App.Current.Shutdown();
    }
}
