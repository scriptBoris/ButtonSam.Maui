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
}