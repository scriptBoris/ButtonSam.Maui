using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Custom
{
    public class SelectionButton : ButtonSam.Maui.Button
    {
        public SelectionButton()
        {
            BackgroundColor = Colors.Transparent;
        }

        public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(
            nameof(IsSelected),
            typeof(bool), 
            typeof(SelectionButton),
            false,
            propertyChanged: (b,o,n) =>
            {
                if (b is SelectionButton self)
                {
                    if ((bool)n)
                    {
                        self.BackgroundColor = Colors.MediumBlue;
                    }
                    else
                    {
                        self.BackgroundColor = Colors.Transparent;
                    }
                }
            }
        );
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }
    }
}
