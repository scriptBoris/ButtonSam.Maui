using System.Windows.Input;

namespace Sample;

public partial class MultipleButtonsPage : ContentPage
{
	public MultipleButtonsPage()
	{
		CommandSelectItem = new Command(ActionSelectItem);
        InitializeComponent();

		var list = new List<string>();
		for (int i = 0; i < 1000; i++)
		{
			list.Add($"Button item Number {i}");
		}

		collectionView.ItemsSource = list;
    }

	public ICommand CommandSelectItem { get; set; }
	private void ActionSelectItem(object item)
	{
        if (item is string str)
        {
			DisplayAlert("Pressed", $"You are pressed {str}", "OK");
        }
	}
}