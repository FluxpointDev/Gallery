using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GUser
    {
        public string id;
        public string name;
        public List<int> roles = new List<int>();
        public int uploadLimit = 0;
        // In mb
        public int sizeLimit = 0;
    }
}
