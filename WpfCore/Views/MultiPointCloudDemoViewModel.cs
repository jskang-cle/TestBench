using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using HelixToolkit.SharpDX.Core;

using SharpDX;

using WpfCore.Loader;

namespace WpfCore;

public partial class MultiPointCloudDemoViewModel : ObservableObject
{
    [ObservableProperty]
    private PointGeometry3D? _source;

    [ObservableProperty]
    private PointGeometry3D? _sourceAligned;

    [ObservableProperty]
    private PointGeometry3D? _target1;

    [ObservableProperty]
    private PointGeometry3D? _target2;

    public MultiPointCloudDemoViewModel()
    {
        Source = LoadPlyFile("Assets/source.ply");
        SourceAligned = LoadPlyFile("Assets/sourceAligned.ply");
        Target1 = LoadPlyFile("Assets/target1.ply");
        Target2 = LoadPlyFile("Assets/target2.ply");
    }

    private static PointGeometry3D LoadPlyFile(string path)
    {
        PlyLoader loader = new();
        var (pointNormals, pointColors) = loader.LoadFile(path);

        var positions = new Vector3Collection(pointNormals.Length);
        var colors = pointColors is not null ? new Color4Collection(pointColors.Length) : null;

        foreach (var pn in pointNormals)
        {
            positions.Add(new(pn.x, -pn.y, -pn.z));
        }

        if (colors is not null)
        {
            foreach (var c in pointColors)
            {
                colors.Add(c);
            }
        }

        var result = new PointGeometry3D();
        result.Positions = positions;
        result.Colors = colors;
        return result;
    }
}
