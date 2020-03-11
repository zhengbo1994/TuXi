using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.OGR;

namespace InfoEarthFrame.Common.ShpUtility
{
    public class ShpReader
    {
        /// <summary>
        /// 默认构造函数，注册GDAL
        /// </summary>
        static ShpReader()
        {
            // 为了支持中文路径，请添加下面这句代码  
            OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
            OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");
            OSGeo.OGR.Ogr.RegisterAll();
        }

        #region 属性
        private readonly DataSource _ogrDataSource = null;
        private readonly OSGeo.OGR.Layer _ogrLayer = null;
        #endregion

        #region Constructors
        /// <summary>
        /// 打开源数据指定层名的Layer
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="layerName"></param>
        public ShpReader(string filename, string layerName)
        {
            _ogrDataSource = OSGeo.OGR.Ogr.Open(filename, 1);
            _ogrLayer = _ogrDataSource.GetLayerByName(layerName);
        }

        /// <summary>
        /// 打开源数据指定层号的Layer
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="layerNum"></param>
        public ShpReader(string filename, int layerNum)
        {
            _ogrDataSource = OSGeo.OGR.Ogr.Open(filename, 0);
            _ogrLayer = _ogrDataSource.GetLayerByIndex(layerNum);
        }
        /// <summary>
        /// 打开源数据的第一个图层
        /// </summary>
        /// <param name="filename"></param>
        public ShpReader(string filename)
            : this(filename, 0)
        {
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// 返回源数据feature数
        /// </summary>
        /// <returns></returns>
        public int GetFeatureCount()
        {
            return _ogrLayer.GetFeatureCount(1);
        }

        /// <summary>
        /// Gets the connection ID of the datasource
        /// </summary>
        public string ConnectionID
        {
            get
            {
                return string.Format("Data Source={0};Layer{1}", _ogrDataSource.name, _ogrLayer.GetName());
            }
        }

        /// <summary>
        /// 判断数据是否是有效数据
        /// </summary>
        /// <returns></returns>
        public bool IsValidDataSource()
        {
            _ogrLayer.ResetReading();
            int featureCount = _ogrLayer.GetFeatureCount(1);
            if (featureCount <= 0)
            {
                return false;
            }
            Feature pFeature = _ogrLayer.GetNextFeature();
            if (pFeature == null)
            {
                return false;
            }
            Geometry pGeom = pFeature.GetGeometryRef();
            if (pGeom == null)
            {
                return false;
            }
            if (pGeom.GetGeometryType() == wkbGeometryType.wkbNone || pGeom.GetGeometryType() == wkbGeometryType.wkbUnknown)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 返回数据的数据类型
        /// </summary>
        /// <returns></returns>
        public string GetGeomType()
        {
            _ogrLayer.ResetReading();
            int pFeatureCount = _ogrLayer.GetFeatureCount(1);
            if (pFeatureCount <= 0)
            {
                return wkbGeometryType.wkbNone.ToString();
            }
            Feature pFeature = _ogrLayer.GetNextFeature();
            if (pFeature == null)
            {
                return wkbGeometryType.wkbNone.ToString();
            }
            Geometry geom = pFeature.GetGeometryRef();
            if (geom == null)
            {
                return wkbGeometryType.wkbNone.ToString();
            }
            string geomType = geom.GetGeometryType().ToString();
            return geomType.Substring(3, geomType.Length - 3).ToUpper();
        }

        /// <summary>
        /// 得到数据空间参考的wkt字符串
        /// </summary>
        /// <returns></returns>
        public string GetSridWkt()
        {
            string wkt = String.Empty;
            OSGeo.OSR.SpatialReference pSpatialReference = _ogrLayer.GetSpatialRef();
            if (pSpatialReference != null)
            {
                pSpatialReference.ExportToWkt(out wkt);
            }
            return wkt;
        }

        public int GetSrid()
        {
            int srid = -1;
            OSGeo.OSR.SpatialReference pSpatialReference = _ogrLayer.GetSpatialRef();
            if (pSpatialReference != null)
            {
                srid = pSpatialReference.AutoIdentifyEPSG();
            }
            return srid;
        }

        /// <summary>
        /// 得到shp文件的维度
        /// </summary>
        /// <returns></returns>
        public int GetDeminsion()
        {
            int deminsion = -1;
            Feature pFeature = _ogrLayer.GetFeature(0);
            Geometry pGeom = pFeature.GetGeometryRef();
            deminsion = pGeom.GetCoordinateDimension();
            return deminsion;
        }

        /// <summary>
        /// 得到相应序号图层的BBox
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Dictionary<string,double> GetLayerBBox()
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            try
            {
                Envelope env = new Envelope();
                int i=_ogrLayer.GetExtent(env,1);
                dic.Add("MinX",env.MinX);
                dic.Add("MaxX", env.MaxX);
                dic.Add("MinY", env.MinY);
                dic.Add("MaxY", env.MaxY);
            }
            catch(Exception ex)
            {

            }

            return dic;
        }

        /// <summary>
        /// 得到shp文件属性名和属性类型的字典表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetAttributeType()
        {
            Dictionary<string, string> attr = new Dictionary<string, string>();
            Feature pFeature = _ogrLayer.GetFeature(0);
            int pFieldCount = pFeature.GetFieldCount();
            for (int i = 0; i < pFieldCount; i++)
            {
                string pFieldName = pFeature.GetFieldDefnRef(i).GetName().Trim();
                string pFieldType = pFeature.GetFieldDefnRef(i).GetFieldType().ToString().Trim();
                attr.Add(pFieldName, pFieldType);
            }
            return attr;
        }

        /// <summary>
        /// 得到一个feature所有的属性名和属性值的字典表
        /// </summary>
        /// <param name="featureID">feature的ID号</param>
        /// <returns></returns>
        public Dictionary<string, string> GetOneFeatureAttribute(int featureID)
        {
            Dictionary<string, string> attr = new Dictionary<string, string>();
            Feature pFeature = _ogrLayer.GetFeature(featureID);
            int pFieldCount = pFeature.GetFieldCount();
            for (int i = 0; i < pFieldCount; i++)
            {
                string pFieldName = pFeature.GetFieldDefnRef(i).GetName().Trim();
                string pFieldValue = pFeature.GetFieldAsString(i).Trim();
                attr.Add(pFieldName, pFieldValue);
            }
            return attr;
        }

        /// <summary>
        /// 得到一个feature选择的属性名和属性值的字典表
        /// </summary>
        /// <param name="featureID">feature的ID号</param>
        /// <param name="selAttr">选择的属性名</param>
        /// <returns></returns>
        public Dictionary<string, string> GetOneFeatureAttribute(int featureID, List<string> selAttr)
        {
            Dictionary<string, string> attr = new Dictionary<string, string>();
            Feature pFeature = _ogrLayer.GetFeature(featureID);
            int pFieldCount = pFeature.GetFieldCount();
            for (int i = 0; i < pFieldCount; i++)
            {
                string pFieldName = pFeature.GetFieldDefnRef(i).GetName().Trim();
                if (!selAttr.Contains(pFieldName))
                {
                    continue;
                }
                string pFieldValue = pFeature.GetFieldAsString(i).Trim();
                attr.Add(pFieldName, pFieldValue);
            }
            return attr;
        }

        /// <summary>
        /// 得到一个feature所有的属性名和属性值的长度，步长，类型信息
        /// </summary>
        /// <param name="featureID"></param>
        /// <returns></returns>
        public List<AttributeModel> GetOneFeatureAttributeModel(int featureID)
        {
            List<AttributeModel> lstAttr =new  List<AttributeModel>();
            Feature pFeature = _ogrLayer.GetFeature(featureID);
            int pFieldCount = pFeature.GetFieldCount();
            for (int i = 0; i < pFieldCount; i++)
            {
                string pFieldName = pFeature.GetFieldDefnRef(i).GetName().Trim();
                string pFieldValue = pFeature.GetFieldAsString(i).Trim();
                AttributeModel attr = new AttributeModel();
                attr.AttributeName = pFieldName;
                attr.AttributeType = pFeature.GetFieldDefnRef(i).GetFieldType();
                attr.AttributeWidth = pFeature.GetFieldDefnRef(i).GetWidth();
                attr.AttributePrecision = pFeature.GetFieldDefnRef(i).GetPrecision();

                lstAttr.Add(attr);
            }
            return lstAttr;
        }

        /// <summary>
        /// 得到一个feature的Geometry的wkt字符串
        /// </summary>
        /// <param name="featureID">feature的ID号</param>
        /// <returns></returns>
        public string GetOneFeatureGeomWkt(int featureID)
        {
            string geomWkt = String.Empty;
            Feature pFeature = _ogrLayer.GetFeature(featureID);
            Geometry pGeom = pFeature.GetGeometryRef();
            pGeom.ExportToWkt(out geomWkt);
            return geomWkt;
        }

        /// <summary>
        /// 关闭ShpReader
        /// </summary>
        public void Close()
        {
            if (_ogrLayer != null)
            {
                _ogrLayer.Dispose();
                _ogrDataSource.Dispose();
            }
        }  
        #endregion
    }
}
