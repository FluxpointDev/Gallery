using Blazor.FileReader;

namespace Gallery
{
    public class GFileInfo
    {
        public GFileInfo(IFileInfo fi)
        {
            Name = fi.Name;
            Size = fi.Size;
            if (Size < 1)
                Status = FileStatus.InvalidSize;
            if (Size > 7340032)
                Status = FileStatus.MaxSize;
            switch (fi.Type)
            {
                case "image/png":
                    Type = "png";
                    break;
                case "image/jpeg":
                    Type = "jpg";
                    break;
                case "image/gif":
                    Type = "gif";
                    break;
                default:
                    Status = FileStatus.InvalidType;
                    break;
            }
        }
        public string Name;
        public long Size;
        public string Type;
        public FileStatus Status = FileStatus.Pending;

        public string GetStatusText()
        {
            switch (Status)
            {
                case FileStatus.Pending:
                    return "Pending";
                case FileStatus.Duplicate:
                    return "Duplicate found";
                case FileStatus.Exception:
                    return "Server error";
                case FileStatus.InvalidSize:
                    return "Invalid size";
                case FileStatus.InvalidType:
                    return "Invalid type";
                case FileStatus.Ok:
                    return "Done!";
                case FileStatus.MaxSize:
                    return "Max size limit";
            }
            return "ERROR!";
        }

        public enum FileStatus
        {
            Pending, Ok, Exception, InvalidSize, MaxSize, InvalidType, Duplicate
        }
    }
}
