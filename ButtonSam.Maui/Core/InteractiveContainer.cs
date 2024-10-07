using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Core;

/// <summary>
/// Базовый класс который обеспечивает отрисовку Borders, Corners.
/// И умеет только получать жесты от платформенных обработчиков.
/// P.S.
/// Если вас не устраивает реализации классов ButtonBase или Button, то
/// вы можете самостоятельно реализовать свой класс кнопки, просто 
/// наследуя данный класс
/// </summary>
[ContentProperty("Content")]
public abstract class InteractiveContainer : Layout, ILayoutManager, IPadding
{
    public static readonly Color DefaultBackgroundColor = Color.FromArgb("#323232");
    private GestureTypes? oldState;

    public InteractiveContainer()
    {
        IsClippedToBounds = true;
    }

    #region bindable props
    // padding
    public new static readonly BindableProperty PaddingProperty = BindableProperty.Create(
        nameof(Padding),
        typeof(Thickness),
        typeof(InteractiveContainer),
        new Thickness(10),
        propertyChanged: (b, o, n) =>
        {
            if (b is Layout self)
            {
                self.Padding = ((Thickness)n);
            }
        }
    );
    public new Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    // border color
    public static readonly BindableProperty BorderColorProperty = BindableProperty.Create(
        nameof(BorderColor),
        typeof(Color),
        typeof(InteractiveContainer),
        null
    );
    public Color? BorderColor
    {
        get => GetValue(BorderColorProperty) as Color;
        set => SetValue(BorderColorProperty, value);
    }

    // border width
    public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create(
        nameof(BorderWidth),
        typeof(double),
        typeof(InteractiveContainer),
        1.0
    );
    public double BorderWidth
    {
        get => (double)GetValue(BorderWidthProperty);
        set => SetValue(BorderWidthProperty, value);
    }

    // corner radius
    public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(InteractiveContainer),
        new CornerRadius(10)
    );
    public CornerRadius CornerRadius
    {
        get => (CornerRadius)GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    // tap color
    public static readonly BindableProperty TapColorProperty = BindableProperty.Create(
        nameof(TapColor),
        typeof(Color),
        typeof(InteractiveContainer),
        Color.FromRgba("#6da9d8")
    );
    public Color TapColor
    {
        get => (Color)GetValue(TapColorProperty);
        set => SetValue(TapColorProperty, value);
    }

    // try ripple effect
    public static readonly BindableProperty TryRippleEffectProperty = BindableProperty.Create(
        nameof(TryRippleEffect),
        typeof(bool),
        typeof(InteractiveContainer),
        true
    );
    public bool TryRippleEffect
    {
        get => (bool)GetValue(TryRippleEffectProperty);
        set => SetValue(TryRippleEffectProperty, value);
    }

    // content
    public static readonly BindableProperty ContentProperty = BindableProperty.Create(
        nameof(Content),
        typeof(View),
        typeof(InteractiveContainer),
        null,
        propertyChanged: (b, o, n) =>
        {
            if (b is InteractiveContainer self) self.UpdateContent(o as View, n as View);
        }
    );
    public View? Content
    {
        get => GetValue(ContentProperty) as View;
        set => SetValue(ContentProperty, value);
    }

    // is clickable
    public static readonly BindableProperty IsClickableProperty = BindableProperty.Create(
        nameof(IsClickable),
        typeof(bool),
        typeof(InteractiveContainer),
        true
    );
    public bool IsClickable
    {
        get => (bool)GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }
    #endregion bindable props

    #region layout methods
    protected override ILayoutManager CreateLayoutManager()
    {
        return this;
    }

    public virtual Size ArrangeChildren(Rect bounds)
    {
        if (Content is IView cv)
        {
            double x = Padding.Left;
            double y = Padding.Top;
            double w = bounds.Width - Padding.HorizontalThickness;
            double h = bounds.Height - Padding.VerticalThickness;

            if (w < cv.DesiredSize.Width)
                w = cv.DesiredSize.Width;

            if (h < cv.DesiredSize.Height)
                h = cv.DesiredSize.Height;

            var rect = new Rect(x, y, w, h);
            cv.Arrange(rect);
        }

        return bounds.Size;
    }

    public virtual Size Measure(double widthConstraint, double heightConstraint)
    {
        var freeWidth = widthConstraint - Padding.HorizontalThickness;
        var freeHeight = heightConstraint - Padding.VerticalThickness;

        var size = new Size(40, 20);
        if (Content is IView content)
            size = content.Measure(freeWidth, freeHeight);

        size += new Size(Padding.HorizontalThickness, Padding.VerticalThickness);
        return size;
    }
    #endregion layout methods

    private void UpdateContent(View? old, View? news)
    {
        // content
        if (old != null)
            Children.Remove(old);

        if (news != null)
            Children.Add(news);
    }

    protected void DirectChangeBackgroundColor(Color color)
    {
        if (Handler is IButtonHandler bh)
            bh.DirectSetBackgroundColor(color);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnInteractive(InteractiveEventArgs args)
    {
        if (args.State != GestureTypes.Running)
        {
            if (oldState == args.State)
                return;
        }

        oldState = args.State;

        bool canRunning = args.State switch
        {
            GestureTypes.Pressed => OnGesturePressed(args),
            GestureTypes.Release => OnGestureRelease(args),
            GestureTypes.Running => OnGestureRunning(args),
            GestureTypes.Entered => OnGestureEntered(args),
            GestureTypes.Exited => OnGestureExited(args),
            GestureTypes.Canceled => OnGestureCanceled(args),
            _ => throw new NotImplementedException(),
        };

        if (canRunning)
        {
            var callbackArgs = new CallbackEventArgs
            {
                X = args.OverrideX ?? args.X,
                Y = args.OverrideY ?? args.Y,
                DeviceInputType = args.DeviceInputType,
                InputType = args.InputType,
                IsRealCallback = args.IsRealState,
            };

            switch (args.State)
            {
                case GestureTypes.Pressed:
                    CallbackPressed(callbackArgs);
                    break;
                case GestureTypes.Release:
                    CallbackRelease(callbackArgs);
                    break;
                case GestureTypes.Running:
                    CallbackRunning(callbackArgs);
                    break;
                case GestureTypes.Entered:
                    CallbackEntered(callbackArgs);
                    break;
                case GestureTypes.Exited:
                    CallbackExited(callbackArgs);
                    break;
                case GestureTypes.Canceled:
                    CallbackCanceled(callbackArgs);
                    break;
                default:
                    break;
            }
        }

        if (args.NextFakeState != null && args.NextFakeState.Value != args.State)
        {
            OnInteractive(new InteractiveEventArgs
            {
                X = args.X,
                Y = args.Y,
                State = args.NextFakeState.Value,
                InputType = args.InputType,
                DeviceInputType = args.DeviceInputType,
                IsRealState = false,
            });
        }
    }

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGesturePressed(InteractiveEventArgs args);

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGestureRelease(InteractiveEventArgs args);

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGestureRunning(InteractiveEventArgs args);

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGestureEntered(InteractiveEventArgs args);

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGestureExited(InteractiveEventArgs args);

    /// <summary>
    /// Allows to control the logic of triggering a callback.
    /// This method should not modify class variables or call logic methods.
    /// </summary>
    /// <returns>
    /// true - callback will triggered; 
    /// false - callback will not triggered
    /// </returns>
    protected abstract bool OnGestureCanceled(InteractiveEventArgs args);

    protected abstract void CallbackPressed(CallbackEventArgs args);
    protected abstract void CallbackRelease(CallbackEventArgs args);
    protected abstract void CallbackRunning(CallbackEventArgs args);
    protected abstract void CallbackEntered(CallbackEventArgs args);
    protected abstract void CallbackExited(CallbackEventArgs args);
    protected abstract void CallbackCanceled(CallbackEventArgs args);
}