using ButtonSam.Maui.Core;
using ButtonSam.Maui.Internal;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace ButtonSam.Maui.Internal
{
    public partial class ClickHandler : ViewHandler<Click, UIKit.UIView>
    {
        private bool isPressed => Button.IsPressed;
        public Button Button => (Button)VirtualView.Button;

        protected override UIView CreatePlatformView()
        {
            var n = new ClickView();
            var g = new GestureClick(OnTap);

            n.UserInteractionEnabled = true;
            n.AddGestureRecognizer(g);
            return n;
        }

        public void OnTap(UILongPressGestureRecognizer press)
        {
            var point = press.LocationInView(press.View);
            float x = (float)point.X;
            float y = (float)point.Y;

            switch (press.State)
            {
                case UIGestureRecognizerState.Began:
                    Button.OnInteractive(new InteractiveEventArgs 
                    { 
                        X = x,
                        Y = y,
                        State = InteractiveStates.Pressed,
                    });
                    break;

                case UIGestureRecognizerState.Changed:
                    var coordinate = press.LocationInView(PlatformView);
                    bool isInside = PlatformView.PointInside(coordinate, null);
                    if (!isInside && isPressed) 
                    {
                        Button.OnInteractive(new InteractiveEventArgs 
                        {
                            X = x,
                            Y = y,
                            State = InteractiveStates.ReleaseCanceled, 
                        });
                    }
                    break;

                case UIGestureRecognizerState.Ended:
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = InteractiveStates.ReleaseCompleted,
                    });
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    Button.OnInteractive(new InteractiveEventArgs
                    {
                        X = x,
                        Y = y,
                        State = InteractiveStates.ReleaseCanceled,
                    });
                    break;
                default:
                    break;
            }
        }
    }

    public class ClickView : UIView
    {
    }

    public class GestureClick : UILongPressGestureRecognizer
    {
        public GestureClick(Action<UILongPressGestureRecognizer> action) : base(action)
        {
            MinimumPressDuration = 0;
        }
    }
}
