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
    protected const string animationName = "SBAnim";

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
    protected Color? MouseOverColor { get; private set; }
    protected Color EndAnimationColor { get; private set; } = Colors.Transparent;
    protected double AnimationProgress { get; private set; }

    protected override void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(propertyName);
        switch (propertyName)
        {
            case nameof(BackgroundColor):
            case nameof(TapColor):
                UpdateMouseOverColor();
                UpdateEndAnimationColor();
                if (IsAnimating || IsPressed)
                    AnimationFrame(AnimationProgress);
                break;
            default:
                break;
        }
    }

    protected virtual void AnimationPropertyColor(Color color)
    {
        DirectChangeBackgroundColor(color);
    }

    protected virtual void AnimationFrame(double x)
    {
        var from = MouseOverColor ?? BackgroundColor ?? ButtonBase.DefaultBackgroundColor;
        var to = EndAnimationColor;
        var result = from.ApplyTint(to, x);
        AnimationPropertyColor(result);
        AnimationProgress = x;
    }

    protected virtual Task<bool> MauiAnimationPressed()
    {
        return this.TransitAnimation(animationName, AnimationProgress, 1, 180, Easing.Default, AnimationFrame);
    }

    protected virtual Task<bool> MauiAnimationReleased()
    {
        return this.TransitAnimation(animationName, AnimationProgress, 0, 180, Easing.Default, AnimationFrame);
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
            MouseOverColor = (BackgroundColor ?? ButtonBase.DefaultBackgroundColor).ApplyTint(TapColor, 0.4f);
        else
            MouseOverColor = null;
    }

    private void UpdateEndAnimationColor()
    {
        EndAnimationColor = (BackgroundColor ?? ButtonBase.DefaultBackgroundColor).ApplyTint(TapColor, 0.7);
    }

    protected virtual bool TryAnimationRippleStart(float x, float y)
    {
        if (Handler is IButtonHandler handler)
            return handler.TryAnimationRippleStart(x, y);
        return false;
    }

    protected virtual bool TryAnimationRippleFinish()
    {
        if (Handler is IButtonHandler handler)
            return handler.TryAnimationRippleEnd();
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
            DirectChangeBackgroundColor(MouseOverColor!);
    }

    protected virtual void AnimationMouseOverRestore()
    {
        UpdateMouseOverColor();
        if (!IsAnimating)
            DirectChangeBackgroundColor(BackgroundColor ?? ButtonBase.DefaultBackgroundColor);
    }

    protected virtual void StopAnim()
    {
        this.AbortAnimation(animationName);
    }
}
