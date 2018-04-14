using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using Kerwin.Directory.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kerwin.Directory.Web.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index(string dir)
        {
            var rootDir = @"d:\";
            var baseVirtualDir = dir ?? "";
            var baseDir = Path.Combine(rootDir, baseVirtualDir.TrimLine());
            if (string.IsNullOrWhiteSpace(baseDir) || !baseDir.StartsWith(rootDir))
            {
                return Redirect("/");
            }
            if (!System.IO.Directory.Exists(baseDir))
            {
                return Redirect("/");
            }
            var mask = FileAttributes.Hidden | FileAttributes.System;

            var di = new DirectoryInfo(baseDir);

            var dirs = di.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(fi => (fi.Attributes & mask) == 0).ToList();

            var files = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                .Where(fi => (fi.Attributes & mask) == 0).ToList();

            var fms = new List<FileInfoModel>();
            foreach (var d in dirs)
            {
                fms.Add(new FileInfoModel()
                {
                    FileName = d.Name,
                    FileType = FileType.Directory,
                    LastModifiedTime = DateTime.Now,
                    Icon = "icon-wenjianjia",
                    FileVirtualPath = d.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase).ToDirFormatter()
                });
            }

            foreach (var file in files)
            {
                fms.Add(new FileInfoModel()
                {
                    FileName = file.Name,
                    FileType = FileType.File,
                    LastModifiedTime = file.LastWriteTime,
                    Size = file.Length,
                    Icon = GetIcon(file.Name),
                    FileVirtualPath = file.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase).ToDirFormatter()
                });
            }

            string parentDir;
            if (di.Parent != null)
            {
                parentDir = "/" + di.Parent.FullName.Replace(rootDir.TrimEnd('\\'), "", StringComparison.OrdinalIgnoreCase).TrimLine().ToDirFormatter();
            }
            else
            {
                parentDir = rootDir;
            }
            ViewBag.ParentDir = parentDir;

            ViewBag.Parmas = dir;

            var querys = baseVirtualDir.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            ViewBag.Querys = querys;

            return View(fms);
        }

        private Dictionary<string, string> icons = new Dictionary<string, string>()
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

        private string GetIcon(string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(ext))
            {
                return "icon-geshi_weizhi";
            }
            ext = ext.ToLower();
            if (icons.ContainsKey(ext))
            {
                return icons[ext];
            }
            return "icon-geshi_weizhi";
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
