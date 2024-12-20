using System.IO;
using SharpDX.WIC;

using SharpDX;

namespace WpfCore.Loader;

public class CoPick3DLoader : IModelLoader
{
    public string Name => "CoPick3D PNG File";

    public string SupportedExtension => ".png";

    public (PointNormal[], Color4[]?) LoadFile(string filePath)
    {
        return LoadCoPick3DFile(filePath);
    }

    private static (PointNormal[], Color4[]) LoadCoPick3DFile(string filePath)
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

        var pointNormals = Enumerable.Zip(xData, yData, zData).Select(xyz => new PointNormal(xyz.First, -xyz.Second, -xyz.Third, 0, 0, 0)).ToArray();

        //int validPoints = zData.AsParallel().Count(p => p != 0);

        //PointNormal[] pointNormals = new PointNormal[validPoints];
        //Color4[] validColors = new Color4[validPoints];

        //int validIndex = 0;
        //for (int i = 0; i < zData.Length; i++)
        //{
        //    if (zData[i] == 0)
        //    {
        //        continue;
        //    }

        //    pointNormals[validIndex] = new PointNormal(xData[i], -yData[i], -zData[i], 0, 0, 0);
        //    validColors[validIndex] = colors[i];

        //    validIndex++;
        //}

        return (pointNormals, colors);
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

        byte[] data = new byte[frameDecode.Size.Width * frameDecode.Size.Height * 3];
        frameDecode.CopyPixels(data, PixelFormat.GetStride(frameDecode.PixelFormat, frameDecode.Size.Width));

        Color4[] result = new Color4[frameDecode.Size.Width * frameDecode.Size.Height];
        Parallel.For(0, result.Length, i =>
        {
            result[i] = new Color4(data[i * 3 + 2] / 255f, data[i * 3 + 1] / 255f, data[i * 3] / 255f, 1.0f);
        });

        //using FormatConverter converter = new(factory);
        //converter.Initialize(frameDecode, PixelFormat.Format128bppRGBAFloat);

        //Color4[] result = new Color4[frameDecode.Size.Width * frameDecode.Size.Height];
        //converter.CopyPixels(result);

        return result;
    }
}
