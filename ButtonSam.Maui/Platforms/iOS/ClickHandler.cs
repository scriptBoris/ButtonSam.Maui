using Microsoft.Maui.Handlers;
using UIKit;

namespace ButtonSam.Maui.Core;

public partial class ClickViewHandler : ViewHandler<ClickView, UIKit.UIView>
{
    private bool isPressed => Button.IsPressed;
    public Button Button => (Button)VirtualView.Button;

    protected override UIView CreatePlatformView()
    {
        var n = new UIClickView();
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
                    State = GestureTypes.Pressed,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
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
                        State = GestureTypes.ReleaseCanceled,
                        InputType = InputTypes.None,
                        DeviceInputType = DeviceInputTypes.Touch,
                    });
                }
                break;

            case UIGestureRecognizerState.Ended:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.ReleaseCompleted,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;

            case UIGestureRecognizerState.Cancelled:
            case UIGestureRecognizerState.Failed:
                Button.OnInteractive(new InteractiveEventArgs
                {
                    X = x,
                    Y = y,
                    State = GestureTypes.ReleaseCanceled,
                    InputType = InputTypes.Touch,
                    DeviceInputType = DeviceInputTypes.Touch,
                });
                break;
            default:
                break;
        }
    }
}

public class UIClickView : UIView
{
}

public class GestureClick : UILongPressGestureRecognizer
{
    public GestureClick(Action<UILongPressGestureRecognizer> action) : base(action)
    {
        MinimumPressDuration = 0;
    }
}
