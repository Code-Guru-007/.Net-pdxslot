using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace PdxSlots.API.Common
{
    public static class StringExtensions
    {
        public static bool IsMobile(this string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
                return false;
            //tablet
            if (Regex.IsMatch(userAgent, "(tablet|ipad|playbook|silk)|(android(?!.*mobile))", RegexOptions.IgnoreCase))
                return true;
            //mobile
            const string mobileRegex =
                "blackberry|iphone|mobile|windows ce|opera mini|htc|sony|palm|symbianos|ipad|ipod|blackberry|bada|kindle|symbian|sonyericsson|android|samsung|nokia|wap|motor";

            if (Regex.IsMatch(userAgent, mobileRegex, RegexOptions.IgnoreCase)) return true;

            //not mobile 
            return false;
        }

        public static string CheckMd5(this string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hashed = md5.ComputeHash(stream);

                    return BitConverter.ToInt32(hashed, 0).ToString();
                }
            }
        }
    }
}
