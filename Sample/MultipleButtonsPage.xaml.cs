namespace Sample;

public partial class MultipleButtonsPage : ContentPage
{
	public MultipleButtonsPage()
	{
		InitializeComponent();

		var list = new List<string>();
		for (int i = 0; i < 1000; i++)
		{
			list.Add($"Button item Number {i}");
		}

		collectionView.ItemsSource = list;
    }
}