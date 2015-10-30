using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Overheads.Core;
using Overheads.Helpers;

namespace Overheads.ViewModels
{
    public class SettingsViewModel : Screen
    {
        private List<string> _bookNames;
        private string _path;
        private ListBoxItem _selectedItem;
        private int _index;

        public List<string> BookNames
        {
            get
            {
                return _bookNames;
            }

            set
            {
                if (Equals(value, _bookNames)) return;
                _bookNames = value;
                NotifyOfPropertyChange(() => BookNames);
            }
        }

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                if (Equals(value, _path)) return;
                _path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                if (Equals(value, _index)) return;
                _index = value;
                NotifyOfPropertyChange(() => Index);
            }
        }

        public ListBoxItem SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                if (Equals(value, _selectedItem)) return;
                _selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        protected override void OnActivate()
        {
            LoadNames();
        }

        public SettingsViewModel()
        {
            this.PrintCommand = new DelegateCommand(this.PrintGrid);
            this.BrowseCommand = new DelegateCommand(this.BrowseToDirectory);
            this.MoveCommand = new DelegateCommand(this.MoveBook);
        }

        public ICommand PrintCommand { get; private set; }
        public ICommand BrowseCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        private void LoadNames()
        {
            BookNames = new List<string>();
            
            foreach(Book book in BookManager.Books)
                BookNames.Add(book.Title);

            Core.Properties.Settings.Default.BookOrder = BookNames;
        }

        public void PrintGrid(object param)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == false)
                return;

            string documentTitle = "Test Document";
            Size pageSize = new Size(printDialog.PrintableAreaWidth, printDialog.PrintableAreaHeight);

            CustomDocumentPaginator paginator = new CustomDocumentPaginator(param as DataGrid, documentTitle, pageSize, new Thickness(30, 20, 30, 20));
            printDialog.PrintDocument(paginator, "Grid");
        }

        public void BrowseToDirectory(object param)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && Core.Properties.Settings.Default.Path != dlg.SelectedPath)
            {
                Path = dlg.SelectedPath;
                Core.Properties.Settings.Default.Path = Path;
                BookManager.Initialize();
                LoadNames();
            }
        }

        public void MoveBook(object param)
        {
            if (param as string == "Up")
            {
                if (Index > 0)
                {
                    BookManager.Books.Insert(Index - 1, BookManager.Books[Index]);
                    BookManager.Books.RemoveAt(Index + 1);
                }
            }
            else
            {
                if (Index + 1 < BookNames.Count)
                {
                    BookManager.Books.Insert(Index + 2, BookManager.Books[Index]);
                    BookManager.Books.RemoveAt(Index);
                }
            }
            LoadNames();
            Core.Properties.Settings.Default.BookOrder = BookNames;
        }
    }
}
