using System;

namespace Kerwin.Directory.Web.Models
{
    public class FileInfoModel
    {
        public string FileName { get; set; }

        public double Size { get; set; }

        public DateTime LastModifiedTime { get; set; }

        public FileType FileType { get; set; }

        public string Icon { get; set; } = "icon-geshi_weizhi";

        public string FileVirtualPath { get; set; }
    }


    public enum FileType
    {
        Directory = 0,

        File = 1,
    }

    public static class StringExtended
    {
        public static string TrimLine(this string inputStr)
        {
            return inputStr.Trim('/').Trim('\\');
        }

        public static string ToDirFormatter(this string inputStr)
        {
            return inputStr.Replace("\\", "/");
        }
    }
}