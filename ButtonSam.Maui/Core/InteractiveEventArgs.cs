using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public class InteractiveEventArgs
{
    internal InteractiveEventArgs()
    {
    }

    /// <summary>
    /// MAUI coordinates
    /// </summary>
    public required float X { get; init; }

    /// <summary>
    /// MAUI coordinates
    /// </summary>
    public required float Y { get; init; }

    /// <summary>
    /// Gesture state
    /// </summary>
    public required GestureTypes State { get; init; }

    /// <summary>
    /// Method input type
    /// </summary>
    public required InputTypes InputType { get; init; }

    /// <summary>
    /// Device input type
    /// </summary>
    public required DeviceInputTypes DeviceInputType { get; init; }

    /// <summary>
    /// Notifies whether the gesture was reproduced by a human or a software method.
    /// </summary>
    public bool IsRealState { get; init; } = true;

    /// <summary>
    /// Specify a value for this property so that the callback methods count
    /// gestures to these coordinates
    /// </summary>
    public float? OverrideX { get; set; }

    /// <summary>
    /// Specify a value for this property so that the callback methods count
    /// gestures to these coordinates
    /// </summary>
    public float? OverrideY { get; set; }

    /// <summary>
    /// Specify a value so that after processing the event, the following event 
    /// with this type of status and the IsRealState property with the value "false"
    /// will be triggered
    /// </summary>
    public GestureTypes? NextFakeState { get; set; }
}
