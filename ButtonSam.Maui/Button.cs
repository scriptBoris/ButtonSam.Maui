using ButtonSam.Maui.Core;
using ButtonSam.Maui.Internal;
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

#if __IOS__
    protected const float _touchSlop = 1000;
#elif __ANDROID__
    protected const float _touchSlop = 10;
#else
    protected const float _touchSlop = 10000;
#endif

    private InternalTimer? _longTapTimer;

    public event EventHandler<CallbackEventArgs>? OnTapped;
    public event EventHandler<CallbackEventArgs>? OnLongTapped;

    #region bindable props
    // tap command
    public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
        nameof(TapCommand),
        typeof(ICommand),
        typeof(Button),
        null
    );
    public ICommand? TapCommand
    {
        get => GetValue(TapCommandProperty) as ICommand;
        set => SetValue(TapCommandProperty, value);
    }

    // tap command parameter
    public static readonly BindableProperty TapCommandParameterProperty = BindableProperty.Create(
        nameof(TapCommandParameter),
        typeof(object),
        typeof(Button),
        null
    );
    public object? TapCommandParameter
    {
        get => GetValue(TapCommandParameterProperty);
        set => SetValue(TapCommandParameterProperty, value);
    }

    // long tap command
    public static readonly BindableProperty LongTapCommandProperty = BindableProperty.Create(
        nameof(LongTapCommand),
        typeof(ICommand),
        typeof(Button),
        null
    );
    public ICommand? LongTapCommand
    {
        get => GetValue(LongTapCommandProperty) as ICommand;
        set => SetValue(LongTapCommandProperty, value);
    }

    // long tap command parameter
    public static readonly BindableProperty LongTapCommandParameterProperty = BindableProperty.Create(
        nameof(LongTapCommandParameter),
        typeof(object),
        typeof(Button),
        null
    );
    public object? LongTapCommandParameter
    {
        get => GetValue(LongTapCommandParameterProperty);
        set => SetValue(LongTapCommandParameterProperty, value);
    }

    // is auto circle
    public static readonly BindableProperty IsAutoCircleProperty = BindableProperty.Create(
        nameof(IsAutoCircle),
        typeof(bool),
        typeof(Button),
        false,
        propertyChanged: (b, o, n) =>
        {
            if (b is Button self && (bool)n)
            {
                self.UpdateAutoCircle(self.Width, self.Height);
            }
        }
    );
    public bool IsAutoCircle
    {
        get => (bool)GetValue(IsAutoCircleProperty);
        set => SetValue(IsAutoCircleProperty, value);
    }
    #endregion bindable props

    protected virtual bool IsAnimating => this.AnimationIsRunning(animationName);
    protected Color? MouseOverColor { get; private set; }
    protected Color EndAnimationColor { get; private set; } = Colors.Transparent;
    protected double AnimationProgress { get; set; }
    protected float StartX { get; private set; }
    protected float StartY { get; private set; }

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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (IsAutoCircle)
        {
            double min = Math.Min(width, height);
            UpdateAutoCircle(min, min);
        }
    }

    private void UpdateAutoCircle(double w, double h)
    {
        double max = Math.Max(w, h);
        if (max > 0)
        {
            CornerRadius = max / 2;
        }
    }

    #region maui animations
    protected virtual Task<bool> MauiAnimationPressed()
    {
        return this.TransitAnimation(animationName, AnimationProgress, 1, 180, Easing.SinIn,
            (x) =>
            {
                AnimationFrame(x);
                AnimationProgress = x;
            });
    }

    protected virtual Task<bool> MauiAnimationReleased()
    {
        return this.TransitAnimation(animationName, AnimationProgress, 0, 180, Easing.Default,
            (x) =>
            {
                AnimationFrame(x);
                AnimationProgress = x;
            });
    }

    protected virtual void AnimationFrame(double x)
    {
        var from = MouseOverColor ?? BackgroundColor ?? ButtonBase.DefaultBackgroundColor;
        var to = EndAnimationColor;
        var result = from.ApplyTint(to, x);
        AnimationPropertyColor(result);
    }

    protected virtual void AnimationPropertyColor(Color color)
    {
        DirectChangeBackgroundColor(color);
    }
    #endregion maui animations

    #region ripple animations
    protected virtual bool TryAnimationRippleStart(float x, float y)
    {
#if ANDROID
        if (TryRippleEffect && Handler is Platforms.Android.ButtonHandler handler)
        {
            handler.AnimationRippleStart(x, y);
            return true;
        }
#endif
        return false;
    }

    protected virtual bool TryAnimationRippleFinish()
    {
#if ANDROID
        if (TryRippleEffect && Handler is Platforms.Android.ButtonHandler handler)
        {
            handler.AnimationRippleEnd();
            return true;
        }
#endif
        return false;
    }
    #endregion ripple animations

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

    /// <summary>
    /// Запускает анимацию нажатия кнопки
    /// </summary>
    /// <param name="x">MAUI coordinates</param>
    /// <param name="y">MAUI coordinates</param>
    protected virtual async void AnimationPressedStart(float x, float y)
    {
        if (!IsEnabled)
            return;

        bool isRipple = TryAnimationRippleStart(x, y);
        if (!isRipple)
        {
            AnimationPressedStop();
            UpdateEndAnimationColor();

            bool complete = await MauiAnimationPressed();
            if (complete && !IsPressed)
                _ = MauiAnimationReleased();
        }
    }

    /// <summary>
    /// Запускает анимацию отжатия кнопки
    /// </summary>
    /// <param name="x">MAUI coordinates</param>
    /// <param name="y">MAUI coordinates</param>
    protected virtual void AnimationPressedRestore(float x, float y)
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

    /// <summary>
    /// Останавливает анимацию нажатия и отжатия
    /// </summary>
    protected virtual void AnimationPressedStop()
    {
        this.AbortAnimation(animationName);
        AnimationProgress = 0;
        TryAnimationRippleFinish();
    }

    /// <summary>
    /// Метод запуска анимации (или визуального состояния), когда мышь 
    /// находится над кнопкой (IsMouseOver == true)
    /// </summary>
    protected virtual void AnimationMouseOverStart()
    {
        UpdateMouseOverColor();
        if (!IsAnimating)
            DirectChangeBackgroundColor(MouseOverColor!);
    }

    /// <summary>
    /// Метод запуска анимации (или визуального состояния), когда мышь
    /// уже не находится над кнопкой (IsMouseOver == false)
    /// </summary>
    protected virtual void AnimationMouseOverRestore()
    {
        MouseOverColor = null;
        if (!IsAnimating)
            DirectChangeBackgroundColor(BackgroundColor ?? ButtonBase.DefaultBackgroundColor);
    }

    /// <summary>
    /// Останавливает анимации IsMouseOver
    /// </summary>
    protected virtual void AnimationMouseOverStop()
    {
        MouseOverColor = null;
        if (!IsAnimating)
            DirectChangeBackgroundColor(BackgroundColor ?? ButtonBase.DefaultBackgroundColor);
    }

    /// <summary>
    /// Останавливает ВСЕ анимации и возвращает состояние кнопки первозданный вид
    /// </summary>
    protected virtual void RestoreButton()
    {
        AnimationPressedStop();
        AnimationMouseOverStop();
        DirectChangeBackgroundColor(BackgroundColor ?? ButtonBase.DefaultBackgroundColor);
    }

    #region gestures
    protected override bool OnGesturePressed(InteractiveEventArgs args)
    {
        if (args.DeviceInputType == DeviceInputTypes.Mouse && args.InputType != InputTypes.MouseLeftButton)
        {
            return false;
        }

        return base.OnGesturePressed(args);
    }

    protected override bool OnGestureRelease(InteractiveEventArgs args)
    {
        return base.OnGestureRelease(args);
    }

    protected override bool OnGestureRunning(InteractiveEventArgs args)
    {
        float deltaX = Math.Abs(StartX - args.X);
        float deltaY = Math.Abs(StartY - args.Y);
        if (IsPressed && (deltaX > _touchSlop || deltaY > _touchSlop))
        {
            args.NextFakeState = GestureTypes.Release;
            //args.SetNextFakeState(GestureTypes.Release);
            return false;
        }

        return base.OnGestureRunning(args);
    }

    protected override bool OnGestureExited(InteractiveEventArgs args)
    {
        if (IsPressed)
            args.NextFakeState = GestureTypes.Release;

        return base.OnGestureExited(args);
    }

    // callbacks
    protected override void CallbackPressed(CallbackEventArgs args)
    {
        StartX = args.X;
        StartY = args.Y;
        IsPressed = true;
        AnimationPressedStart(args.X, args.Y);

        if (args.IsRealCallback && args.DeviceInputType == DeviceInputTypes.TouchScreen)
        {
            _longTapTimer?.Dispose();
            _longTapTimer = this.Dispatcher.CreateAndStartTimer(TimeSpan.FromMilliseconds(500), () =>
            {
                if (IsPressed)
                {
                    OnLongTapCompleted(args);
                    CallbackRelease(new CallbackEventArgs
                    {
                        X = args.X,
                        Y = args.Y,
                        DeviceInputType = args.DeviceInputType,
                        InputType = args.InputType,
                        IsRealCallback = false,
                    });
                }
            });
        }
    }

    protected override void CallbackRelease(CallbackEventArgs args)
    {
        if (IsPressed)
        {
            IsPressed = false;
            _longTapTimer?.Dispose();

            if (args.IsRealCallback)
            {
                OnTapCompleted(args);
                AnimationPressedRestore(args.X, args.Y);
            }
            else
            {
                RestoreButton();
            }
        }
    }

    protected override void CallbackRunning(CallbackEventArgs args)
    {
    }

    protected override void CallbackEntered(CallbackEventArgs args)
    {
        if (!IsMouseOver && IsEnabled)
        {
            IsMouseOver = true;
            AnimationMouseOverStart();
        }
    }

    protected override void CallbackExited(CallbackEventArgs args)
    {
        if (IsMouseOver)
        {
            IsMouseOver = false;
            AnimationMouseOverRestore();
        }
    }

    protected override void CallbackCanceled(CallbackEventArgs args)
    {
        IsMouseOver = false;
        IsPressed = false;
        RestoreButton();
    }
    #endregion gestures

    protected virtual void OnTapCompleted(CallbackEventArgs args)
    {
        OnTapped?.Invoke(this, args);

        if (TapCommand == null)
            return;

        if (TapCommand.CanExecute(TapCommandParameter))
            TapCommand.Execute(TapCommandParameter);
    }

    protected virtual void OnLongTapCompleted(CallbackEventArgs args)
    {
        OnLongTapped?.Invoke(this, args);

        if (LongTapCommand == null)
            return;

        if (LongTapCommand.CanExecute(LongTapCommandParameter))
            LongTapCommand.Execute(LongTapCommandParameter);
    }
}
