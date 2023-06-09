using ButtonSam.Maui.Core;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSam.Maui;

public class Button : ButtonBase
{
    protected Color? MouseOverColor { get; set; }

    protected bool IsRippleEffectSupport
    {
        get
        {
#if ANDROID
            return Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop && TryRippleEffect;
#else
            return false;
#endif
        }
    }

    public virtual void OnAnimationStart()
    {
        if (!IsEnabled)
            return;

        bool isRipple = TryAnimationRippleStart(StartX, StartY);
        if (!isRipple)
        {
            var from = CurrentBackgroundColor ?? Colors.Transparent;
            this.ColorTo(from, TapColor, c => ChangeBackgroundColor(c), 100);
        }
    }

    public virtual void OnAnimationFinish()
    {
        if (!IsEnabled)
            return;

        bool isRipple = TryAnimationRippleFinish();
        if (!isRipple)
        {
            var from = CurrentBackgroundColor ?? Colors.Transparent;
            var to = MouseOverColor ?? BackgroundColor;
            this.ColorTo(from, to, c => ChangeBackgroundColor(c), 180);
        }
    }

    protected virtual bool TryAnimationRippleStart(float x, float y)
    {
#if ANDROID
        if (TryRippleEffect && ClickableView?.Handler is ClickViewHandler h)
            return h.TryAnimationRippleStart(StartX, StartY);
#endif
        return false;
    }

    protected virtual bool TryAnimationRippleFinish()
    {
#if ANDROID
        if (TryRippleEffect && ClickableView?.Handler is ClickViewHandler h)
            return h.TryAnimationRippleEnd();
#endif
        return false;
    }

    protected override void OnGesturePressed(InteractiveEventArgs args)
    {
        if (args.DeviceInputType == DeviceInputTypes.Mouse && args.InputType != InputTypes.MouseLeftButton)
            return;

        bool oldIsPressed = IsPressed;
        base.OnGesturePressed(args);

        if (IsEnabled && oldIsPressed != IsPressed)
            OnAnimationStart();
    }

    protected override void OnGestureRelease(InteractiveEventArgs args)
    {
        bool oldIsPressed = IsPressed;
        base.OnGestureRelease(args);

        if (oldIsPressed != IsPressed)
            OnAnimationFinish();
    }

    protected override void OnGestureReleaseCanceled(InteractiveEventArgs args)
    {
        bool oldIsPressed = IsPressed;
        base.OnGestureReleaseCanceled(args);

        if (oldIsPressed != IsPressed)
            OnAnimationFinish();
    }

    protected override void OnGestureMove(InteractiveEventArgs args)
    {
        bool oldIsMouseOver = IsMouseOver;
        bool oldIsPressed = IsPressed;
        base.OnGestureMove(args);

        if (oldIsPressed != IsPressed)
        {
            OnAnimationFinish();
            return;
        }

        if (IsPressed || IsRippleEffectSupport)
            return;

        if (oldIsMouseOver != IsMouseOver)
        {
            if (IsMouseOver && IsEnabled)
                AnimationMouseOverStart();
            else if (!IsMouseOver)
                AnimationMouseOverRestore();
        }
    }

    protected override void OnGestureMoveCanceled(InteractiveEventArgs args)
    {
        bool oldIsMouseOver = IsMouseOver;
        base.OnGestureMoveCanceled(args);

        if (oldIsMouseOver != IsMouseOver)
            AnimationMouseOverRestore();
    }

    protected virtual void AnimationMouseOverStart()
    {
        MouseOverColor = TapColor.MultiplyAlpha(0.3f);
        ChangeBackgroundColor(MouseOverColor);
    }

    protected virtual void AnimationMouseOverRestore()
    {
        MouseOverColor = null;
        this.CancelAnimation();
        ChangeBackgroundColor(BackgroundColor);
    }
}
