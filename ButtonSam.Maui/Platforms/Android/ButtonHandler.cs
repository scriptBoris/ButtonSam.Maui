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

    public static readonly PropertyMapper<InteractiveContainer, ButtonHandler> PropertyMapper = new(ViewMapper)
    {
        [nameof(InteractiveContainer.BackgroundColor)] = (h, v) =>
        {
            if (h.Native != null)
            {
                var color = v.BackgroundColor ?? InteractiveContainer.DefaultBackgroundColor;
                h.Native.SetupBackgroundColor(color.ToPlatform());
            }
        },
        [nameof(InteractiveContainer.CornerRadius)] = (h, v) =>
        {
            if (h.Native != null)
                h.Native.CornerRadius = v.CornerRadius;
        },
        [nameof(InteractiveContainer.BorderColor)] = (h, v) =>
        {
            h.Native?.Invalidate();
        },
        [nameof(InteractiveContainer.BorderWidth)] = (h, v) =>
        {
            h.Native?.Invalidate();
        },
        [nameof(InteractiveContainer.TapColor)] = (h, v) =>
        {
            if (h.Native != null && v.TryRippleEffect)
                h.Native.SetRippleColor(v.TapColor.ToPlatform());
        }
    };

    public InteractiveContainer Proxy => (InteractiveContainer)VirtualView;
    public ButtonDroid? Native => PlatformView as ButtonDroid;

    public void DirectSetBackgroundColor(Microsoft.Maui.Graphics.Color color)
    {
        if (Native != null)
            Native.SetupBackgroundColor(color.ToPlatform());
    }

    public void AnimationRippleStart(float x, float y)
    {
        float den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
        x *= den;
        y *= den;

        Native?.RippleStart(x, y);
    }

    public void AnimationRippleEnd()
    {
        Native?.RippleFinish();
    }

    protected override LayoutViewGroup CreatePlatformView()
    {
        var n = new ButtonDroid(this, Context);
        return n;
    }
}
