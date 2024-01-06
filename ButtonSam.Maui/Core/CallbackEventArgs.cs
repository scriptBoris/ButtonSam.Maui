using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public class CallbackEventArgs
{
    public required float X { get; init; }
    public required float Y { get; init; }
    public required bool IsRealCallback { get; init; }
}
