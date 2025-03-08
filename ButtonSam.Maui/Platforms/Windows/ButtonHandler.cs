using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using ButtonSam.Maui.Core;
using WThickness = Microsoft.UI.Xaml.Thickness;
using WCornerRadius = Microsoft.UI.Xaml.CornerRadius;

namespace ButtonSam.Maui.Platforms.Windows;

public class ButtonHandler : Microsoft.Maui.Handlers.LayoutHandler, IButtonHandler
{
    private InputTypes pressedInputType = InputTypes.None;

    public ButtonHandler() : base(PropertyMapper)
    {
    }

    public static readonly PropertyMapper<InteractiveContainer, ButtonHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(InteractiveContainer.BackgroundColor)] = (h, v) =>
        {
            h.DirectSetBackgroundColor(v.BackgroundColor);
        },
        [nameof(InteractiveContainer.CornerRadius)] = (h, v) =>
        {
            if (h.Wrapper != null)
                h.Wrapper.CornerRadius = new WCornerRadius
                {
                    TopLeft = v.CornerRadius.TopLeft,
                    TopRight = v.CornerRadius.TopRight,
                    BottomRight = v.CornerRadius.BottomRight,
                    BottomLeft = v.CornerRadius.BottomLeft,
                };
        },
        [nameof(InteractiveContainer.BorderWidth)] = (h, v) =>
        {
            if (h.Wrapper == null)
                return;

            if (h.IsUseBorder)
            {
                h.Wrapper.BorderThickness = new WThickness(v.BorderWidth);
            }
            else
            {
                h.Wrapper.BorderBrush = null;
                h.Wrapper.BorderThickness = new WThickness(0);
            }
        },
        [nameof(InteractiveContainer.BorderColor)] = (h, v) =>
        {
            if (h.Wrapper == null)
                return;

            if (h.IsUseBorder)
            {
                h.Wrapper.BorderBrush = v.BorderColor!.ToPlatform();
            }
            else
            {
                h.Wrapper.BorderBrush = null;
                h.Wrapper.BorderThickness = new WThickness(0);
            }
        },
        [nameof(InteractiveContainer.InputTransparent)] = (h, v) =>
        {
            var native = h.PlatformView;
            bool enable = !v.InputTransparent;
            if (native != null)
            {
                native.IsHitTestVisible = enable;
                native.IsTapEnabled = enable;
                if (h.ContainerView != null)
                    h.ContainerView.IsHitTestVisible = enable;
            }
        },
        [nameof(InteractiveContainer.IsClickable)] = (h, v) =>
        {
            var native = h.PlatformView;
            bool clickable = v.IsClickable;
            if (native != null)
            {
                native.IsTapEnabled = clickable;
                native.IsHitTestVisible = clickable;
            }
        },
    };

    public override bool NeedsContainer => true;
    public InteractiveContainer Proxy => (InteractiveContainer)VirtualView;
    public WrapperView? Wrapper => ContainerView as WrapperView;
    public bool IsUseBorder => Proxy.BorderColor != null && Proxy.BorderWidth > 0;

    protected override LayoutPanel CreatePlatformView()
    {
        var n = base.CreatePlatformView();
        n.IsHitTestVisible = true;
        n.IsTapEnabled = true;
        n.PointerPressed += N_PointerPressed;
        n.PointerReleased += N_PointerReleased;
        n.PointerExited += N_PointerExited;
        n.PointerEntered += N_PointerEntered;
        n.PointerMoved += N_PointerMoved;
        n.PointerCanceled += N_PointerCanceled;
        return n;
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        PlatformView.Background = Colors.Transparent.ToPlatform();
    }

    protected override void SetupContainer()
    {
        base.SetupContainer();
        PropertyMapper.UpdateProperties(this, Proxy);
    }

    #region touch handles
    private unsafe void N_PointerCanceled(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

#if DEBUG
        if (Initializer.UseDebugInfo)
        {
            System.Diagnostics.Debug.WriteLine("Pointer is canceled");
        }
#endif

        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Canceled,
            InputType = GetInputType(point),
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }

    private unsafe void N_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Entered,
            InputType = GetInputType(point),
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }

    private unsafe void N_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Exited,
            InputType = GetInputType(point),
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }

    private unsafe void N_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

#if DEBUG
        if (Initializer.UseDebugInfo)
        {
            System.Diagnostics.Debug.WriteLine($"pointer moved: x{x}; y{y}");
        }
#endif

        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Running,
            InputType = pressedInputType,
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }

    private unsafe void N_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

        pressedInputType = GetInputType(point);
        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Pressed,
            InputType = pressedInputType,
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }

    private unsafe void N_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var point = e.GetCurrentPoint((UIElement)sender);
        float x = (float)point.Position.X;
        float y = (float)point.Position.Y;
        var ptr = stackalloc InteractiveEventArgsSrc[1];
        nint src = (nint)ptr;

        Proxy.OnInteractive(new InteractiveEventArgs
        {
            X = x,
            Y = y,
            State = GestureTypes.Release,
            InputType = GetInputType(point),
            DeviceInputType = GetDeviceInputType(point),
            SrcPointer = src,
        });
    }
    #endregion touch handles

    public void DirectSetBackgroundColor(Color color)
    {
        if (Wrapper != null)
            Wrapper.Background = (color ?? InteractiveContainer.DefaultBackgroundColor).ToPlatform();
    }

    private static InputTypes GetInputType(Microsoft.UI.Input.PointerPoint p)
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
                return InputTypes.TouchTap;
        }
        return InputTypes.None;
    }

    private static DeviceInputTypes GetDeviceInputType(Microsoft.UI.Input.PointerPoint p)
    {
        switch (p.PointerDeviceType)
        {
            case Microsoft.UI.Input.PointerDeviceType.Mouse:
            case Microsoft.UI.Input.PointerDeviceType.Touchpad:
                return DeviceInputTypes.Mouse;
            case Microsoft.UI.Input.PointerDeviceType.Touch:
                return DeviceInputTypes.TouchScreen;

            default:
                return DeviceInputTypes.Unknown;
        }
    }
}