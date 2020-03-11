using InfoEarthFrame.Common.ShpUtility;
using iTelluro.DataTools.Utility.DLG;
using iTelluro.DataTools.Utility.Geometries;
using iTelluro.Explorer.Vector;
using iTelluro.Explorer.Vector.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace InfoEarthFrame.ServerInterfaceApp
{

    public class District
    {
        public string Code = string.Empty;
        public string Name = string.Empty;
        public List<string> WKTs = new List<string>();
    }

    /// <summary>
    /// 行政区划空间范围提供类
    /// </summary>
    public class DistrictSpatialProvider : IDisposable
    {

        private List<SpatialAttributeObj> _geoObjs = null;
        //private DistrictLevel _level = DistrictLevel.None;
        private string _level = "";
        private string _shpDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\行政区划");
        private long _id = 0;

        public DistrictSpatialProvider(string level)
        {

            _level = level;
            VectorReader reader = null;
            switch (level)
            {
                case "省":
                    reader = new VectorReader(Path.Combine(_shpDataDir, "Province.shp"));
                    break;
                case "市":
                    reader = new VectorReader(Path.Combine(_shpDataDir, "City.shp"));
                    break;
                case "县":
                    reader = new VectorReader(Path.Combine(_shpDataDir, "County.shp"));
                    break;
                default:
                    break;
            }
            _geoObjs = reader.GetAllSpatialAttributeObj();
            reader.Dispose();
        }

        /// <summary>
        /// 根据行政区划编号获取行政区划空间范围多边形
        /// </summary>
        /// <param name="DistrictCodes"></param>
        /// <returns></returns>
        public List<District> GetDistrictPolygon(List<string> DistrictCodes)
        {
            List<District> polys = new List<District>();
            if (_level == "" || _geoObjs == null || DistrictCodes == null || DistrictCodes.Count == 0)
            {
                return polys;
            }
            for (int i = 0; i < _geoObjs.Count; i++)
            {
                string code = _geoObjs[i].AttriValue["COR_NUMBER"].Trim();
                string name = _geoObjs[i].AttriValue["COR_NAME"].Trim();
                for (int j = 0; j < DistrictCodes.Count; j++)
                {
                    if (code == DistrictCodes[j].Trim())
                    {
                        District district = new District();
                        district.Code = code;
                        district.Name = name;
                        List<Polygon> polyList = this.ConvertFeature2Polygon(_geoObjs[i].SpatialValue);
                        if (polyList != null && polyList.Count > 0)
                        {
                            List<string> list = new List<string>();
                            for (int k = 0; k < polyList.Count; k++)
                            {
                                polyList[k].Fill = false;
                                polyList[k].Outline = true;
                                polyList[k].LineWidth = 4;
                                polyList[k].OutlineColor = Color.FromArgb(180, 0, 255, 255);
                                polyList[k].Tag = code;

                                list.Add(Polygon2WKT(polyList[k]));
                            }
                            district.WKTs = list;
                        }
                        polys.Add(district);
                    }
                }
            }
            return polys;
        }

        /// <summary>
        /// 将OSGeo.OGR.Feature转为Polygon
        /// </summary>
        /// <param name="feature">OSGeo.OGR.Feature对象</param>
        /// <returns>Polygon对象</returns>
        private List<Polygon> ConvertFeature2Polygon(OSGeo.OGR.Feature feature)
        {
            List<Polygon> polys = new List<Polygon>();
            if (feature != null)
            {
                ConvertFromFeature convert = new ConvertFromFeature();
                GeneralGeometry geo = convert.Feature2Geometry(feature);
                switch (geo.GeometryType)
                {
                    case WKBGeometryType.wkbPolygon:
                        Polygon poly = geo.MyPolygon;
                        poly.Id = _id++;
                        polys.Add(poly);
                        break;
                    case WKBGeometryType.wkbMultiPolygon:
                        polys = geo.MyMultiPolygon;
                        for (int i = 0; i < polys.Count; i++)
                        {
                            polys[i].Id = _id++;
                        }
                        break;
                    default:
                        break;
                }
            }
            return polys;
        }

        private string Polygon2WKT(Polygon polygon)
        {
            string wkt = string.Empty;

            if (polygon != null)
            {
                wkt = polygon.OuterBoundary.Points.Aggregate("POLYGON ((", (current, t) => current + (t.X.ToString() + ' ' + t.Y.ToString() + ','));
                wkt = wkt.Substring(0, wkt.Length - 1);
                wkt += "))";
            }

            return wkt;
        }

        public void Dispose()
        {
            _geoObjs = null;
        }
    }
}
