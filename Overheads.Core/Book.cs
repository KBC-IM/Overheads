using System.Collections.Generic;

namespace Overheads.Core
{
    public class Book
    {
        public string Key { get; set; }
        public string Title { get; set; }
        public List<SearchSong> Songs { get; set; } 
    }
}
