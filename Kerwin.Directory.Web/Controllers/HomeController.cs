using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Kerwin.Directory.Web.Models;
using Kerwin.Directory.Web.Models.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kerwin.Directory.Web.Controllers
{
    public class HomeController : Controller
    {

        [Route("")]
        public IActionResult Index(string dir, string password)
        {
            var rootDir = ConfigSettings.RootDir;
            var baseVirtualDir = dir ?? "";
            var baseDir = Path.Combine(rootDir, baseVirtualDir.TrimLine());
            if (string.IsNullOrWhiteSpace(baseDir) || !baseDir.StartsWith(rootDir))
            {
                return Redirect("/");
            }

            var querys = baseVirtualDir.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            ViewBag.Querys = querys;

            var dic = HttpContext.Session.GetString("allowedaccesspath");
            var aap = string.IsNullOrWhiteSpace(dic)
                ? new List<string>() { }
                : JsonConvert.DeserializeObject<List<string>>(dic);

            if (!AppSettings.AllowAccessPath(baseDir, out var needPwd, password, aap))
            {
                ViewBag.Msg = string.IsNullOrWhiteSpace(password) ? "" : "密码错误~~~";
                return View("NeedPassword");
            }

            var vp = baseDir.ToVirtualPath(ConfigSettings.RootDir);
            if (needPwd && !aap.Contains(vp))
            {
                aap.Add(vp);
                HttpContext.Session.SetString("allowedaccesspath", JsonConvert.SerializeObject(aap));
            }
            if (!System.IO.Directory.Exists(baseDir))
            {
                if (System.IO.File.Exists(baseDir))
                {
                    var expiredTime = DateTime.Now.AddMinutes(ConfigSettings.ShareDownloadExpiredMin);
                    return Redirect($"/{ConfigSettings.DownloadRequestVirtualDir}{baseVirtualDir}?expiredtime={expiredTime:yyyy-MM-dd HH:mm:ss}&token={baseVirtualDir.GenerateDlToken(expiredTime)}");
                }
                return Redirect("/");
            }
            var mask = ConfigSettings.IsShowHidden ? FileAttributes.System : FileAttributes.Hidden | FileAttributes.System;

            var di = new DirectoryInfo(baseDir);

            var dirs = di.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
                .Where(fi => (fi.Attributes & mask) == 0).ToList();

            var files = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly)
                .Where(fi => (fi.Attributes & mask) == 0).ToList();

            var fms = new List<FileInfoModel>();
            foreach (var d in dirs)
            {
                if (ConfigSettings.HiddenFileRules.FirstOrDefault(r => new Regex(r).IsMatch(d.Name)) != null)
                {
                    continue;
                }
                fms.Add(new FileInfoModel()
                {
                    FileName = d.Name,
                    FileType = FileType.Directory,
                    LastModifiedTime = d.LastWriteTime,
                    Icon = "icon-wenjianjia",
                    FileVirtualPath = d.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase).ToUrlEncodeAndFormatter()
                });
            }

            foreach (var file in files)
            {
                if (ConfigSettings.HiddenFileRules.FirstOrDefault(r => new Regex(r).IsMatch(file.Name)) != null)
                {
                    continue;
                }
                fms.Add(new FileInfoModel()
                {
                    FileName = file.Name,
                    FileType = FileType.File,
                    LastModifiedTime = file.LastWriteTime,
                    Size = file.Length,
                    Icon = file.Name.ToIcon(),
                    FileVirtualPath = file.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase).ToUrlEncodeAndFormatter()
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
            //parentDir
            ViewBag.ParentDir = parentDir.ToUrlEncodeAndFormatter();

            ViewBag.Parmas = dir;

            return View(fms);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
