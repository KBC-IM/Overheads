using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public bool IsChords
        {
            get { return Type == LineType.Chord; }
        }
    }
}
