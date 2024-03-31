using ButtonSam.Maui.Core;
using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace ButtonSam.Maui.Platforms.iOS
{
    public class ButtonIos : LayoutView
    {
        private readonly CAShapeLayer _maskLayer = new();
        private readonly ButtonHandler _parent;
        private CornerRadius _cornerRadius;
        private double _borderWidth;
        private UIColor? _borderColor;

        public ButtonIos(ButtonHandler parent)
        {
            _parent = parent;
            Layer.Mask = _maskLayer;
            Layer.EdgeAntialiasingMask = CAEdgeAntialiasingMask.All;
            Layer.BackgroundColor = Colors.Transparent.ToCGColor();
            ClipsToBounds = true;
            IsClickable = true;
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

        public bool IsClickable { get; set; }

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

        public override void TouchesBegan(NSSet touches, UIEvent? evt)
        {
            if (!IsClickable)
                return;

            var press = touches.Cast<UITouch>().First();
            var point = press.LocationInView(this);
            float x = (float)point.X;
            float y = (float)point.Y;

#if DEBUG
            if (Initializer.UseDebugInfo)
            {
                Console.WriteLine($"[x{x};y{y}] state Began");
            }
#endif
            _parent.Proxy.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = GestureTypes.Pressed,
                InputType = InputTypes.TouchTap,
                DeviceInputType = DeviceInputTypes.TouchScreen
            });
        }

        public override void TouchesMoved(NSSet touches, UIEvent? evt)
        {
            if (!IsClickable)
                return;

            var press = touches.Cast<UITouch>().First();
            var point = press.LocationInView(this);
            float x = (float)point.X;
            float y = (float)point.Y;

            if (this.PointInside(point, null))
            {
#if DEBUG
                if (Initializer.UseDebugInfo)
                {
                    Console.WriteLine($"[x{x};y{y}] state Moved");
                }
#endif
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
        }

        public override void TouchesEnded(NSSet touches, UIEvent? evt)
        {
            var press = touches.Cast<UITouch>().First();
            var point = press.LocationInView(this);
            float x = (float)point.X;
            float y = (float)point.Y;

#if DEBUG
            if (Initializer.UseDebugInfo)
            {
                Console.WriteLine($"[x{x};y{y}] state Ended");
            }
#endif
            if (!IsClickable)
            {
                _parent.Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.Canceled,
                    InputType = InputTypes.TouchTap,
                    DeviceInputType = DeviceInputTypes.TouchScreen
                });
                return;
            }

            _parent.Proxy.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = GestureTypes.Release,
                InputType = InputTypes.TouchTap,
                DeviceInputType = DeviceInputTypes.TouchScreen
            });
        }

        public override void TouchesCancelled(NSSet touches, UIEvent? evt)
        {
            var press = touches.Cast<UITouch>().First();
            var point = press.LocationInView(this);
            float x = (float)point.X;
            float y = (float)point.Y;

#if DEBUG
            if (Initializer.UseDebugInfo)
            {
                Console.WriteLine($"[x{x};y{y}] state Cancelled");
            }
#endif
            _parent.Proxy.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = GestureTypes.Canceled,
                InputType = InputTypes.TouchTap,
                DeviceInputType = DeviceInputTypes.TouchScreen
            });
        }

        public override void TouchesEstimatedPropertiesUpdated(NSSet touches)
        {
            base.TouchesEstimatedPropertiesUpdated(touches);
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
}
