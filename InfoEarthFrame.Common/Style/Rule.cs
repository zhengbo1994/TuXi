using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common.Style
{
    /// <summary>
    /// 规则
    /// </summary>
    public class Rule
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = null;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title = null;

        /// <summary>
        /// 描述
        /// </summary>
        public string Abstract = null;

        /// <summary>
        /// 最小比例尺
        /// </summary>
        public string MinScaleDenominator = null;

        /// <summary>
        /// 最大比例尺
        /// </summary>
        public string MaxScaleDenominator = null;

        [XmlElement(ElementName = @"Filter", Namespace = "http://www.opengis.net/ogc")]
        /// <summary>
        /// 过滤条件
        /// </summary>
        public Filter Filter = null;

        /// <summary>
        /// 特征
        /// </summary>
        public PointSymbolizer PointSymbolizer = null;

        [XmlElement(ElementName = @"LineSymbolizer")]
        public List<LineSymbolizer> LineSymbolizer = null;

        public PolygonSymbolizer PolygonSymbolizer = null;
        public TextSymbolizer TextSymbolizer = null;
        public RasterSymbolizer RasterSymbolizer = null;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Name) ? string.Empty : string.Format("<sld:Name>{0}</sld:Name>", Name);
        //    string titleStr = string.IsNullOrEmpty(Title) ? string.Empty : string.Format("<sld:Title>{0}</sld:Title>", Title);
        //    string abstractStr = string.IsNullOrEmpty(Abstract) ? string.Empty : string.Format("<sld:Abstract>{0}</sld:Abstract>", Abstract);
        //    string minScaleStr = string.IsNullOrEmpty(MinScaleDenominator) ? string.Empty : string.Format("<sld:MinScaleDenominator>{0}</sld:MinScaleDenominator>", MinScaleDenominator);
        //    string maxScaleStr = string.IsNullOrEmpty(MaxScaleDenominator) ? string.Empty : string.Format("<sld:MaxScaleDenominator>{0}</sld:MaxScaleDenominator>", MaxScaleDenominator);
        //    string filterStr = Filter == null ? string.Empty : Filter.ToString();
        //    string symbolizerStr = PointSymbolizer == null ? string.Empty : PointSymbolizer.ToString();

        //    return string.Format("<sld:Rule>{0}{1}{2}{3}{4}{5}{6}</sld:Rule>", nameStr, titleStr, abstractStr, minScaleStr, maxScaleStr, filterStr, symbolizerStr);
        //}

        public string Count= null;
    }
}
