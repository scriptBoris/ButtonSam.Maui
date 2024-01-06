using ButtonSam.Maui.Core;
using ButtonSam.Maui.Platforms.iOS;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Microsoft.Maui.Platform;
using UIKit;

namespace ButtonSam.Maui.Platforms.iOS;

public class ButtonHandler : Microsoft.Maui.Handlers.LayoutHandler, IButtonHandler
{
    public ButtonHandler() : base(PropertyMapper)
    {
    }

    public static readonly PropertyMapper<ButtonBase, ButtonHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(ButtonBase.BackgroundColor)] = (h, v) =>
        {
            var color = v.BackgroundColor ?? ButtonBase.DefaultBackgroundColor;
            h.DirectSetBackgroundColor(color);
        },
        [nameof(ButtonBase.BorderWidth)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.BorderWidth = v.BorderWidth;
        },
        [nameof(ButtonBase.BorderColor)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.BorderColor = v.BorderColor?.ToPlatform();
        },
        [nameof(ButtonBase.CornerRadius)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.CornerRadius = v.CornerRadius;
        },
    };

    public ButtonBase Proxy => (ButtonBase)VirtualView;
    public ButtonIos? Native => PlatformView as ButtonIos;
    public bool IsUseBorder => Proxy.BorderColor != null && Proxy.BorderWidth > 0;
    protected UILongPressGestureRecognizer? ButtonGesture { get; private set; }

    protected override LayoutView CreatePlatformView()
    {
        var native = new ButtonIos();
        native.Layer.BackgroundColor = Colors.Transparent.ToCGColor();
        native.ClipsToBounds = true;
        native.UserInteractionEnabled = true;
        native.AccessibilityTraits = UIAccessibilityTrait.Button;

        ButtonGesture = new UILongPressGestureRecognizer(OnTap)
        {
            MinimumPressDuration = 0,
            ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously
        };
        native.AddGestureRecognizer(ButtonGesture);
        return native;
    }

    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);
        var color = Proxy.BackgroundColor ?? ButtonBase.DefaultBackgroundColor;
        DirectSetBackgroundColor(color);
    }

    public virtual void DirectSetBackgroundColor(Color color)
    {
        Native?.SetupBackground(color.ToPlatform());
    }

    protected virtual bool ShouldRecognizeSimultaneously(UIGestureRecognizer buttonGesture, UIGestureRecognizer other)
    {
#if DEBUG
        if (Initializer.UseDebugInfo)
        {
            Console.WriteLine($"a = {buttonGesture.GetType().Name}");
            Console.WriteLine($"b = {other.GetType().Name}");
            Console.WriteLine($"a state = {buttonGesture.State}");
            Console.WriteLine($"b state = {other.State}");
        }
#endif

        if (other.State == UIGestureRecognizerState.Began)
            buttonGesture.State = UIGestureRecognizerState.Failed;

        return true;
    }

    protected virtual void OnTap(UILongPressGestureRecognizer press)
    {
        var point = press.LocationInView(press.View);
        float x = (float)point.X;
        float y = (float)point.Y;

        switch (press.State)
        {
            case UIGestureRecognizerState.Began:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Pressed,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen
                });
                break;

            case UIGestureRecognizerState.Changed:
                var coordinate = press.LocationInView(PlatformView);
                bool isInside = PlatformView.PointInside(coordinate, null);
                if (!isInside && Proxy.IsPressed)
                {
                    Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.Canceled,
                        InputType = InputTypes.TouchTap,
                        DeviceInputType = DeviceInputTypes.TouchScreen
                    });
                }
                break;

            case UIGestureRecognizerState.Ended:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Release,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen
                });
                break;

            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Canceled,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen
                });
                break;
            default:
                break;
        }
    }
}