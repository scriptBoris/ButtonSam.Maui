using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using WBorder = Microsoft.UI.Xaml.Controls.Border;
using WColor = Microsoft.UI.Colors;
using WRect = Windows.Foundation.Rect;
using WSize = Windows.Foundation.Size;
using WThickness = Microsoft.UI.Xaml.Thickness;
using WCornerRadius = Microsoft.UI.Xaml.CornerRadius;
using WShapes = Microsoft.UI.Xaml.Shapes;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using Microsoft.UI.Xaml.Hosting;
using WControls = Microsoft.UI.Xaml.Controls;
using WView = Microsoft.UI.Xaml.FrameworkElement;
using Microsoft.UI.Xaml;
using Windows.Devices.Radios;

namespace ButtonSam.Maui.Core
{
    public partial class ButtonHandler : LayoutHandler, IButtonHandler
    {
        private InputTypes pressedInputType = InputTypes.None;
        private bool isPressed;

        public ButtonHandler() : base(PropertyMapper)
        {
        }

        public static readonly PropertyMapper<ButtonBase, ButtonHandler> PropertyMapper = new(ViewMapper)
        {
            [nameof(ButtonBase.BackgroundColor)] = (h, v) =>
            {
                h.DirectSetBackgroundColor(v.BackgroundColor);
            },
            [nameof(ButtonBase.CornerRadius)] = (h, v) =>
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
            [nameof(ButtonBase.BorderWidth)] = (h, v) =>
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
            [nameof(ButtonBase.BorderColor)] = (h, v) =>
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
        };

        public override bool NeedsContainer => true;
        public ButtonBase Proxy => (ButtonBase)VirtualView;
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
            n.PointerMoved += N_PointerMoved;
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
        private void N_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;
            Proxy.OnInteractive(new InteractiveEventArgs
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
                Proxy.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.ReleaseCanceled,
                    InputType = pressedInputType,
                    DeviceInputType = GetDeviceInputType(point),
                });
            }

            Proxy.OnInteractive(new InteractiveEventArgs
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
            Proxy.OnInteractive(new InteractiveEventArgs
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
            Proxy.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = GestureTypes.ReleaseCompleted,
                InputType = pressedInputType,
                DeviceInputType = GetDeviceInputType(point),
            });
        }
        #endregion touch handles

        public void DirectSetBackgroundColor(Color color)
        {
            if (Wrapper != null)
                Wrapper.Background = (color ?? ButtonBase.DefaultBackgroundColor).ToPlatform();
        }

        public bool TryAnimationRippleStart(float x, float y)
        {
            return false;
        }

        public bool TryAnimationRippleEnd()
        {
            return false;
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
                    return InputTypes.Touch;
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
                    return DeviceInputTypes.Touch;

                default:
                    return DeviceInputTypes.None;
            }
        }
    }
}