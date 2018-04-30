using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Kerwin.Directory.Web.Models.Utils
{
    public static class AppSettings
    {

        public static readonly Dictionary<string, string> Icons = new Dictionary<string, string>()
        {
            // Archives
            {".7z","icon-zip1" },
            {".bz","icon-zip1" },
            {".gz","icon-zip1" },
            {".rar","icon-zip1" },
            {".tar","icon-zip1" },
            {".zip","icon-zip1" },

            // Audio
            {".aac","icon-music" },
            {".flac","icon-music" },
            {".mid","icon-music" },
            {".midi","icon-music" },
            {".mp3","icon-music" },
            {".ogg","icon-music" },
            {".wma","icon-music" },
            {".wav","icon-music" },

            // Code
            {".vb","icon-daima1" },
            {".c","icon-daima1" },
            {".cs","icon-daima1" },
            {".class","icon-daima1" },
            {".cpp","icon-daima1" },
            {".css","icon-daima1" },
            {".erb","icon-daima1" },
            {".htm","icon-daima1" },
            {".html","icon-daima1" },
            {".java","icon-daima1" },
            {".js","icon-daima1" },
            {".php","icon-daima1" },
            {".pl","icon-daima1" },
            {".py","icon-daima1" },
            {".rb","icon-daima1" },
            {".xhtml","icon-daima1" },
            {".xml","icon-daima1" },
            {".csproj","icon-daima1" },

            // Databases
            {".accdb","icon-database" },
            {".db","icon-database" },
            {".dbf","icon-database" },
            {".mdb","icon-database" },
            {".pdb","icon-database" },
            {".sql","icon-database" },

            // Documents
            {".csv","icon-wenjian1" },
            {".doc","icon-wenjian1" },
            {".docx","icon-wenjian1" },
            {".odt","icon-wenjian1" },
            {".pdf","icon-wenjian1" },
            {".xls","icon-wenjian1" },
            {".xlsx","icon-wenjian1" },

            // Executables
            //{".app","icon-exe" },
            //{".com","icon-exe" },
            //{".exe","icon-exe" },
            //{".jar","icon-exe" },
            //{".msi","icon-exe" },

            // Fonts
            {".eot","icon-font" },
            {".otf","icon-font" },
            {".ttf","icon-font" },
            {".woff","icon-font" },

            // Game Files
            {".gam","icon-gamepad" },
            {".nes","icon-gamepad" },
            {".rom","icon-gamepad" },
            {".sav","icon-gamepad" },

            // Images
            {".bmp","icon-13" },
            {".gif","icon-13" },
            {".jpg","icon-13" },
            {".jpeg","icon-13" },
            {".png","icon-13" },
            {".psd","icon-13" },
            {".tga","icon-13" },
            {".tif","icon-13" },

            // Package Files
            {".box","icon-archive" },
            {".deb","icon-archive" },
            {".rpm","icon-archive" },

            // Scripts
            {".bat","icon-terminal" },
            {".cmd","icon-terminal" },
            {".sh","icon-terminal" },

            // Text
            {".cfg","icon-uploadicon01" },
            {".ini","icon-uploadicon01" },
            {".log","icon-uploadicon01" },
            {".md","icon-uploadicon01" },
            {".rtf","icon-uploadicon01" },
            {".txt","icon-uploadicon01" },
            {".json","icon-uploadicon01" },

            // Vector Images
            {".ai","icon-13" },
            {".drw","icon-13" },
            {".eps","icon-13" },
            {".ps","icon-13" },
            {".svg","icon-13" },

            // Video
            {".avi","icon-shipinbofangyingpian" },
            {".flv","icon-shipinbofangyingpian" },
            {".mkv","icon-shipinbofangyingpian" },
            {".mov","icon-shipinbofangyingpian" },
            {".mp4","icon-shipinbofangyingpian" },
            {".mpg","icon-shipinbofangyingpian" },
            {".ogv","icon-shipinbofangyingpian" },
            {".webm","icon-shipinbofangyingpian" },
            {".wmv","icon-shipinbofangyingpian" },
            {".swf","icon-shipinbofangyingpian" },
        };

        /// <summary>
        /// 递归判断filepath是否需要密码
        /// </summary>
        /// <param name="filePath">判断的路径</param>
        /// <param name="passwordPath">需要密码的路径</param>
        /// <returns></returns>
        private static bool IsNeedPassword(string filePath, out string passwordPath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                passwordPath = filePath;
                return false;
            }
            var dir = (Path.GetDirectoryName(filePath) ?? "").ToDirFormatter();
            passwordPath = dir;
            if (string.IsNullOrWhiteSpace(dir))
            {
                return false;
            }
            if (ConfigSettings.PasswordAccessPaths.ContainsKey(dir))
            {
                return true;
            }

            return IsNeedPassword(dir, out passwordPath);
        }

        /// <summary>
        /// 判断是否可以直接访问改路径
        /// </summary>
        /// <param name="filePath">判断的路径</param>
        /// <param name="needPwd">返回是否需要密码</param>
        /// <param name="password">输入的密码</param>
        /// <param name="aap">已经运行访问的路径</param>
        /// <returns></returns>
        public static bool AllowAccessPath(string filePath, out bool needPwd, string password = null, List<string> aap = null)
        {
            aap = aap ?? new List<string>();
            var virtualPath = filePath.ToVirtualPath(ConfigSettings.RootDir);

            var hasPath = aap.FirstOrDefault(a => a == virtualPath || virtualPath.StartsWith(a));
            if (hasPath != null)
            {
                needPwd = false;
                return true;
            }

            if (!string.IsNullOrWhiteSpace(ConfigSettings.PasswordForAccess))
            {
                needPwd = true;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    return ConfigSettings.PasswordForAccess == password;
                }
                return false;
            }
            if (ConfigSettings.PasswordAccessPaths.ContainsKey(virtualPath))
            {
                needPwd = true;
                if (!string.IsNullOrWhiteSpace(password))
                {
                    return ConfigSettings.PasswordAccessPaths[virtualPath] == password;
                }
                return false;
            }

            needPwd = false;
            if (!IsNeedPassword(virtualPath, out var passwordPath)) return true;

            hasPath = aap.FirstOrDefault(a => a == passwordPath);// || passwordPath.StartsWith(a)
            if (hasPath != null)
            {
                needPwd = false;
                return true;
            }

            needPwd = true;
            if (!string.IsNullOrWhiteSpace(password))
            {
                return ConfigSettings.PasswordAccessPaths[passwordPath] == password;
            }
            return false;
        }

        /// <summary>
        /// 生成下载的token
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public static string GenerateDlToken(this string fileName, DateTime expiredTime)
        {
            return $"{fileName}@{ConfigSettings.DownloadTokenSalt}${expiredTime.ToString("yyyyMMddHHmmss")}".ToMd5EncryptFromString(Encoding.UTF8);
        }

        /// <summary>
        /// 生成下载的token
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="expiredTime"></param>
        /// <returns></returns>
        public static bool CheckDlToken(this string fileName, DateTime expiredTime, string token)
        {
            return fileName.GenerateDlToken(expiredTime) == token;
        }
    }
}
