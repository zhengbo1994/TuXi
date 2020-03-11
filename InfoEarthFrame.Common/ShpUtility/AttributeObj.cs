using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.ShpUtility
{
    public class AttributeObj
    {
        private int _oId;
        /// <summary>
        /// id
        /// </summary>
        public int OId
        {
            get { return _oId; }
            set { _oId = value; }
        }
        private Dictionary<string, string> _attributeValue;
        /// <summary>
        /// 属性Dic
        /// </summary>
        public Dictionary<string, string> AttributeValue
        {
            get { return _attributeValue; }
            set { _attributeValue = value; }
        }
    }
}
