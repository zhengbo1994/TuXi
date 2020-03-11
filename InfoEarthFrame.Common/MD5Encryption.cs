using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace InfoEarthFrame.Common
{
    public class MD5Encryption
    {
        /// <summary>
        /// 16位MD5加密
        /// </summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns></returns>
        public static string ConvertString16(string strInput)
        {
            string result = string.Empty;
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            result = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(strInput)),4,8);
            result = result.Replace("-","");
            return result;
        }

        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns></returns>
        public static string ConvertString32(string strInput)
        {
            string result = string.Empty;

            MD5 mds = MD5.Create();

            byte[] s = mds.ComputeHash(Encoding.UTF8.GetBytes(strInput));

            for (int i = 0; i < s.Length;i++ )
            {
                result += s[i].ToString("x2");
            }
            return result;
        }
    }
}
