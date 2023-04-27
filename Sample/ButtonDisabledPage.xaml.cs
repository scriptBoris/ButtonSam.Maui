using System.Windows.Input;

namespace Sample;

public partial class ButtonDisabledPage
{
	public ButtonDisabledPage()
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
    private int count;

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