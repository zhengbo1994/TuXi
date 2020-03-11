using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    /// <summary>
    /// 翻译
    /// </summary>
    public class UtilityMessageConvert
    {
        private static List<CMessage> ListCMessage = new List<CMessage>();

        private static string Key = ConfigurationManager.AppSettings.Get("Language");
        private static string FilePath = ConfigurationManager.AppSettings.Get("LanguagePackFilePath");

        public static string Get(string value)
        {
            if (ListCMessage == null || ListCMessage.Count == 0)
            {
                InitMessage();
            }
            CMessage cMessage = ListCMessage.Find(t => t.CN == value || t.Id == value);
            if (cMessage != null && !string.IsNullOrEmpty(Key) && Key.ToUpper().Equals("ENGLISH"))
            {
                return cMessage.EN;
            }
            return value;
        }

        private static void InitMessage()
        {
            string path = !string.IsNullOrWhiteSpace(FilePath) ? FilePath : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\Message.txt");
            if (File.Exists(path))
            {
                string[] lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    string[] values = line.Split('#');
                    if (values.Length >= 3)
                    {
                        ListCMessage.Add(new CMessage() { Id = values[0], CN = values[1], EN = values[2] });
                    }
                }
            }
        }
    }

    public class CMessage
    {
        public string Id;
        public string CN;
        public string EN;
    }
}
