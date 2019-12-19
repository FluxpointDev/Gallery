using System;

namespace Gallery.Data
{
    public class GComment
    {
        public string author;
        public string text = "";
        public bool hidden = false;
        public bool anonymous = false;
        public DateTime date = DateTime.UtcNow;
    }
}
