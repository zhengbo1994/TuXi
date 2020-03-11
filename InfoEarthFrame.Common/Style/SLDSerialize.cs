using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InfoEarthFrame.Common.Style
{
    /// <summary>
    /// SLD序列化类
    /// </summary>
    public class SLDSerialize
    {
        /// <summary>
        /// StyledLayerDescriptor对象序列化成xml字符串
        /// </summary>
        /// <param name="obj">StyledLayerDescriptor对象</param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            Encoding encoding = Encoding.UTF8;

            if (obj == null)
                throw new ArgumentNullException("obj");
            if (encoding == null)
                throw new ArgumentNullException("encoding");

            string xml = "";
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = encoding;

                    //OmitXmlDeclaration表示不生成声明头，默认是false，OmitXmlDeclaration为true，会去掉<?xml version="1.0" encoding="UTF-8"?>
                    //settings.OmitXmlDeclaration = true;

                    XmlWriter writer = XmlWriter.Create(stream, settings);

                    //强制指定命名空间，覆盖默认的命名空间，可以添加多个，如果要在xml节点上添加指定的前缀，可以在跟节点的类上面添加[XmlRoot(Namespace = "http://www.w3.org/2001/XMLSchema-instance", IsNullable = false)]，Namespace指定哪个值，xml节点添加的前缀就是哪个命名空间(这里会添加ceb)
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add("ogc", "http://www.opengis.net/ogc");
                    namespaces.Add("gml", "http://www.opengis.net/gml");
                    namespaces.Add("sld", "http://www.opengis.net/sld");
                    namespaces.Add("xlink", "http://www.w3.org/1999/xlink");

                    XmlSerializer serializer = new XmlSerializer(obj.GetType());
                    serializer.Serialize(writer, obj, namespaces);
                    writer.Close();

                    stream.Position = 0;
                    using (StreamReader reader = new StreamReader(stream, encoding))
                    {
                        xml = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return xml;
        }

        /// <summary>
        /// xml字符串反序列化StyledLayerDescriptor对象
        /// </summary>
        /// <param name="type">实体类型</param>
        /// <param name="sldXml">xml字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string sldXml)
        {
            byte[] array = Encoding.UTF8.GetBytes(sldXml);
            MemoryStream stream = new MemoryStream(array);
            //Stream stream = ms;
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
    }
}
