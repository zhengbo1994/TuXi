using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OSGeo.OGR;
using InfoEarthFrame.Common.ShpUtility;

namespace InfoEarthFrame.Common.ShpUtility
{
    public class ShpWriter
    {
        static ShpWriter()
        {
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            OSGeo.OGR.Ogr.RegisterAll();
        }

        private string _shpFileName;
        private List<AttributeModel> _lstAttribute;
        private OSGeo.OGR.wkbGeometryType _geoType;
        private List<string> _lstWkt;
        private List<AttributeObj> _lstAttributeObj;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="nFileName"></param>
        public ShpWriter(string nFileName)
        {
            _shpFileName = nFileName;            
        }
        /// <summary>
        /// 写shp文件
        /// </summary>
        /// <param name="lstAttribute"></param>
        /// <param name="geometryType"></param>
        /// <param name="lstWkt"></param>
        /// <param name="lstAttributeObj"></param>
        /// <param name="srsWkt"></param>
        /// <returns></returns>
        public bool DoExport(List<AttributeModel> lstAttribute, OSGeo.OGR.wkbGeometryType geometryType, List<string> lstWkt, List<AttributeObj> lstAttributeObj, string srsWkt)
        {
            _lstAttribute = lstAttribute;
            _geoType = geometryType;
            _lstWkt = lstWkt;
            _lstAttributeObj = lstAttributeObj;

            try
            {
                //注册
                string pszDriverName = "ESRI Shapefile";

                //调用对Shape文件读写的Driver接口
                OSGeo.OGR.Driver poDriver = OSGeo.OGR.Ogr.GetDriverByName(pszDriverName);
                if (poDriver == null)
                    throw new Exception("Driver Error");

                //用此Driver创建Shape文件
                OSGeo.OGR.DataSource poDS;
                poDS = poDriver.CreateDataSource(_shpFileName, null);
                if (poDS == null)
                    throw new Exception("DataSource Creation Error");

                //定义坐标系
                OSGeo.OSR.SpatialReference srs = new OSGeo.OSR.SpatialReference(srsWkt);
                //创建层Layer
                OSGeo.OGR.Layer poLayer = null;
                string layerName = Path.GetFileNameWithoutExtension(_shpFileName);
                poLayer = poDS.CreateLayer(layerName, srs, _geoType, null);
                
                if (poLayer == null)
                    throw new Exception("Layer Creation Failed");

                //创建属性列
                foreach (AttributeModel att in _lstAttribute)
                {
                    OSGeo.OGR.FieldDefn oField = new OSGeo.OGR.FieldDefn(att.AttributeName, att.AttributeType);
                    //if (att.AttributeWidth > 0)
                    //{
                        oField.SetWidth(att.AttributeWidth);
                    //}
                    oField.SetPrecision(att.AttributePrecision);


                    poLayer.CreateField(oField, att.AttributeApproxOK);
                }

                //创建一个Feature,一个Geometry
                OSGeo.OGR.Feature poFeature = new OSGeo.OGR.Feature(poLayer.GetLayerDefn());
                OSGeo.OGR.wkbGeometryType wkbGeotype = OSGeo.OGR.wkbGeometryType.wkbGeometryCollection;
                OSGeo.OGR.Geometry geo = new OSGeo.OGR.Geometry(wkbGeotype);

                for (int i = 0; i < _lstWkt.Count; i++)
                {                    
                    foreach (KeyValuePair<string, string> item in _lstAttributeObj[i].AttributeValue)
                    {
                        poFeature.SetField(item.Key, item.Value);
                    }
                    geo = OSGeo.OGR.Geometry.CreateFromWkt(_lstWkt[i]);
                    poFeature.SetGeometry(geo);
                    poLayer.CreateFeature(poFeature);
                }
                //关闭文件读写
                poFeature.Dispose();
                poDS.Dispose();

                return true;
            }
            catch (System.Exception e)
            {
                return false;
                //throw new Exception(e.Message);
            }
        }
    }
}
