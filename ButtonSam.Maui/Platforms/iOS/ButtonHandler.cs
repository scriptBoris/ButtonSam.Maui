using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace ButtonSam.Maui.Core
{
    public partial class ButtonHandler : LayoutHandler, IButtonHandler
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
                var den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
                var layer = h.PlatformView.Layer;
                if (h.IsUseBorder)
                    layer.BorderWidth = (float)v.BorderWidth;
                else
                    layer.BorderWidth = 0;
            },
            [nameof(ButtonBase.BorderColor)] = (h, v) =>
            {
                var layer = h.PlatformView.Layer;

                if (h.IsUseBorder)
                {
                    layer.BorderColor = v.BorderColor!.ToCGColor();
                    layer.BorderWidth = (float)v.BorderWidth;
                }
                else
                {
                    layer.BorderColor = null;
                    layer.BorderWidth = 0;
                }
            },
            [nameof(ButtonBase.CornerRadius)] = (h, v) =>
            {
                var layer = h.PlatformView.Layer;
                layer.CornerRadius = (float)v.CornerRadius;
            },
        };

        public ButtonBase Proxy => (ButtonBase)VirtualView;
        public bool IsUseBorder => Proxy.BorderColor != null && Proxy.BorderWidth > 0;
        protected UILongPressGestureRecognizer? ButtonGesture { get; private set; }

        protected override LayoutView CreatePlatformView()
        {
            var native = base.CreatePlatformView();
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
            PlatformView.BackgroundColor = color.ToPlatform();
        }

        public bool TryAnimationRippleStart(float x, float y)
        {
            return false;
        }

        public bool TryAnimationRippleEnd()
        {
            return false;
        }

        public virtual void DirectSetBackgroundColor(Color color)
        {
            if (PlatformView != null)
            {
                PlatformView.BackgroundColor = color.ToPlatform();
            }
        }

        protected virtual bool ShouldRecognizeSimultaneously(UIGestureRecognizer buttonGesture, UIGestureRecognizer other)
        {
            //Console.WriteLine($"a = {buttonGesture.GetType().Name}");
            //Console.WriteLine($"b = {other.GetType().Name}");
            //Console.WriteLine($"a state = {buttonGesture.State}");
            //Console.WriteLine($"b state = {other.State}");

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
                        InputType = InputTypes.Touch,
                        DeviceInputType = DeviceInputTypes.Touch,
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
                            State = GestureTypes.ReleaseCanceled,
                            InputType = InputTypes.None,
                            DeviceInputType = DeviceInputTypes.Touch,
                        });
                    }
                    break;

                case UIGestureRecognizerState.Ended:
                    Proxy.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureTypes.ReleaseCompleted,
                        InputType = InputTypes.Touch,
                        DeviceInputType = DeviceInputTypes.Touch,
                    });
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    Proxy.OnInteractive(new InteractiveEventArgs
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
        }
    }
}