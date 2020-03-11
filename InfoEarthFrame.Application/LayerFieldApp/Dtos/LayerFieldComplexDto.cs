using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Application.Services.Dto;

namespace InfoEarthFrame.Application.LayerFieldApp.Dtos
{
    public class LayerFieldComplexDto : EntityDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 图层目录ID
        /// </summary>
        public string LayerID { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性描述
        /// </summary>
        public string AttributeDesc { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        public string AttributeType { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        public string AttributeTypeName { get; set; }

        public string CodeName { get; set; }

        /// <summary>
        /// 属性长度
        /// </summary>
        public string AttributeLength { get; set; }
        /// <summary>
        /// 属性小数位
        /// </summary>
        public string AttributePrecision { get; set; }
        /// <summary>
        /// 输入控制
        /// </summary>
        public string AttributeInputCtrl { get; set; }
        /// <summary>
        /// 最大值
        /// </summary>
        public string AttributeInputMax { get; set; }
        /// <summary>
        /// 最小值
        /// </summary>
        public string AttributeInputMin { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string AttributeDefault { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public string AttributeIsNull { get; set; }
        /// <summary>
        /// 输入格式
        /// </summary>
        public string AttributeInputFormat { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateDT { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string AttributeUnit { get; set; }
        /// <summary>
        /// 数据分类
        /// </summary>
        public string AttributeDataType { get; set; }
        /// <summary>
        /// 文字符连接
        /// </summary>
        public string AttributeValueLink { get; set; }
        /// <summary>
        /// 数据源
        /// </summary>
        public string AttributeDataSource { get; set; }
        /// <summary>
        /// 计算公式
        /// </summary>
        public string AttributeCalComp { get; set; }
        /// <summary>
        /// 属性排序
        /// </summary>
        public int? AttributeSort { get; set; }

        /// <summary>
        /// 属性字典数组
        /// </summary>
        public string AttributeDict { get; set; }
    }
}
