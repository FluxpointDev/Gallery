using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gallery.Data
{
    public class GAccess
    {
        public bool canView = false;
        public bool canManage = false;
        public bool canDelete = false;
        public bool canUpload = false;
        public bool canEdit = false;
        public bool approveOnly = false;
    }
}
