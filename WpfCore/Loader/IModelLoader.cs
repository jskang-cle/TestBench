using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HelixToolkit.SharpDX.Core;

using SharpDX;

namespace WpfCore.Loader;

public interface IModelLoader
{
    string Name { get; }
    string SupportedExtension { get; }
    (PointNormal[], Color4[]?) LoadFile(string filePath);
}
