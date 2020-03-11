using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTelluro.DataTools.Utility.GIS;
using OSGeo.GDAL;
using iTelluro.DataTools.Utility.Marshal;
using InfoEarthFrame.Common.ShpUtility;
using System.IO;
using System.Configuration;

namespace InfoEarthFrame.Common
{
    public class CoordTransformHelper
    {
        public CoordTransformHelper()
        {

        }

        /// <summary>
        /// 控制点转换
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="tragetFile"></param>
        /// <param name="multiPoint"></param>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public bool ControlPointTransform(string srcFile, string traFile, string wkt, string[] multiPoint)
        {
            bool success = false;
            if (multiPoint.Length <= 0)
            {
                return success;
            }
            else
            {
                try
                {
                    //"[[12,21,23,24],[45,56,21,2],{45,56,21,2},{45,56,21,2}]";
                    //if(multiPoint.Contains("},{"))
                    //{
                    //    multiPoint = multiPoint.Replace("},{","#");
                    //}
                    //multiPoint = multiPoint.TrimStart('{').TrimStart('[').TrimEnd('}').TrimEnd(']');
                    //string[] point = multiPoint.Split('#');

                    GCP[] pGCPS = new GCP[multiPoint.Length];
                    for (int i = 0; i < multiPoint.Length; i++)
                    {
                        string[] point = multiPoint[i].Split(',');
                        GCP gcp = new GCP(Double.Parse(point[0]), Double.Parse(point[1]), 0.0, Double.Parse(point[2]), Double.Parse(point[3]), "", i.ToString());
                        pGCPS[i] = gcp;
                    }

                    GCPWarpMethod warpMethod = GCPWarpMethod.Order1;

                    InfoNotify info = (i) => { System.Diagnostics.Debug.WriteLine(i);};
                    ErrNotify err = (i) => { System.Diagnostics.Debug.WriteLine(i); };
                    wkt = GetWKTText(wkt);
                    success = GCPWarp.VectorWarp(srcFile, traFile, pGCPS, warpMethod, wkt, "UTF-8", info, err);
                }
                catch(Exception ex)
                {
                }
                return success;
            }
        }

        /// <summary>
        /// 七参数转换
        /// </summary>
        /// <param name="srcFile">需要转换的Shp文件</param>
        /// <param name="toWktStr">转换之后的坐标参考字符串</param>
        /// <param name="args">七参数数组</param>
        /// <returns></returns>
        public bool SevenParameterTransform(string srcFile,string tarFile, string toWktStr, string[] args)
        {
            /*算法逻辑
            1、先将矢量文件统一转换成WGS84坐标参考。
            2、将矢量文件进行7参数转换，只有在WGS84坐标参考下才能进行换算。
            3、将转换之后的文件再进行一次坐标转换，转换成需要的坐标参考。
            */
            bool success = false;
            string tempPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ShpTemp", Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempPath);

            try
            {

                //输出文件路径
                //tarFile = Path.Combine(Path.GetDirectoryName(srcFile), Path.GetFileNameWithoutExtension(srcFile) + "_T" + Path.GetExtension(srcFile));
                if (File.Exists(srcFile))
                {
                    //得到WGS84坐标参考字符串
                    string strkk = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563,AUTHORITY[\"EPSG\",7030]],AUTHORITY[\"EPSG\",6326]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",8901]],UNIT[\"DMSH\",0.0174532925199433,AUTHORITY[\"EPSG\",9108]],AXIS[\"Lat\",NORTH],AXIS[\"Long\",EAST],AUTHORITY[\"EPSG\",4326]]";
                    //string strkk = GetWKTText(toWktStr);
                    //OSGeo.OSR.SpatialReference sr = new OSGeo.OSR.SpatialReference(strwkt);
                    //sr.SetWellKnownGeogCS("EPSG:4326");
                    //string strkk = string.Empty;
                    //sr.ExportToWkt(out strkk);

                    //判断原始文件是否是WGS84坐标参考，如果是就不进行转换。
                    ShpReader shpReader = new ShpReader(srcFile);
                    string str = shpReader.GetSridWkt();

                    string tmpFile = string.Empty;
                    if (!str.Contains("WGS_1984"))
                    {
                        tmpFile = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(srcFile) + "_T" + Path.GetExtension(srcFile));
                        iTelluro.DataTools.Utility.GIS.CoordTransform.Ogrtransform(str, strkk, srcFile, tmpFile);
                        srcFile = tmpFile;
                    }

                    //进行7参数转换
                    string tmpFile2 = string.Empty;
                    if (File.Exists(srcFile) && args != null && args.Length == 7)
                    {
                        string argStr = string.Join(",", args);
                        string wkt = "GEOGCS[\"WGS84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS84\",6378137,298.257223563,AUTHORITY[\"EPSG\",7030]],TOWGS84[" + argStr + "],AUTHORITY[\"EPSG\",6326]],PRIMEM[\"Greenwich\",0,AUTHORITY[\"EPSG\",8901]],UNIT[\"DMSH\",0.0174532925199433,AUTHORITY[\"EPSG\",9108]],AXIS[\"Lat\",NORTH],AXIS[\"Long\",EAST],AUTHORITY[\"EPSG\",4326]]";

                        ShpReader shpReader2 = new ShpReader(srcFile);
                        string str2 = shpReader2.GetSridWkt();

                        tmpFile2 = Path.Combine(tempPath, Guid.NewGuid().ToString() + "_T" + Path.GetExtension(srcFile));
                        iTelluro.DataTools.Utility.GIS.CoordTransform.Ogrtransform(str2, wkt, srcFile, tmpFile2);

                        srcFile = tmpFile2;
                    }

                    toWktStr = GetWKTText(toWktStr);

                    //转换成需要的坐标参考
                    if (!string.IsNullOrEmpty(toWktStr))
                    {

                        ShpReader shpReader2 = new ShpReader(srcFile);
                        string str2 = shpReader2.GetSridWkt();
                        iTelluro.DataTools.Utility.GIS.CoordTransform.Ogrtransform(str2, toWktStr, srcFile, tarFile);
                    }

                    //Directory.Delete(tempPath, true);
                }
                success = true;
            }
            catch(Exception ex)
            {

            }
            return success;
        }

        /// <summary>
        /// 获取WKT串
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public string GetWKTText(string wkt)
        {
            string filePath = Path.Combine(ConfigurationManager.AppSettings["CoordPath"].ToString(), "Geographic Coordinate Systems");
            string fileContent = string.Empty;
            GetFileContent(filePath,wkt,ref fileContent);
            return fileContent;
        }

        /// <summary>
        /// 获取文件内容
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="wktString"></param>
        /// <returns></returns>
        public void GetFileContent(string filePath,string wktString,ref string fileContent)
        {
            if (Directory.Exists(filePath))
            {
                DirectoryInfo folder = new DirectoryInfo(filePath);
                if(folder.GetFiles().Count()> 0)
                { 
                    foreach (FileInfo file in folder.GetFiles())
                    {
                        string fileName = file.Name.Substring(0, file.Name.IndexOf("."));
                        if(fileName.Equals(wktString))
                        {
                            fileContent = File.ReadAllText(@file.FullName,Encoding.UTF8);
                            return;
                        }
                    }
                }
                else if(folder.GetDirectories().Count() > 0)
                {
                    foreach(DirectoryInfo direct in folder.GetDirectories())
                    {
                        GetFileContent(direct.FullName, wktString, ref fileContent);
                    }
                }
            }
        }
    }
}
