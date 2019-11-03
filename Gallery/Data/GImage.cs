using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GImage
    {
        public int id;
        public string author;
        public List<int> tags = new List<int>();
    }
}
