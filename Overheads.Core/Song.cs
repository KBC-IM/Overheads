using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Overheads.Core
{
    public class Song : INotifyPropertyChanged
    {
        private string _firstLineOfNextVerse;
        private int _currentVerseIndex;
        private string _title;
        private List<string> _verses;
        private string _currentVerse;
        private string _songText;
        private bool _showCords;
        private string _fullVerse;
        private string _verseWithoutChords;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public List<string> Verses
        {
            get { return _verses; }
            set
            {
                if (Equals(value, _verses)) return;
                _verses = value;
                OnPropertyChanged("Verses");
            }
        }

        public string CurrentVerse
        {
            get { return _currentVerse; }
            set
            {
                if (value == _currentVerse) return;
                _currentVerse = value;
                OnPropertyChanged("CurrentVerse");
            }
        }

        public string FirstLineOfNextVerse
        {
            get { return _firstLineOfNextVerse; }
            set
            {
                if (value == _firstLineOfNextVerse) return;
                _firstLineOfNextVerse = value;
                OnPropertyChanged("FirstLineOfNextVerse");
            }
        }

        public Song()
        {
            _currentVerseIndex = 0;
        }

        public void ToggleCords()
        {
            _showCords = !_showCords;
            SetVerse();
        }

        public void SetSongText(string text)
        {
            _songText = text;
            var songSections = _songText.Split('=');
            Title = songSections.First();
            Verses = songSections.Skip(1).Take(songSections.Length - 1).ToList();
            SetVerse();
        }

        public void SetVerse(int? overrideCurrentIndex = null)
        {
            if (overrideCurrentIndex != null)
            {
                _currentVerseIndex = overrideCurrentIndex.Value;
            }

            _fullVerse = Verses.ElementAt(_currentVerseIndex);

            if (_showCords)
            {
                CurrentVerse = _fullVerse;
            }
            else
            {
                var lines = _fullVerse.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                _verseWithoutChords = "";
                foreach (var line in lines)
                {
                    if (!line.Contains("%"))
                    {
                        _verseWithoutChords += line + Environment.NewLine;
                    }
                }

                CurrentVerse = _verseWithoutChords;
            }

            SetFirstLineOfNextVerse();
        }

        private void SetFirstLineOfNextVerse()
        {
            var nextVerse = Verses.ElementAtOrDefault(_currentVerseIndex + 1);

            if (nextVerse != null)
            {
                var nextVerseLines = nextVerse.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                var nextLine = nextVerseLines.First();
                if (nextLine.Contains("%"))
                {
                    nextLine = nextVerseLines.Skip(1).First();
                }

                FirstLineOfNextVerse = nextLine + "...";
            }
            else
            {
                FirstLineOfNextVerse = "";
            }  
        }

        public void PreviousVerse()
        {
            if (_currentVerseIndex > 0)
            {
                _currentVerseIndex--;
            }

            SetVerse();
        }

        public void NextVerse()
        {
            if (_currentVerseIndex < Verses.Count() - 1)
            {
                _currentVerseIndex++;
            }

            SetVerse();
        }

        public void Refresh()
        {
            SetVerse(0);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
