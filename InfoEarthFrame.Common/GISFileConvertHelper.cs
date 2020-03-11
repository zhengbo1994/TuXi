using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Configuration;
using System.IO;

namespace InfoEarthFrame.Common
{
    public class GISFileConvertHelper
    {
        public string[] MapgisToArcgis(string filePath,string savePath)
        {
            List<string> filenames = new List<string>();

            DateTime dtStart = DateTime.Now;
            object fileName = filePath;
            if (fileName == null || string.IsNullOrEmpty(fileName.ToString()))
            {
                return null;
            }

            Environment.CurrentDirectory = ConfigurationManager.AppSettings["Map2ShpPath"];

            string dir = savePath;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                Process.Start(ConfigurationManager.AppSettings["Map2ShpPath"] + "iTelluro.DataTools.Console.exe", " Map2Shp -s:" + fileName + " -t:" + dir).WaitForExit(30 * 1000);

                DirectoryInfo folder = new DirectoryInfo(dir);
                foreach (FileInfo file in folder.GetFiles())
                {
                    filenames.Add(file.FullName);
                }
                string[] filenameslst = (string[])filenames.ToArray();
                return filenameslst;
            }
            catch
            {
                return null;
            }
        }
    }
}
