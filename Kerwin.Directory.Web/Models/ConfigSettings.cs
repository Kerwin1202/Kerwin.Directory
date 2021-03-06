﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kerwin.Utils.Core.DataFormat;
using Kerwin.Utils.Core.FileExtend;
using Kerwin.Utils.Core.Security;
using Kerwin.Utils.Core.StringExtend;

namespace Kerwin.Directory.Web.Models
{
    public static class ConfigSettings
    {
        static ConfigSettings()
        {
            IniConfigPath.ExistsFile(true);

            var ini = new Ini(IniConfigPath);

            #region 基本配置
            RootDir = ini.GetValue("RootDir", "Common", @"d:\");
            SiteName = ini.GetValue("SiteName", "Common", "只为资源站");
            DownloadRequestVirtualDir = ini.GetValue("DownloadRequestVirtualDir", "Common", "static");
            DateFormatter = ini.GetValue("DateFormatter", "Common", "yyyy-MM-dd HH:mm:ss");
            IsShowHidden = ini.GetValue("IsShowHidden", "Common", "false").ToBool();
            HiddenFileRules = ini.GetValue("HiddenFileRules", "Common", "[]").DeserializeByJson<List<string>>() ?? new List<string>();
            PasswordForAccess = ini.GetValue("PasswordForAccess", "Common", "");
            UserName = ini.GetValue("UserName", "Common", "admin");
            Password = ini.GetValue("Password", "Common", "123456");
            IsShowAnnouncement = ini.GetValue("IsShowAnnouncement", "Common", "true").ToBool(true);
            AnnouncementContent = ini.GetValue("AnnouncementContent", "Common", "加密密码<code> kerwin.cn </code>!");
            ShareDownloadExpiredMin = ini.GetValue("ShareDownloadExpiredMin", "Common", "30").ToInt32(30);
            DownloadTokenSalt = ini.GetValue("DownloadTokenSalt", "Common", "Kerwin.Directory");


            PasswordKey = ini.GetValue("PasswordKey", "Common", "Kerwin.Directory");
            PasswordIv = ini.GetValue("PasswordIv", "Common", "Kerwin.Directory");


            var paps = ini.GetValue("PasswordAccessPaths", "Common");
            if (!paps.IsNullOrWhiteSpace())
            {
                PasswordAccessPaths = paps.ToAesDecrypt(PasswordKey, PasswordIv).DeserializeByJson<Dictionary<string, string>>() ?? new Dictionary<string, string>();
            }
            else
            {
                PasswordAccessPaths = new Dictionary<string, string>();
            }
            #endregion

            #region Cdn配置

            var aliyunCdnSection = "Cdn";
            Cdn.CdnEnabled = ini.GetSections().Contains(aliyunCdnSection);
            Cdn.Domains = ini.GetValue("Domains", aliyunCdnSection, "[]").DeserializeByJson<List<string>>();

            #endregion
        }

        private static readonly string IniConfigPath = AppDomain.CurrentDomain.BaseDirectory.Combine("config", "config.ini");

        private static string PasswordKey { get; set; }
        private static string PasswordIv { get; set; }

        public static void AddNewPwd(this string path, string pwd)
        {
            PasswordAccessPaths.Add(path, pwd);
            SavePwd();
        }
        public static void RemovePwd(this string path)
        {
            PasswordAccessPaths.Remove(path);
            SavePwd();
        }
        public static void SavePwd()
        {
            var ini = new Ini(IniConfigPath);
            ini.WriteValue("PasswordAccessPaths", "Common", PasswordAccessPaths.SerializeByJson().ToAesEncrypt(PasswordKey, PasswordIv));
            ini.Save();
        }


        /// <summary>
        /// Whether to show hidden files or hidden directories
        /// 是否显示隐藏文件
        /// </summary>
        public static bool IsShowHidden { get; private set; }

        /// <summary>
        /// Root dir     linux like "/root/"
        /// 根目录 实际文件所在的目录
        /// </summary>
        public static string RootDir { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static string SiteName { get; private set; }

        /// <summary>
        /// The directory of the download file request is only useful for static files, if the download directory is not valid
        /// 下载文件请求的目录  只对静态文件有用,,如果是下载目录此设置无效
        /// </summary>
        public static string DownloadRequestVirtualDir { get; private set; }

        /// <summary>
        /// The formatter for last modify time 
        /// 输出的时间格式
        /// </summary>
        public static string DateFormatter { get; private set; }

        ///// <summary>
        ///// File hashing threshold default 100MB unit b
        ///// 计算hash的文件限制，小于等于这个值才会计算
        ///// </summary>
        //public static long HashLimitSize { get; set; } = 100 * 1024 * 1024;

        /// <summary>
        /// Hidden files rules support regex
        /// 隐藏名的规则
        /// </summary>
        public static List<string> HiddenFileRules { get; private set; }

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

        /// <summary>
        /// When this is not empty all directories or files require password access, Highest priority  PasswordAccessPaths Is invalid
        /// 如果不为空 则所有访问都需要密码  相当于私人网盘了.. 优先级最高 PasswordAccessPaths 则无效  
        /// </summary>
        public static string PasswordForAccess { get; private set; }

        /// <summary>
        /// Virtual path correspondence Password collection Case sensitivity because of the distinction in linux
        /// 虚拟路径 对应 密码集合  区分大小写 因为linux中区分
        /// </summary>
        public static Dictionary<string, string> PasswordAccessPaths { get; private set; }

        /// <summary>
        /// UserName for set access password
        /// 设置访问密码的账号
        /// </summary>
        public static string UserName { get; private set; }
        /// <summary>
        /// The password set for access password
        /// 设置访问密码的密码
        /// </summary>
        public static string Password { get; private set; }

        ///// <summary>
        ///// 是否允许上传
        ///// </summary>
        //public static bool UploadEnabled { get; set; } = false;

        /// <summary>
        /// Whether to display the announcement
        /// 是否显示公告
        /// </summary>
        public static bool IsShowAnnouncement { get; private set; }

        /// <summary>
        /// Announcement Content
        /// 公告内容
        /// </summary>
        public static string AnnouncementContent { get; private set; }

        /// <summary>
        /// Generated Download Links Download Expiration Time
        /// 生成的下载链接下载过期时间
        /// </summary>
        public static int ShareDownloadExpiredMin { get; private set; }

        /// <summary>
        /// Downloading the encrypted salt is mainly used to produce the salt used in the download link expiration time.
        /// 下载加密的盐 主要是生产下载链接过期时间所用的盐 需要修改，否则人人一样
        /// </summary>
        public static string DownloadTokenSalt { get; private set; }


        #region cdn

        #region aliyun cdn

        public static class Cdn
        {
            public static bool CdnEnabled { get; set; }

            public static List<string> Domains { get; set; }
        }


        #endregion

        #endregion

    }
}