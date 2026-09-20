using System;
using System.Collections.Generic;
using System.Text;

namespace GuitLong.Code.Models
{
    public class Section
    {
        public string Name { get; set; } = "";
        public List<Row> Rows { get; set; } = new List<Row>();
    }
    public class Row 
    { 
        public List<Chord> Chords { get; set; } = new List<Chord>();
        public string Lyrics { get; set; } = "";
    }

    public class Chord
    {
        public int CharacterPosition = 0;
        public string Name { get; set; } = "";
    }

    public class StrummingPattern
    {
        public List<Strum> Strums { get; set; } = new();
    }

    public class Strum
    {
        public StrumDirection Direction { get; set; }
        public double Beat { get; set; }
    }

    public enum StrumDirection
    {
        Down,
        Up,
        None
    }


}
