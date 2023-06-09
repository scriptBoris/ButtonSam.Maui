using CoreAnimation;
using CoreFoundation;
using CoreGraphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace ButtonSam.Maui.Core
{
    public partial class ButtonHandler : LayoutHandler
    {
        public ButtonBase Proxy => (ButtonBase)VirtualView;

        protected override LayoutView CreatePlatformView()
        {
            var n = new ButtonLayoutView(this);
            return n;
        }

        public override void SetVirtualView(IView view)
        {
            base.SetVirtualView(view);

            if (PlatformView.BackgroundColor == null) 
            {
                PlatformView.BackgroundColor = Proxy.BackgroundColor.ToPlatform();
            }
        }

        public void UpdateCALayer()
        {
            var den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
            var layer = PlatformView.Layer;
            if (Proxy.BorderWidth > 0 && Proxy.BorderColor != null)
            {
                layer.BorderWidth = (float)Proxy.BorderWidth / den;
                layer.BorderColor = Proxy.BorderColor?.ToCGColor();
                layer.BackgroundColor = Colors.Transparent.ToCGColor();
            }
            else
            {
                layer.BorderWidth = 0;
                layer.BorderColor = null;
            }

            layer.CornerRadius = (float)Proxy.CornerRadius;
        }
    }

    public class ButtonLayoutView : LayoutView
    {
        private readonly ButtonHandler handler;

        public ButtonLayoutView(ButtonHandler handler)
        {
            this.handler = handler;
        }

        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
        }

        public override void DrawLayer(CALayer layer, CGContext context)
        {
            base.DrawLayer(layer, context);

            if (handler.Proxy.BorderWidth > 0 && handler.Proxy.BorderColor != null)
            {
                var den = (float)Microsoft.Maui.Devices.DeviceDisplay.Current.MainDisplayInfo.Density;
                layer.BorderWidth = (float)handler.Proxy.BorderWidth / den; 
                layer.BorderColor = handler.Proxy.BorderColor?.ToCGColor();
                layer.BackgroundColor = Colors.Transparent.ToCGColor();
            }

            layer.CornerRadius = (float)handler.Proxy.CornerRadius;
        }

        public override void WillDrawLayer(CALayer layer)
        {
            base.WillDrawLayer(layer);
        }
    }
}