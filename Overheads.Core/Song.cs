using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Overheads.Core
{
    public class Song : NotifyBase
    {
        private List<Line> _firstLineOfNextVerse;
        private int _currentVerseIndex;
        private string _title;
        private string _subtitle;
        private string _bookNumber;
        private List<Verse> _verses;
        private string _chords;
        private string _topic;
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

        public string Subtitle
        {
            get { return _subtitle; }
            set
            {
                if (value == _subtitle) return;
                _subtitle = value;
                OnPropertyChanged("Subtitle");
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

        public string Chords
        {
            get { return _chords; }
            set
            {
                if (Equals(value, _chords)) return;
                _chords = value;
                OnPropertyChanged("Chords");
            }
        }

        public string Topic
        {
            get { return _topic; }
            set
            {
                if (Equals(value, _topic)) return;
                _topic = value;
                OnPropertyChanged("Topic");
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

        public List<Line> FirstLineOfNextVerse 
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

        public bool ChordsVisible { get { return _showCords; } }

        public string Key { get; set; }

        public Song(string text, string key)
        {
            Key = key;
            _currentVerseIndex = 0;
            Verses = new List<Verse>();

            SongText = text;
            var songSections = SongText.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

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

            SetChords();
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

            foreach (var part in headerParts)
            {

                var keyValue = part.Split(new char[] { ':' }, 2);
                if (keyValue[0].ToLower() == "title")
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
                else if (keyValue[0].ToLower() == "chords")
                {
                    _chords = keyValue[1];
                }
                else if (keyValue[0].ToLower() == "topic")
                {
                    _topic = keyValue[1];
                }
                else if (keyValue[0].ToLower() == "subtitle")
                {
                    Subtitle = keyValue[1];
                }
                else
                {
                    Title = part;
                }

            }
        }

        private void PreprocessOrder(string orderString)
        {
            _order = new List<OrderItem>();
            var orderList = orderString.Split(new Char[] { ',', ';' });
            OrderItem currentOrderItem = null;
            
            foreach(var orderItem in orderList)
            {
                var intValue = 0;
                bool result = Int32.TryParse(orderItem, out intValue);

                if (result)
                {

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
                else
                {
                    Console.WriteLine("Failed Line");
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
                
                if (_showCords)
                {
                    CurrentVerse.ShowChords();
                }
                else
                {
                    CurrentVerse.HideChords();
                }
                SetFirstLineOfNextVerse(_showCords);
            }
            catch (Exception)
            {
                CurrentVerse = new Verse("", true, 0);
                CurrentVerse.SetupErrorVerse();
            }
        }

        private void SetFirstLineOfNextVerse(bool showChords)
        {
            var orderItem = _order.ElementAtOrDefault(_currentVerseIndex + 1);
            
            List<Line> nextLines = new List<Line>();

            if (orderItem != null)
            {
                var nextVerse = Verses.FirstOrDefault(x => x.VerseNumber == orderItem.VerseNumber);
                for (int i = 0; i < nextVerse.FirstLine.Count; i++)
                {
                    if (nextVerse.FirstLine[i] != null)
                    {
                        if (nextVerse.FirstLine[i].Type == LineType.Text)
                            nextLines.Add(new Line(nextVerse.FirstLine[i].Text + "...", LineType.Text));
                        else if (showChords)
                            nextLines.Add(new Line(nextVerse.FirstLine[i].Text, LineType.Chord));
                    }
                }
            }

            FirstLineOfNextVerse = nextLines;
        }

        private void SetChords()
        {
            Chords = "";
            foreach (Verse verse in Verses)
            {
                foreach (Line line in verse.AllLines)
                {
                    if (line.IsNotText)
                        Chords += line.Text + " ";
                }
            }
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            Chords = regex.Replace(Chords, @" ");

            string[] chords = Chords.Split(new string[] { " ", "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            chords = chords.Distinct().ToArray();

            Chords = "";

            foreach(string chord in chords)
            {
                if(!chord.Contains("(") || !chord.Contains(")"))
                    Chords += chord + " ";
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