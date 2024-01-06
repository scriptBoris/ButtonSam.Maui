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
using Android.OS;

namespace ButtonSam.Maui.Platforms.Android;

public class ButtonDroid : LayoutViewGroup
{
    private readonly ButtonHandler _handler;
    private readonly RippleDrawable _bgRipple;
    private readonly ColorDrawable _bgColorDrawable;
    private readonly APath _pathCorners = new();
    private readonly APath _pathBorders = new();
    private readonly double _density;
    private CornerRadius _cornerRadius;
    private float[] corners = Array.Empty<float>();

    public ButtonDroid(ButtonHandler handler, Context context) : base(context)
    {
        _handler = handler;
        _density = Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
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

    public CornerRadius CornerRadius
    {
        get => _cornerRadius;
        set
        {
            _cornerRadius = value;
            var metrics = Context!.Resources!.DisplayMetrics;
            var cornerTL = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)value.TopLeft, metrics);
            var cornerTR = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)value.TopRight, metrics);
            var cornerBR = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)value.BottomRight, metrics);
            var cornerBL = TypedValue.ApplyDimension(ComplexUnitType.Dip, (float)value.BottomLeft, metrics);

            corners = new float[]
            {
                cornerTL, cornerTL,
                cornerTR, cornerTR,
                cornerBR, cornerBR,
                cornerBL, cornerBL,
            };
            this.Invalidate();
        }
    }

    public bool HasCornerRadius => _cornerRadius.TopLeft > 0 || _cornerRadius.TopRight > 0 || _cornerRadius.BottomRight > 0 || _cornerRadius.BottomLeft > 0;
    public bool HasBorders => _handler.Proxy.BorderColor != null && _handler.Proxy.BorderWidth > 0;

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

    private static RippleDrawable CreateRipple(AColor tapColor)
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

    public override void Draw(Canvas? canvas)
    {
        if (canvas == null)
            return;

        var rect = (HasCornerRadius || HasBorders) ? new ARectF(0, 0, Width, Height) : null;

        if (HasCornerRadius)
        {
            _pathCorners.Reset();
            _pathCorners.AddRoundRect(rect!, corners, APath.Direction.Cw!);
            canvas.ClipPath(_pathCorners);
        }

        base.Draw(canvas);

        if (HasBorders)
        {
            float bw = (float)(_handler.Proxy.BorderWidth * _density);
            using var paint = new APaint();
            paint.Color = _handler.Proxy.BorderColor!.ToPlatform();
            paint.StrokeWidth = bw * 2;
            paint.SetStyle(APaint.Style.Stroke);

            _pathBorders.Reset();
            _pathBorders.AddRoundRect(rect!, corners, APath.Direction.Cw!);
            canvas.DrawPath(_pathBorders, paint);
        }
    }
}