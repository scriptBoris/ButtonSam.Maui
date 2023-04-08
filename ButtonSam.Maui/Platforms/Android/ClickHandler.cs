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
            return n;
        }

        private void OnLayoutChanged()
        {
            if (rippleLayout == null)
                return;

            rippleLayout.Bottom = PlatformView.Bottom;
            rippleLayout.Right = PlatformView.Right;
        }

        private void RippleStart(float x, float y)
        {
            if (rippleLayout == null)
            {
                rippleLayout = new VG(Context);
                rippleLayout.Bottom = Super.Bottom;
                rippleLayout.Right = Super.Right;
                rippleLayout.Background = CreateRipple(Button.TapColor);
                Super.AddView(rippleLayout, 0);
            }

            if (rippleLayout.Width > 200 || rippleLayout.Height > 200)
                rippleLayout.Background?.SetHotspot(x,y);

            rippleLayout.Pressed = true;
        }

        private void RippleEnd()
        {
            if (rippleLayout == null) 
                return;

            rippleLayout.Pressed = false;
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

        public void AnimationStart(float x, float y)
        {
            if (IsSdk21 && Button.TryRippleEffect)
                RippleStart(x, y);
            else
                Button.OnTapStart();
        }

        public void AnimationFinish(bool needTrigger)
        {
            if (IsSdk21 && Button.TryRippleEffect)
                RippleEnd();
            else
                Button.OnTapFinish();

            if (needTrigger)
                Button.ThrowTap();
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

        public override bool OnTouchEvent(MotionEvent e)
        {
            float x = e.GetX();
            float y = e.GetY();

            switch (e.ActionMasked)
            {
                case MotionEventActions.Down:
                    isPressedAndIdle = true;
                    startX = x;
                    startY = y;
                    _host.AnimationStart(x, y);
                    break;

                case MotionEventActions.Move:
                    float deltaX = Math.Abs(startX - x);
                    float deltaY = Math.Abs(startY - y);

                    if (deltaX > _touchSlop || deltaY > _touchSlop)
                    {
                        isPressedAndIdle = false;
                        _host.AnimationFinish(false);
                    }
                    break;

                case MotionEventActions.Up:
                    _host.AnimationFinish(isPressedAndIdle);
                    isPressedAndIdle = false;
                    break;

                case MotionEventActions.Cancel:
                    _host.AnimationFinish(false);
                    isPressedAndIdle = false;
                    break;

                default:
                    break;
            }

            return true;
        }
    }
}
