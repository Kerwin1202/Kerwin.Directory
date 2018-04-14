using System.Collections.Generic;

namespace Kerwin.Directory.Web.Models
{
    public static class ConfigSettings
    {
        /// <summary>
        /// Whether to show hidden files or hidden directories
        /// 是否显示隐藏文件
        /// </summary>
        public static bool IsShowHidden { get; set; } = false;

        /// <summary>
        /// Root dir
        /// 根目录 实际文件所在的目录
        /// </summary>
        public static string RootDir { get; set; } = @"h:\";

        /// <summary>
        /// 下载文件请求的目录  只对静态文件有用,,如果是下载目录此设置无效
        /// </summary>
        public static string DownloadRequestVirtualDir { get; set; } = "static";

        /// <summary>
        /// The formatter for last modify time 
        /// 输出的时间格式
        /// </summary>
        public static string DateFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss";

        ///// <summary>
        ///// File hashing threshold default 100MB unit b
        ///// 计算hash的文件限制，小于等于这个值才会计算
        ///// </summary>
        //public static long HashLimitSize { get; set; } = 100 * 1024 * 1024;

        /// <summary>
        /// Hidden files rules support regex
        /// 隐藏名的规则
        /// </summary>
        public static List<string> HiddenFileRules { get; set; } = new List<string>()
        {
            //@"\."// example .git .gitignore and so..
        };

        ///// <summary>
        ///// 
        ///// </summary>
        //public static bool OpenInNewWindow { get; set; } = false;

        ///// <summary>
        ///// 
        ///// </summary>
        //public static bool DownloadInNewWindow { get; set; } = true;

        ///// <summary>
        ///// 
        ///// </summary>
        //public static bool AllowDownloadDir { get; set; } = false;

        ///// <summary>
        ///// This only works when AllowDownloadDir is false
        ///// </summary>
        //public static List<string> AllowDownloadDirs { get; set; } = new List<string>();

        ///// <summary>
        ///// This only works when AllowDownloadDir is true
        ///// </summary>
        //public static List<string> NotAllowedDownloadDirs { get; set; } = new List<string>();

        ///// <summary>
        ///// When this is not empty all directories or files require password access
        ///// </summary>
        //public static string PasswordForAccess { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public static Dictionary<string, string> PasswordAccessPaths { get; set; } = new Dictionary<string, string>()
        //{

        //};

        //public static string Name { get; set; } = "admin";

        //public static string Password { get; set; } = "123456";

        ///// <summary>
        ///// 是否允许上传
        ///// </summary>
        //public static bool UploadEnabled { get; set; } = false;

        /// <summary>
        /// 是否显示公告
        /// </summary>
        public static bool IsShowAnnouncement { get; set; } = true;

    }
}