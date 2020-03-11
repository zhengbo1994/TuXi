using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.ShpUtility
{
    public class AttributeModel
    {
        private string _attributeName;
        /// <summary>
        /// 属性字段名
        /// </summary>
        public string AttributeName
        {
            get { return _attributeName; }
            set { _attributeName = value; }
        }
        private OSGeo.OGR.FieldType _attributeType;
        /// <summary>
        /// 属性字段类型
        /// </summary>
        public OSGeo.OGR.FieldType AttributeType
        {
            get { return _attributeType; }
            set { _attributeType = value; }
        }

        private string _attributeTypeName;

        /// <summary>
        /// 属性字段名称
        /// </summary>
        public string AttributeTypeName
        {
            get { return _attributeTypeName; }
            set { _attributeTypeName = value; }
        }

        private int _attributeWidth;
        /// <summary>
        /// 属性字段长度
        /// </summary>
        public int AttributeWidth
        {
            get { return _attributeWidth; }
            set { _attributeWidth = value; }
        }

        private int _attributePrecision;

        /// <summary>
        ///精度值
        /// </summary>
        public int AttributePrecision
        {
            get { return _attributePrecision; }
            set { _attributePrecision = value; }
        }
        private int _attributeApproxOK = 0;
        /// <summary>
        /// 属性是否可以近似
        /// </summary>
        public int AttributeApproxOK
        {
            get { return _attributeApproxOK; }
            set { _attributeApproxOK = value; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AttributeModel()
        {
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="attName"></param>
        /// <param name="attType"></param>
        /// <param name="attWidth"></param>
        /// <param name="attApprox"></param>
        public AttributeModel(string attName, OSGeo.OGR.FieldType attType, int attWidth, int attApprox)
        {
            _attributeApproxOK = attApprox;
            _attributeType = attType;
            _attributeWidth = attWidth;
            _attributeName = attName;
        }
    }
}
