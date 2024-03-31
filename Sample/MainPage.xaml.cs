namespace Sample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

#if RELEASE
        lbl.IsVisible = true;
#endif
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

    private void Button_Clicked_7(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonDisabledPage());
    }

    private void Button_Clicked_8(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonWithShadowsPage());
    }

    private void Button_Clicked_9(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MultipleButtonsPage());
    }

    private void Button_Clicked_10(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SelectionButtonsPage());
    }

    private void Button_Clicked_11(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ButtonRoundedPage());
    }

    private void Button_Clicked_12(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NestedButtonsPage());
    }

    private void Button_Clicked_13(object sender, EventArgs e)
    {
        Navigation.PushAsync(new InputTransparentButtonPage());
    }

    private void Button_Clicked_14(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ScaledButtonPage());
    }

    private void Button_Clicked_15(object sender, EventArgs e)
    {
        Navigation.PushAsync(new NoClickableButtonPage());
    }
}

