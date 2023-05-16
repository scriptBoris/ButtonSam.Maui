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
using Windows.Devices.Radios;

namespace ButtonSam.Maui
{
    public partial class ButtonHandler : LayoutHandler
    {
        public Button Proxy => (Button)VirtualView;
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
            return n;
        }

        protected override void SetupContainer()
        {
            base.SetupContainer();
            UpdateCornerRadius(Proxy.CornerRadius);
            UpdateBorderWidth(Proxy.BorderWidth);
            UpdateBorderColor(Proxy.BorderColor);

            var defaultColor = Proxy.BackgroundColor;
            Wrapper!.Background = defaultColor.ToPlatform();
        }
    }
}