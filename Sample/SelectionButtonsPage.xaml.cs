using Sample.Models;
using System.Windows.Input;

namespace Sample;

public partial class SelectionButtonsPage : ContentPage
{
	public SelectionButtonsPage()
	{
		InitializeComponent();

        var list = new List<SelectedItem>
        {
            new()
            {
                Name = "Home",
			},
            new()
            {
                Name = "Account",
            },
            new()
            {
                Name = "Settings",
            },
            new()
            {
                Name = "About app",
            },
        };
        BindableLayout.SetItemsSource(stackLayout, list);
        CommandSelectedItem = new Command<SelectedItem>(ActionSelectedItem);
        CommandSelectedItem.Execute(list[0]);
    }

    private SelectedItem? lastSelectedItem;

    public ICommand CommandSelectedItem { get; set; }
    private void ActionSelectedItem(SelectedItem item)
    {
        if (lastSelectedItem != null)
            lastSelectedItem.IsSelected = false;

        item.IsSelected = true;

        lastSelectedItem = item;
    }
}