using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overheads.Core
{
    public class Verse : NotifyBase
    {
        public List<Line> AllLines { get; set; }
        public int VerseNumber { get; set; }

        private List<Line> _displayLines;
        public List<Line> DisplayLines
        {
            get { return _displayLines; }
            set
            {
                if (value == _displayLines) return;
                _displayLines = value;
                OnPropertyChanged("DisplayLines");
            }
        }

        private Line _firstLine;
        public Line FirstLine
        {
            get { return _firstLine; }
            set
            {
                if (value == _firstLine) return;
                _firstLine = value;
                OnPropertyChanged("FirstLine");
            }
        }

        private Line _firstLineChords;
        public Line FirstLineChords
        {
            get { return _firstLineChords; }
            set
            {
                if (value == _firstLineChords) return;
                _firstLineChords = value;
                OnPropertyChanged("FirstLineChords");
            }
        }

        public Verse(string verseText, bool showChords, int verseNumber)
        {
            DisplayLines = new List<Line>();
            AllLines = new List<Line>();

            VerseNumber = verseNumber;

            var lines = verseText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var lineText in lines)
            {
                AllLines.Add(new Line(lineText));
            }

            if(showChords)
            {
                ShowChords();
            }
            else
            {
                HideChords();
            }

            FirstLineChords = AllLines.FirstOrDefault(x => x.Type == LineType.Chord);
            FirstLine = AllLines.FirstOrDefault(x => x.Type == LineType.Text);
        }

        public void AddRepeatLine(int numberOfRepeats)
        {
            //We have already added the repeat line
            if (AllLines.Any(x => x.Type == LineType.Repeat))
            {
                return;
            }

            var repeat = "repeat";
            if(numberOfRepeats > 1)
            {
                repeat = string.Format("{0} {1}x", repeat, numberOfRepeats);
            }

            var line = new Line("(" + repeat + ")", LineType.Repeat);

            AllLines.Add(line);
        }

        public void SetupErrorVerse()
        {
            var line = new Line("There was an error loading the song");

            AllLines.Add(line);
            DisplayLines.Add(line);
            FirstLineChords = line;
            FirstLine = line;
        }

        public void ShowChords()
        {
            DisplayLines = new List<Line>(AllLines);
            FirstLineChords = AllLines.FirstOrDefault(x => x.Type == LineType.Chord);
            FirstLine = AllLines.FirstOrDefault(x => x.Type == LineType.Text);
        }

        public void HideChords()
        {
            DisplayLines = AllLines.Where(x => x.Type == LineType.Text || x.Type == LineType.Repeat).ToList();
            FirstLineChords = null;
            FirstLine = AllLines.FirstOrDefault(x => x.Type == LineType.Text);
        }
    }
}
