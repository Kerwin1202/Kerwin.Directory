using System;

namespace Kerwin.Directory.Web.Models.Utils
{
    public class FileInfoModel
    {
        public string FileName { get; set; }

        public double Size { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public FileType FileType { get; set; }

        public string Icon { get; set; } = "icon-geshi_weizhi";

        public string FileVirtualPath { get; set; }

        public bool IsLock { get; set; }
    }


    public enum FileType
    {
        Directory = 0,

        File = 1,
    }
}