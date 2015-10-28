using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Overheads.Core
{
    public class Line : NotifyBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (value == _text) return;
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public LineType Type { get; set; }

        public bool IsNotText
        {
            get { return Type == LineType.Chord || Type == LineType.Repeat; }
        }

        public Line(string lineText, LineType? overrideLineType = null)
        {
            var trimmedLineText = lineText.TrimEnd();

            if (overrideLineType != null)
            {
                Type = overrideLineType.Value;
                Text = lineText;
            }
            //The % sign at the end of the line is a convention 
            //that indicates that the line is a set of chords
            else if (trimmedLineText.EndsWith("%"))
            {
                Type = LineType.Chord;

                int lastLocation = trimmedLineText.LastIndexOf("%");

                if(lastLocation >= 0)
                    Text = trimmedLineText.Substring(0, lastLocation);

                int count = 0;

                for (int c = 0; c < Text.Length; c += 2)
                {
                    if (Char.IsWhiteSpace(Text[c]) && c > 0 && !Char.IsWhiteSpace(Text[c-1]))
                    {
                        count = 0;
                    }
                    Text = Text.Insert(c - count, " ");

                    count += 1;
                }
            }
            else
            {
                Type = LineType.Text;
                Text = trimmedLineText;
            }
        }
            
    }
}
