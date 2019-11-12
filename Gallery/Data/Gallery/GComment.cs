using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GComment
    {
        public int id;
        public string author;
        public string text;
        public bool hidden = false;
        public bool anonymous = false;
        public DateTime date = DateTime.UtcNow;
    }
}
