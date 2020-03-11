using System;
using System.Collections.Generic;

namespace InfoEarthFrame.Application
{
    public class LayerListOutput
    {
        /// <summary>
        /// 图系类型ID
        /// </summary>
        public string MappingTypeID { get; set; }

        /// <summary>
        ///图系名称 
        /// </summary>
        public string MappingClassName { get; set; }

        /// <summary>
        /// 创建开始日期
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 创建结束日期
        /// </summary>
        public string EndDate { get; set; }

        /// <summary>
        /// 图系主信息ID
        /// </summary>
        public string DataMainID { get; set; }

        /// <summary>
        /// 元数据ID
        /// </summary>
        public string MetaDataID { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// 图片ID
        /// </summary>
        public int ImageID { get; set; }


        /// <summary>
        /// 图片父ID
        /// </summary>
        public int ImagePID { get; set; }

        /// <summary>
        /// 图片名称
        /// </summary>
        public string ImageText { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImageURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ImageDataServerKEY { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int ImageTileSize { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ImageZeroLevelSize { get; set; }

        /// <summary>
        /// 图类型
        /// </summary>
        public string ImagePicType { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public string MDDataSt { get; set; }

        /// <summary>
        /// 专题类型
        /// </summary>
        public string TPCat { get; set; }

        /// <summary>
        /// 绘图单位
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 摘要信息
        /// </summary>
        public string IdAbs { get; set; }

        /// <summary>
        /// 发布单位
        /// </summary>
        public string PusUnit { get; set; }

        /// <summary>
        /// SHP文件名称
        /// </summary>
        public string MainShpFileName { get; set; }

        /// <summary>
        /// 下载权限
        /// </summary>
        public bool isDownload { get; set; }

        /// <summary>
        /// 浏览下载
        /// </summary>
        public bool isBrowse { get; set; }

        /// <summary>
        /// 下载文件路径
        /// </summary>
        public string DownLoadFilePath { get; set; }

        /// <summary>
        /// Shp文件路径
        /// </summary>
        public string ShpFilePath { get; set; }

        public DateTime? PublishTime { get; set; }

        public string ThumbFilePath { get; set; }
    }
}
