using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public class InteractiveEventArgs
{
    public required float X { get; init; }
    public required float Y { get; init; }
    public required GestureTypes State { get; init; }
    public required InputTypes InputType { get; init; }
    public required DeviceInputTypes DeviceInputType { get; init; }
    public bool IsRealState { get; init; } = true;

    public float? OverrideX { get; set; }
    public float? OverrideY { get; set; }
    public GestureTypes? NextFakeState { get; set; }
}
