﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GTag
    {
        public int id;
        public string link;
        public string name;
        public bool isNsfw = false;
        public bool isCharacter = false;
        public string description = "";
    }
}
