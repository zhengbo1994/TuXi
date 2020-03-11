using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OracleClient;
using System.Configuration;

namespace iTelluro.GeologicMap.TopologyCheck
{
    public class DicInfoReader
    {
        public DicInfoReader()
        {
            this.GetData();
        }

        private List<TxLayerInfo> _txLyrList = new List<TxLayerInfo>();
        /// <summary>
        /// 图系图层信息
        /// </summary>
        public List<TxLayerInfo> TxLyrList
        {
            get
            {
                return _txLyrList;
            }
        }

        private List<LayerAttInfo> _lyrAttList = new List<LayerAttInfo>();
        /// <summary>
        /// 图层属性信息
        /// </summary>
        public List<LayerAttInfo> LyrAttList
        {
            get
            {
                return _lyrAttList;
            }
        }

        private List<AttValueInfo> _attValueList = new List<AttValueInfo>();
        /// <summary>
        /// 字典项字段信息
        /// </summary>
        public List<AttValueInfo> AttValueList
        {
            get
            {
                return _attValueList;
            }
        }

        private void GetData()
        {
            InfoEarthFrame.Data.IDatabase DBAccess = InfoEarthFrame.Data.Factory.GetDBAccess(ConfigurationManager.ConnectionStrings["Default"].ConnectionString, InfoEarthFrame.Data.AccessDBType.Oracle);

            _txLyrList.Clear();
            _lyrAttList.Clear();
            _attValueList.Clear();

            //图系图层信息
            //string sqlStr = "select trim(GUID) as GUID,trim(TXNAME) AS TXNAME,trim(TJNAME) AS TJNAME ,trim(LAYERNAME) AS LAYERNAME,trim(JHTZ) AS JHTZ from DIC_TX_LAYER order by SERIALNUM";
            string sqlStr = "select  Max(\"Id\") as GUID,\"TXName\",\"TJName\" ,\"LayerName\", \"DataLayer\" from DIC_TX_LAYERPROPERTY  group by \"TXName\",\"TJName\" ,\"LayerName\", \"DataLayer\"";
            DataSet dataset = DBAccess.GetDataSetFromExcuteCommand(sqlStr, null);
            if (dataset != null && dataset.Tables.Count > 0)
            {
                DataTable tbl = dataset.Tables[0];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    TxLayerInfo info = new TxLayerInfo()
                    {
                        Guid = tbl.Rows[i]["GUID"] as string,
                        TxName = tbl.Rows[i]["TXNAME"] as string,
                        TjName = tbl.Rows[i]["TJNAME"] as string,
                        LayerName = tbl.Rows[i]["LAYERNAME"] as string,
                        DataLayer = tbl.Rows[i]["DataLayer"] as string,
                        GeoType = "",//tbl.Rows[i]["JHTZ"] as string,
                    };
                    _txLyrList.Add(info);
                }
            }
            //图层属性信息
            //sqlStr = "select trim(GUID) as GUID,trim(LAYERID) AS LAYERID,trim(DATANAME) AS DATANAME ,trim(DATACODE) AS DATACODE,trim(DES) AS DES,trim(DATATYPE) AS DATATYPE,trim(CONSTRAINT) AS CONSTRAINT,trim(VALUETYPE) AS VALUETYPE,trim(UNIT) AS UNIT,trim(MEMO) AS MEMO from DIC_TX_LAYERPROPERTY order by LAYERID";
            sqlStr = "select  \"Id\" as GUID,\"TXName\",\"TJName\" ,\"LayerName\",trim(\"FieldName\") AS DATANAME, trim(\"InputControl\") AS INPUTCONTROL, trim(\"DataLayer\") as DATALAYER ,trim(\"FieldCode\") AS DATACODE ,\"FieldType\", \"FieldLen\",\"FieldDec\",trim(\"InputControl\") AS CONSTRAINT from DIC_TX_LAYERPROPERTY";
            dataset = DBAccess.GetDataSetFromExcuteCommand(sqlStr, null);
            if (dataset != null && dataset.Tables.Count > 0)
            {
                DataTable tbl = dataset.Tables[0];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    LayerAttInfo info = new LayerAttInfo()
                    {
                        Guid = tbl.Rows[i]["GUID"] as string,
                        TxName = tbl.Rows[i]["TXName"] as string,
                        TjName = tbl.Rows[i]["TJName"] as string,
                        LayerName = tbl.Rows[i]["LayerName"] as string,
                        DataName = tbl.Rows[i]["DATANAME"] as string,
                        DataCode = tbl.Rows[i]["DATACODE"] as string,
                        Des = "",//tbl.Rows[i]["DES"] as string,
                        DataLayer = tbl.Rows[i]["DATALAYER"] as string,
                        DataType = ConvertDataType(tbl.Rows[i]), //tbl.Rows[i]["DATATYPE"] as string,
                        Constraint = tbl.Rows[i]["CONSTRAINT"] as string,
                        ValueType = "自由文本",//tbl.Rows[i]["VALUETYPE"] as string,
                        Unit =  "/",//tbl.Rows[i]["U_NAME"] as string,
                        Memo = "/", //tbl.Rows[i]["MEMO"] as string,
                        InputControl = tbl.Rows[i]["INPUTCONTROL"] as string,
                    };
                    _lyrAttList.Add(info);
                }
            }
            //字典项字段信息
            //sqlStr = "select trim(GUID) as GUID,trim(PROPERTYID) AS PROPERTYID,trim(VALUE) AS VALUE ,trim(MEMO) AS MEMO,trim(PARENTPROPERTYID) AS PARENTPROPERTYID from DIC_TX_PROPERTYVALUE";
            sqlStr = "select trim(\"Id\") as GUID, trim(\"FieldValue\") AS VALUE from DIC_TX_PROPERTYVALUE";
            dataset = DBAccess.GetDataSetFromExcuteCommand(sqlStr, null);
            if (dataset != null && dataset.Tables.Count > 0)
            {
                DataTable tbl = dataset.Tables[0];
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    AttValueInfo info = new AttValueInfo()
                    {
                        Guid = tbl.Rows[i]["GUID"] as string,
                        AttId = "",//tbl.Rows[i]["PROPERTYID"] as string,
                        Value = tbl.Rows[i]["VALUE"] as string,
                        Memo = "",//tbl.Rows[i]["MEMO"] as string,
                        ParentId = "",//tbl.Rows[i]["PARENTPROPERTYID"] as string
                    };
                    _attValueList.Add(info);
                }
            }
        }

        private string ConvertDataType(DataRow dr)
        {
            switch (dr["FieldType"].ToString().ToLower())
            {
                case "char":
                    return "C" + dr["FieldLen"];
                case "float":
                    return "F" + dr["FieldLen"] + "." + dr["FieldDec"];
                default:
                    return "";
            }
        }
    }
}
