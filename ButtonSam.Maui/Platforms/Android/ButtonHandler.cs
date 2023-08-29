﻿using Android.Content;
using Android.Graphics;
using Android.Util;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AView = Android.Views.View;
using APaint = Android.Graphics.Paint;
using AColor = Android.Graphics.Color;
using ARectF = Android.Graphics.RectF;
using APath = Android.Graphics.Path;
using ARegion = Android.Graphics.Region;
using Android.Graphics.Drawables;

namespace ButtonSam.Maui.Core
{
    public partial class ButtonHandler : LayoutHandler, IButtonHandler
    {
        public ButtonBase Proxy => (ButtonBase)VirtualView;
        public float CornerRadius { get; private set; }

        public bool OverrideAdd(object? value)
        {
            return false;
        }

        public bool OverrideBackgroundColor(Microsoft.Maui.Graphics.Color color)
        {
            if (PlatformView is ButtonLayout buttonLayout)
                buttonLayout.SetBackgroundColor(color);
            return true;
        }

        public bool OverrideInsert(object? value)
        {
            return false;
        }

        public void UpdateCornerRadius(double radius)
        {
            var metrics = Context.Resources!.DisplayMetrics;
            CornerRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)radius, metrics);
        }

        protected override LayoutViewGroup CreatePlatformView()
        {
            var n = new ButtonLayout(this, Context);
            UpdateCornerRadius(Proxy.CornerRadius);
            return n;
        }
    }

    internal class ButtonLayout : LayoutViewGroup
    {
        //private readonly APaint _paint = new APaint(PaintFlags.AntiAlias);
        private readonly APath _path = new APath();
        //private readonly ARectF _bounds = new ARectF();
        private readonly ButtonHandler _handler;
        private readonly ColorDrawable _backgroundDrawable;
        private double den;

        public ButtonLayout(ButtonHandler handler, Context context) : base(context)
        {
            _handler = handler;
            den = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
            _backgroundDrawable = new ColorDrawable(ButtonBase.DefaultBackgroundColor.ToPlatform());
            Background = _backgroundDrawable;
        }

        public override Drawable? Background 
        {
            get => base.Background;
            set 
            {
                if (value != null)
                    base.Background = value; 
            }
        }

        public void SetBackgroundColor(Microsoft.Maui.Graphics.Color? color)
        {
            if (color != null)
                _backgroundDrawable.Color = color.ToPlatform();
        }

        public override void Draw(Canvas? canvas)
        {
            var corner = _handler.CornerRadius;
            if (corner > 0)
            {
                _path.Reset();
                var rect = new ARectF(0, 0, Width, Height);
                _path.AddRoundRect(rect, corner, corner, APath.Direction.Cw!);
                canvas?.ClipPath(_path);
            }

            base.Draw(canvas);

            if (_handler.Proxy.BorderColor != null && _handler.Proxy.BorderWidth > 0)
            {
                var bw = (float)(_handler.Proxy.BorderWidth * den);
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
                canvas.DrawRoundRect(new ARectF(x, y, w, h), corner, corner, paint);
            }
        }
    }
}
