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
        public DateTime date = DateTime.UtcNow;
        public string hash;
        public int width;
        public int height;
        public int filesize;
    }
}
