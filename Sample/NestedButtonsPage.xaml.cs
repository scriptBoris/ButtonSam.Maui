namespace Sample;

public partial class NestedButtonsPage : ContentPage
{
	public NestedButtonsPage()
	{
		InitializeComponent();
	}

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
		DisplayAlert("Info", "You opened Wikipedia", "OK");
    }

    private void TapGestureRecognizer_Tapped_1(object sender, TappedEventArgs e)
    {
        DisplayAlert("Info", "You clicked on the parent element", "OK");
    }
}