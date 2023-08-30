using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Platform;

using Android.Graphics.Drawables;
using Android.Content.Res;
using ButtonSam.Maui.Core;
using Android.Content;
using Android.Graphics;
using Android.Util;
using AView = Android.Views.View;
using APaint = Android.Graphics.Paint;
using AColor = Android.Graphics.Color;
using ARectF = Android.Graphics.RectF;
using APath = Android.Graphics.Path;
using ARegion = Android.Graphics.Region;
using AndroidX.Core.Graphics.Drawable;
using Java.Lang.Annotation;

namespace ButtonSam.Maui.Core
{
    public class ButtonDroid : LayoutViewGroup
    {
        private readonly APath _path = new();
        private readonly ButtonHandler _handler;
        private readonly double _den;
        private readonly RippleDrawable _bgRipple;
        private readonly ColorDrawable _bgColorDrawable;
        private AView? _content;

        public ButtonDroid(ButtonHandler handler, Context context) : base(context)
        {
            _handler = handler;
            _den = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
            _bgColorDrawable = new ColorDrawable();
            _bgRipple = CreateRipple(AColor.White);
            Clickable = true;
            LongClickable = true;
            TouchDelegate = new ButtonTouchDelegate(handler.Proxy, null, this);

            Background = new LayerDrawable(new Drawable[]
            {
                _bgColorDrawable,
                _bgRipple,
            });
        }

        public override Drawable? Background
        {
            get => base.Background;
            set
            {
                if (value != null && base.Background == null)
                    base.Background = value;
            }
        }

        public AView? Content
        {
            get => _content;
            set
            {
                if (_content != null)
                    RemoveView(_content);

                _content = value;
                AddView(_content);
            }
        }

        private float CornerRadius { get; set; }

        public void SetupBackgroundColor(AColor color)
        {
            _bgColorDrawable.Color = color;
        }

        public void SetRippleColor(AColor color)
        {
            _bgRipple.SetColor(GetRippleColorSelector(color));
        }

        public void RippleStart(float x, float y)
        {
            _bgRipple.SetHotspot(x, y);
            Pressed = true;
        }

        public void RippleFinish()
        {
            Pressed = false;
        }

        public void UpdateCornerRadius(float radius)
        {
            var metrics = Context.Resources!.DisplayMetrics;
            CornerRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)radius, metrics);
            this.Invalidate();
        }

        private RippleDrawable CreateRipple(AColor tapColor)
        {
            var mask = new ColorDrawable(AColor.White);
            return new RippleDrawable(GetRippleColorSelector(tapColor), null, mask);
        }

        private static ColorStateList GetRippleColorSelector(int pressedColor)
        {
            return new ColorStateList
            (
                new int[][] { new int[] { } },
                new int[] { pressedColor, }
            );
        }

        private int lastPathW;
        private int lastPathH;
        public override void Draw(Canvas? canvas)
        {
            var corner = CornerRadius;
            if (corner > 0)
            {
                if (lastPathW != Width || lastPathH != Height)
                {
                    _path.Reset();
                    var rect = new ARectF(0, 0, Width, Height);
                    _path.AddRoundRect(rect, corner, corner, APath.Direction.Cw!);
                    
                    lastPathW = Width;
                    lastPathH = Height;
                }
                canvas?.ClipPath(_path);
            }

            base.Draw(canvas);

            if (_handler.Proxy.BorderColor != null && _handler.Proxy.BorderWidth > 0)
            {
                var bw = (float)(_handler.Proxy.BorderWidth * _den);
                using var paint = new APaint();
                paint.Color = _handler.Proxy.BorderColor.ToPlatform();
                paint.StrokeWidth = bw;
                paint.SetStyle(APaint.Style.Stroke);
                //paint.SetPathEffect(new CornerPathEffect(corner)); // радиус закругления углов

                //canvas.DrawRect(new ARectF(0, 0, Width, Height), paint);
                var del = bw / 2;
                var x = del;
                var y = del;
                var w = Width - del;
                var h = Height - del;
                canvas?.DrawRoundRect(new ARectF(x, y, w, h), corner, corner, paint);
            }
        }
    }
}
