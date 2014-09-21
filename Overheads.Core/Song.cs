using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Overheads.Core
{
    public class Song : NotifyBase
    {
        private string _firstLineOfNextVerse;
        private int _currentVerseIndex;
        private string _title;
        private List<Verse> _verses;
        private Verse _currentVerse;
        private bool _showCords;
        private string _songText;

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

        public List<Verse> Verses
        {
            get { return _verses; }
            set
            {
                if (Equals(value, _verses)) return;
                _verses = value;
                OnPropertyChanged("Verses");
            }
        }

        public Verse CurrentVerse
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

        public string SongText
        {
            get { return _songText; }
            set
            {
                if (value == _songText) return;
                _songText = value;
                OnPropertyChanged("SongText");
            }
        }

        public string Key { get; set; }

        public Song()
        {
            _currentVerseIndex = 0;
            Verses = new List<Verse>();
        }

        public void ToggleCords()
        {
            _showCords = !_showCords;
            SetVerse();
        }

        public void SetSongText(string text)
        {
            SongText = text;
            var songSections = SongText.Split('=');
            Title = songSections.First();
            var verseTexts = songSections.Skip(1).Take(songSections.Length - 1).ToList();

            foreach(var verseText in verseTexts)
            {
                var verse = new Verse();
                verse.Setup(verseText);

                Verses.Add(verse);
            }

            SetVerse();
        }

        public void SetVerse(int? overrideCurrentIndex = null)
        {
            try
            {
                if (overrideCurrentIndex != null)
                {
                    _currentVerseIndex = overrideCurrentIndex.Value;
                }

                CurrentVerse = Verses.ElementAt(_currentVerseIndex);
                
                if(_showCords)
                {
                    CurrentVerse.ShowChords();
                }
                else
                {
                    CurrentVerse.HideChords();
                }

                SetFirstLineOfNextVerse();
            }
            catch (Exception)
            {
                CurrentVerse = new Verse();
                CurrentVerse.SetupErrorVerse();
            }
            
        }

        private void SetFirstLineOfNextVerse()
        {
            var nextVerse = Verses.ElementAtOrDefault(_currentVerseIndex + 1);

            if (nextVerse != null)
            {
                var nextLine =  nextVerse.FirstLine;

                FirstLineOfNextVerse = nextLine.Text + "...";
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
    }
}
