using Android.Content;
using Android.Views;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AView = Android.Views.View;
using ARect = Android.Graphics.Rect;
using Android.OS;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Widget;
using Microsoft.Maui.Platform;
using ButtonSam.Maui.Core;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, ViewGroup>, IClickHandler
    {
        private AView? rippleLayout;

        public bool IsSdk21 => Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop;
        public Button Button => VirtualView.Button;
        public ViewGroup Super => (VirtualView.Button.Handler?.PlatformView as ViewGroup)!;

        protected override ViewGroup CreatePlatformView()
        {
            var n = new VG(Context, OnLayoutChanged);
            n.TouchDelegate = new GestureT(this, null, n);
            n.Clickable = true;
            n.LongClickable = true;

            if (IsSdk21)
                rippleLayout = CreateRippleLayout();

            return n;
        }

        private void OnLayoutChanged()
        {
            if (rippleLayout == null)
                return;

            rippleLayout.Bottom = PlatformView.Bottom;
            rippleLayout.Right = PlatformView.Right;
        }

        private VG CreateRippleLayout()
        {
            var rippleLayout = new VG(Context);
            rippleLayout.Bottom = Super.Bottom;
            rippleLayout.Right = Super.Right;
            rippleLayout.Background = CreateRipple(Button.TapColor);
            Super.AddView(rippleLayout, 0);
            return rippleLayout;
        }

        public bool TryAnimationRippleStart(float x, float y)
        {
            if (!IsSdk21)
                return false;

            float den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
            x *= den;
            y *= den;

            if (rippleLayout == null)
                rippleLayout = CreateRippleLayout();

            if (rippleLayout.Width > 200 || rippleLayout.Height > 200)
                rippleLayout.Background?.SetHotspot(x,y);

            rippleLayout.Pressed = true;
            return true;
        }

        public bool TryAnimationRippleEnd()
        {
            if (!IsSdk21)
                return false;

            if (rippleLayout != null) 
                rippleLayout.Pressed = false;

            return true;
        }

        private RippleDrawable CreateRipple(Color color)
        {
            var mask = new ColorDrawable(Colors.White.ToPlatform());
            return new RippleDrawable(GetRippleColorSelector(color.ToPlatform()), null, mask);
        }

        private static ColorStateList GetRippleColorSelector(int pressedColor)
        {
            return new ColorStateList
            (
                new int[][] { new int[] { } },
                new int[] { pressedColor, }
            );
        }

        public void UpdateTapColor(Color color)
        {
            if (rippleLayout?.Background is RippleDrawable rd)
                rd.SetColor(GetRippleColorSelector(color.ToPlatform()));
        }
    }

    public class VG : ViewGroup
    {
        private readonly Action? onLayoutChanged;

        public VG(Context? context, Action? onLayoutChanged = null) : base(context)
        {
            this.onLayoutChanged = onLayoutChanged;
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            onLayoutChanged?.Invoke();
        }
    }

    internal class GestureT : TouchDelegate
    {
        private readonly ClickHandler _host;
        private readonly int _touchSlop;
        private float startX;
        private float startY;
        private bool isPressedAndIdle;

        public GestureT(ClickHandler host, ARect? bounds, AView delegateView) : base(bounds, delegateView)
        {
            _host = host;
            _touchSlop = ViewConfiguration.Get(host.Context)?.ScaledTouchSlop ?? 5;
        }

        private Button Button => _host.Button;

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
                    //isPressedAndIdle = true;
                    //startX = x;
                    //startY = y;
                    //_host.AnimationStart(x, y);
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureStatus.Started,
                    });
                    break;

                case MotionEventActions.Move:
                    //float deltaX = Math.Abs(startX - x);
                    //float deltaY = Math.Abs(startY - y);

                    //if (deltaX > _touchSlop || deltaY > _touchSlop)
                    //{
                    //    isPressedAndIdle = false;
                    //    _host.AnimationFinish(false);
                    //}
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureStatus.Running,
                    });
                    break;

                case MotionEventActions.Up:
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureStatus.Completed,
                    });
                    break;

                case MotionEventActions.Cancel:
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = GestureStatus.Canceled,
                    });
                    break;

                default:
                    break;
            }

            return true;
        }
    }
}
