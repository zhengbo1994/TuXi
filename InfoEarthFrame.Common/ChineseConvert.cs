using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPinyin;

namespace InfoEarthFrame.Common
{
    public class ChineseConvert
    {
        public string GetPinyin(string text)
        {
            return NPinyin.Pinyin.GetPinyin(text);
        }

        public string GetPinyinInitials(string text)
        {
            Encoding gb2312 = Encoding.GetEncoding("GB2312");
            string stext = NPinyin.Pinyin.ConvertEncoding(text, Encoding.UTF8, gb2312);
            return CheckTableName(NPinyin.Pinyin.GetInitials(stext, gb2312).ToLower());
        }

        public string CheckTableName(string tableName)
        {
            string newName = "";

            foreach (char c in tableName)
            {
                int ascll = System.Text.Encoding.ASCII.GetBytes(new char[] { c })[0];
                if ((ascll >= 48 && ascll <= 57) || (ascll >= 65 && ascll <= 90) || (ascll >= 97 && ascll <= 122) || ascll == 95)
                {
                    newName += c;
                }
            }
            return newName;
        }
    }
}
