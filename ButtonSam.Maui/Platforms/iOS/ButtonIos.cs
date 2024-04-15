using ButtonSam.Maui.Core;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace ButtonSam.Maui.Platforms.iOS;

public class ButtonIos : LayoutView
{
    private ButtonHandler _parent;
    private CAShapeLayer _maskLayer = new();
    private UILongPressGestureRecognizer _gesture;

    private CornerRadius _cornerRadius;
    private double _borderWidth;
    private UIColor? _borderColor;

    public ButtonIos(ButtonHandler parent)
    {
        _parent = parent;
        _gesture = new UILongPressGestureRecognizer(OnTap)
        {
            MinimumPressDuration = 0,
            ShouldRecognizeSimultaneously = ShouldRecognizeSimultaneously
        };
        AddGestureRecognizer(_gesture);
        Layer.Mask = _maskLayer;
        Layer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
        Layer.BackgroundColor = Colors.Transparent.ToCGColor();
        ClipsToBounds = true;
        UserInteractionEnabled = true;
        AccessibilityTraits = UIAccessibilityTrait.Button;
    }

    public CornerRadius CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = value;
            SetNeedsDisplay();
        }
    }

    public double BorderWidth
    {
        get => _borderWidth;
        set
        {
            _borderWidth = value;
            SetNeedsDisplay();
        }
    }

    public UIColor? BorderColor
    {
        get => _borderColor;
        set
        {
            _borderColor = value;
            SetNeedsDisplay();
        }
    }

    public bool IsClickable
    { 
        get => _gesture.Enabled;
        set => _gesture.Enabled = value;
    }

    public void SetupBackground(UIColor backgroundColor)
    {
        BackgroundColor = backgroundColor;
    }

    public override void Draw(CGRect rect)
    {
        base.Draw(rect);

        _maskLayer.Frame = rect;
        if (CornerRadius.TopLeft > 0 || CornerRadius.TopRight > 0 || CornerRadius.BottomRight > 0 || CornerRadius.BottomLeft > 0)
        {
            var path = RoundedRectWithCustomCorners(rect, CornerRadius);
            _maskLayer.Path = path?.CGPath;
        }
        else
        {
            _maskLayer.Path = CGPath.FromRect(rect);
        }

        if (BorderColor != null && BorderWidth > 0)
        {
            var pathBorder = RoundedRectWithCustomCorners(rect, CornerRadius, BorderWidth / 2);
            if (pathBorder != null)
            {
                pathBorder.LineWidth = (nfloat)BorderWidth;
                var strokeColor = BorderColor;
                strokeColor.SetStroke();
                pathBorder.Stroke();
            }

            if (pathBorder?.CGPath != null)
            {
                using var context = UIGraphics.GetCurrentContext();
                if (context == null)
                    return;

                context.AddPath(pathBorder.CGPath);
            }
        }
    }

    public override void LayoutSubviews()
    {
        base.LayoutSubviews();
        SetNeedsDisplay();
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

        if (buttonGesture == _gesture && other is UITapGestureRecognizer)
        {
            bool isChildren = other.View.IsDescendantOfView(this);
            bool isParent = this.IsDescendantOfView(other.View);

            if (isChildren)
                buttonGesture.State = UIGestureRecognizerState.Failed;
            else if (isParent)
                other.State = UIGestureRecognizerState.Failed;

            return true;
        }

        if (other.State == UIGestureRecognizerState.Began)
            buttonGesture.State = UIGestureRecognizerState.Failed;

        return true;
    }

    protected virtual void OnTap(UILongPressGestureRecognizer press)
    {
        var point = press.LocationInView(press.View);
        float x = (float)point.X;
        float y = (float)point.Y;

#if DEBUG
        if (Initializer.UseDebugInfo)
        {
            Console.WriteLine($"[x{x};y{y}] state {press.State}");
        }
#endif

        switch (press.State)
        {
            case UIGestureRecognizerState.Began:
                _parent.Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Pressed,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen
                });
                break;

            case UIGestureRecognizerState.Changed:
                if (this.PointInside(point, null))
                {
                    _parent.Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.Running,
                        InputType = InputTypes.TouchTap,
                        DeviceInputType = DeviceInputTypes.TouchScreen
                    });
                }
                else
                {
#if DEBUG
                    if (Initializer.UseDebugInfo)
                    {
                        Console.WriteLine($"[x{x};y{y}] state Exited");
                    }
#endif
                    _parent.Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.Exited,
                        InputType = InputTypes.TouchTap,
                        DeviceInputType = DeviceInputTypes.TouchScreen
                    });
                }
                break;

            case UIGestureRecognizerState.Ended:
                if (IsClickable)
                {
                    _parent.Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.Release,
                        InputType = InputTypes.TouchTap,
                        DeviceInputType = DeviceInputTypes.TouchScreen
                    });
                }
                else
                {
                    _parent.Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.Canceled,
                        InputType = InputTypes.TouchTap,
                        DeviceInputType = DeviceInputTypes.TouchScreen
                    });
                }
                break;

            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                _parent.Proxy.OnInteractive(new InteractiveEventArgs
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

    protected override void Dispose(bool disposing)
    {
        RemoveGestureRecognizer(_gesture);
        _parent = null!;
        _maskLayer = null!;
        _gesture = null!;

        base.Dispose(disposing);
    }

    public static UIBezierPath? RoundedRectWithCustomCorners(CGRect rect, CornerRadius cornerRadius,
        double insets = 0)
    {
        if (rect.GetMaxX() <= 0 || rect.GetMaxY() <= 0)
            return null;

        if (insets > 0)
            rect = rect.Inset((nfloat)insets, (nfloat)insets);

        var TLRadius = (nfloat)(cornerRadius.TopLeft);
        var TRRadius = (nfloat)(cornerRadius.TopRight);
        var BLRadius = (nfloat)(cornerRadius.BottomLeft);
        var BRRadius = (nfloat)(cornerRadius.BottomRight);
        var path = new UIBezierPath();
        var minX = rect.GetMinX();
        var minY = rect.GetMinY();
        var maxX = rect.GetMaxX();
        var maxY = rect.GetMaxY();

        if (maxX <= 0 || maxY <= 0)
            return null;

        var tl = new CGPoint(minX + TLRadius, minY + TLRadius);
        path.ArcWithCenter(tl, TLRadius, 180, 90, true);
        path.AddLineTo(new CGPoint(maxX - TRRadius, minY));

        var tr = new CGPoint(maxX - TRRadius, minY + TRRadius);
        path.ArcWithCenter(tr, TRRadius, 90, 0, true);
        path.AddLineTo(new CGPoint(maxX, maxY - BRRadius));

        var br = new CGPoint(maxX - BRRadius, maxY - BRRadius);
        path.ArcWithCenter(br, BRRadius, 0, 270, true);
        path.AddLineTo(new CGPoint(minX + BLRadius, maxY));

        var bl = new CGPoint(minX + BLRadius, maxY - BLRadius);
        path.ArcWithCenter(bl, BLRadius, 270, 180, true);

        path.ClosePath();
        return path;
    }
}

public static class DrawExtensions
{
    public static UIBezierPath ArcWithCenter(this UIBezierPath path, CGPoint center, nfloat radius, nfloat startAngleDegrees, nfloat endAngleDegrees, bool clockwise)
    {
        nfloat startAngleRadians = DegreesToRadians(startAngleDegrees);
        nfloat endAngleRadians = DegreesToRadians(endAngleDegrees);

        startAngleRadians = -startAngleRadians;
        endAngleRadians = -endAngleRadians;

        path.AddArc(center, radius, startAngleRadians, endAngleRadians, clockwise);

        return path;
    }

    private static nfloat DegreesToRadians(nfloat degrees)
    {
        return (nfloat)(degrees * Math.PI / 180);
    }
}