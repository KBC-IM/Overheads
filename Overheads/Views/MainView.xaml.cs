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
            Loaded += (sender, e) =>
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
}
