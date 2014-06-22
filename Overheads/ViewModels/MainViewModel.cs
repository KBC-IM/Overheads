using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Overheads.Core;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Screen = Caliburn.Micro.Screen;

namespace Overheads.ViewModels
{
    public class MainViewModel : Screen
    {
        private BookManager _bookManager;
        private Song _currentSong;
        private List<SearchSong> _searchResults;
        private string _searchString;
        private int _currentSearchIndex;
        private SearchSong _selectedSearchSong;

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

        public SearchSong SelectedSearchSong
        {
            get { return _selectedSearchSong; }
            set
            {
                if (Equals(value, _selectedSearchSong)) return;
                _selectedSearchSong = value;
                NotifyOfPropertyChange(() => SelectedSearchSong);
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

        protected override void OnActivate()
        {
            _bookManager = new BookManager();
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

            var sr = _bookManager.SearchSongs(SearchString);

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

            CurrentSong = _bookManager.LoadSong(song);
            
            SearchResults = null;
            SelectedSearchSong = null;
            SearchString = null;
        }

        public void PreviewKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown(); 
                    break;
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
                    if (SearchString.Length > 0)
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
                case Key.F5:
                    CurrentSong.Refresh();
                    break;
                case Key.Back:
                    if (SearchString.Length > 0)
                    {
                        SearchString = SearchString.Substring(0, SearchString.Length - 1);    
                    }
                    break;
                case Key.LeftShift:
                case Key.RightShift:
                    //dont do anything for these keys
                    break;
                default:
                    SearchString = SearchString + KeyboardHelper.GetCharFromKey(e.Key);
                    break;
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
