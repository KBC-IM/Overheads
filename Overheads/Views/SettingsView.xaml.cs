using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Overheads.Core;
using Overheads.Properties;

namespace Overheads.Views
{
    /// <summary>
    /// Interaction logic for EditView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public List<string> BookNames;

        public SettingsView()
        {
            InitializeComponent();
        }
        
        private void btnMoveUp_Click(object sender, RoutedEventArgs e)
        {
            if (lbxBookOrder.SelectedIndex > 0)
            {
                BookManager.Books.Insert(lbxBookOrder.SelectedIndex - 1, BookManager.Books[lbxBookOrder.SelectedIndex]);
                BookManager.Books.RemoveAt(lbxBookOrder.SelectedIndex + 1);
                LoadNames();
            }

            Core.Properties.Settings.Default.BookOrder = BookNames;
        }

        private void btnMoveDown_Click(object sender, RoutedEventArgs e)
        {
            if(lbxBookOrder.SelectedIndex + 1 < lbxBookOrder.Items.Count && lbxBookOrder.SelectedItem != null)
            {
                BookManager.Books.Insert(lbxBookOrder.SelectedIndex + 2, BookManager.Books[lbxBookOrder.SelectedIndex]);
                BookManager.Books.RemoveAt(lbxBookOrder.SelectedIndex);
                LoadNames();
            }

            Core.Properties.Settings.Default.BookOrder = BookNames;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadNames();
        }

        private void LoadNames()
        {
            BookNames = new List<string>();

            for (int i = 0; i < BookManager.Books.Count; i++)
                BookNames.Add(BookManager.Books[i].Title);

            lbxBookOrder.ItemsSource = BookNames;

            Core.Properties.Settings.Default.BookOrder = BookNames;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cbxThemeShortcut.IsChecked = Settings.Default.ThemeShortcutEnabled;
            cbxFullScreen.IsChecked = Settings.Default.Fullscreen;
            cbxMaximizeToSecondary.IsChecked = Settings.Default.MaximizeToSecondary;
            cbxMaximizeToSecondary.IsEnabled = (bool)cbxFullScreen.IsChecked;
            cbxHideCursor.IsChecked = Settings.Default.HideCursor;
            tbxSongsDirectory.Text = Core.Properties.Settings.Default.Path;
        }

        private void checkBox_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ThemeShortcutEnabled = (bool)cbxThemeShortcut.IsChecked;
        }

        private void cbxFullScreen_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Fullscreen = (bool)cbxFullScreen.IsChecked;
            cbxMaximizeToSecondary.IsEnabled = (bool)cbxFullScreen.IsChecked;
        }

        private void cbxMaximizeToSecondary_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.MaximizeToSecondary = (bool)cbxMaximizeToSecondary.IsChecked;
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void cbxHideCursor_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.HideCursor = (bool)cbxHideCursor.IsChecked;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var browseDialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = browseDialog.ShowDialog();

            if(result == System.Windows.Forms.DialogResult.OK)
            {
                tbxSongsDirectory.Text = browseDialog.SelectedPath;
                Core.Properties.Settings.Default.Path = tbxSongsDirectory.Text;
                BookManager.Initialize();
            }
        }
    }
}