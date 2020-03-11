using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Configuration;
using System.Web;
using InfoEarthFrame.Common.ShpUtility;

namespace InfoEarthFrame.Common
{
    public class PostgrelVectorHelper
    {
        private string _connStr = ConfigurationManager.AppSettings["PostGIS"].ToString();
        private int _count = 500;


        //static PostgrelVectorHelper()
        //{
        //    try
        //    {
        //        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DATA", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data"));
        //        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_DRIVER_PATH", System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins"));
        //        ////使路径支持中文
        //        //OSGeo.GDAL.Gdal.SetConfigOption("GDAL_FILENAME_IS_UTF8", "NO");
        //        ////使属性字段支持中文
        //        //OSGeo.GDAL.Gdal.SetConfigOption("SHAPE_ENCODING", "");

        //        //OSGeo.OGR.Ogr.RegisterAll();
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}

        /// <summary>
        /// 构造函数
        /// </summary>
        public PostgrelVectorHelper()
        {
        }

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="tableName"></param>
        public bool ExceuteSQL(string strSQL, string tableName)
        {
            bool bFlag = false;

            using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
            {
                try
                {
                    sqlConn.Open();
                    NpgsqlCommand sqlComm = new NpgsqlCommand(strSQL, sqlConn);
                    sqlComm.CommandTimeout = 1000;
                    sqlComm.ExecuteNonQuery();
                    bFlag = true;
                }
                catch (Exception ex)
                {
                    Abp.Logging.LogHelper.LogException(ex);
                    bFlag = false;
                }
                finally
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }

            return bFlag;
        }

        /// <summary>
        /// 返回数据条数
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int DataRowSQL(string tableName)
        {
            int rowCount = 0;
            using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
            {
                string sqlStr = String.Format("select Max(SID) from {0}", tableName);
                NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
                object obj = sqlC.ExecuteScalar();
                rowCount = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
            }
            return rowCount;
        }

        //public void InsertData(string layerID,string fileName)
        //{
        //    string conn = ConfigurationManager.AppSettings["PostGIS"].ToString();
        //    string filePath = ConfigurationManager.AppSettings["UploadFilePath"].ToString()+fileName;
        //    //string tableName = Path.GetFileNameWithoutExtension(data.Filename).ToUpper();
        //    //string filename = Path.GetFileName(data.Filename);
        //    ShpReader shpReader = new ShpReader(filePath);

        //    string tableName = "";
        //    int srid = shpReader.GetSrid();
        //    try
        //    {
        //        using (NpgsqlConnection sqlConn = new NpgsqlConnection(conn))
        //        {
        //            sqlConn.Open();
        //            //建表
        //            Dictionary<string, string> attr = shpReader.GetAttributeType();
        //            List<string> shpAttr = new List<string>();
        //            Hashtable hashTable = new Hashtable();
        //            foreach (KeyValuePair<string, string> item in attr)
        //            {
        //                shpAttr.Add(item.Key);
        //                hashTable.Add(item.Key, item.Value);
        //            }

        //            //if (IsTableNameExist(_connStr, tableName))
        //            //{
        //            //    NpgsqlCommand NpgsqlCommand = new NpgsqlCommand(string.Format("drop table {0}", tableName.ToUpper()), sqlConn);
        //            //    NpgsqlCommand.ExecuteNonQuery();
        //            //}

        //            string sqlCommStr = String.Format("CREATE TABLE {0}(SID SERIAL primary key,", tableName);
        //            for (int i = 0; i < shpAttr.Count; i++)
        //            {
        //                string attrName = shpAttr[i];
        //                sqlCommStr += String.Format("{0} {1},", attrName, Utility.GdalTypeToNpSqlType(hashTable[attrName].ToString()));
        //            }
        //            sqlCommStr += String.Format("geom geometry)");
        //            NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
        //            sqlComm.ExecuteNonQuery();
        //            //向表中添加数据
        //            int pFeatureCount = shpReader.GetFeatureCount();

        //            string sqlStr = String.Format("select Max(SID) from {0}", tableName);
        //            NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
        //            object obj = sqlC.ExecuteScalar();
        //            int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
        //            int maxNum = maxValue + 1;
        //            for (int i = 0; i < pFeatureCount; i++)
        //            {
        //                try
        //                {
        //                    string sqlInsert = String.Format("insert into {0} values({1},", tableName, maxNum + i);
        //                    Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
        //                    foreach (KeyValuePair<string, string> item in attr1)
        //                    {
        //                        if (hashTable[item.Key].ToString() == "OFTString")
        //                        {
        //                            if (item.Value.Contains("'"))
        //                            {
        //                                string newValue = item.Value.Replace("'", "''");
        //                                sqlInsert += String.Format("'{0}',", newValue);
        //                            }
        //                            else
        //                            {
        //                                sqlInsert += String.Format("'{0}',", item.Value);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (!string.IsNullOrEmpty(item.Value))
        //                            {
        //                                sqlInsert += String.Format("{0},", item.Value);
        //                            }
        //                            else
        //                            {
        //                                sqlInsert += String.Format("'{0}',", item.Value);
        //                            }
        //                        }
        //                    }
        //                    sqlInsert += String.Format("'{0}')", shpReader.GetOneFeatureGeomWkt(i));
        //                    sqlComm.CommandText = sqlInsert;
        //                    sqlComm.ExecuteNonQuery();
        //                }
        //                catch (Exception ex)
        //                {
        //                    //NpgsqlEventLog.
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        using (NpgsqlConnection sqlConn = new NpgsqlConnection(conn))
        //        {
        //            sqlConn.Open();
        //            string sqlCommStr = string.Format("drop table {0}", tableName.ToUpper());
        //            NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
        //            sqlComm.ExecuteNonQuery();
        //            sqlConn.Close();
        //        }
        //        throw new Exception("NpgsqlVectorDataService:WriteVector出错,错误原因:" + ex.Message);
        //    }

        //}

        /// <summary>
        /// 在给定的数据库中查询是否存在同表名//
        /// </summary>
        /// <param name="sqlConnStr"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private bool IsTableNameExist(string strSql, string tableName)
        {
            bool isExist = false;
            NpgsqlConnection sqlConn = new NpgsqlConnection(strSql);
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
                return isExist;
            }
            finally
            {
                sqlConn.Close();
                sqlConn.Dispose();
            }
        }

        /// <summary>
        /// 返回查询数据
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public DataTable getDataTable(string strSQL)
        {
            DataTable dt = new DataTable();
            using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
            {
                try
                {
                    sqlConn.Open();
                    NpgsqlCommand sqlComm = new NpgsqlCommand(strSQL, sqlConn);
                    NpgsqlDataAdapter NpgsqlDataAdapter = new NpgsqlDataAdapter(sqlComm);
                    NpgsqlDataAdapter.Fill(dt);
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
            return dt;
        }

        public void CreateTable(string tableName, List<string> cols)
        {

            if (!IsTableNameExist(_connStr, tableName))
            {
                string sqlCommStr = String.Format("CREATE TABLE {0}(SID SERIAL primary key,", tableName);
                for (int i = 0; i < cols.Count; i++)
                {
                    string attrName = cols[i];
                    sqlCommStr += String.Format("{0} {1},", attrName, "text");
                }
                sqlCommStr += String.Format("geom geometry)");

                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    try
                    {
                        sqlConn.Open();
                        NpgsqlCommand sqlComm = new NpgsqlCommand(sqlCommStr, sqlConn);
                        sqlComm.ExecuteNonQuery();
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
            }
        }

        /// <summary>
        /// 获取执行的结果
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public object GetExecuteScalar(string sql)
        {
            try
            {

                object obj;
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    sqlConn.Open();
                    NpgsqlCommand sqlC = new NpgsqlCommand(sql, sqlConn);
                    obj = sqlC.ExecuteScalar();
                }
                return obj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool ImportTable(string tableName, string filePath, List<string> listAtrr)
        {
            if (File.Exists(filePath))
            {
                ShpReader shpReader = new ShpReader(filePath);
                // 检查矢量文件的有效性
                if (!shpReader.IsValidDataSource())
                {
                    return false;
                }
                string wkt = shpReader.GetSridWkt();
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
                            if (listAtrr.Exists(t => t.ToUpper() == item.Key.ToUpper()))
                            {
                                shpAttr.Add(item.Key);
                                hashTable.Add(item.Key, item.Value);
                            }
                        }

                        if (shpAttr.Count <= 0)
                        {
                            return false;
                        }

                        //向表中添加数据
                        int pFeatureCount = shpReader.GetFeatureCount();

                        string sqlStr = String.Format("select Max(SID) from {0}", tableName);
                        NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
                        object obj = sqlC.ExecuteScalar();


                        NpgsqlCommand sqlComm = new NpgsqlCommand();

                        int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                        int maxNum = maxValue + 1;
                        string sqlCStr = string.Empty;
                        for (int i = 0; i < pFeatureCount; i++)
                        {
                            string colStr = string.Empty;
                            try
                            {
                                string valueStr = string.Empty;

                                Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                                foreach (KeyValuePair<string, string> item in attr1)
                                {
                                    colStr += "\"" + item.Key + "\"" + ",";
                                    if (hashTable[item.Key].ToString() == "OFTString")
                                    {
                                        if (item.Value.Contains("'"))
                                        {
                                            string newValue = item.Value.Replace("'", "''");
                                            valueStr += String.Format("'{0}',", newValue);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", item.Value);
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(item.Value))
                                        {
                                            valueStr += String.Format("{0},", item.Value);
                                        }
                                        else
                                        {
                                            valueStr += String.Format("'{0}',", item.Value);
                                        }
                                    }
                                }
                                colStr = colStr.TrimEnd(',');
                                valueStr = valueStr.TrimEnd(',');

                                string sid = string.Format("{0}", maxNum + i);
                                string geomStr = String.Format("'{0}'", shpReader.GetOneFeatureGeomWkt(i));

                                //string sqlInsert = String.Format("insert into {0}(sid,geom,{1}) values({2},{3},{4})", tableName, colStr, sid, geomStr, valueStr);
                                //sqlComm.Connection = sqlConn;
                                //sqlComm.CommandText = sqlInsert;
                                //sqlComm.ExecuteNonQuery();
                                sqlCStr += string.Format("({0},{1},{2}),", sid, geomStr, valueStr);
                            }
                            catch (Exception ex)
                            {
                                Abp.Logging.LogHelper.LogException(ex);
                                //throw ex;
                            }
                            if ((i % _count == 0) || (i == pFeatureCount - 1))
                            {
                                sqlCStr = sqlCStr.TrimEnd(',');
                                string sqlInsert = String.Format("insert into {0}(sid,geom,{1}) values{2}", tableName, colStr, sqlCStr);
                                //sqlComm.Connection = sqlConn;
                                //sqlComm.CommandText = sqlInsert;
                                //sqlComm.ExecuteNonQuery();
                                ExceuteSQL(sqlInsert, string.Empty);
                                sqlCStr = string.Empty;
                            }
                        }
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Abp.Logging.LogHelper.LogException(ex);
                    //throw ex;
                }
            }
            return false;
        }

        public bool ImportTable(string tableName, ShpReader shpReader, List<string> shpAttr, Hashtable hashTable)
        {
            //if (File.Exists(filePath))
            //{
            //    ShpReader shpReader = new ShpReader(filePath);
            //    // 检查矢量文件的有效性
            //    if (!shpReader.IsValidDataSource())
            //    {
            //        return false;
            //    }
            string wkt = shpReader.GetSridWkt();
            int srid = shpReader.GetSrid();
            try
            {
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    sqlConn.Open();


                    //向表中添加数据
                    int pFeatureCount = shpReader.GetFeatureCount();

                    string sqlStr = String.Format("select Max(SID) from {0}", tableName);
                    NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
                    object obj = sqlC.ExecuteScalar();


                    NpgsqlCommand sqlComm = new NpgsqlCommand();

                    int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                    int maxNum = maxValue + 1;

                    string sqlCStr = string.Empty;
                    for (int i = 0; i < pFeatureCount; i++)
                    {
                        string colStr = string.Empty;
                        try
                        {
                            string valueStr = string.Empty;
                            Dictionary<string, string> attr1 = new Dictionary<string, string>();
                            if (pFeatureCount > 0)
                            {
                                attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                            }
                            foreach (KeyValuePair<string, string> item in attr1)
                            {
                                colStr += "\"" + item.Key + "\"" + ",";
                                if (hashTable[item.Key].ToString() == "OFTString")
                                {
                                    if (item.Value.Contains("'"))
                                    {
                                        string newValue = item.Value.Replace("'", "''");
                                        valueStr += String.Format("'{0}',", newValue);
                                    }
                                    else
                                    {
                                        valueStr += String.Format("'{0}',", item.Value);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(item.Value))
                                    {
                                        valueStr += String.Format("{0},", item.Value);
                                    }
                                    else
                                    {
                                        valueStr += String.Format("'{0}',", item.Value);
                                    }
                                }
                            }
                            colStr = colStr.TrimEnd(',');
                            valueStr = valueStr.TrimEnd(',');

                            string sid = string.Format("{0}", maxNum + i);
                            string geomStr = String.Format("'{0}'", shpReader.GetOneFeatureGeomWkt(i));


                            //string sqlInsert = String.Format("insert into {0}(sid,geom,{1}) values({2},{3},{4})", tableName, colStr, sid, geomStr, valueStr);
                            //sqlComm.Connection = sqlConn;
                            //sqlComm.CommandText = sqlInsert;
                            //sqlComm.ExecuteNonQuery();
                            sqlCStr += string.Format("({0},{1},{2}),", sid, geomStr, valueStr);
                        }
                        catch (Exception ex)
                        {
                            Abp.Logging.LogHelper.LogException(ex);
                            //throw ex;
                        }
                        if ((i % _count == 0) || (i == pFeatureCount - 1))
                        {
                            sqlCStr = sqlCStr.TrimEnd(',');
                            string sqlInsert = String.Format("insert into {0}(sid,geom,{1}) values{2}", tableName, colStr, sqlCStr);
                            //sqlComm.Connection = sqlConn;
                            //sqlComm.CommandText = sqlInsert;
                            //sqlComm.ExecuteNonQuery();
                            ExceuteSQL(sqlInsert, string.Empty);
                            sqlCStr = string.Empty;
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Abp.Logging.LogHelper.LogException(ex);
                return false;
            }
        }

        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="shpReader"></param>
        /// <param name="shpAttr"></param>
        /// <param name="hashTable"></param>
        /// <returns></returns>
        public bool ImportTableMulti(string tableName, ShpReader shpReader, List<string> shpAttr, Hashtable hashTable)
        {
            string wkt = shpReader.GetSridWkt();
            int srid = shpReader.GetSrid();
            try
            {
                using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
                {
                    sqlConn.Open();


                    //向表中添加数据
                    int pFeatureCount = shpReader.GetFeatureCount();

                    string sqlStr = String.Format("select Max(SID) from {0}", tableName);
                    NpgsqlCommand sqlC = new NpgsqlCommand(sqlStr, sqlConn);
                    object obj = sqlC.ExecuteScalar();


                    NpgsqlCommand sqlComm = new NpgsqlCommand();

                    int maxValue = (obj is System.DBNull) ? 0 : Convert.ToInt32(obj);
                    int maxNum = maxValue + 1;
                    string sqlInsert = "";
                    for (int i = 0; i < pFeatureCount; i++)
                    {
                        string colStr = string.Empty;
                        string valueStr = string.Empty;

                        Dictionary<string, string> attr1 = shpReader.GetOneFeatureAttribute(i, shpAttr);
                        foreach (KeyValuePair<string, string> item in attr1)
                        {
                            colStr += "\"" + item.Key + "\"" + ",";
                            if (hashTable[item.Key].ToString() == "OFTString")
                            {
                                if (item.Value.Contains("'"))
                                {
                                    string newValue = item.Value.Replace("'", "''");
                                    valueStr += String.Format("'{0}',", newValue);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(item.Value))
                                {
                                    valueStr += String.Format("{0},", item.Value);
                                }
                                else
                                {
                                    valueStr += String.Format("'{0}',", item.Value);
                                }
                            }
                        }
                        colStr = colStr.TrimEnd(',');
                        valueStr = valueStr.TrimEnd(',');

                        string sid = string.Format("{0}", maxNum + i);
                        string geomStr = String.Format("'{0}'", shpReader.GetOneFeatureGeomWkt(i));

                        if (i == 0)
                        {
                            sqlInsert += String.Format("insert into {0}(sid,geom,{1}) values", tableName, colStr);
                        }

                        sqlInsert += String.Format(" ({2},{3},{4}),", sid, geomStr, valueStr);
                    }

                    sqlInsert = sqlInsert.TrimEnd(',') + ";";

                    sqlComm.Connection = sqlConn;
                    sqlComm.CommandText = sqlInsert;
                    sqlComm.ExecuteNonQuery();

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable GetData(string strSQL)
        {
            DataTable dt = new DataTable();
            using (NpgsqlConnection sqlConn = new NpgsqlConnection(_connStr))
            {
                try
                {
                    sqlConn.Open();
                    NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(strSQL, sqlConn);
                    DataSet ds = new DataSet();
                    adapter.Fill(ds);
                    dt = ds.Tables[0];
                    return dt;
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
            return dt;
        }
    }
}
