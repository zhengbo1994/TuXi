using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.ServerInterfaceApp.Dtos
{
    public class CreateLayerDto
    {
        /// <summary>
        /// 图层编号
        /// </summary>
        public string LayerId { get; set; }
        /// <summary>
        /// 新图层名称
        /// </summary>
        public string  NewLayerName { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        public string AttributeName { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public List<Element> AttributeValues { get; set; }
    }

    public class Element
    {
        /// <summary>
        /// 要素编号
        /// </summary>
        public string ElementId { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string AttributeValue { get; set; }
    }
}
