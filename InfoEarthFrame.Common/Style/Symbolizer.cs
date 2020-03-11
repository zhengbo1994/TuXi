using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common.Style
{
    public abstract class Symbolizer
    {

    }

    public class PointSymbolizer : Symbolizer
    {
        public Graphic Graphic = null;

        //public override string ToString()
        //{
        //    string str = Graphic == null ? string.Empty : Graphic.ToString();
        //    return string.Format("<sld:PointSymbolizer>{0}</sld:PointSymbolizer>", str);
        //}
    }

    public class LineSymbolizer : Symbolizer
    {
        public Stroke Stroke = null;

        //public override string ToString()
        //{
        //    string str = Stroke == null ? string.Empty : Stroke.ToString();
        //    return string.Format("<sld:LineSymbolizer>{0}</sld:LineSymbolizer>", str);
        //}
    }
    public class PolygonSymbolizer : Symbolizer
    {
        public Stroke Stroke = null;

        public Fill Fill = null;

        //public override string ToString()
        //{
        //    string str = Stroke == null ? string.Empty : Stroke.ToString();
        //    string fillStr = Fill == null ? string.Empty : Fill.ToString();
        //    return string.Format("<sld:PolygonSymbolizer>{0}{1}</sld:PolygonSymbolizer>", str, fillStr);
        //}
    }
    public class TextSymbolizer : Symbolizer
    {

    }
    public class RasterSymbolizer : Symbolizer
    {

    }

    public class Label
    {
        public string PropertyName = null;

        //public override string ToString()
        //{
        //    string str = string.IsNullOrEmpty(PropertyName) ? string.Empty : string.Format("<ogc:PropertyName>{0}</ogc:PropertyName>", PropertyName);
        //    return string.Format("<sld:Label>{0}</sld:Label>", PropertyName, str);
        //}
    }

    public class Font
    {
        [XmlElement(ElementName = @"CssParameter")]
        public List<CssParameter> CssParameters = null;

        //public override string ToString()
        //{
        //    string content = string.Empty;
        //    foreach (CssParameter pa in CssParameters)
        //    {
        //        content += pa.ToString();
        //    }
        //    return string.Format("<sld:Font>{0}</sld:Font>", content);
        //}

    }

    public class Fill
    {
        public GraphicFill GraphicFill = null;

        [XmlElement(ElementName = @"CssParameter")]
        public List<CssParameter> CssParameters = null;

        //public override string ToString()
        //{
        //    string content = string.Empty;
        //    foreach (CssParameter pa in CssParameters)
        //    {
        //        content += pa.ToString();
        //    }

        //    string str = GraphicFill == null ? string.Empty : GraphicFill.ToString();
        //    return string.Format("<sld:Fill>{0}{1}</sld:Fill>", str, content);
        //}
    }

    public class GraphicFill
    {
        public Graphic Graphic = null;
        //public override string ToString()
        //{
        //    string str = Graphic == null ? string.Empty : Graphic.ToString();
        //    return string.Format("<sld:GraphicFill>{0}{1}</sld:GraphicFill>", str);
        //}
    }

    public class Stroke
    {
        [XmlElement(ElementName = @"CssParameter")]
        public List<CssParameter> CssParameters = null;

        public GraphicStroke GraphicStroke = null;

        //public override string ToString()
        //{
        //    string content = string.Empty;
        //    foreach (CssParameter pa in CssParameters)
        //    {
        //        content += pa.ToString();
        //    }
        //    return string.Format("<sld:Stroke>{0}</sld:Stroke>", content);
        //}
    }

    public class GraphicStroke
    {
        public Graphic Graphic = null;
    }

    public class CssParameter
    {
        [XmlAttribute(AttributeName = "name")]
        public string name;

        [XmlText]
        public string value;

        //public override string ToString()
        //{
        //    return string.Format("<sld:CssParameter name=\"{0}\">{1}</sld:CssParameter>", name, value);
        //}
    }

    public class Mark
    {
        public string WellKnownName = null;

        public Fill Fill = null;

        public Stroke Stroke = null;

        //public override string ToString()
        //{
        //    string str = string.IsNullOrEmpty(WellKnownName) ? string.Empty : string.Format("<sld:WellKnownName>{0}</sld:WellKnownName>", WellKnownName);
        //    string fillStr = Fill == null ? string.Empty : Fill.ToString();
        //    return string.Format("<sld:Mark>{0}{1}</sld:Mark>", str, fillStr);
        //}
    }

    public class ExternalGraphic
    {
        public OnlineResource OnlineResource = null;
        public string Format = "image/png";
        //public override string ToString()
        //{
        //    string onlineResourceStr = string.IsNullOrEmpty(OnlineResource) ? string.Empty : string.Format("<sld:OnlineResource xmlns:xlink=\"http://www.w3.org/1999/xlink\" xlink:type=\"simple\" xlink:href=\"{0}\" />", OnlineResource);
        //    string formatStr = string.IsNullOrEmpty(Format) ? string.Empty : string.Format("<sld:Format>{0}</sld:Format>", Format);
        //    return string.Format("<sld:ExternalGraphic>{0}{1}</sld:ExternalGraphic>", onlineResourceStr, formatStr);
        //}
    }

    public class OnlineResource
    {
        //[XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/1999/xlink")]
        //public string xlink = @"http://www.w3.org/1999/xlink";

        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/1999/xlink")]
        public string type = @"simple";

        [XmlAttribute(AttributeName = "href", Namespace = "http://www.w3.org/1999/xlink")]
        public string href = null;
    }


    public class Graphic
    {
        public Mark Mark = null;

        public ExternalGraphic ExternalGraphic = null;

        public string Size = null;

        public string Rotation = null;

        //public override string ToString()
        //{
        //    string markStr = Mark == null ? string.Empty : Mark.ToString();
        //    string externalGraphicStr = ExternalGraphic == null ? string.Empty : ExternalGraphic.ToString();
        //    string sizeStr = string.IsNullOrEmpty(Size) ? string.Empty : string.Format("<sld:Size>{0}</sld:Size>", Size);
        //    string rotationStr = string.IsNullOrEmpty(Rotation) ? string.Empty : string.Format("<sld:Rotation>{0}</sld:Rotation>", Size);
        //    return string.Format("<sld:Graphic>{0}{1}{2}{3}</sld:Graphic>", markStr, externalGraphicStr, sizeStr, rotationStr);
        //}
    }
}
