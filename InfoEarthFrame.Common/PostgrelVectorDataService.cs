using InfoEarthFrame.Common.Data;
using InfoEarthFrame.Common.ShpUtility;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace InfoEarthFrame.Common
{
    public class PostgrelVectorDataService 
    {
        #region 属性
        private string _connStr = String.Empty;
        private string _geometryColumnsTableName = "ITELLURO_GEOMETRY";
        #endregion

        static PostgrelVectorDataService()
        {
            try
            {
                //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.Combine(Application.StartupPath, "data"));
                //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DRIVER_PATH", System.IO.Path.Combine(Application.StartupPath, "plugins"));
                ////使路径支持中文
                //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
                ////使属性字段支持中文
                //OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

                //OSGeo.OGR.Ogr.RegisterAll();
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        public PostgrelVectorDataService(PostgrelDBObject odbObj)
        {
            _connStr = odbObj.ConnStr;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="odbObj"></param>
        /// <param name="tbIndex"></param>
        public PostgrelVectorDataService(PostgrelDBObject odbObj, string indexTable)
        {
            _connStr = odbObj.ConnStr;
            _geometryColumnsTableName = indexTable;
        }

        #region IVectorDataService 成员

        bool WriteData(VectorDataType data, out string dataGuid)
        {
            try
            {
                return WriteData2Sql(data, out dataGuid);
            }
            catch
            {
                dataGuid = null;
                return false;
            }
        }
        public bool WriteData2Sql(VectorDataType data, out string dataGuid)
        {
            string tableName = null;
            return WriteData2Sql(data, out dataGuid, out tableName);
        }
        public bool WriteData2Sql(VectorDataType data, out string dataGuid, out string tablename)
        {
            if (!File.Exists(data.Filename))
            {
                dataGuid = null;
                tablename = null;
                return false;
            }

            string tableName = Path.GetFileNameWithoutExtension(data.Filename).ToUpper();
            string filename = Path.GetFileName(data.Filename);
            ShpReader shpReader = new ShpReader(data.Filename);
            // 检查矢量文件的有效性
            if (!shpReader.IsValidDataSource())
            {
                dataGuid = null;
                tablename = null;
                return false;
            }
            // 创建索引表
            //if (!IsTableNameExist(_connStr, _geometryColumnsTableName))
            //{
            //    CreateGeometryColumnsTable(_connStr);
            //}
            int srid = shpReader.GetSrid();
            try
            {
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    sqlConn.Open();
                    //建表
                    Dictionary<string, string> attr = shpReader.GetAttributeType();
                    List<string> shpAttr = new List<string>();
                    Hashtable hashTable = new Hashtable();
                    foreach (KeyValuePair<string, string> item in attr)
                    {
                        shpAttr.Add(item.Key);
                        hashTable.Add(item.Key, item.Value);
                    }

                    if (IsTableNameExist(_connStr, tableName))
                    {
                        NpgsqlCommand NpgsqlCommand = new NpgsqlCommand(string.Format("drop table {0}", tableName.ToUpper()), sqlConn);
                        NpgsqlCommand.ExecuteNonQuery();
                    }

                    string sqlCommStr = String.Format("CREATE TABLE {0}(SID SERIAL primary key,", tableName);
                    for (int i = 0; i < shpAttr.Count; i++)
                    {
                        string attrName = shpAttr[i];
                        sqlCommStr += String.Format("{0} {1},", attrName, Utility.GdalTypeToNpSqlType(hashTable[attrName].ToString()));
                    }
                    sqlCommStr += String.Format("geom geometry)");
                    NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                    sqlComm.ExecuteNonQuery();
                    //向表中添加数据
                    int pFeatureCount = shpReader.GetFeatureCount();

                    string sqlStr = String.Format("select Max(SID) from {0}", tableName);
                    NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
                    object obj = sqlC.ExecuteScalar();
                    int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                    int maxNum = maxValue + 1;
                    for (int i = 0; i < pFeatureCount; i++)
                    {
                        try
                        {
                            string sqlInsert = String.Format("insert into {0} values({1},", tableName, maxNum + i);
                            Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                            foreach (KeyValuePair<string, string> item in attr1)
                            {
                                if (hashTable[item.Key].ToString() == "OFTString")
                                {
                                    if (item.Value.Contains("'"))
                                    {
                                        string newValue = item.Value.Replace("'", "''");
                                        sqlInsert += String.Format("'{0}',", newValue);
                                    }
                                    else
                                    {
                                        sqlInsert += String.Format("'{0}',", item.Value);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        sqlInsert += String.Format("{0},", item.Value);
                                    }
                                    else
                                    {
                                        sqlInsert += String.Format("'{0}',", item.Value);
                                    }
                                }
                            }
                            sqlInsert += String.Format("'{0}')", shpReader.GetOneFeatureGeomWkt(i));
                            sqlComm.CommandText = sqlInsert;
                            sqlComm.ExecuteNonQuery();
                        }
                        catch(Exception ex)
                        {
                            //NpgsqlEventLog.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    sqlConn.Open();
                    string sqlCommStr = string.Format("drop table {0}", tableName.ToUpper());
                    NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                    sqlComm.ExecuteNonQuery();
                    sqlConn.Close();
                }
                throw new Exception("NpgsqlVectorDataService:WriteVector出错,错误原因:" + ex.Message);
            }

            //zhangheng 2014-06-24
            //如果索引表中已有记录，先删除.
            //DelIndexData(filename);

            //InsertIntoGeomColmn(_connStr, data.DataGuid, filename, tableName, shpReader.GetDeminsion(), shpReader.GetGeomType(), shpReader.GetSridWkt());

            dataGuid = data.DataGuid;
            tablename = tableName;
            return true;
        }

        bool WriteDataAndTile(VectorDataType data, out string dataGuid)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 读数据
        /// </summary>
        /// <param name="dataGuid"></param>
        /// <param name="outFileFullPath">导出路径，不包括文件名</param>
        /// <returns></returns>
        VectorDataType ReadData(string dataGuid, string outFileFullPath)
        {
            //修改：输出文件目录不存在的时候创建目录
            //修改日期：20130911
            //修改人：XiongZK
            if (!Directory.Exists(outFileFullPath))
            {
                Directory.CreateDirectory(outFileFullPath);
            }

            string tableName = String.Empty;
            string wktReference = String.Empty;
            List<string> geomList = new List<string>();
            string geoType = String.Empty;
            List<AttributeModel> lstattr = new List<AttributeModel>();//属性列
            List<AttributeObj> lstAttributeObj = new List<AttributeObj>();

            try
            {
                GetTableName(dataGuid, ref tableName);
                if (string.IsNullOrEmpty(tableName))
                    return null;
                GetRefAndGeoType(tableName, ref wktReference, ref geoType);
                GetAttributeColumn(tableName, ref lstattr);
                GetSpatialAttrbute(tableName, ref geomList);
                lstAttributeObj = GetAttribute(tableName, lstattr);

                string outFileFullname = Path.Combine(outFileFullPath, tableName + ".shp");
                ShpWriter shpWriter = new ShpWriter(outFileFullname);
                shpWriter.DoExport(lstattr, Utility.GetGeoTypeByString(geoType), geomList, lstAttributeObj, wktReference);

                VectorDataType vdt = new VectorDataType();
                vdt.DataGuid = dataGuid;
                vdt.Filename = outFileFullname;
                return vdt;
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:ReadVector出错,错误原因:" + ex.Message);
            }
        }

        bool DelData(string dataGuid)
        {
            try
            {
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    string tableName = String.Empty;
                    GetTableName(dataGuid, ref tableName);
                    if (string.IsNullOrEmpty(tableName))
                        return true;
                    sqlConn.Open();
                    string sqlCommStr = string.Format("drop table {0}", tableName.ToUpper());
                    NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                    sqlComm.ExecuteNonQuery();
                    sqlComm.CommandText = String.Format("delete from {0} where TABLENAME='{1}'", _geometryColumnsTableName, tableName);
                    sqlComm.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:DelVectorData出错，错误原因：" + ex.Message);
            }
        }

        bool IsContainData(string dataGuid)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                if (!IsTableNameExist(_connStr, _geometryColumnsTableName))
                    return false;

                sqlConn.Open();
                string sqlCommStr = String.Format("select TABLENAME from {0} where GUID='{1}'", _geometryColumnsTableName, dataGuid);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                object obj = sqlComm.ExecuteScalar();
                if (obj == null)
                    return false;
                else if (string.IsNullOrEmpty(obj.ToString()))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Vector:IsContainData出错，错误原因：" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 通过文件名判断是否存在同名文件
        /// 文件名有后缀
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        bool IsContainDataName(string filename)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                if (!IsTableNameExist(_connStr, _geometryColumnsTableName))
                    return false;

                sqlConn.Open();
                string sqlCommStr = String.Format("select GUID from {0} where FILENAME='{1}'", _geometryColumnsTableName, filename);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                object obj = sqlComm.ExecuteScalar();
                if (obj == null)
                    return false;
                else if (string.IsNullOrEmpty(obj.ToString()))
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Vector:IsContainDataName出错，错误原因：" + ex.Message);
            }
            finally
            {
            }
        }
        /// <summary>
        /// 获得数据列表
        /// </summary>
        /// <returns></returns>
        public List<VectorInfo> GetAll()
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                if (!IsTableNameExist(_connStr, _geometryColumnsTableName))
                    return null;

                sqlConn.Open();
                string sqlCommStr = String.Format("select GUID, FILENAME, TABLENAME, DATATYPE, SPATIALREF from {0}", _geometryColumnsTableName);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                NpgsqlDataReader sqlReader = sqlComm.ExecuteReader();

                if (sqlReader == null)
                    return null;
                List<VectorInfo> result = new List<VectorInfo>();
                while (sqlReader.Read())
                {
                    VectorInfo info = new VectorInfo();
                    info.GUID = sqlReader.GetString(0);
                    info.FILENAME = sqlReader.GetString(1);
                    info.TABLENAME = sqlReader.GetString(2);
                    info.DATATYPE = sqlReader.GetString(3);
                    info.SPATIALREF = sqlReader.GetString(4);
                    result.Add(info);
                }
                sqlReader.Close();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:GetAll出错,错误原因:" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        #endregion

        /// <summary>
        /// 创建geometry_columns表//
        /// </summary>
        /// <param name="sqlConnStr"></param>
        private void CreateGeometryColumnsTable(string sqlConnStr)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(sqlConnStr);
            try
            {
                sqlConn.Open();
                string sqlCommStr = String.Format("create table {0}({1},{2},{3},{4},{5},{6})",
                    _geometryColumnsTableName, "GUID varchar(2048) primary key", "FILENAME varchar(256) not null", "TABLENAME varchar(256) not null",
                    "COORDDIMENSION int not null", "DATATYPE varchar(30) not null", "SPATIALREF varchar(1000) not null");
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                sqlComm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:CreateGeometryColumnsTable(string)出错，错误原因:" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 将数据写入geometry_columns表中
        /// </summary>
        /// <param name="sqlConnstr"></param>
        /// <param name="tableName"></param>
        /// <param name="dimension"></param>
        /// <param name="srid"></param>
        /// <param name="geomType"></param>
        private void InsertIntoGeomColmn(string sqlConnstr, string guid, string filename, string tableName, int dimension, string geomType, string sRefWkt)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(sqlConnstr);
            try
            {
                sqlConn.Open();

                string sqlCommStr = String.Format("insert into {0} (GUID,FILENAME,TABLENAME,COORDDIMENSION,DATATYPE,SPATIALREF) values('{1}','{2}','{3}',{4},'{5}','{6}')",
                    _geometryColumnsTableName, guid, filename, tableName, dimension, geomType, sRefWkt);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                sqlComm.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:InsertIntoGeomColumn(string,string,int,int,string)出错，错误原因:" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 在给定的数据库中查询是否存在同表名//
        /// </summary>
        /// <param name="sqlConnStr"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool IsTableNameExist(string sqlConnStr, string tableName)
        {
            bool isExist = false;
            NpgsqlConnection sqlConn = new NpgsqlConnection(sqlConnStr);
            try
            {
                sqlConn.Open();
                string sqlCommStr = "select tablename from pg_tables where schemaname='public'";
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                NpgsqlDataAdapter NpgsqlDataAdapter = new NpgsqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                NpgsqlDataAdapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    if (String.Equals(tableName.ToUpper(), dr[0].ToString().ToUpper()))
                    {
                        isExist = true;
                        break;
                    }
                }
                return isExist;
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:IsTablenameExist(string,string)出错，错误原因:" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 根据文件GUID获得文件名称 删除数据需要的
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="filename"></param>
        private void GetTableName(string guid, ref string tableName)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                sqlConn.Open();
                string sqlCommStr = String.Format("select TABLENAME from {0} where GUID='{1}'", _geometryColumnsTableName, guid);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                object obj = sqlComm.ExecuteScalar();
                if (obj != null)
                {
                    tableName = obj.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:GetDataInfo出错,错误原因:" + ex.Message);
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 获得geom
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="srid"></param>
        /// <param name="geometryType"></param>
        /// <param name="srtext"></param>
        private void GetRefAndGeoType(string tableName, ref string srtext, ref string geometryType)
        {
            NpgsqlConnection sqlCon = new NpgsqlConnection(_connStr);
            try
            {
                sqlCon.Open();
                string sqlCommStr = String.Format("select SPATIALREF from {0} where TABLENAME='{1}'", _geometryColumnsTableName, tableName);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlCon);
                srtext = sqlComm.ExecuteScalar().ToString();

                sqlComm.CommandText = String.Format("select DATATYPE from {0} where TABLENAME='{1}'", _geometryColumnsTableName, tableName);
                geometryType = sqlComm.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                sqlCon.Close();
                sqlCon.Dispose();
            }
        }
        /// <summary>
        /// 返回属性对象列表
        /// </summary>
        /// <returns></returns>
        private List<AttributeObj> GetAttribute(string tableName, List<AttributeModel> lstAttributeModel)
        {
            List<AttributeObj> lstAttrObj = new List<AttributeObj>();
            DataTable dt = new DataTable();
            using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
            {
                sqlConn.Open();
                string sqlCommStr = String.Format("select * from {0}", tableName);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                NpgsqlDataAdapter NpgsqlDataAdapter = new NpgsqlDataAdapter(sqlComm);
                NpgsqlDataAdapter.Fill(dt);
                int fid = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    AttributeObj attObj = new AttributeObj();
                    Dictionary<string, string> dicAttValue = new Dictionary<string, string>();
                    foreach (AttributeModel attributeM in lstAttributeModel)
                    {
                        attObj.OId = fid;
                        string cloumnName = attributeM.AttributeName;
                        string rowValue = dr[cloumnName].ToString().Trim();
                        dicAttValue.Add(cloumnName, rowValue.ToString());
                        fid++;
                    }
                    attObj.AttributeValue = dicAttValue;
                    lstAttrObj.Add(attObj);
                }
            }
            return lstAttrObj;
        }
        /// <summary>
        /// 得到属性列信息列表
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="lstAttr"></param>
        private void GetAttributeColumn(string tablename, ref List<AttributeModel> lstAttr)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                sqlConn.Open();
                string sqlCommStr = String.Format("select COLUMN_NAME,DATA_TYPE from ServerSIG.INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='{0}'", tablename);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(sqlComm);
                DataTable dt = new DataTable();
                dataAdapter.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    AttributeModel at = new AttributeModel();
                    if (dr[0].ToString() != "SID" && dr[0].ToString() != "geom")
                    {
                        at.AttributeName = dr[0].ToString();
                        at.AttributeType = Utility.SqlTypeToGdalType(dr[1].ToString());
                        lstAttr.Add(at);
                    }
                }
            }
            catch
            {
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }

        }
        /// <summary>
        /// geom
        /// </summary>
        /// <param name="tablename"></param>
        private void GetSpatialAttrbute(string tablename, ref List<string> lstGeom)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            sqlConn.Open();
            string sqlCommStr = String.Format("select geom from {0}", tablename);
            NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
            NpgsqlDataAdapter adpter = new NpgsqlDataAdapter(sqlComm);
            DataTable dt = new DataTable();
            adpter.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                lstGeom.Add(dr["geom"].ToString());
            }
        }
        /// <summary>
        /// 获取某列数据类型
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="dataType"></param>
        private void GetColumnDataTypeAndLength(string tableName, string columnName, ref string dataType)
        {
            NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr);
            try
            {
                sqlConn.Open();
                string sqlCommStr = String.Format("select data_type from {0} where columnName={1}", tableName, columnName);
                NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                dataType = sqlComm.ExecuteScalar().ToString();
            }
            catch
            {
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }
        /// <summary>
        /// 删除索引表中已有记录
        /// </summary>
        /// <param name="filename">矢量文件名</param>
        private void DelIndexData(string filename)
        {
            NpgsqlConnection oConn = new NpgsqlConnection(_connStr);
            try
            {
                oConn.Open();
                using (NpgsqlCommand oComm = new NpgsqlCommand())
                {
                    oComm.Connection = oConn;

                    if (!IsTableNameExist(_connStr, _geometryColumnsTableName))
                        return;

                    oComm.CommandText = String.Format
                        ("select GUID from {0} where FILENAME = '{1}'", _geometryColumnsTableName, filename);
                    object obj = oComm.ExecuteScalar();
                    if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                        return;

                    oComm.CommandText = String.Format
                        ("delete from {0} where FILENAME = '{1}'", _geometryColumnsTableName, filename);
                    oComm.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("NpgsqlVectorDataService:DelIndexData出错,错误原因:" + ex.Message);
            }
            finally
            {
                oConn.Close();
                oConn.Dispose();
            }
        }
    }
}
