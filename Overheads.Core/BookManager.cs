using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Overheads.Core
{
    public static class BookManager
    {
        public static List<Book> Books { get; set; }

        public static void Initialize()
        {
            Books = new List<Book>();
            var dir = Directory.GetCurrentDirectory() + "/books";
            var bookDirList = Directory.GetDirectories(dir);

            foreach (var bookdir in bookDirList)
            {
                var book = new Book
                    {
                        Key = bookdir,
                        Title = Path.GetFileName(bookdir)
                    };

                Books.Add(book);
                LoadSongs(book.Key, book);
            }
        }

        private static void LoadSongs(string path, Book book)
        {
            book.Songs = new List<SearchSong>();
            var songsList = Directory.GetFiles(path);

            foreach (var s in songsList)
            {
                var song = new SearchSong();
                song.Key = s;
                string fileName = Path.GetFileName(s);
                if (fileName != null && fileName.Length >= 6)
                {
                    song.Book = fileName.Substring(0, 3);
                    song.Number = fileName.Substring(3, 3);
                }

                var stream = File.OpenRead(s);
                var sr = new StreamReader(stream);

                song.Title = sr.ReadLine() ?? ""; //Title is always first line
                sr.ReadLine(); //skip the = sign
                song.FirstLine = sr.ReadLine() ?? "";

                sr.Close();

                book.Songs.Add(song);
            }
        }

        public static Song LoadSong(string key)
        {
            var song = new Song();

            var stream = File.OpenRead(key);
            var sr = new StreamReader(stream);
            
            var wholeSong = sr.ReadToEnd();
            song.Key = key;
            song.SetSongText(wholeSong);

            sr.Close();

            return song;
        }

        public static void SaveSong(Song song) 
        {
            var stream = File.OpenWrite(song.Key);
            var sw = new StreamWriter(stream);
            sw.Write(song.SongText);

            sw.Close();
        }

        public static IEnumerable<SearchSong> SearchSongs(string searchString, string bookKey = null)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return null;
            }

            var query = Books.SelectMany(x => x.Songs);

            if (bookKey != null)
            {
                query = Books.Where(x => x.Key == bookKey).SelectMany(x => x.Songs);
            }

            var ex = new Regex("[0-9]");
            if (ex.IsMatch(searchString))
            {
                return query.Where(x => x.Number.StartsWith(searchString)).OrderBy(x => x.Number);
            }
                
            return query.Where(x => x.FirstLine.ToUpper().Contains(searchString.ToUpper()) || x.Title.ToUpper().Contains(searchString.ToUpper()));
        }
    }
}
