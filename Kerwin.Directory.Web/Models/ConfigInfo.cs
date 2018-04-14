using System.Collections.Generic;

namespace Kerwin.Directory.Web.Models
{
    public class ConfigInfo
    {
        /// <summary>
        /// Whether to show hidden files or hidden directories
        /// </summary>
        public bool IsShowHidden { get; set; } = false;

        /// <summary>
        /// Root dir
        /// </summary>
        public string RootDir { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DownloadRequestVirtualDir { get; set; } = "static";

        /// <summary>
        /// The formatter for last modify time 
        /// </summary>
        public string DateFormatter { get; set; } = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// File hashing threshold default 100MB unit b
        /// </summary>
        public long HashLimitSize { get; set; } = 100 * 1024 * 1024;

        /// <summary>
        /// Hidden files rules support regex
        /// </summary>
        public List<string> HiddenFileRules { get; set; } = new List<string>()
        {
            @"\.*"// example .git .gitignore and so..
        };

        /// <summary>
        /// 
        /// </summary>
        public bool OpenInNewWindow { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public bool DownloadInNewWindow { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        public bool AllowDownloadDir { get; set; } = false;

        /// <summary>
        /// This only works when AllowDownloadDir is false
        /// </summary>
        public List<string> AllowDownloadDirs { get; set; } = new List<string>();

        /// <summary>
        /// This only works when AllowDownloadDir is true
        /// </summary>
        public List<string> NotAllowedDownloadDirs { get; set; } = new List<string>();

        /// <summary>
        /// When this is true all directories or files require password access
        /// </summary>
        public bool PasswordAccess { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> PasswordAccessPaths { get; set; } = new Dictionary<string, string>();

        public string Name { get; set; } = "admin";

        public string Password { get; set; } = "123456";

        public bool UploadEnabled { get; set; } = false;

    }
}