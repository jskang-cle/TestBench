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

using HelixToolkit.SharpDX.Core;
using HelixToolkit.Wpf;
using HelixToolkit.Wpf.SharpDX;

using SharpDX;
using SharpDX.WIC;

namespace WpfCore;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private PointNormal[]? _pointNormals;

    [ObservableProperty]
    private System.Windows.Media.Color _pointColor;

    public MainWindowViewModel()
    {
        // PointGeometry = LoadPlyFile(@"Assets\scene_0000.ply");

        var result = LoadCoPick3DFile("Assets/0012_IMG_Texture_8Bit.png");

        PointNormals = result.Item1;
        PointColor = System.Windows.Media.Colors.Red;
    }

    //private static PointGeometry3D LoadPlyFile(string plyFilePath)
    //{
    //    using Stream f = File.OpenRead(plyFilePath);
    //    using StreamReader sr = new StreamReader(f);

    //    int vertexCount = 0;

    //    string? line;
    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        if (line == "end_header")
    //        {
    //            break;
    //        }

    //        if (line.StartsWith("element vertex"))
    //        {
    //            vertexCount = int.Parse(line[15..]);
    //        }
    //    }

    //    var vertices = new SharpDX.Vector3[vertexCount];
    //    var colors = new SharpDX.Color4[vertexCount];

    //    int vertexIndex = 0;
    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        var parts = line.Split(' ').ToList();

    //        if (parts.Count >= 3)
    //        {
    //            var coords = parts.Take(3).Select(float.Parse).ToList();
    //            vertices[vertexIndex] = new SharpDX.Vector3(coords[0], coords[1], coords[2]);
    //        }

    //        if (parts.Count >= 9)
    //        {
    //            var channels = parts[6..9].Select(int.Parse).ToList();
    //            colors[vertexIndex] = new SharpDX.Color4(channels[0] / 255f, channels[1] / 255f, channels[2] / 255f, 1.0f);
    //        }

    //        vertexIndex++;
    //    }

    //    return new PointGeometry3D()
    //    {
    //        Positions = new Vector3Collection(vertices),
    //        Colors = new Color4Collection(colors),
    //    };
    //}

    private static (PointNormal[], Vector3[]) LoadCoPick3DFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.", filePath);
        }

        // filenames:
        //  - {name}_IMG_Texture_8Bit.png
        //  - {name}_IMG_PointCloud_X.tif
        //  - {name}_IMG_PointCloud_Y.tif
        //  - {name}_IMG_PointCloud_Z.tif

        string directory = Path.GetDirectoryName(filePath) ?? string.Empty;
        string? name = Path.GetFileNameWithoutExtension(filePath).Split("_IMG").FirstOrDefault();

        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException("Invalid input file name.", nameof(filePath));
        }

        string textureFilePath = Path.Combine(directory, $"{name}_IMG_Texture_8Bit.png");
        string xFilePath = Path.Combine(directory, $"{name}_IMG_PointCloud_X.tif");
        string yFilePath = Path.Combine(directory, $"{name}_IMG_PointCloud_Y.tif");
        string zFilePath = Path.Combine(directory, $"{name}_IMG_PointCloud_Z.tif");

        Color4[] colors = ReadTextureData(textureFilePath);

        float[] xData = ReadPointFileData(xFilePath);
        float[] yData = ReadPointFileData(yFilePath);
        float[] zData = ReadPointFileData(zFilePath);

        PointNormal[] pointNormals = new PointNormal[xData.Length];

        for (int i = 0; i < xData.Length; i++)
        {
            pointNormals[i] = new PointNormal(xData[i], yData[i], zData[i], 0, 0, 0);
        }

        return (pointNormals, null);
    }

    private static float[] ReadPointFileData(string filePath)
    {
        using var factory = new ImagingFactory();
        using var stream = new WICStream(factory, filePath, SharpDX.IO.NativeFileAccess.Read);

        string extension = Path.GetExtension(filePath).ToLower();

        using BitmapDecoder decoder = new TiffBitmapDecoder(factory);

        using var decoderUsing = decoder;
        decoder.Initialize(stream, DecodeOptions.CacheOnDemand);

        using var frameDecode = decoder.GetFrame(0);
        Guid pixelFormat = frameDecode.PixelFormat;
        float[] data = new float[frameDecode.Size.Width * frameDecode.Size.Height];

        frameDecode.CopyPixels(data);

        return data;
    }

    private static Color4[] ReadTextureData(string filePath)
    {
        using var factory = new ImagingFactory();
        using var stream = new WICStream(factory, filePath, SharpDX.IO.NativeFileAccess.Read);

        string extension = Path.GetExtension(filePath).ToLower();

        using BitmapDecoder decoder = new PngBitmapDecoder(factory);
        decoder.Initialize(stream, DecodeOptions.CacheOnDemand);

        using var frameDecode = decoder.GetFrame(0);

        using FormatConverter converter = new(factory);
        converter.Initialize(frameDecode, PixelFormat.Format128bppRGBAFloat);

        Color4[] result = new Color4[frameDecode.Size.Width * frameDecode.Size.Height];
        converter.CopyPixels(result);

        return result;
    }
}
