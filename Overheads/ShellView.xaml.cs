using System.Windows;
using System.Windows.Input;

namespace Overheads
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            //WPF does not have a focused Item when it first starts up. This puts focus on the first element.
            //This happens to be the main grid which has the key capture logic. Thus we are able to capture the keys.
            Loaded += (sender, e) =>
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

            SourceInitialized += ShellViewModel.OnSourceInitialized;
            Closing += ShellViewModel.OnWindowClosing;
            KeyDown += ShellViewModel.OnKeyDown;
        }
    }
}
