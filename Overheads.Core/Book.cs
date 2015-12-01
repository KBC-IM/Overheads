using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Overheads.Core
{
    public class Book : NotifyBase
    {
        private string _title;
        public string Key { get; set; }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public List<SearchSong> Songs { get; set; }
    }
}
