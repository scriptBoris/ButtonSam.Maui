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

namespace ButtonSam.Maui.Platforms.Android;

internal class ButtonTouchDelegate : TouchDelegate
{
    public ButtonTouchDelegate(ButtonBase host, Rect? bounds, ButtonDroid delegateView) : base(bounds, delegateView)
    {
        Button = host;
    }

    private ButtonBase Button { get; set; }

    public override bool OnTouchEvent(MotionEvent e)
    {
        float den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        float x = e.GetX() / den;
        float y = e.GetY() / den;

#if DEBUG
        System.Diagnostics.Debug.WriteLine($"Coordinates: x={x}; y={y}");
        System.Diagnostics.Debug.WriteLine($"Action: {e.Action}");
        System.Diagnostics.Debug.WriteLine($"ActionMasked: {e.ActionMasked}");
#endif

        switch (e.ActionMasked)
        {
            case MotionEventActions.Down:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Pressed,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;

            case MotionEventActions.Move:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Running,
                    InputType = InputTypes.None,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;

            case MotionEventActions.Up:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.ReleaseCompleted,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;

            case MotionEventActions.Cancel:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.ReleaseCanceled,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;

            default:
                break;
        }

        return true;
    }
}
