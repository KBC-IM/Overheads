using System.Windows.Controls;
using System.Windows.Input;

namespace Overheads.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

            //WPF does not have a focused Item when it first starts up. This puts focus on the first element.
            //This happens to be the main grid which has the key capture logic. Thus we are able to capture the keys.
            Loaded += (sender, e) =>
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
