using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WpfCore;

[Serializable]
[DebuggerDisplay("({x}, {y}, {z}) ({nx}, {ny}, {nz})")]
[StructLayout(LayoutKind.Sequential)]
public struct PointNormal
{
    public float x;
    public float y;
    public float z;
    public float nx;
    public float ny;
    public float nz;

    public PointNormal() { }

    public PointNormal(float x, float y, float z, float nx, float ny, float nz)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.nx = nx;
        this.ny = ny;
        this.nz = nz;
    }

    public override readonly string ToString()
    {
        return $"({x}, {y}, {z}) ({nx}, {ny}, {nz})";
    }
}
