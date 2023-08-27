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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class ButtonHandler
    {
    }

    [ContentProperty("Content")]
    public abstract class ButtonBase : Layout, ILayoutManager
    {
#if __MOBILE__
        protected const float _touchSlop = 10;
#else
        protected const float _touchSlop = 100000;
#endif

        public ButtonBase()
        {
            IsClippedToBounds = true;
        }

        #region bindable props
        // background color
        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(ButtonBase),
            Color.FromArgb("#323232"),
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self)
                    self.ChangeBackgroundColor((Color)n);
            }
        );
        public new Color BackgroundColor
        {
            get => (Color)GetValue(BackgroundColorProperty);
            set => SetValue(BackgroundColorProperty, value);
        }

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
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self) self.UpdateBorderColor();
            }
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
            1.0,
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self) self.UpdateBorderWidth();
            }
        );
        public double BorderWidth
        {
            get => (double)GetValue(BorderWidthProperty);
            set => SetValue(BorderWidthProperty, value);
        }

        // corner radius
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
            nameof(CornerRadius),
            typeof(double),
            typeof(ButtonBase),
            10.0,
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self) self.UpdateCornerRadius();
            }
        );
        public double CornerRadius
        {
            get => (double)GetValue(CornerRadiusProperty);
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
            Color.FromRgba("#6da9d8"),
            propertyChanged: (b, o, n) =>
            {
                if (b is ButtonBase self && self.ClickableView?.Handler is IClickViewHandler h)
                    h.UpdateTapColor((Color)n);
            }
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
                if (b is ButtonBase self) self.Redraw(o as View, n as View);
            }
        );
        public View? Content
        {
            get => GetValue(ContentProperty) as View;
            set => SetValue(ContentProperty, value);
        }
        #endregion bindable props

        #region props
        protected Color CurrentBackgroundColor => base.BackgroundColor;
        protected View? ClickableView { get; private set; }

        protected float StartX { get; set; }
        protected float StartY { get; set; }
        #endregion props

        protected override void OnParentSet()
        {
            base.OnParentSet();
            Redraw(null, null);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

#if WINDOWS
        public Size ArrangeChildren(Rect bounds)
        {
            if (Content is IView cv)
            {
                double x = Padding.Left;
                double y = Padding.Top;
                double w = bounds.Width - Padding.HorizontalThickness;
                double h = bounds.Height - Padding.VerticalThickness;

                if (w < cv.DesiredSize.Width) w = cv.DesiredSize.Width;
                if (h < cv.DesiredSize.Height) h = cv.DesiredSize.Height;

                var r = new Rect(x, y, w, h);
                cv.Arrange(r);
            }

            if (ClickableView is IView c)
            {
                c.Arrange(bounds);
            }

            return bounds.Size;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            double w = widthConstraint - Padding.HorizontalThickness;
            double h = heightConstraint - Padding.VerticalThickness;

            var size = new Size(40,20);
            if (Content is IView content)
                size = content.Measure(w, h);

            size += new Size(Padding.HorizontalThickness, Padding.VerticalThickness);

            return size;
        }
#else
        public Size ArrangeChildren(Rect bounds)
        {
            if (Content is IView cv)
            {
                double bw = (BorderWidth > 0 && BorderColor != null) ? BorderWidth : 0;
                double x = Padding.Left + bw;
                double y = Padding.Top + bw;
                double w = bounds.Width - (Padding.HorizontalThickness + bw * 2);
                double h = bounds.Height - (Padding.VerticalThickness + bw * 2);

                if (w < cv.DesiredSize.Width) w = cv.DesiredSize.Width;
                if (h < cv.DesiredSize.Height) h = cv.DesiredSize.Height;

                var rect = new Rect(x, y, w, h);
                cv.Arrange(rect);
            }

            if (ClickableView is IView c)
            {
                c.Arrange(bounds);
            }

            return bounds.Size;
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            double bw = BorderWidth > 0 && BorderColor != null ? BorderWidth * 2 : 0;
            var w = widthConstraint - (Padding.HorizontalThickness + bw);
            var h = heightConstraint - (Padding.VerticalThickness + bw);

            var size = Content?.Measure(w, h) ?? new Size(40, 20);
            size += new Size(Padding.HorizontalThickness, Padding.VerticalThickness);
            size += new Size(bw, bw);

            return size;
        }
#endif

        private void Redraw(View? old, View? news)
        {
            // content
            if (old != null)
                Children.Remove(old);

            if (news != null)
                Children.Insert(0, news);

            // clickable
            if (ClickableView == null)
            {
                ClickableView = new ClickView(this);
                Children.Add(ClickableView);
            }
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

        protected void ChangeBackgroundColor(Color color)
        {
            base.BackgroundColor = color;
        }

        private void UpdateCornerRadius()
        {
#if ANDROID
            if (Handler is ButtonHandler buttonHandler)
                buttonHandler.UpdateCornerRadius(CornerRadius);

            if (this is IView v) 
                v.InvalidateMeasure();
#elif WINDOWS
            if (Handler is ButtonHandler buttonHandler)
                buttonHandler.UpdateCornerRadius(CornerRadius);
#elif IOS
            if (Handler is ButtonHandler handler) handler.UpdateCALayer();
#endif
        }

        private void UpdateBorderColor()
        {
#if ANDROID
            if (this is IView v) v.InvalidateMeasure();
#elif WINDOWS
            if (Handler is ButtonHandler handler) handler.UpdateBorderColor(BorderColor);
#elif IOS
            if (Handler is ButtonHandler handler) handler.UpdateCALayer();
#endif
        }

        private void UpdateBorderWidth()
        {
#if ANDROID
            if (this is IView v) v.InvalidateMeasure();
#elif WINDOWS
            if (Handler is ButtonHandler handler) handler.UpdateBorderWidth(BorderWidth);
#elif IOS
            if (Handler is ButtonHandler handler) handler.UpdateCALayer();
#endif
        }
    }

    public static class ViewExtensions
    {
        public static Task<bool> ColorTo(this VisualElement self, Color fromColor, Color toColor, Action<Color> callback, uint length = 250, Easing easing = null)
        {
            Func<double, Color> transform = (t) =>
                Color.FromRgba(fromColor.Red + t * (toColor.Red - fromColor.Red),
                               fromColor.Green + t * (toColor.Green - fromColor.Green),
                               fromColor.Blue + t * (toColor.Blue - fromColor.Blue),
                               fromColor.Alpha + t * (toColor.Alpha - fromColor.Alpha));
            return ColorAnimation(self, "ColorTo", transform, callback, length, easing);
        }

        public static void CancelAnimation(this VisualElement self)
        {
            self.AbortAnimation("ColorTo");
        }

        private static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
    }
}
