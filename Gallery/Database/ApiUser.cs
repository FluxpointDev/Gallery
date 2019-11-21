using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Database
{
    public class ApiUser
    {
        [JsonProperty("id")]
        public string ID;
        public string Name;
        public string Token;
        public DateTime Created;
        public DateTime? Expire = null;
        public bool Donator = false;
        public bool Owner = false;
        public bool Disabled = false;
        public string GalleryUser = "";
        public int GalleryUserID = 0;

    }
}
