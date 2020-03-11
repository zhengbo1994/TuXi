using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    public class RarOrZipUtil
    {

        public static void Compress(string soruceDir, string rarFileName, out string rarFullPath)
        {
            rarFullPath = Path.Combine(soruceDir, rarFileName);
            RarHelper.CompressFilesToRar(Directory.GetFiles(soruceDir).ToList(), rarFullPath, rarFullPath);
        }

        public static void DeCompress(string fileName, string saveDir)
        {
            fileName = fileName.ToLower();
            if (fileName.EndsWith(".rar"))
            {
                RarHelper.UnCompressRar(fileName, saveDir);
            }
            else
            {
                using (var zip = new Ionic.Zip.ZipFile(fileName, Encoding.Default))
                {
                    zip.ExtractAll(saveDir, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        /// <summary>
        /// 将传入的文件列表压缩到指定的目录下
        /// </summary>
        /// <param name="sourceFilesPaths">要压缩的文件路径列表</param>
        /// <param name="compressFileSavePath">压缩文件存放路径</param>
        /// <param name="compressFileName">压缩文件名（全名）</param>
        public static void Compress(IEnumerable<string> sourceFilesPaths, string compressFileSavePath)
        {
            if (File.Exists(compressFileSavePath))
            {
                try
                {
                    File.Delete(compressFileSavePath);
                }
                catch 
                { 
                
                }
            }

            try
            {
                using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(compressFileSavePath, Encoding.Default))
                {
                    //加密压缩
                    //将要压缩的文件夹添加到zip对象中去(要压缩的文件夹路径和名称)
                    zip.AddFiles(sourceFilesPaths.Distinct(), "");
                    //将要压缩的文件添加到zip对象中去,如果文件不存在抛错FileNotFoundExcept
                    //zip.AddFile(@"E:\\yangfeizai\\12051214544443\\"+"Jayzai.xml");
                    zip.Save();

                }

            }
            catch
            {
                using (ZipFile zip = ZipFile.Create(compressFileSavePath))
                {
                    zip.BeginUpdate();
                    sourceFilesPaths.ToList().ForEach(p =>
                    {
                        zip.Add(p, Path.GetFileName(p));
                    });
                    zip.CommitUpdate();
                }
            }
        }
    }
}
