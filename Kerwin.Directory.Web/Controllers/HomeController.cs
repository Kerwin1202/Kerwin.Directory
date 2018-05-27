using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Kerwin.Directory.Web.Models;
using Kerwin.Directory.Web.Models.Utils;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QRCoder;

namespace Kerwin.Directory.Web.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// 是否登陆
        /// </summary>
        private bool IsLogin => HttpContext.Session.GetInt32("islogin") == 1;

        [Route("")]
        [Route("login")]
        public IActionResult Index(string dir, string accessPassword, string name, string password)
        {
            #region login
            if (!string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(password))
            {
                if (ConfigSettings.UserName == name && ConfigSettings.Password == password)
                {
                    HttpContext.Session.SetInt32("islogin", 1);
                    return Redirect("/");
                }
                else
                {
                    ViewBag.Name = name;
                }
            }
            if (Request.Path == "/login")
            {
                ViewBag.GotoLogin = true;
            }
            ViewBag.IsLogin = IsLogin;
            #endregion

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

            bool needPwd = false;
            if (!IsLogin && !AppSettings.AllowAccessPath(baseDir, out needPwd, accessPassword, aap) && needPwd)
            {
                ViewBag.Msg = string.IsNullOrWhiteSpace(accessPassword) ? "" : "密码错误~~~";
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
                    return Redirect($"/{ConfigSettings.DownloadRequestVirtualDir}{baseVirtualDir.ToUrlEncodeAndFormatter()}?expiredtime={expiredTime:yyyy-MM-dd HH:mm:ss}&token={baseVirtualDir.GenerateDlToken(expiredTime)}");
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

                var vpp = d.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase);
                fms.Add(new FileInfoModel()
                {
                    FileName = d.Name,
                    FileType = FileType.Directory,
                    LastModifiedTime = d.LastWriteTime,
                    Icon = "icon-wenjianjia",
                    FileVirtualPath = vpp.ToUrlEncodeAndFormatter(),
                    IsLock = ConfigSettings.PasswordAccessPaths.ContainsKey(vpp)
                });
            }

            foreach (var file in files)
            {
                if (ConfigSettings.HiddenFileRules.FirstOrDefault(r => new Regex(r).IsMatch(file.Name)) != null)
                {
                    continue;
                }
                var vpp = file.FullName.Replace(rootDir, "/", StringComparison.OrdinalIgnoreCase);
                fms.Add(new FileInfoModel()
                {
                    FileName = file.Name,
                    FileType = FileType.File,
                    LastModifiedTime = file.LastWriteTime,
                    Size = file.Length,
                    Icon = file.Name.ToIcon(),
                    FileVirtualPath = vpp.ToUrlEncodeAndFormatter(),
                    IsLock = ConfigSettings.PasswordAccessPaths.ContainsKey(vpp)
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

        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pwdforpath"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("setpwd")]
        public IActionResult SetPwd(string path, string pwdforpath)
        {
            if (!IsLogin) return Redirect("/login");
            if (string.IsNullOrWhiteSpace(pwdforpath) || string.IsNullOrWhiteSpace(path)) return Json(false);
            path.AddNewPwd(pwdforpath);
            return Json(true);
        }
        /// <summary>
        /// 设置密码
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("removepwd")]
        public IActionResult RemovePwd(string path)
        {
            if (!IsLogin) return Redirect("/login");
            if (string.IsNullOrWhiteSpace(path)) return Json(false);
            path.RemovePwd();
            return Json(true);
        }


        [Route("qrcode")]
        public FileResult QrCode()
        {
            PayloadGenerator.Url generator = new PayloadGenerator.Url(Request.Headers.ContainsKey("referer") ? Request.Headers["referer"][0] : Request.GetUri().ToString());
            string payload = generator.ToString();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(5);
            return File(qrCodeAsBitmapByteArr, "image/png");
        }

    }
}
