using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    public class LayerOutputDto
    {
        /// <summary>
        /// 图层编号
        /// </summary>
        [ColumnAttribute("LayerId", ColumnAlias = "图层编号", ColumnType = "字符串")]
        public string LayerId { get; set; }
        /// <summary>
        /// 图层名称
        /// </summary>
        [ColumnAttribute("LayerName", ColumnAlias = "图层名称", ColumnType = "字符串")]
        public string LayerName { get; set; }
        /// <summary>
        /// 图层类型
        /// </summary>
        [ColumnAttribute("DataType", ColumnAlias = "图层类型", ColumnType = "字符串")]
        public string DataType { get; set; }
        /// <summary>
        /// 图层边界空间
        /// </summary>
        [ColumnAttribute("LayerBBox", ColumnAlias = "图层边界空间", ColumnType = "字符串")]
        public string LayerBBox { get; set; }
        /// <summary>
        /// 图层分类
        /// </summary>
        [ColumnAttribute("LayerType", ColumnAlias = "图层分类", ColumnType = "字符串")]
        public string LayerType { get; set; }
        /// <summary>
        /// 图层标签
        /// </summary>
        [ColumnAttribute("LayerTag", ColumnAlias = "图层标签", ColumnType = "字符串")]
        public string LayerTag { get; set; }
        /// <summary>
        /// 图层描述
        /// </summary>
        [ColumnAttribute("LayerDesc", ColumnAlias = "图层描述", ColumnType = "字符串")]
        public string LayerDesc { get; set; }
        /// <summary>
        /// 空间参考
        /// </summary>
        [ColumnAttribute("LayerRefence", ColumnAlias = "空间参考", ColumnType = "字符串")]
        public string LayerRefence { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [ColumnAttribute("CreateDT", ColumnAlias = "创建时间", ColumnType = "日期")]
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 图层样式名称
        /// </summary>
        [ColumnAttribute("DataStyleName", ColumnAlias = "图层样式名称", ColumnType = "字符串")]
        public string DataStyleName { get; set; }
        /// <summary>
        /// 图层顺序
        /// </summary>
        [ColumnAttribute("DataSort", ColumnAlias = "图层顺序", ColumnType = "整型")]
        public int? DataSort { get; set; }
    }
}
