using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overheads.Core
{
    public static class HeaderValues
    {
        public static string Title = "title";
        public static string Order = "order";
        public static string Chords = "chords";
        public static string Photo = "photo";
        public static string Language = "language";
        public static string Subtitle = "subtitle";

        public static bool IsHeaderValue(string value)
        {
            return value == Title ||
                   value == Order ||
                   value == Chords ||
                   value == Photo ||
                   value == Language ||
                   value == Subtitle;
        }
    }
}
