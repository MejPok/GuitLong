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


}
