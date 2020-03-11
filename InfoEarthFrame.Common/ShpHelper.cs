using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using iTelluro.DataTools.Utility.SHP;
using InfoEarthFrame.Common.ShpUtility;

namespace InfoEarthFrame.Common
{
    public class ShpHelper
    {
        private readonly string shpFilePath = ConfigurationManager.AppSettings["UploadFilePath"];
        private readonly iTelluro.DataTools.Utility.SHP.ShpReader shpReader;

        public ShpHelper(string shpName)
        {
            shpFilePath = shpFilePath + shpName;
            shpReader = new iTelluro.DataTools.Utility.SHP.ShpReader(shpFilePath);
        }

        public ShpHelper(string filePath,string fileName)
        {
            shpReader = new iTelluro.DataTools.Utility.SHP.ShpReader(Path.Combine(filePath, fileName));
        }

        public Dictionary<string,string> DataList()
        {
            return shpReader.GetAttrList();
        }

        public List<OSGeo.OGR.Feature> GetAllFeatures()
        {
            return shpReader.GetAllFeatures();
        }

        public string GetWKTName(string wkt)
        {
            //CoordSystem cs = new CoordSystem(wkt);
            return "";
        }

        public bool IsException()
        {
            if (!shpReader.IsValidDataSource())
                return false;
            else
                return true;
        }

        public bool IsInvalidSpatialRefence()
        {
            string csSrc = shpReader.GetSridWkt();
            string SpatialRefence = System.Configuration.ConfigurationManager.AppSettings["SpatialRefence"].ToString();

            if (!csSrc.Contains(SpatialRefence))
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public List<AttributeModel> GetShpFieldList()
        {
            return new List<AttributeModel>();
        }

        //public List<string> AttributeList()
        //{
        //    List<iTelluro.DataTools.Utility.SHP.LayerField>  list= shpReader.GetAttributes();
        //    foreach(var item in list)
        //    {
        //        string name = item.FieldName;
        //        string type = item.FieldType;
        //    }
        //    List<OSGeo.OGR.Feature> listData = shpReader.GetAllFeatures();

        //    foreach(var item in listData)
        //    {
        //        string strTest = item.
        //    }
        //    //return "";
        //}
    }
}
