using System.Windows.Input;

namespace Sample;

public partial class MultipleButtonsPage : ContentPage
{
	public MultipleButtonsPage()
	{
		CommandSelectItem = new Command(ActionSelectItem);
		CommandLongTapItem = new Command(ActionLongTapItem);
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

    public ICommand CommandLongTapItem { get; }
	public void ActionLongTapItem(object item)
	{
        if (item is string str)
        {
            DisplayAlert("Long tap", $"You are pressed {str}", "OK");
        }
    }
}