using System;
using System.Data;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.IO;
using System.Text;

namespace InfoEarthFrame.Common
{
    public class MySqlHelper
    {
        private string _connStr = ConfigurationManager.ConnectionStrings["Default"].ConnectionString;

        /// <summary>
        /// 执行操作
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="tableName"></param>
        public bool ExecuteNonQuery(string strSQL)
        {
            bool bFlag = false;
            using (MySqlConnection sqlConn = new MySqlConnection(_connStr))
            {
                try
                {
                    sqlConn.Open();
                    MySqlCommand sqlComm = new MySqlCommand();
                    sqlComm.Connection = sqlConn;
                    sqlComm.CommandTimeout = 1000;
                    sqlComm.CommandText = strSQL;
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
        /// 返回查询数据集合
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public DataTable ExecuteQuery(string strSQL)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection sqlConn = new MySqlConnection(_connStr))
            {
                try
                {
                    sqlConn.Open();
                    MySqlCommand sqlComm = new MySqlCommand(strSQL, sqlConn);
                    MySqlDataReader dr = sqlComm.ExecuteReader();
                    dt.Load(dr);
                    dr.Close();
                }
                catch (Exception ex)
                {
                    Abp.Logging.LogHelper.LogException(ex);
                    return dt;
                }
                finally
                {
                    sqlConn.Close();
                    sqlConn.Dispose();
                }
            }
            return dt;
        }

        /// <summary>
        /// 批量导入数据
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="insertCount"></param>
        /// <returns></returns>
        public bool MultiBulkInsert(DataTable dt, ref int insertCount)
        {
            if(!string.IsNullOrEmpty(dt.TableName))
            {
                return false;
            }
            else
            {
                string tmpPath = Path.GetTempFileName();
                string csv = DataTableToCSV(dt);
                File.WriteAllText(tmpPath,csv);
                insertCount = 0;

                using(MySqlConnection conn = new MySqlConnection(_connStr))
                {
                    try
                    {
                        conn.Open();
                        MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                        {
                            FieldTerminator = ",",
                            FieldQuotationCharacter = '"',
                            EscapeCharacter = '"',
                            LineTerminator = "\r\n",
                            FileName = tmpPath,
                            NumberOfLinesToSkip = 0,
                            TableName = dt.TableName,
                        };
                        insertCount = bulk.Load();
                        File.Delete(tmpPath);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        return false;
                    }
                }
               
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private string DataTableToCSV(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            DataColumn colum;

            foreach(DataRow row in dt.Rows)
            {
                for(int i=0;i< dt.Columns.Count;i++)
                {
                    colum = dt.Columns[i];
                    if (i != 0)
                        sb.Append(",");
                    if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else
                        sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
    }
}
