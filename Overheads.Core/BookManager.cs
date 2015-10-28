using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Overheads.Core.Properties;

namespace Overheads.Core
{
    public static class BookManager
    {
        public static List<Book> Books { get; set; }

        public static void Initialize()
        {
            Books = new List<Book>();
            var dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/books";
            if(Settings.Default.Path == "")
                Settings.Default.Path = dir;
            var bookDirList = Directory.GetDirectories(Settings.Default.Path);

            foreach (var bookdir in bookDirList)
            {
                var book = new Book
                    {
                        Key = bookdir,
                        Title = Path.GetFileName(bookdir)
                    };
                if (Settings.Default.BookOrder != null && Settings.Default.BookOrder.Contains(book.Title) && Books.Count > Settings.Default.BookOrder.IndexOf(book.Title))
                {
                    Books.Insert(Settings.Default.BookOrder.IndexOf(book.Title), book);
                }
                else
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
                if (Path.GetExtension(s).ToLower() == ".txt")
                {
                    var song = new SearchSong();
                    song.Key = s;
                    string fileName = Path.GetFileName(s);
                    if (fileName != null && fileName.Length >= 6)
                    {
                        song.Book = fileName.Substring(0, 3);
                        song.Number = fileName.Substring(3, 3).TrimStart(new char[] { '0' });
                    }

                    var stream = File.OpenRead(s);
                    var sr = new StreamReader(stream);

                    song.Title = GetTitle(sr);
                    sr.ReadLine(); //skip the = sign
                    song.FirstLine = sr.ReadLine() ?? "";
                    // If the first line has chords skip to the next line

                    bool pastHeader = false;
                    while (!pastHeader)
                    {
                        if (song.FirstLine.TrimEnd().EndsWith("%") || song.FirstLine.Contains("=") || song.FirstLine.Contains(":") || String.IsNullOrWhiteSpace(song.FirstLine))
                            song.FirstLine = sr.ReadLine() ?? "";
                        else
                            pastHeader = true;
                    }

                    sr.Close();

                    book.Songs.Add(song);
                }
            }
        }
        
        private static string GetTitle(StreamReader sr)
        {
            var potentialTitle = sr.ReadLine() ?? "";
            var splitTitle = potentialTitle.Split(':');
            
            if (splitTitle.Count() == 2)
            {
                var headerValue = splitTitle[0].ToLower();

                //The : defines header values but a colon can also exist in the title
                //So we need to check if we are getting header values or not
                if( headerValue == HeaderValues.Title)
                {
                    return splitTitle[1];
                }
                else if (HeaderValues.IsHeaderValue(headerValue))
                {
                    return GetTitle(sr);
                }
                else
                {
                    return potentialTitle;
                }
            }
            else
            {
                return potentialTitle;
            }
        }

        public static Song LoadSong(string key)
        {
            var stream = File.OpenRead(key);
            var sr = new StreamReader(stream);
            
            var wholeSong = sr.ReadToEnd();
            var song = new Song(wholeSong, key);

            sr.Close();

            return song;
        }

        public static void SaveSong(Song song) 
        {
            bool needsLoaded = false;
            if (!File.Exists(song.Key))
                needsLoaded = true;

            var stream = File.OpenWrite(song.Key);
            var sw = new StreamWriter(stream);
            sw.Write(song.SongText);

            sw.Close();

            if (needsLoaded)
                Initialize();
        }

        public static void NewSong(Song song)
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

            var ex = new Regex("^[0-9]+$");
            if (ex.IsMatch(searchString))
            {
                return query.Where(x => x.Number.StartsWith(searchString)).OrderBy(x => x.Number);
            }
                
            return query.Where(x => x.FirstLine.ToUpper().Contains(searchString.ToUpper()) || x.Title.ToUpper().Contains(searchString.ToUpper()));
        }
    }
}
