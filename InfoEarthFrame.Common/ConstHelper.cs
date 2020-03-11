using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Common
{
    public class ConstHelper
    {
        public const string CONST_SYSTEMCODE = "000000";
        public const string CONST_SYSTEMNAME = "ADMIN";
        public const string UPLOAD_SUCCESS = "1";

        public const string IMAGE_FORMAT = "IMAGE";
        public const string VECTOR_FORMAT = "VECTOR";

        /// <summary>
        /// 数据类型，矢量，影像文件
        /// </summary>
        public static IDictionary<string, string> DataFormatHelper = new Dictionary<string, string>()
        { 
            { IMAGE_FORMAT, "application/openlayers" },
            { VECTOR_FORMAT , "image/png"}
        };
    }

    public class ConfigHelper
    {
        public static string AreaFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings.Get("AreaFilePath"));

        public static string TIFF_RemoveBackGroundColor = ConfigurationManager.AppSettings.Get("TIFF_RemoveBackGroundColor");

        public static string IsEnglish = ConfigurationManager.AppSettings.Get("Language").ToUpper().Equals("ENGLISH") ? "1" : "0";

        public static string TranslateFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("TranslateFilePath")) ? "" : ConfigurationManager.AppSettings.Get("TranslateFilePath"));

        public static string UploadFilePath = ConfigurationManager.AppSettings.Get("UploadFilePath");

        public static string ThumbnailPath = ConfigurationManager.AppSettings.Get("ThumbnailPath");
    }

    /// <summary>
    /// 数据类型，矢量，影像文件
    /// </summary>
    public enum DataTypeHelper
    {
        矢量 = 1,
        影像 = 2
    }
}
