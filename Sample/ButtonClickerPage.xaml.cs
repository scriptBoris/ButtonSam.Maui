using System.Windows.Input;

namespace Sample;

public partial class ButtonClickerPage : ContentPage
{
	private int count = 0;

	public ButtonClickerPage()
	{
		InitializeComponent();
		BindingContext = this;
	}

	public ICommand CommandTap => new Command(() =>
	{
		count++;
		Test = $"click counter: {count}";
	});

	private string _test = "click counter: 0";
	public string Test
	{
		get => _test;
		set
		{
			_test = value;
			OnPropertyChanged(nameof(Test));
		}
	}
}