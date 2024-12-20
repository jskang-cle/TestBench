using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HelixToolkit.SharpDX.Core;

using SharpDX;

namespace WpfCore.Loader;

public class PlyLoader : IModelLoader
{

    public string Name { get; } = "PLY File";

    public string SupportedExtension { get; } = ".ply";

    public (PointNormal[], Color4[]?) LoadFile(string filePath)
    {
        using Stream f = File.OpenRead(filePath);
        using StreamReader sr = new StreamReader(f);

        var header = ParsePlyHeader(sr);

        var vertices = new PointNormal[header.VertexCount];

        int xIndex = header.GetPropertyIndex("x");
        int yIndex = header.GetPropertyIndex("y");
        int zIndex = header.GetPropertyIndex("z");

        int nxIndex = header.GetPropertyIndex("nx");
        int nyIndex = header.GetPropertyIndex("ny");
        int nzIndex = header.GetPropertyIndex("nz");

        bool hasNormals = nxIndex != -1 && nyIndex != -1 && nzIndex != -1;

        int rIndex = header.GetPropertyIndex("red");
        int gIndex = header.GetPropertyIndex("green");
        int bIndex = header.GetPropertyIndex("blue");

        bool hasColors = rIndex != -1 && gIndex != -1 && bIndex != -1;

        Color4[]? colors = null;
        if (hasColors)
        {
            colors = new Color4[header.VertexCount];
        }

        string? line;
        int vertexIndex = 0;
        while ((line = sr.ReadLine()) != null)
        {
            var parts = line.Split(' ').ToList();

            var vert = new PointNormal();
            vert.x = float.Parse(parts[xIndex]);
            vert.y = float.Parse(parts[yIndex]);
            vert.z = -float.Parse(parts[zIndex]);

            if (hasNormals)
            {
                vert.nx = float.Parse(parts[nxIndex]);
                vert.ny = float.Parse(parts[nyIndex]);
                vert.nz = float.Parse(parts[nzIndex]);
            }

            vertices[vertexIndex] = vert;

            if (hasColors)
            {
                var r = int.Parse(parts[rIndex]);
                var g = int.Parse(parts[gIndex]);
                var b = int.Parse(parts[bIndex]);
                colors![vertexIndex] = new Color4(r / 255f, g / 255f, b / 255f, 1.0f);
            }

            vertexIndex++;
        }

        return (vertices, colors);
    }

    private PlyHeader ParsePlyHeader(TextReader tr)
    {
        var header = new PlyHeader();
        string? line;
        while ((line = tr.ReadLine()) != null)
        {
            if (line == "end_header")
            {
                break;
            }
            if (line.StartsWith("element vertex"))
            {
                header.VertexCount = int.Parse(line[15..]);
            }
            else if (line.StartsWith("element face"))
            {
                header.FaceCount = int.Parse(line[13..]);
            }
            else if (line.StartsWith("format"))
            {
                header.Format = line[7..] switch
                {
                    "ascii 1.0" => PlyFormat.Ascii,
                    "binary_little_endian 1.0" => PlyFormat.BinaryLittleEndian,
                    "binary_big_endian 1.0" => PlyFormat.BinaryBigEndian,
                    _ => throw new NotSupportedException("Unsupported PLY format."),
                };
            }
            else if (line.StartsWith("property"))
            {
                var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
                if (parts.Count < 3)
                {
                    throw new FormatException("Invalid property definition.");
                }

                var type = parts[1] switch
                {
                    "char" => PlyPropertyType.Char,
                    "uchar" => PlyPropertyType.Uchar,
                    "short" => PlyPropertyType.Short,
                    "ushort" => PlyPropertyType.Ushort,
                    "int" => PlyPropertyType.Int,
                    "uint" => PlyPropertyType.Uint,
                    "float" => PlyPropertyType.Float,
                    "double" => PlyPropertyType.Double,
                    _ => throw new NotSupportedException("Unsupported property type."),
                };

                header.Properties.Add(new PlyProperty(parts[2], type));
            }
        }
        return header;
    }

    public sealed record class PlyProperty(string Name, PlyPropertyType Type);

    public sealed record class PlyHeader
    {
        public PlyFormat Format { get; set; }
        public int VertexCount { get; set; }
        public int FaceCount { get; set; }

        public List<PlyProperty> Properties { get; init; } = new();

        public int GetPropertyIndex(string name) => Properties.FindIndex(p => p.Name == name);
    }

    public enum PlyFormat
    {
        Ascii,
        BinaryLittleEndian,
        BinaryBigEndian,
    }

    public enum PlyPropertyType
    {
        Char,
        Uchar,
        Short,
        Ushort,
        Int,
        Uint,
        Float,
        Double,
    }
}
