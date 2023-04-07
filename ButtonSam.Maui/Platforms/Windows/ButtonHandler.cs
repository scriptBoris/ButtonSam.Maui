using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml.Controls;
using WBorder = Microsoft.UI.Xaml.Controls.Border;
using WColor = Microsoft.UI.Colors;
using WThickness = Microsoft.UI.Xaml.Thickness;
using WCornerRadius = Microsoft.UI.Xaml.CornerRadius;
using Microsoft.Maui.Controls.Shapes;
using System.ComponentModel;
using Microsoft.UI.Xaml.Hosting;

namespace ButtonSam.Maui
{
    public partial class ButtonHandler : LayoutHandler, IButtonHandler
    {
        public Button Proxy => (Button)VirtualView;
        public ButtonLayout? Native { get; set; }
        public bool IsUseBorder => Proxy.BorderColor != null && Proxy.BorderWidth > 0;
        public override bool NeedsContainer => true;
        public WrapperView? Wrapper => ContainerView as WrapperView;

        public void UpdateCornerRadius(double radius)
        {
            if (Native == null || !IsUseBorder)
                return;

            //Native.Border.CornerRadius = new WCornerRadius(radius);
        }

        protected override LayoutPanel CreatePlatformView()
        {
            var n = new ButtonLayout(this);
            return n;
        }

        protected override void SetupContainer()
        {
            base.SetupContainer();
            ContainerView?.ClipToBounds(true);
            //var container = (WrapperView)ContainerView!;
            //container.Clip = new RoundRectangleGeometry
            //{
            //    CornerRadius = new CornerRadius(10),
            //};
        }
    }

    public class ButtonLayout : LayoutPanel
    {
        private readonly ButtonHandler handler;

        //public WBorder Border { get; private set; }

        public ButtonLayout(ButtonHandler handler)
        {
            this.handler = handler;
            //Border = new WBorder();

            //if (handler.IsUseBorder)
            //{
            //    Border.BorderBrush = handler.Proxy.BorderColor?.ToPlatform();
            //    Border.BorderThickness = new WThickness(handler.Proxy.BorderWidth);
            //    Border.CornerRadius = new WCornerRadius(handler.Proxy.CornerRadius);
            //}

            //Children.Add(Border);
        }

        //protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        //{
        //    Border.Arrange(new Windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height));
        //    var res = base.ArrangeOverride(finalSize);
        //    return res;
        //}

        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            //if (!Children.Contains(Border))
            //    Children.Insert(0, Border);

            //Border.Measure(availableSize);

            var m = base.MeasureOverride(availableSize);

            if (handler.Wrapper != null)
                handler.Wrapper.Clip = new RoundRectangleGeometry
                {
                    CornerRadius = new CornerRadius(10),
                    Rect = new Rect(0, 0, m.Width, m.Height),
                };

            return m;
        }
    }

    public static class Ext
    {
        public static void ClipToBounds(this Microsoft.UI.Xaml.DependencyObject d, bool isCliping)
        {
            if (d is Microsoft.UI.Xaml.UIElement element)
            {
                var clipToBounds = isCliping;
                var visual = ElementCompositionPreview.GetElementVisual(element);
                visual.Clip = clipToBounds ? visual.Compositor.CreateInsetClip() : null;
            }
        }
    }
}