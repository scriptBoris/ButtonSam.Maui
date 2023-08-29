using ButtonSam.Maui.Core;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSam.Maui;

public class Button : ButtonBase
{
    private const string animationName = "SBAnim";
    private double animationProgress;
    private Color? mouseOverColor;
    private Color endAnimationColor = Colors.Transparent;

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
    protected virtual bool IsAnimating => this.AnimationIsRunning(animationName);

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == BackgroundColorProperty.PropertyName)
        {
            UpdateMouseOverColor();
            UpdateEndAnimationColor();
        }
    }

    protected virtual Task<bool> MauiAnimationPressed()
    {
        return this.TransitAnimation(animationName, animationProgress, 1, 180, Easing.Default, (x) =>
        {
            var from = mouseOverColor ?? BackgroundColor;
            var to = endAnimationColor;
            var result = from.ApplyTint(to, x);
            DirectChangeBackgroundColor(result);
            animationProgress = x;
        });
    }

    protected virtual Task<bool> MauiAnimationReleased()
    {
        return this.TransitAnimation(animationName, animationProgress, 0, 180, Easing.Default, (x) =>
        {
            var start = mouseOverColor ?? BackgroundColor;
            var end = endAnimationColor;
            var result = start.ApplyTint(end, x);
            DirectChangeBackgroundColor(result);
            animationProgress = x;
        });
    }

    public virtual async void OnAnimationStart()
    {
        if (!IsEnabled)
            return;

        bool isRipple = TryAnimationRippleStart(StartX, StartY);
        if (!isRipple)
        {
            StopAnim();
            UpdateEndAnimationColor();

            bool complete = await MauiAnimationPressed();
            if (complete && !IsPressed)
                _ = MauiAnimationReleased();
        }
    }

    public virtual void OnAnimationFinish()
    {
        if (!IsEnabled)
            return;

        bool isRipple = TryAnimationRippleFinish();
        if (!isRipple)
        {
            if (IsAnimating)
                return;

            _ = MauiAnimationReleased();
        }
    }

    private void UpdateMouseOverColor()
    {
        if (IsMouseOver)
            mouseOverColor = BackgroundColor.ApplyTint(TapColor, 0.4f);
        else
            mouseOverColor = null;
    }

    private void UpdateEndAnimationColor()
    {
        endAnimationColor = BackgroundColor.ApplyTint(TapColor, 0.7);
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
        UpdateMouseOverColor();
        if (!IsAnimating)
            DirectChangeBackgroundColor(mouseOverColor!);
    }

    protected virtual void AnimationMouseOverRestore()
    {
        UpdateMouseOverColor();
        if (!IsAnimating)
            DirectChangeBackgroundColor(BackgroundColor);
    }

    protected virtual void StopAnim()
    {
        this.AbortAnimation(animationName);
    }
}
