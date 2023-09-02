using ButtonSam.Maui.Core;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSam.Maui.Core
{
    [ContentProperty("Content")]
    public abstract class ButtonBase : Layout, ILayoutManager
    {
#if __MOBILE__
        protected const float _touchSlop = 10;
#else
        protected const float _touchSlop = 100000;
#endif

        internal static Color DefaultBackgroundColor = Color.FromArgb("#323232");

        public ButtonBase()
        {
            IsClippedToBounds = true;
        }

        #region bindable props
        // padding
        public new static readonly BindableProperty PaddingProperty = BindableProperty.Create(
            nameof(Padding),
            typeof(Thickness),
            typeof(ButtonBase),
            new Thickness(10),
            propertyChanged: (b, o, n) =>
            {
                if (b is IView view)
                {
                    ((Layout)view).Padding = (Thickness)n;
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
            typeof(ButtonBase),
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
            typeof(ButtonBase),
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
            typeof(ButtonBase),
            new CornerRadius(10)
        );
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // tap command
        public static readonly BindableProperty TapCommandProperty = BindableProperty.Create(
            nameof(TapCommand),
            typeof(ICommand),
            typeof(ButtonBase),
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
            typeof(ButtonBase),
            null
        );
        public object? TapCommandParameter
        {
            get => GetValue(TapCommandParameterProperty);
            set => SetValue(TapCommandParameterProperty, value);
        }

        // tap color
        public static readonly BindableProperty TapColorProperty = BindableProperty.Create(
            nameof(TapColor),
            typeof(Color),
            typeof(ButtonBase),
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
            typeof(ButtonBase),
            true
        );
        public bool TryRippleEffect
        {
            get => (bool)GetValue(TryRippleEffectProperty);
            set => SetValue(TryRippleEffectProperty, value);
        }

        // is pressed
        public static readonly BindableProperty IsPressedProperty = BindableProperty.Create(
            nameof(IsPressed),
            typeof(bool),
            typeof(ButtonBase),
            false
        );
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }

        // is mouse over
        public static readonly BindableProperty IsMouseOverProperty = BindableProperty.Create(
            nameof(IsMouseOver),
            typeof(bool),
            typeof(ButtonBase),
            false
        );
        public bool IsMouseOver
        {
            get => (bool)GetValue(IsMouseOverProperty);
            set => SetValue(IsMouseOverProperty, value);
        }

        // content
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
            nameof(Content),
            typeof(View),
            typeof(ButtonBase),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self) self.UpdateContent(o as View, n as View);
            }
        );
        public View? Content
        {
            get => GetValue(ContentProperty) as View;
            set => SetValue(ContentProperty, value);
        }
        #endregion bindable props

        #region props
        protected float StartX { get; set; }
        protected float StartY { get; set; }
        #endregion props

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        public Size ArrangeChildren(Rect bounds)
        {
            if (Content is IView cv)
            {
                double x = Padding.Left;
                double y = Padding.Top;
                double w = bounds.Width - Padding.HorizontalThickness;
                double h = bounds.Height - Padding.VerticalThickness;
                var rect = new Rect(x, y, w, h);
                cv.Arrange(rect);
            }

            return bounds.Size;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            var freeWidth = widthConstraint - Padding.HorizontalThickness;
            var freeHeight = heightConstraint - Padding.VerticalThickness;

            var size = new Size(40, 20);
            if (Content is IView content)
                size = content.Measure(freeWidth, freeHeight);

            size += new Size(Padding.HorizontalThickness, Padding.VerticalThickness);
            return size;
        }

        private void UpdateContent(View? old, View? news)
        {
            // content
            if (old != null)
                Children.Remove(old);

            if (news != null)
                Children.Add(news);
        }

        public void OnInteractive(InteractiveEventArgs args)
        {
            bool isPressedOld = IsPressed;
            switch (args.State)
            {
                case GestureTypes.Pressed:
                    OnGesturePressed(args);
                    break;
                case GestureTypes.ReleaseCompleted:
                    OnGestureRelease(args);
                    break;
                case GestureTypes.ReleaseCanceled:
                    OnGestureReleaseCanceled(args);
                    break;
                case GestureTypes.Running:
                    OnGestureMove(args);
                    break;
                case GestureTypes.RunningCanceled:
                    OnGestureMoveCanceled(args);
                    break;
                default:
                    break;
            }

            if (isPressedOld == IsPressed)
                return;

            if (isPressedOld && args.State == GestureTypes.ReleaseCompleted)
            {
                OnTapComleted();
            }
        }

        protected virtual void OnGesturePressed(InteractiveEventArgs args)
        {
            StartX = args.X;
            StartY = args.Y;
            IsPressed = true;
        }

        protected virtual void OnGestureRelease(InteractiveEventArgs args)
        {
            IsPressed = false;
        }

        protected virtual void OnGestureReleaseCanceled(InteractiveEventArgs args)
        {
            IsPressed = false;
        }

        protected virtual void OnGestureMove(InteractiveEventArgs args)
        {
            float deltaX = Math.Abs(StartX - args.X);
            float deltaY = Math.Abs(StartY - args.Y);

            if (deltaX > _touchSlop || deltaY > _touchSlop)
                IsPressed = false;

            if (args.DeviceInputType == DeviceInputTypes.Mouse)
                IsMouseOver = new Rect(0, 0, Frame.Width, Frame.Height).Contains(args.X, args.Y);
            else
                IsMouseOver = false;
        }

        protected virtual void OnGestureMoveCanceled(InteractiveEventArgs args)
        {
            IsMouseOver = false;
        }

        protected virtual void OnTapComleted()
        {
            if (TapCommand == null || !IsEnabled)
                return;

            if (TapCommand.CanExecute(TapCommandParameter))
                TapCommand.Execute(TapCommandParameter);
        }

        protected void DirectChangeBackgroundColor(Color color)
        {
            if (Handler is IButtonHandler bh)
                bh.DirectSetBackgroundColor(color);
        }
    }

    public static class ViewExtensions
    {
        public static Task<bool> TransitAnimation(this View view,
            string animName,
            double start,
            double end,
            uint length,
            Easing? easing,
            Action<double> f)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            view.Animate(animName, f, start, end, length: length, easing: easing, finished: (x, b) =>
            {
                taskCompletionSource.SetResult(!b);
            });
            return taskCompletionSource.Task;
        }

        public static Color ApplyTint(this Color from, Color to, double percent)
        {
            return Color.FromRgba(
                from.Red + percent * (to.Red - from.Red),
                from.Green + percent * (to.Green - from.Green),
                from.Blue + percent * (to.Blue - from.Blue),
                from.Alpha + percent * (to.Alpha - from.Alpha)
            );
        }
    }
}
