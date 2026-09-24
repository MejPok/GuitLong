using System;
using System.Collections.Generic;
using System.Text;

namespace GuitLong.Code.Models
{
    public class Song
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";

        public int? Capo { get; set; }
        public string Difficulty { get; set; } = "";
        public string Tuning { get; set; } = "";

        public List<Section> Sections { get; set; } = new List<Section>();
    }
}
