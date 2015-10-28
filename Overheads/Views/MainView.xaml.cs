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
        }

        private void MainGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            if(Properties.Settings.Default.HideCursor)
                Mouse.OverrideCursor = Cursors.None;
        }

        private void MainGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
