using ButtonSam.Maui.Core;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

public partial class ClickViewHandler : ViewHandler<ClickView, Microsoft.UI.Xaml.Controls.Panel>
{
    private InputTypes pressedInputType = InputTypes.None;
    private bool isPressed;
    public ButtonBase Button => VirtualView.Button;

    protected override Panel CreatePlatformView()
    {
        var n = new ClickPanel();
        n.Background = Colors.Transparent.ToPlatform();
        n.IsHitTestVisible = true;
        n.IsTapEnabled = true;
        n.PointerPressed += N_PointerPressed;
        n.PointerReleased += N_PointerReleased;
        n.PointerExited += N_PointerExited;
        n.PointerMoved += N_PointerMoved;
        return n;
    }

    private void N_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        Button.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Running,
            InputType = InputTypes.None,
            DeviceInputType = GetDeviceInputType(point),
        });
    }

    private void N_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;

        if (isPressed)
        {
            isPressed = false;
            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = GestureTypes.ReleaseCanceled,
                InputType = pressedInputType,
                DeviceInputType = GetDeviceInputType(point),
            });
        }

        Button.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.RunningCanceled,
            InputType = InputTypes.None,
            DeviceInputType = GetDeviceInputType(point),
        });
    }

    private void N_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;

        pressedInputType = GetInputType(point);
        isPressed = true;
        Button.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Pressed,
            InputType = pressedInputType,
            DeviceInputType = GetDeviceInputType(point),
        });
    }

    private void N_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;

        isPressed = false;
        Button.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.ReleaseCompleted,
            InputType = pressedInputType,
            DeviceInputType = GetDeviceInputType(point),
        });
    }

    private InputTypes GetInputType(Microsoft.UI.Input.PointerPoint p)
    {
        switch (p.PointerDeviceType)
        {
            case Microsoft.UI.Input.PointerDeviceType.Mouse:
            case Microsoft.UI.Input.PointerDeviceType.Touchpad:

                if (p.Properties.IsLeftButtonPressed)
                    return InputTypes.MouseLeftButton;
                else if (p.Properties.IsRightButtonPressed)
                    return InputTypes.MouseRightButton;
                else if (p.Properties.IsMiddleButtonPressed)
                    return InputTypes.MouseMiddleButton;

                break;
            default:
                return InputTypes.Touch;
        }
        return InputTypes.None;
    }

    private DeviceInputTypes GetDeviceInputType(Microsoft.UI.Input.PointerPoint p)
    {
        switch (p.PointerDeviceType)
        {
            case Microsoft.UI.Input.PointerDeviceType.Mouse:
            case Microsoft.UI.Input.PointerDeviceType.Touchpad:
                return DeviceInputTypes.Mouse;

            default:
                return DeviceInputTypes.Touch;
        }
    }
}

public class ClickPanel : Microsoft.UI.Xaml.Controls.Panel
{
}
