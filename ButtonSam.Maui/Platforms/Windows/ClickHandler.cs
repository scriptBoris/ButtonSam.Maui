using ButtonSam.Maui.Core;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, Microsoft.UI.Xaml.Controls.Panel>
    {
        private bool isPressed;
        public Button Button => VirtualView.Button;

        protected override Panel CreatePlatformView()
        {
            var n = new ClickPanel();
            n.Background = Colors.Transparent.ToPlatform();
            n.IsHitTestVisible = true;
            n.IsTapEnabled = true;
            n.PointerPressed += N_PointerPressed;
            n.PointerReleased += N_PointerReleased;
            n.PointerExited += N_PointerExited;
            n.PointerMoved += N_PointerMoved;
            return n;
        }

        private void N_PointerMoved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;
            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = InteractiveStates.Running,
            });
        }

        private void N_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;

            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = InteractiveStates.ReleaseCanceled,
            });

            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = InteractiveStates.RunningCanceled,
            });
        }

        private void N_PointerPressed(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;
            isPressed = true;
            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = InteractiveStates.Pressed,
            });
        }

        private void N_PointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint((UIElement)sender);
            float x = (float)point.Position.X;
            float y = (float)point.Position.Y;

            isPressed = false;
            Button.OnInteractive(new InteractiveEventArgs
            {
                X = x,
                Y = y,
                State = InteractiveStates.ReleaseCompleted,
            });
        }
    }

    public class ClickPanel : Microsoft.UI.Xaml.Controls.Panel
    {
    }
}
