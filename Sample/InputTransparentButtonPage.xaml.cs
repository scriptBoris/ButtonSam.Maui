namespace Sample;

public partial class InputTransparentButtonPage
{
	public InputTransparentButtonPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
		DisplayAlert("Clicked", "You pressed on dotnet bot!", "OK");
    }
}