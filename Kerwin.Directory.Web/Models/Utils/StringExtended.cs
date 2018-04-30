using System.IO;
using System.Web;

namespace Kerwin.Directory.Web.Models.Utils
{
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


        public static string ToVirtualPath(this string filePath, string rootDir)
        {
            // \转为/方便linux和windows互转
            var vp= filePath.Replace(rootDir.TrimEnd('\\').TrimEnd('/'), "").ToDirFormatter();
            return string.IsNullOrWhiteSpace(vp) ? "/" : vp;
        }

        /// <summary>
        /// For beautiful
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToBackslashDecode(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            return url.Replace("%2f", "/").Replace("%2F", "/");
        }

        /// <summary>
        /// Because space urlencode to plus(+) 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ToPlusEncode(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            return url.Replace("+", "%20");
        }

        public static string ToUrlEncodeAndFormatter(this string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }
            return HttpUtility.UrlEncode(url.ToDirFormatter()).ToBackslashDecode().ToPlusEncode();
        }



        public static string ToIcon(this string fileName)
        {
            var ext = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(ext))
            {
                return "icon-geshi_weizhi";
            }
            ext = ext.ToLower();
            if (AppSettings.Icons.ContainsKey(ext))
            {
                return AppSettings.Icons[ext];
            }
            return "icon-geshi_weizhi";
        }

    }
}
