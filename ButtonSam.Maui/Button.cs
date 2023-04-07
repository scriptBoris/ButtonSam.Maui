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
    public interface IButtonHandler
    {
        void UpdateCornerRadius(double radius);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class ButtonHandler
    {
    }

    [ContentProperty("Content")]
    public class Button : Layout, ILayoutManager
    {
        private View? clickable;

        public Button()
        {
            IsClippedToBounds = true;
            //base.BackgroundColor = (Color)BackgroundColorProperty.DefaultValue;
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
                if (b is IView v) v.InvalidateMeasure();
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
                if (b is IView v) v.InvalidateMeasure();
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
                if (b is IView self)
                {
                    if (b is Button btn && btn.Handler is IButtonHandler h)
                        h.UpdateCornerRadius((double)n);

                    self.InvalidateMeasure();
                }
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
        }

        public Size Measure(double widthConstraint, double heightConstraint)
        {
            var size = Content?.Measure(widthConstraint, heightConstraint) ?? new Size(40, 20);
            size = size + new Size(Padding.HorizontalThickness, Padding.VerticalThickness);

            if (BorderWidth > 0 && BorderColor != null)
                size += new Size(BorderWidth*2, BorderWidth*2);

            return size;
        }

        private void Redraw(View? old, View? news)
        {
            // content
            if (old != null)
                Children.Remove(old);

            if (news != null)
                Children.Add(news);

            // clickable
            if (clickable == null)
            {
                clickable = new Click(this);
                Children.Add(clickable);
            }
        }

        internal void OnClickedStart()
        {
            var b = base.BackgroundColor ?? Colors.Transparent;
            this.ColorTo(b, TapColor, c => base.BackgroundColor = c, 100);
        }

        internal void OnClickedFinish()
        {
            var b = base.BackgroundColor ?? Colors.Transparent;
            this.ColorTo(b, BackgroundColor, c => base.BackgroundColor = c, 180);
        }

        internal void ThrowTap()
        {
            if (TapCommand == null)
                return;

            if (TapCommand.CanExecute(TapCommandParameter))
                TapCommand.Execute(TapCommandParameter);
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
