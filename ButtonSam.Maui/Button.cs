using ButtonSam.Maui.Core;
using ButtonSam.Maui.Internal;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ButtonSam.Maui
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class ButtonHandler
    {
    }

    [ContentProperty("Content")]
    public class Button : Layout, ILayoutManager
    {
        private const float _touchSlop = 10;
        private float startX;
        private float startY;
        private View? clickable;

        public Button()
        {
            IsClippedToBounds = true;
        }

        #region bindable props
        // background color
        public new static readonly BindableProperty BackgroundColorProperty = BindableProperty.Create(
            nameof(BackgroundColor),
            typeof(Color),
            typeof(Button),
            Color.FromArgb("#323232"),
            propertyChanged: (b, o, n) =>
            {
                if (b is View self)
                    self.BackgroundColor = (Color)n;
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
            typeof(Button),
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
            typeof(Button),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is Button self) self.UpdateBorderColor();
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
            typeof(Button),
            1.0,
            propertyChanged: (b, o, n) =>
            {
                if (b is Button self) self.UpdateBorderWidth();
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
            typeof(Button),
            10.0,
            propertyChanged: (b, o, n) =>
            {
                if (b is Button self) self.UpdateCornerRadius();
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

        // tap color
        public static readonly BindableProperty TapColorProperty = BindableProperty.Create(
            nameof(TapColor),
            typeof(Color),
            typeof(Button),
            Color.FromRgba("#6da9d8"),
            propertyChanged: (b, o, n) =>
            {
                if (b is Button self && self.clickable?.Handler is IClickHandler h)
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
            typeof(Button),
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
            typeof(Button),
            false
        );
        public bool IsPressed
        {
            get => (bool)GetValue(IsPressedProperty);
            set => SetValue(IsPressedProperty, value);
        }

        // content
        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
            nameof(Content),
            typeof(View),
            typeof(Button),
            null,
            propertyChanged: (b, o, n) =>
            {
                if (b is Button self) self.Redraw(o as View, n as View);
            }
        );
        public View? Content
        {
            get => GetValue(ContentProperty) as View;
            set => SetValue(ContentProperty, value);
        }
        #endregion bindable props

        protected override void OnParentSet()
        {
            base.OnParentSet();
            Redraw(null, null);
        }

        protected override ILayoutManager CreateLayoutManager()
        {
            return this;
        }

        public Size ArrangeChildren(Rect bounds)
        {
#if WINDOWS
            return ArrangeChildrenForWindows(bounds);
#else
            if (Content is IView cv)
            {
                double bw = 0;

                if (BorderWidth > 0 && BorderColor != null)
                    bw = BorderWidth;

                double x = Padding.Left + bw;
                double y = Padding.Top + bw;
                double w = bounds.Width - (Padding.HorizontalThickness + bw);
                double h = bounds.Height - (Padding.VerticalThickness + bw);
                cv.Arrange(new Rect(x, y, w, h));
            }

            if (clickable is IView c)
            {
                c.Arrange(bounds);
            }

            return bounds.Size;
#endif

        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
#if WINDOWS
            return MeasureForWindows(widthConstraint, heightConstraint);
#else
            var size = Content?.Measure(widthConstraint, heightConstraint) ?? new Size(40, 20);
            size = size + new Size(Padding.HorizontalThickness, Padding.VerticalThickness);

            if (BorderWidth > 0 && BorderColor != null)
                size += new Size(BorderWidth*2, BorderWidth*2);

            return size;
#endif
        }

        private Size ArrangeChildrenForWindows(Rect bounds)
        {
            if (Content is IView cv)
            {
                double x = Padding.Left;
                double y = Padding.Top;
                double w = bounds.Width - Padding.HorizontalThickness;
                double h = bounds.Height - Padding.VerticalThickness;

                var r = new Rect(x, y, w, h);
                cv.Arrange(r);
            }

            if (clickable is IView c)
            {
                c.Arrange(bounds);
            }

            return bounds.Size;
        }

        private Size MeasureForWindows(double widthConstraint, double heightConstraint)
        {
            double w = widthConstraint - Padding.HorizontalThickness;
            double h = heightConstraint - Padding.VerticalThickness;

            var size = Content?.Measure(w, h) ?? new Size(40, 20);
            size += new Size(Padding.HorizontalThickness, Padding.VerticalThickness);

            return size;
        }

        private void Redraw(View? old, View? news)
        {
            // content
            if (old != null)
                Children.Remove(old);

            if (news != null)
                Children.Insert(0, news);

            // clickable
            if (clickable == null)
            {
                clickable = new Click(this);
                Children.Add(clickable);
            }
        }

        public virtual void OnAnimationStart()
        {
            if (!IsEnabled)
                return;

            bool isRipple = TryAnimationRippleStart(startX, startY);
            if (!isRipple)
            {
                var b = base.BackgroundColor ?? Colors.Transparent;
                this.ColorTo(b, TapColor, c => base.BackgroundColor = c, 100);
            }
        }

        public virtual void OnAnimationFinish()
        {
            if (!IsEnabled && !IsPressed)
                return;

            bool isRipple = TryAnimationRippleFinish();
            if (!isRipple)
            {
                var b = base.BackgroundColor ?? Colors.Transparent;
                this.ColorTo(b, BackgroundColor, c => base.BackgroundColor = c, 180);
            }
        }

        protected virtual bool TryAnimationRippleStart(float x, float y)
        {
#if ANDROID
            if (TryRippleEffect && clickable?.Handler is ClickHandler h)
                return h.TryAnimationRippleStart(startX, startY);
#endif
            return false;
        }

        protected virtual bool TryAnimationRippleFinish()
        {
#if ANDROID
            if (TryRippleEffect && clickable?.Handler is ClickHandler h)
                return h.TryAnimationRippleEnd();
#endif
            return false;
        }

        protected virtual void HandleInteractiveStarted(HandleInteractiveStartedArgs args)
        {
            args.StartX = args.Input.X;
            args.StartY = args.Input.Y;
            args.IsPressed = true;
        }

        protected virtual void HandleInteractiveCompleted(HandleInteractiveCompletedArgs args)
        {
            args.IsPressed = false;
        }

        protected virtual void HandleInteractiveCanceled(HandleInteractiveCanceledArgs args)
        {
            args.IsPressed = false;
        }

        protected virtual void HandleInteractiveRunning(HandleInteractiveRunningArgs args)
        {
            float deltaX = Math.Abs(startX - args.Input.X);
            float deltaY = Math.Abs(startY - args.Input.Y);

            if (deltaX > _touchSlop || deltaY > _touchSlop)
                args.IsPressed = false;
        }

        internal virtual void OnInteractive(InteractiveEventArgs args)
        {
            bool isPressedOld = IsPressed;
            switch (args.State)
            {
                case GestureStatus.Started:
                    var startedArgs = new HandleInteractiveStartedArgs { Input = args };
                    HandleInteractiveStarted(startedArgs);

                    if (startedArgs.StartX != null)
                        startX = startedArgs.StartX.Value;

                    if (startedArgs.StartY != null)
                        startY = startedArgs.StartY.Value;

                    if (startedArgs.IsPressed != null)
                        IsPressed = startedArgs.IsPressed.Value;

                    break;
                case GestureStatus.Completed:
                    var completedArgs = new HandleInteractiveCompletedArgs { Input = args };
                    HandleInteractiveCompleted(completedArgs);

                    if (completedArgs.IsPressed != null)
                        IsPressed = completedArgs.IsPressed.Value;
                    break;
                case GestureStatus.Canceled:
                    var cancelArgs = new HandleInteractiveCanceledArgs { Input = args };
                    HandleInteractiveCanceled(cancelArgs);

                    if (cancelArgs.IsPressed != null)
                        IsPressed = cancelArgs.IsPressed.Value;
                    break;
                case GestureStatus.Running:
                    var runningArgs = new HandleInteractiveRunningArgs { Input = args };
                    HandleInteractiveRunning(runningArgs);

                    if (runningArgs.IsPressed != null)
                        IsPressed = runningArgs.IsPressed.Value;
                    break;
                default:
                    break;
            }

            if (isPressedOld == IsPressed)
                return;

            if (IsPressed)
                OnAnimationStart();
            else
                OnAnimationFinish();

            if (isPressedOld && args.State == GestureStatus.Completed)
            {
                if (TapCommand == null || !IsEnabled)
                    return;

                if (TapCommand.CanExecute(TapCommandParameter))
                    TapCommand.Execute(TapCommandParameter);
            }
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

        static Task<bool> ColorAnimation(VisualElement element, string name, Func<double, Color> transform, Action<Color> callback, uint length, Easing easing)
        {
            easing = easing ?? Easing.Linear;
            var taskCompletionSource = new TaskCompletionSource<bool>();

            element.Animate<Color>(name, transform, callback, 16, length, easing, (v, c) => taskCompletionSource.SetResult(c));
            return taskCompletionSource.Task;
        }
    }
}
