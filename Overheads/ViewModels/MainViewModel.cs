using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Overheads.Core;
using Overheads.Helpers;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Screen = Caliburn.Micro.Screen;

namespace Overheads.ViewModels
{
    public class MainViewModel : Screen
    {
        private Song _currentSong;
        private List<SearchSong> _searchResults;
        private string _searchString;
        private int _currentSearchIndex;
        private SearchSong _selectedSearchSong;
        private ScreenSettings _screenSettings;
        private Book _currentBook;

        public bool IsSearching
        {
            get { return string.IsNullOrEmpty(SearchString) == false; }
        }

        public int CurrentSearchIndex
        {
            get
            {
                if (SearchResults != null && SearchResults.Any())
                {
                    return _currentSearchIndex + 1;
                }

                return 0;
            }
        }

        public ScreenSettings ScreenSettings
        {
            get { return _screenSettings; }
            set
            {
                if (Equals(value, _screenSettings)) return;
                _screenSettings = value;
                NotifyOfPropertyChange(() => ScreenSettings);
            }
        }

        public Song CurrentSong
        {
            get { return _currentSong; }
            set
            {
                if (Equals(value, _currentSong)) return;
                _currentSong = value;
                
                NotifyOfPropertyChange(() => CurrentSong);
            }
        }

        public Book CurrentBook
        {
            get { return _currentBook; }
            set
            {
                if (Equals(value, _currentBook)) return;
                _currentBook = value;
                NotifyOfPropertyChange(() => CurrentBook);
            }
        }

        public SearchSong SelectedSearchSong
        {
            get { return _selectedSearchSong; }
            set
            {
                if (Equals(value, _selectedSearchSong)) return;
                _selectedSearchSong = value;
                NotifyOfPropertyChange(() => SelectedSearchSong);
                NotifyOfPropertyChange(() => CurrentSearchIndex);
            }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (value == _searchString) return;
                _searchString = value;
                SearchSongs();
                NotifyOfPropertyChange(() => SearchString);
                NotifyOfPropertyChange(() => IsSearching);
            }
        }

        public int? SearchResultsCount
        {
            get
            {
                if (SearchResults != null)
                {
                    return SearchResults.Count();
                }

                return null;
            }
        }

        public List<SearchSong> SearchResults
        {
            get { return _searchResults; }
            set
            {
                if (value == _searchResults) return;
                _searchResults = value;
                NotifyOfPropertyChange(() => SearchResults);
                NotifyOfPropertyChange(() => SearchResultsCount);
            }
        }

        public MainViewModel()
        {
            ScreenSettings = new ScreenSettings();
        }

        protected override void OnActivate()
        {
            try
            {
                BookManager.Initialize();
            }
            catch (Exception)
            {
                CurrentSong = new Song("We have a problem", "NoKey");
            }
        }

        public void SearchSongs()
        {
            if (string.IsNullOrEmpty(SearchString))
            {
                SearchResults = null;
                _currentSearchIndex = 0;
                SelectedSearchSong = null;
                return;
            }

            var bookKey = CurrentBook != null ? CurrentBook.Key : null;

            var sr = BookManager.SearchSongs(SearchString, bookKey);

            if (sr != null)
            {
                SearchResults = sr.ToList();
                SelectedSearchSong = SearchResults.FirstOrDefault();
                _currentSearchIndex = 0;
            }
        }

        public void SetSong(SearchSong song)
        {
            if (song == null)
            {
                return;
            } 

            CurrentSong = BookManager.LoadSong(song.Key);
            CurrentSong.BookNumber = song.BookNumber;

            SearchResults = null;
            SelectedSearchSong = null;
            SearchString = null;
        }

        public void OnKeyPress(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (CurrentSong != null)
                    {
                        CurrentSong.PreviousVerse();
                    }
                    break;
                case Key.Right:
                    if (CurrentSong != null)
                    {
                        CurrentSong.NextVerse();
                    }
                    break;
                case Key.Space:
                    if (SearchString != null && SearchString.Length > 0)
                    {
                        SearchString = SearchString + " ";
                    }
                    else
                    {
                        CurrentSong = null;
                    }
                    break;
                case Key.Enter:
                    SetSong(SelectedSearchSong);
                    break;
                case Key.Up:
                    PreviousSearchResult();
                    break;
                case Key.Down:
                    NextSearchResult();
                    break;
                case Key.F9:
                    if (CurrentSong != null)
                    {
                        CurrentSong.Refresh();
                    }
                    break;
                case Key.Back:
                    if (SearchString.Length > 0)
                    {
                        SearchString = SearchString.Substring(0, SearchString.Length - 1);    
                    }
                    break;
                case Key.OemMinus:
                    ScreenSettings.InvertColors();
                    break;
                case Key.OemPlus:
                    if (CurrentSong != null)
                    {
                        CurrentSong.ToggleCords();
                    }
                    break;
                case Key.F1:
                    SetCurrentBook(1);
                    break;
                case Key.F2:
                    SetCurrentBook(2);
                    break;
                case Key.F3:
                    SetCurrentBook(3);
                    break;
                case Key.F4:
                    SetCurrentBook(4);
                    break;
                case Key.F5:
                    SetCurrentBook(5);
                    break;
                case Key.F6:
                    SetCurrentBook(6);
                    break;

                case Key.LeftShift:
                case Key.RightShift:
                case Key.Tab:
                    //dont do anything for these keys
                    break;
                default:
                    SearchString = SearchString + KeyboardHelper.GetCharFromKey(e.Key);
                    break;
            }

            e.Handled = true;
        }

        public void SetCurrentBook(int sequence)
        {
            if (BookManager.Books.Count >= sequence)
            {
                var potentialBook = BookManager.Books[sequence - 1];

                if (potentialBook == CurrentBook)
                {
                    CurrentBook = null;
                }
                else
                {
                    CurrentBook = potentialBook;
                }
            } 
        }

        public void NextSearchResult()
        {
            if (SearchResults == null || !SearchResults.Any()) return;

            if (_currentSearchIndex < SearchResults.Count() - 1)
            {
                _currentSearchIndex++;
            }

            SelectedSearchSong = SearchResults.ElementAt(_currentSearchIndex);
        }

        public void PreviousSearchResult()
        {
            if (SearchResults == null || !SearchResults.Any()) return;

            if (_currentSearchIndex > 0)
            {
                _currentSearchIndex--;
            }

            SelectedSearchSong = SearchResults.ElementAt(_currentSearchIndex);
        }
    }
}
