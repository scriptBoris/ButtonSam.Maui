using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public struct InteractiveEventArgs
{
    public required float X { get; set; }
    public required float Y { get; set; }
    public required GestureTypes State { get; set; }
    public required InputTypes InputType { get; set; }
    public required DeviceInputTypes DeviceInputType { get; set; }
}
