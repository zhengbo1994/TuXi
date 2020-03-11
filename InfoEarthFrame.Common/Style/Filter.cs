using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common.Style
{
    /// <summary>
    /// 属性过滤
    /// </summary>
    public class Filter : Compare
    {
        public And And = null;
        public Or Or = null;
        public Not Not = null;
    }

    public class Compare
    {
        public PropertyIsEqualTo PropertyIsEqualTo = null;
        public PropertyIsNotEqualTo PropertyIsNotEqualTo = null;
        public PropertyIsLessThan PropertyIsLessThan = null;
        public PropertyIsGreaterThan PropertyIsGreaterThan = null;
        public PropertyIsLessThanOrEq PropertyIsLessThanOrEq = null;
        public PropertyIsGreaterThanO PropertyIsGreaterThanO = null;
        public PropertyIsLike PropertyIsLike = null;
        public PropertyIsNull PropertyIsNull = null;
        public PropertyIsBetween PropertyIsBetween = null;
        public BBox BBox = null;
    }

    public class And : Compare
    {

    }

    public class Or : Compare
    {

    }
    public class Not : Compare
    {

    }

    public class Expression
    {
        public LogicalOperatorEnum LogicalOperatorEnum = LogicalOperatorEnum.Default;

        public List<Condition> Conditions = new List<Condition>();

        public override string ToString()
        {
            string str = GetExpressionStr(LogicalOperatorEnum);

            string content = string.Empty;
            foreach (Condition c in Conditions)
            {
                content += c.ToString();
            }
            return string.Format(str, content);
        }

        private string GetExpressionStr(LogicalOperatorEnum logicalOperatorEnum)
        {
            string lStr = string.Format("{0}");
            switch (logicalOperatorEnum)
            {
                case LogicalOperatorEnum.Default:
                    break;
                case LogicalOperatorEnum.And:
                    lStr = string.Format("<ogc:And>{0}</ogc:And>");
                    break;
                case LogicalOperatorEnum.Or:
                    lStr = string.Format("<ogc:Or>{0}</ogc:Or>");
                    break;
                case LogicalOperatorEnum.Not:
                    lStr = string.Format("<ogc:Not>{0}</ogc:Not>");
                    break;
                default: break;
            }
            return lStr;
        }
    }

    public abstract class Condition
    {
        public string PropertyName = null;
        public string Literal = null;
    }

    public class PropertyIsEqualTo : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsEqualTo>{0}{1}</ogc:PropertyIsEqualTo>", pStr, lStr);
        //}
    }

    public class PropertyIsNotEqualTo : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsNotEqualTo>{0}{1}</ogc:PropertyIsNotEqualTo>", pStr, lStr);
        //}
    }

    public class PropertyIsLessThan : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsLessThan>{0}{1}</ogc:PropertyIsLessThan>", pStr, lStr);
        //}
    }

    public class PropertyIsGreaterThan : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsGreaterThan>{0}{1}</ogc:PropertyIsGreaterThan>", pStr, lStr);
        //}
    }

    public class PropertyIsLessThanOrEq : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsLessThanOrEq>{0}{1}</ogc:PropertyIsLessThanOrEq>", pStr, lStr);
        //}
    }

    public class PropertyIsGreaterThanO : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsGreaterThanO>{0}{1}</ogc:PropertyIsGreaterThanO>", pStr, lStr);
        //}
    }

    public class PropertyIsLike : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:Literal>{0}</ogc:Literal>", Literal);
        //    return string.Format("<ogc:PropertyIsLike>{0}{1}</ogc:PropertyIsLike>", pStr, lStr);
        //}
    }

    public class PropertyIsNull : Condition
    {
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    return string.Format("<ogc:PropertyIsNull>{0}{1}</ogc:PropertyIsNull>", pStr);
        //}
    }

    public class PropertyIsBetween : Condition
    {
        public string LowerBoundary = null;
        public string UpperBoundary = null;
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = string.Format("<ogc:LowerBoundary>{0}</ogc:LowerBoundary>", LowerBoundary);
        //    string uStr = string.Format("<ogc:UpperBoundary>{0}</ogc:UpperBoundary>", UpperBoundary);
        //    return string.Format("<ogc:PropertyIsBetween>{0}{1}{2}</ogc:PropertyIsBetween>", pStr, lStr, uStr);
        //}
    }


    /// <summary>
    /// 比较操作符
    /// </summary>
    public enum ComparisonOperatorEnum
    {
        PropertyIsEqualTo,
        PropertyIsNotEqualTo,
        PropertyIsLessThan,
        PropertyIsGreaterThan,
        PropertyIsLessThanOrEq,
        PropertyIsGreaterThanO,
        PropertyIsLike,
        PropertyIsNull,
        PropertyIsBetween
    }

    /// <summary>
    /// 逻辑操作符
    /// </summary>
    public enum LogicalOperatorEnum
    {
        Default, And, Or, Not
    }

    public class BBox : Condition
    {
        public Envelope Envelope = null;
        //public override string ToString()
        //{
        //    if (string.IsNullOrEmpty(PropertyName))
        //    {
        //        return string.Empty;
        //    }
        //    string pStr = string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    string lStr = Envelope == null ? string.Empty : Envelope.ToString();
        //    return string.Format("<ogc:BBox>{0}{1}</ogc:BBox>", pStr, lStr);
        //}
    }

    public class Envelope
    {
        [XmlAttribute(AttributeName = "srsName")]
        public string srsName = null;

        public string lowerCorner = null;
        public string upperCorner = null;

        //public override string ToString()
        //{
        //    string pStr = string.IsNullOrEmpty(srsName) ? string.Empty : string.Format("srsName=\"{0}\"", srsName);
        //    string lStr = string.Format("<ogc:lowerCorner>{0}</ogc:lowerCorner>", lowerCorner);
        //    string uStr = string.Format("<ogc:upperCorner>{0}</ogc:upperCorner>", upperCorner);
        //    return string.Format("<ogc:Envelope {0}>{1}{2}</ogc:Envelope>", lStr, uStr);
        //}
    }

    /// <summary>
    /// 地理操作符
    /// </summary>
    public enum SpatialOperatorEnum
    {
        Equals,
        Disjoint,
        Touches,
        Within,
        Overlaps,
        Crosses,
        Intersects,
        Contains,
        DWithin,
        Beyond,
        BBox
    }
}
