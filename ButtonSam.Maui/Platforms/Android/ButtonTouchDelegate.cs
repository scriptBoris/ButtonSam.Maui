using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Android.Views;
using AView = Android.Views.View;
using APaint = Android.Graphics.Paint;
using AColor = Android.Graphics.Color;
using ARectF = Android.Graphics.RectF;
using APath = Android.Graphics.Path;
using ARegion = Android.Graphics.Region;
using ButtonSam.Maui.Core;
using System.Runtime.InteropServices;

namespace ButtonSam.Maui.Platforms.Android;

internal class ButtonTouchDelegate : TouchDelegate
{
    public ButtonTouchDelegate(InteractiveContainer host, Rect? bounds, ButtonDroid delegateView) : base(bounds, delegateView)
    {
        Proxy = host;
    }

    private InteractiveContainer Proxy { get; set; }

    public unsafe override bool OnTouchEvent(MotionEvent e)
    {
        float den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        float x = e.GetX() / den;
        float y = e.GetY() / den;

#if DEBUG
        if (Initializer.UseDebugInfo)
        {
            System.Diagnostics.Debug.WriteLine(
                $"Coordinates: x={x}; y={y}\n" +
                $"Action: {e.Action}\n" +
                $"ActionMasked: {e.ActionMasked}"
            );
        }
#endif

        var pptr = stackalloc InteractiveEventArgsSrc[1];
        nint ptr = (nint)pptr;

        switch (e.ActionMasked)
        {
            case MotionEventActions.Down:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Pressed,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen,
                    SrcPointer = ptr,
                });
                break;

            case MotionEventActions.Move:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Running,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen,
                    SrcPointer = ptr,
                });
                break;

            case MotionEventActions.Up:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Release,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen,
                    SrcPointer = ptr,
                });
                break;

            case MotionEventActions.Cancel:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Canceled,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen,
                    SrcPointer = ptr,
                });
                break;

            default:
                break;
        }

        return true;
    }
}
