using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public class CallbackEventArgs
{
    /// <summary>
    /// MAUI coordinates
    /// </summary>
    public required float X { get; init; }

    /// <summary>
    /// MAUI coordinates
    /// </summary>
    public required float Y { get; init; }

    /// <summary>
    /// Method input type
    /// </summary>
    public required InputTypes InputType { get; init; }

    /// <summary>
    /// Device input type
    /// </summary>
    public required DeviceInputTypes DeviceInputType { get; init; }

    /// <summary>
    /// Notifies whether the gesture was reproduced by a human or a software method
    /// </summary>
    public required bool IsRealCallback { get; init; }
}
