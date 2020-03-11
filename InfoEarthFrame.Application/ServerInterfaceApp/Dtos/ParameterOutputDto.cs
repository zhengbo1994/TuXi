using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    public class ParameterOutputDto
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [ColumnAttribute("ParameterName", ColumnAlias = "参数名称", ColumnType ="字符串")]
        public string ParameterName { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        [ColumnAttribute("ParameterType", ColumnAlias = "参数类型", ColumnType = "字符串")]
        public string ParameterType { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        [ColumnAttribute("ParameterValue", ColumnAlias = "参数值", ColumnType = "字符串")]
        public string ParameterValue { get; set; }
        /// <summary>
        /// 是否为可选参数
        /// </summary>
        [ColumnAttribute("ParameterIsMust", ColumnAlias = "是否为可选参数", ColumnType = "字符串")]
        public string ParameterIsMust { get; set; }
        /// <summary>
        /// 参数描述
        /// </summary>
        [ColumnAttribute("ParameterDesc", ColumnAlias = "参数描述", ColumnType = "字符串")]
        public string ParameterDesc { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [ColumnAttribute("ParameterSort", ColumnAlias = "排序", ColumnType = "整型")]
        public int? ParameterSort { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; set; }
        public string ColumnAlias { get; set; }
        public string ColumnType { get; set; }
        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}
