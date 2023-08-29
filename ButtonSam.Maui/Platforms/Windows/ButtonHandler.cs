using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using WBorder = Microsoft.UI.Xaml.Controls.Border;
using WColor = Microsoft.UI.Colors;
using WRect = Windows.Foundation.Rect;
using WSize = Windows.Foundation.Size;
using WThickness = Microsoft.UI.Xaml.Thickness;
using WCornerRadius = Microsoft.UI.Xaml.CornerRadius;
using WShapes = Microsoft.UI.Xaml.Shapes;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using Microsoft.UI.Xaml.Hosting;
using WControls = Microsoft.UI.Xaml.Controls;
using WView = Microsoft.UI.Xaml.FrameworkElement;

namespace ButtonSam.Maui.Core
{
    public partial class ButtonHandler : LayoutHandler, IButtonHandler
    {
        private WShapes.Rectangle? animLayer;

        public ButtonBase Proxy => (ButtonBase)VirtualView;
        public bool IsUseBorder => Proxy.BorderColor != null && Proxy.BorderWidth > 0;
        public override bool NeedsContainer => true;
        public WrapperView? Wrapper => ContainerView as WrapperView;

        public void UpdateCornerRadius(double radius)
        {
            if (Wrapper != null)
                Wrapper.CornerRadius = new WCornerRadius(radius);
        }

        public void UpdateBorderWidth(double width)
        {
            if (Wrapper == null)
                return;

            if (!IsUseBorder)
            {
                Wrapper.BorderBrush = null;
                Wrapper.BorderThickness = new WThickness(0);
                return;
            }

            Wrapper.BorderThickness = new WThickness(width);
        }

        public void UpdateBorderColor(Color? color)
        {
            if (Wrapper == null) 
                return;

            if (!IsUseBorder)
            {
                Wrapper.BorderBrush = null;
                Wrapper.BorderThickness = new WThickness(0);
                return;
            }

            Wrapper.BorderBrush = color?.ToPlatform();
        }

        protected override LayoutPanel CreatePlatformView()
        {
            var n = base.CreatePlatformView();
            n.Background = null;
            return n;
        }

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);

            UpdateCornerRadius(Proxy.CornerRadius);
            UpdateBorderWidth(Proxy.BorderWidth);
            UpdateBorderColor(Proxy.BorderColor);
            Wrapper.Background = (Proxy.BackgroundColor ?? ButtonBase.DefaultBackgroundColor).ToPlatform();

            animLayer = new();
            PlatformView.Children.Insert(0, animLayer);
            PlatformView.Background = null;
        }

        public override void PlatformArrange(Rect rect)
        {
            base.PlatformArrange(rect);
            animLayer?.Arrange(new WRect(0, 0, rect.Width, rect.Height));
        }

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = base.GetDesiredSize(widthConstraint, heightConstraint);
            animLayer?.Measure(new WSize(size.Width, size.Height));
            return size;
        }

        public bool OverrideAdd(object? value)
        {
            if (value is LayoutHandlerUpdate udpate)
                PlatformView.Children.Insert(1, udpate.View.ToPlatform(MauiContext));
            return true;
        }

        public bool OverrideInsert(object? value)
        {
            if (value is LayoutHandlerUpdate update)
                PlatformView.Children.Insert(1, update.View.ToPlatform(MauiContext));
            return true;
        }

        public bool OverrideBackgroundColor(Color? color)
        {
            if (Wrapper != null)
                Wrapper.Background = (color ?? ButtonBase.DefaultBackgroundColor).ToPlatform();

            return true;
        }
    }
}