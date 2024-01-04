using Android.Content;
using Android.Graphics;
using Android.Util;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;
using ButtonSam.Maui.Core;
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
using Android.Content.Res;
using static Android.Provider.MediaStore;

namespace ButtonSam.Maui.Platforms.Android;

public class ButtonHandler : Microsoft.Maui.Handlers.LayoutHandler, IButtonHandler
{
    public ButtonHandler() : base(PropertyMapper)
    {
    }

    public static readonly PropertyMapper<ButtonBase, ButtonHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(ButtonBase.BackgroundColor)] = (h, v) =>
        {
            if (h.Native != null)
            {
                var color = v.BackgroundColor ?? ButtonBase.DefaultBackgroundColor;
                h.Native.SetupBackgroundColor(color.ToPlatform());
            }
        },
        [nameof(ButtonBase.CornerRadius)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.CornerRadius = v.CornerRadius;
        },
        [nameof(ButtonBase.BorderColor)] = (h, v) =>
        {
            h.Native?.Invalidate();
        },
        [nameof(ButtonBase.BorderWidth)] = (h, v) =>
        {
            h.Native?.Invalidate();
        },
        [nameof(ButtonBase.TapColor)] = (h, v) =>
        {
            if (h.Native != null && v.TryRippleEffect)
                h.Native.SetRippleColor(v.TapColor.ToPlatform());
        }
    };

    public ButtonBase Proxy => (ButtonBase)VirtualView;
    public ButtonDroid? Native => PlatformView as ButtonDroid;

    public void DirectSetBackgroundColor(Microsoft.Maui.Graphics.Color color)
    {
        if (Native != null)
            Native.SetupBackgroundColor(color.ToPlatform());
    }

    public bool TryAnimationRippleStart(float x, float y)
    {
        if (!Proxy.TryRippleEffect)
            return false;

        float den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        x *= den;
        y *= den;

        Native?.RippleStart(x, y);
        return true;
    }

    public bool TryAnimationRippleEnd()
    {
        Native?.RippleFinish();
        return Proxy.TryRippleEffect;
    }

    protected override LayoutViewGroup CreatePlatformView()
    {
        var n = new ButtonDroid(this, Context);
        return n;
    }
}
