using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Overheads.Core
{
    public class BookManager
    {
        public List<Book> Books { get; set; }

        public BookManager()
        {
            GetBooks();
        }

        private void GetBooks()
        {
            Books = new List<Book>();
            var dir = Directory.GetCurrentDirectory() + "/books";
            var bookDirList = Directory.GetDirectories(dir);

            foreach (var bookdir in bookDirList)
            {
                var book = new Book
                    {
                        Key = bookdir
                    };

                Books.Add(book);
                LoadSongs(book.Key, book);
            }
        }

        private void LoadSongs(string path, Book book)
        {
            book.Songs = new List<SearchSong>();
            var songsList = Directory.GetFiles(path);

            foreach (var s in songsList)
            {
                var song = new SearchSong();
                song.Key = s;

                var stream = File.OpenRead(s);
                var sr = new StreamReader(stream);

                song.Title = sr.ReadLine() ?? "";//Title is always first line
                sr.ReadLine(); //skip the = sign
                song.FirstLine = sr.ReadLine() ?? "";

                sr.Close();
                stream.Close();

                book.Songs.Add(song);
            }
        }

        public Song LoadSong(SearchSong searchSong)
        {
            var song = new Song();

            var stream = File.OpenRead(searchSong.Key);
            var sr = new StreamReader(stream);
            
            var wholeSong = sr.ReadToEnd();
            song.SetSongText(wholeSong);

            sr.Close();
            stream.Close();

            return song;
        }

        public IEnumerable<SearchSong> SearchSongs(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return null;
            }

            return
                Books.SelectMany(x => x.Songs)
                .Where(x => x.FirstLine.ToUpper().Contains(searchString.ToUpper()) || x.Title.ToUpper().Contains(searchString.ToUpper()));
        }
    }
}
