
namespace Overheads.Core
{
    public class SearchSong
    {
        public string Title { get; set; }
        public string FirstLine { get; set; }
        public string Key { get; set; }
        public string Number { get; set; }
        public string Book { get; set; }

        public string BookNumberAndTitle
        {
            get { return string.Format("{0} {1} - {2} - ", Book, Number, Title); }
        }

        public string BookNumber
        {
            get { return string.Format("{0} {1}", Book, Number); }
        }
    }
}
