using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.FileReader;

namespace Gallery
{
    public class GFileInfo
    {
        public GFileInfo(IFileInfo fi)
        {
            Name = fi.Name;
            Size = fi.Size;
            Type = fi.Type;
            LastModified = fi.LastModified;
            LastModifiedDate = fi.LastModifiedDate;
        }
        public string Name;
        public long Size;
        public string Type;
        public long? LastModified;
        public DateTime? LastModifiedDate;
        public FileStatus Status = GFileInfo.FileStatus.List;
        public enum FileStatus
        {
            List, Error, Ok, Dupe
        }
    }
}
