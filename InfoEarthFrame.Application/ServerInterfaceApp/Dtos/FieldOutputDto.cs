using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    public class FieldOutputDto
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [ColumnAttribute("FieldName", ColumnAlias = "字段名称", ColumnType = "字符串")]
        public string FieldName { get; set; }
        /// <summary>
        /// 字段描述
        /// </summary>
        [ColumnAttribute("FieldDesc", ColumnAlias = "字段描述", ColumnType = "字符串")]
        public string FieldDesc { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        [ColumnAttribute("FieldType", ColumnAlias = "字段类型", ColumnType = "字符串")]
        public string FieldType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        [ColumnAttribute("FieldLength", ColumnAlias = "字段长度", ColumnType = "字符串")]
        public string FieldLength { get; set; }
        /// <summary>
        /// 字段小数位
        /// </summary>
        [ColumnAttribute("FieldPrecision", ColumnAlias = "字段小数位", ColumnType = "字符串")]
        public string FieldPrecision { get; set; }
        /// <summary>
        /// 字段输入控制
        /// </summary>
        [ColumnAttribute("FieldInputCtrl", ColumnAlias = "字段输入控制", ColumnType = "字符串")]
        public string FieldInputCtrl { get; set; }
        /// <summary>
        /// 字段最大值
        /// </summary>
        [ColumnAttribute("FieldInputMax", ColumnAlias = "字段最大值", ColumnType = "字符串")]
        public string FieldInputMax { get; set; }
        /// <summary>
        /// 字段最小值
        /// </summary>
        [ColumnAttribute("FieldInputMin", ColumnAlias = "字段最小值", ColumnType = "字符串")]
        public string FieldInputMin { get; set; }
        /// <summary>
        /// 字段默认值
        /// </summary>
        [ColumnAttribute("FieldDefaultValue", ColumnAlias = "字段默认值", ColumnType = "字符串")]
        public string FieldDefaultValue { get; set; }
        /// <summary>
        /// 字段是否为空
        /// </summary>
        [ColumnAttribute("FieldIsNull", ColumnAlias = "是否为空", ColumnType = "字符串")]
        public string FieldIsNull { get; set; }
        /// <summary>
        /// 字段输入格式
        /// </summary>
        [ColumnAttribute("FieldInputFormat", ColumnAlias = "字段输入格式", ColumnType = "字符串")]
        public string FieldInputFormat { get; set; }
        /// <summary>
        /// 字段单位
        /// </summary>
        [ColumnAttribute("FieldUnit", ColumnAlias = "字段单位", ColumnType = "字符串")]
        public string FieldUnit { get; set; }
        /// <summary>
        /// 字段数据分类
        /// </summary>
        [ColumnAttribute("FieldDataType", ColumnAlias = "字段数据分类", ColumnType = "字符串")]
        public string FieldDataType { get; set; }
        /// <summary>
        /// 字段文字符连接
        /// </summary>
        [ColumnAttribute("FieldValueLink", ColumnAlias = "字段文字符连接", ColumnType = "字符串")]
        public string FieldValueLink { get; set; }
        /// <summary>
        /// 字段数据源
        /// </summary>
        [ColumnAttribute("FieldDataSource", ColumnAlias = "字段数据源", ColumnType = "字符串")]
        public string FieldDataSource { get; set; }
        /// <summary>
        /// 字段计算公式
        /// </summary>
        [ColumnAttribute("FieldCalComp", ColumnAlias = "字段计算公式", ColumnType = "字符串")]
        public string FieldCalComp { get; set; }
        /// <summary>
        /// 字段排序
        /// </summary>
        [ColumnAttribute("FieldSort", ColumnAlias = "字段排序", ColumnType = "整型")]
        public int? FieldSort { get; set; }
        /// <summary>
        /// 字段备注
        /// </summary>
        [ColumnAttribute("Remark", ColumnAlias = "字段备注", ColumnType = "字符串")]
        public string Remark { get; set; }
    }
}
