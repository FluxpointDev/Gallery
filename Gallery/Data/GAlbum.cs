using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GAlbum
    {
        public int id;
        public string link;
        public string name;
        public bool isPublic = false;
        public bool isNsfw = false;
        public string owner = "";
        public Dictionary<int, GAccess> roleAccess = new Dictionary<int, GAccess>();
    }
}
