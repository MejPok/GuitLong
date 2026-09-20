using System;
using System.Collections.Generic;
using System.Text;

namespace GuitLong.Code.Models
{
    public class Song
    {
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";

        public string Capo { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
