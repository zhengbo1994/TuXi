using System;
using System.Collections.Generic;
using System.Text;
using OSGeo.OGR;

namespace InfoEarthFrame.Common.ShpUtility
{
    public static class Utility
    {
        /// <summary>
        /// 将SQL数据库类型转换为GDAL的数据类型
        /// </summary>
        /// <returns></returns>
        public static FieldType SqlTypeToGdalType(string sqlType)
        {
            FieldType fieldType = FieldType.OFTBinary;
            switch (sqlType)
            {
                case "binary(50)":
                    fieldType = FieldType.OFTBinary;
                    break;
                case "date":
                    fieldType = FieldType.OFTDate;
                    break;
                case "datetime":
                    fieldType = FieldType.OFTDateTime;
                    break;
                case "int":
                    fieldType = FieldType.OFTInteger;
                    break;
                case "real":
                    fieldType = FieldType.OFTReal;
                    break;
                case "nvarchar":
                    fieldType = FieldType.OFTString;
                    break;
            }
            return fieldType;
        }

        /// <summary>
        /// 将数据类型转换为GDAL类型
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static FieldType DataTypeToGdalType(string dataType)
        {
            FieldType fieldType = FieldType.OFTBinary;
            switch (dataType)
            {
                case "Short Integer":
                    fieldType = FieldType.OFTInteger;
                    break;
                case "Long Integer":
                    fieldType = FieldType.OFTInteger;
                    break;
                case "Date":
                    fieldType = FieldType.OFTDateTime;
                    break;
                case "Float":
                    fieldType = FieldType.OFTReal;
                    break;
                case "Double":
                    fieldType = FieldType.OFTReal;
                    break;
                case "Text":
                    fieldType = FieldType.OFTString;
                    break;
                default:
                    fieldType = FieldType.OFTString;
                    break;
            }
            return fieldType;
        }

        /// <summary>
        /// 将GDAL的数据类型转换为SQL数据库类型
        /// </summary>
        /// <returns></returns>
        public static string GdalTypeToSqlType(string gdalType)
        {
            string sqlType = string.Empty;
            switch (gdalType)
            {
                case "OFTBinary":
                    sqlType = "binary(50)";
                    break;
                case "OFTDate":
                    sqlType = "date";
                    break;
                case "OFTDateTime":
                    sqlType = "datetime";
                    break;
                case "OFTInteger":
                    sqlType = "int";
                    break;
                case "OFTIntegerList":
                    sqlType = "";
                    break;
                case "OFTReal":
                    sqlType = "real";
                    break;
                case "OFTRealList":
                    sqlType = "";
                    break;
                case "OFTString":
                    sqlType = "nvarchar(max)";
                    break;
                case "OFTStringList":
                    sqlType = "";
                    break;
                case "OFTTime":
                    sqlType = "";
                    break;
                case "OFTWideString":
                    sqlType = "";
                    break;
                case "OFTWideStringList":
                    sqlType = "";
                    break;
            }
            return sqlType;
        }

        /// <summary>
        /// 将GDAL的数据类型转换为SQL数据库类型
        /// </summary>
        /// <returns></returns>
        public static string GdalTypeToNpSqlType(string gdalType)
        {
            string sqlType = string.Empty;
            switch (gdalType)
            {
                case "OFTBinary":
                    sqlType = "bit";
                    break;
                case "OFTDate":
                    sqlType = "date";
                    break;
                case "OFTDateTime":
                    sqlType = "date";
                    break;
                case "OFTInteger":
                    sqlType = "int";
                    break;
                case "OFTIntegerList":
                    sqlType = "";
                    break;
                case "OFTReal":
                    sqlType = "real";
                    break;
                case "OFTRealList":
                    sqlType = "";
                    break;
                case "OFTString":
                    sqlType = "text";
                    break;
                case "OFTStringList":
                    sqlType = "";
                    break;
                case "OFTTime":
                    sqlType = "";
                    break;
                case "OFTWideString":
                    sqlType = "";
                    break;
                case "OFTWideStringList":
                    sqlType = "";
                    break;
            }
            return sqlType;
        }

        /// <summary>
        /// 根据数据库中的存储字段，获得空间数据的存储类型
        /// </summary>
        /// <param name="geoType"></param>
        /// <returns></returns>
        public static wkbGeometryType GetGeoTypeByString(string geoType)
        {
            if (geoType == "POLYGON")
                return wkbGeometryType.wkbMultiPolygon;
            else if (geoType == "POINT")
                return wkbGeometryType.wkbPoint;
            else if (geoType == "LINESTRING")
                return wkbGeometryType.wkbLineString;
            else
                return wkbGeometryType.wkbNone;
        }

        public static string wktSpatialReference(string spatialReference)
        {
            switch(spatialReference)
            {
                case "1":
                    return "GEOGCS[\"WGS 84\",DATUM[\"WGS_1984\",SPHEROID[\"WGS 84\",6378137,298.257223563,AUTHORITY[\"EPSG\",\"7030\"]],AUTHORITY[\"EPSG\",\"6326\"]],PRIMEM[\"Greenwich\",0.0,AUTHORITY[\"EPSG\",\"8901\"]],UNIT[\"degree\",0.0174532925199433,AUTHORITY[\"EPSG\",\"912\"]],AUTHORITY[\"EPSG\",\"4326\"]]";
                    break;
                case "2":
                    return "";
                    break;
                case "3":
                    return "";
                    break;
                case "4":
                    return "";
                    break;
                default:
                    return "";
                    break;
            }
        }
    }
}
