namespace Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonClickerPage());
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonNoRipplePage());
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonAdvanceContentPage());
    }

    private void Button_Clicked_3(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonFixedSizePage());
    }

    private void Button_Clicked_4(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonWithBorders());
    }

    private void Button_Clicked_5(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonTapColorPage());
    }

    private void Button_Clicked_6(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonGlobalStylePage());
    }
}

