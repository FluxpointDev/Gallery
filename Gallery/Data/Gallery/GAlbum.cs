﻿using Gallery.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GAlbum
    {
        public int id;
        public string link = "";
        public string name = "";
        public bool isPublic = false;
        public bool isNsfw = false;
        public string owner = "";
        public int subAlbum = 0;
        public Dictionary<int, GAccess> roleAccess = new Dictionary<int, GAccess>();
        public void Add()
        {
            DB.R.Db("Gallery").Table("Albums").Insert(this).RunNoReply(DB.Con);
        }
        public void Update()
        {
            DB.R.Db("Gallery").Table("Albums").Get(id).Update(this).RunNoReply(DB.Con);
        }
    }
}