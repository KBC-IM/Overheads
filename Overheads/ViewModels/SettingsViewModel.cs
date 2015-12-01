using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Caliburn.Micro;
using Overheads.Core;
using System.Windows;
using System.Windows.Xps.Packaging;
using System.IO;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

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

        public string BookPath
        {
            get
            {
                return _path;
            }

            set
            {
                if (Equals(value, _path)) return;
                _path = value;
                NotifyOfPropertyChange(() => BookPath);
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
            this.PrintIndexCommand = new DelegateCommand(this.PrintIndex);
            this.PrintSongCommand = new DelegateCommand(this.PrintSong);
            this.BrowseCommand = new DelegateCommand(this.BrowseToDirectory);
            this.MoveCommand = new DelegateCommand(this.MoveBook);
        }

        public ICommand PrintIndexCommand { get; private set; }
        public ICommand PrintSongCommand { get; private set; }
        public ICommand BrowseCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        private void LoadNames()
        {
            BookNames = new List<string>();
            
            foreach(Book book in BookManager.Books)
                BookNames.Add(book.Title);

            Core.Properties.Settings.Default.BookOrder = BookNames;
        }

        public void PrintIndex(object param)
        {
            PrintDoc(PrintableDocuments.IndexDocument(BookManager.Books), "Overheads Index");
        }

        public void PrintSong(object param)
        {
            if(BookManager.LastSong == null)
            {
                MessageBox.Show("A song is not selected.", "No Song Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                PrintDoc(PrintableDocuments.SongDocument(BookManager.LastSong), BookManager.LastSong.Title, true);
            }
        }

        public void PrintDoc(FlowDocument doc, string title, bool spanWidth = false)
        {
            PrintDialog printDlg = new PrintDialog();

            System.Printing.PrintDocumentImageableArea ia = null;
            System.Windows.Xps.XpsDocumentWriter docWriter = System.Printing.PrintQueue.CreateXpsDocumentWriter(ref ia);

            if (docWriter != null && ia != null)
            {
                DocumentPaginator paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;

                paginator.PageSize = new Size(ia.MediaSizeWidth, ia.MediaSizeHeight);
                doc.PageHeight = printDlg.PrintableAreaHeight;
                doc.PageWidth = printDlg.PrintableAreaWidth;
                Thickness t = new Thickness(72);
                doc.PagePadding = new Thickness(
                    Math.Max(ia.OriginWidth, t.Left),
                    Math.Max(ia.OriginHeight, t.Top),
                    Math.Max(ia.MediaSizeWidth - (ia.OriginWidth + ia.ExtentWidth), t.Right),
                    Math.Max(ia.MediaSizeHeight - (ia.OriginHeight + ia.ExtentHeight), t.Bottom));
                if(spanWidth)
                    doc.ColumnWidth = double.PositiveInfinity;

                docWriter.Write(paginator);
            }
            else
            {
                MessageBox.Show("No data is available to print.", "No Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void BrowseToDirectory(object param)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dlg.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK && Core.Properties.Settings.Default.Path != dlg.SelectedPath)
            {
                BookPath = dlg.SelectedPath;
                Core.Properties.Settings.Default.Path = BookPath;
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
