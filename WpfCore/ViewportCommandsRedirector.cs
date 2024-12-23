using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using HelixToolkit.Wpf.SharpDX;

namespace WpfCore;

public static class ViewportCommandsRedirector
{
    public static RoutedCommand Reset => ViewportCommands.Reset;
}
