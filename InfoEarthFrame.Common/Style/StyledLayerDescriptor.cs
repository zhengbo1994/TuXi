using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common.Style
{
    [XmlRoot(ElementName = @"StyledLayerDescriptor", Namespace = "http://www.opengis.net/sld")]
    public class StyledLayerDescriptor
    {
        [XmlAttribute(AttributeName = "version")]
        public string version = "1.0.0";

        [XmlElement(ElementName = @"UserLayer")]
        public List<UserLayer> UserLayers = null;

        [XmlElement(ElementName = @"NamedLayer")]
        public List<NamedLayer> NamedLayers = null;

        //public override string ToString()
        //{
        //    string title = string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");

        //    string content = string.Empty;
        //    foreach (UserLayer us in UserLayers)
        //    {
        //        content += us;
        //    }

        //    string content1 = string.Empty;
        //    foreach (NameLayer us in NameLayers)
        //    {
        //        content1 += us;
        //    }
        //    return string.Format("{0}<sld:StyledLayerDescriptor xmlns=\"http://www.opengis.net/sld\" xmlns:sld=\"http://www.opengis.net/sld\" xmlns:gml=\"http://www.opengis.net/gml\" xmlns:ogc=\"http://www.opengis.net/ogc\" version=\"1.0.0\">{1}{2}</sld:StyledLayerDescriptor>", title, content, content1);
        //}

    }

    public class UserLayer
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = null;

        public string Description = null;
        
        public FeatureTypeConstraints LayerFeatureConstraints = null;

        [XmlElement(ElementName = @"UserStyle")]
        public List<UserStyle> UserStyles =null;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Name) ? string.Empty : string.Format("<sld:Name>{0}</sld:Name>", Name);
        //    string desStr = string.IsNullOrEmpty(Description) ? string.Empty : string.Format("<sld:Description>{0}</sld:Description>", Description);

        //    string content = string.Empty;
        //    foreach (UserStyle us in UserStyles)
        //    {
        //        content += us;
        //    }
        //    return string.Format("<sld:UserLayer>{0}{1}{2}{3}</sld:UserLayer>", nameStr, desStr, LayerFeatureConstraints.ToString(), content);
        //}
    }

    public class NamedLayer
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name = string.Empty;

        public string Description = null;

        [XmlElement(ElementName = @"UserStyle")]
        public List<UserStyle> UserStyles = null;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Name) ? string.Empty : string.Format("<sld:Name>{0}</sld:Name>", Name);
        //    string desStr = string.IsNullOrEmpty(Description) ? string.Empty : string.Format("<sld:Description>{0}</sld:Description>", Description);

        //    string content = string.Empty;
        //    if (UserStyles != null)
        //    {
        //        foreach (UserStyle us in UserStyles)
        //        {
        //            content += us;
        //        }
        //    }
        //    return string.Format("<sld:NameLayer>{0}{1}{2}</sld:NameLayer>", nameStr, desStr, content);
        //}
    }


    public class FeatureTypeConstraints
    {
        [XmlElement(ElementName = @"FeatureTypeConstraint")]
        public List<FeatureTypeConstraint> FeatureTypeConstraint = null;

        //public override string ToString()
        //{
        //    return string.Format("<sld:LayerFeatureConstraints><sld:FeatureTypeConstraint /></sld:LayerFeatureConstraints>");
        //}
    }

    public class FeatureTypeConstraint
    {
    }

    public class UserStyle
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

        [XmlElement(ElementName = @"FeatureTypeStyle")]
        public List<FeatureTypeStyle> FeatureTypeStyles = null;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Name) ? string.Empty : string.Format("<sld:Name>{0}</sld:Name>", Name);
        //    string titleStr = string.IsNullOrEmpty(Title) ? string.Empty : string.Format("<sld:Title>{0}</sld:Title>", Title);
        //    string abstractStr = string.IsNullOrEmpty(Abstract) ? string.Empty : string.Format("<sld:Abstract>{0}</sld:Abstract>", Abstract);

        //    string featureTypeNameStr = string.Empty;
        //    foreach (FeatureTypeStyle us in FeatureTypeStyles)
        //    {
        //        featureTypeNameStr += us;
        //    }

        //    return string.Format("<sld:UserStyle>{0}{1}{2}{3}</sld:UserStyle>", nameStr, titleStr, abstractStr, featureTypeNameStr);
        //}
    }

    public class FeatureTypeStyle
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

        public string FeatureTypeName = null;

        [XmlElement(ElementName = @"SemanticTypeIdentifier")]
        public List<string> SemanticTypeIdentifiers = null;

        [XmlElement(ElementName = @"Rule")]
        public List<Rule> Rules = null;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Name) ? string.Empty : string.Format("<sld:Name>{0}</sld:Name>", Name);
        //    string titleStr = string.IsNullOrEmpty(Title) ? string.Empty : string.Format("<sld:Title>{0}</sld:Title>", Title);
        //    string abstractStr = string.IsNullOrEmpty(Abstract) ? string.Empty : string.Format("<sld:Abstract>{0}</sld:Abstract>", Abstract);
        //    string featureTypeNameStr = string.IsNullOrEmpty(FeatureTypeName) ? string.Empty : string.Format("<sld:FeatureTypeName>{0}</sld:FeatureTypeName>", FeatureTypeName);

        //    string semanticTypeIdentifierStr = string.Empty;
        //    foreach (string c in SemanticTypeIdentifiers)
        //    {
        //        semanticTypeIdentifierStr += c.ToString();
        //    }

        //    string ruleStr = string.Empty;
        //    foreach (Rule r in Rules)
        //    {
        //        ruleStr += r.ToString();
        //    }

        //    return string.Format("<sld:FeatureTypeStyle>{0}{1}{2}{3}{4}{5}</sld:FeatureTypeStyle>", nameStr, titleStr, abstractStr, featureTypeNameStr, semanticTypeIdentifierStr, ruleStr);
        //}

    }

    public class SemanticTypeIdentifier
    {
        public string Value = string.Empty;

        //public override string ToString()
        //{
        //    string nameStr = string.IsNullOrEmpty(Value) ? string.Empty : string.Format("<sld:SemanticTypeIdentifier>{0}</sld:SemanticTypeIdentifier>", Value);
        //    return nameStr;
        //}
    }
}

