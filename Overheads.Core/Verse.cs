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
        
        public Verse()
        {
            DisplayLines = new List<Line>();
            AllLines = new List<Line>();
        }

        public void Setup(string verseText)
        {
            var lines = verseText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var lineText in lines)
            {
                var line = new Line();
                var trimmedLineText = lineText.Trim();

                if (trimmedLineText.EndsWith("%"))
                {
                    line.Type = LineType.Chord;

                    line.Text = trimmedLineText.Substring(0, trimmedLineText.Length - 1);
                }
                else
                {
                    line.Type = LineType.Text;
                    line.Text = trimmedLineText;
                }

                AllLines.Add(line);
            }

            HideChords();
            FirstLine = AllLines.First(x => x.Type == LineType.Text);
        }

        public void SetupErrorVerse()
        {
            var line = new Line();
            line.Type = LineType.Text;
            line.Text = "There was an error loading the song";

            AllLines.Add(line);
            DisplayLines.Add(line);
            FirstLine = line;
        }

        public void ShowChords()
        {
            DisplayLines = new List<Line>(AllLines);
        }

        public void HideChords()
        {
            DisplayLines = AllLines.Where(x => x.Type == LineType.Text).ToList();
        }
    }
}
