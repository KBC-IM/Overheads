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
        private string _bookNumber;
        private List<Verse> _verses;
        private Verse _currentVerse;
        private bool _showCords;
        private string _songText;
        private List<OrderItem> _order;
        private string _language;

        public string BookNumber
        {
            get { return _bookNumber; }
            set
            {
                if (value == _bookNumber) return;
                _bookNumber = value;
                OnPropertyChanged("BookNumber");
            }
        }

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

        public Song(string text, string key)
        {
            Key = key;
            _currentVerseIndex = 0;
            Verses = new List<Verse>();

            SongText = text;
            var songSections = SongText.Split('=');

            ProcessHeader(songSections.First());
            
            var verseTexts = songSections.Skip(1).Take(songSections.Length - 1).ToList();
            var verseNumber = 1;

            foreach (var verseText in verseTexts)
            {
                Verses.Add(new Verse(verseText, _showCords, verseNumber));
                verseNumber++;
            }

            if (_order == null)
            {
                _order = Verses.Select(x => new OrderItem
                {
                    VerseNumber = x.VerseNumber,
                    RepeatCount = 0
                }).ToList();
            }

            SetVerse();
        }

        public void ToggleCords()
        {
            _showCords = !_showCords;
            SetVerse();
        }

        private void ProcessHeader(string header)
        {
            var headerParts = header.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            if(headerParts.Count() == 1)
            {
                Title = headerParts.First();
            }
            else
            {
                foreach(var part in headerParts)
                {
                    var keyValue = part.Split(':');
                    if(keyValue[0].ToLower() == "title")
                    {
                        Title = keyValue[1];
                    }
                    else if (keyValue[0].ToLower() == "order")
                    {
                        PreprocessOrder(keyValue[1]);
                    }
                    else if (keyValue[0].ToLower() == "language")
                    {
                        _language = keyValue[1];
                    }
                }
            }
        }

        private void PreprocessOrder(string orderString)
        {
            _order = new List<OrderItem>();
            var orderList = orderString.Split(',');
            OrderItem currentOrderItem = null;
            
            foreach(var orderItem in orderList)
            {
                var intValue = Int32.Parse(orderItem);

                if (currentOrderItem != null && currentOrderItem.VerseNumber == intValue)
                {
                    currentOrderItem.RepeatCount++;
                }
                else
                {
                    currentOrderItem = new OrderItem
                    {
                        VerseNumber = intValue,
                        RepeatCount = 0
                    };

                    _order.Add(currentOrderItem);
                }
            }
        }

        public void SetVerse(int? overrideCurrentIndex = null)
        {
            try
            {
                if (overrideCurrentIndex != null)
                {
                    _currentVerseIndex = overrideCurrentIndex.Value;
                }

                var orderItem = _order.ElementAt(_currentVerseIndex);

                CurrentVerse = Verses.FirstOrDefault(x => x.VerseNumber == orderItem.VerseNumber);

                if (orderItem.RepeatCount > 0)
                {
                    CurrentVerse.AddRepeatLine(orderItem.RepeatCount);
                }
                
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
            catch (Exception e)
            {
                CurrentVerse = new Verse("", true, 0);
                CurrentVerse.SetupErrorVerse();
            }
            
        }

        private void SetFirstLineOfNextVerse()
        {
            var orderItem = _order.ElementAtOrDefault(_currentVerseIndex + 1);

            if(orderItem != null)
            {
                

                var nextVerse = Verses.FirstOrDefault(x => x.VerseNumber == orderItem.VerseNumber);

                var nextLine = nextVerse.FirstLine;

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
            if (_currentVerseIndex < _order.Count() - 1)
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
