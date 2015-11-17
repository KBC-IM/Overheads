using System.Windows.Controls;
using AurelienRibon.Ui.SyntaxHighlightBox;

namespace Overheads.Views
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class EditView : UserControl
    {
        public EditView()
        {
            InitializeComponent();
            box.CurrentHighlighter = HighlighterManager.Instance.Highlighters["OverheadsSyntax"];
        }
    }
}
