using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;

namespace iTelluro.GeologicMap.TopologyCheck
{
    /// <summary>
    /// shp图层实用类，提供ShpReader功能以外的附加功能
    /// </summary>
    public static class ShpUtility
    {
        /// <summary>
        /// 获取shp图层要素的个数
        /// </summary>
        /// <param name="shpPath">shp文件路径</param>
        /// <returns>图层所有要素的个数</returns>
        public static int GetFeatureNum(string shpPath)
        {
            try
            {
                string shxPath = shpPath.Substring(0, shpPath.Length - 1) + "x";
                if (File.Exists(shxPath) == false)
                {
                    //MessageBox.Show("索引文件*.shx文件丢失");
                    return -1;
                }
                FileStream fs = new FileStream(shxPath, FileMode.Open, FileAccess.Read);
                int shpNum = (int)(fs.Length - 100) / 8;
                fs.Close();
                return shpNum;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// 根据查询语句获取shp图层的属性记录
        /// </summary>
        /// <param name="shpPath">shp文件路径</param>
        /// <param name="selSQL">sql查询语句，可以为Null或""</param>
        /// <returns>shp图层的属性记录</returns>
        public static DataTable GetRecords(string shpPath, string selSQL)
        {
            try
            {
                string temPath = Path.GetTempPath();
                string conStr = "provider=microsoft.jet.oledb.4.0;data source=" + temPath + ";extended properties='dbase 5.0;hdr=false';OLE DB Services=-4";
                string cmdStr = "select * from temp.dbf";
                if (string.IsNullOrEmpty(selSQL) == false)
                {
                    cmdStr = selSQL;
                }

                string dbfPath = shpPath.Substring(0, shpPath.Length - 3) + "dbf";
                if (File.Exists(dbfPath) == false)
                {
                    //MsgBox.ShowErro("属性记录文件*.dbf文件丢失");
                    return null;
                }
                //用oledb读取dbf文件对文件名有要求，不能包含某些字符，所以这里先做拷贝，重命名之，待优化
                File.Copy(dbfPath, temPath + "temp.dbf", true);

                OleDbConnection con = new OleDbConnection(conStr);
                OleDbDataAdapter adapter = new OleDbDataAdapter(cmdStr, con);
                DataTable table = new DataTable();
                adapter.Fill(table);

                adapter.Dispose();
                con.Close();
                con.Dispose();

                return table;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 获取某个字段的最大值，若有异常或者没有值，则返回null
        /// </summary>
        /// <param name="shpPath">shp文件路径</param>
        /// <param name="fldName">字段名称</param>
        /// <returns>最大值</returns>
        public static string GetMaxValue(string shpPath, string fldName)
        {
            try
            {
                string temPath = Path.GetTempPath();
                string conStr = "provider=microsoft.jet.oledb.4.0;data source=" + temPath + ";extended properties='dbase 5.0;hdr=false';OLE DB Services=-4";
                string cmdStr = "select max("+fldName+ ") from temp.dbf";
            
                string dbfPath = shpPath.Substring(0, shpPath.Length - 3) + "dbf";
                if (File.Exists(dbfPath) == false)
                {
                    //MsgBox.ShowErro("属性记录文件*.dbf文件丢失");
                    return null;
                }
                //用oledb读取dbf文件对文件名有要求，不能包含某些字符，所以这里先做拷贝，重命名之，待优化
                File.Copy(dbfPath, temPath + "temp.dbf", true);

                OleDbConnection conn=new OleDbConnection(conStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(cmdStr, conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string maxValue = reader[0].ToString();
                    reader.Close();
                    conn.Close();
                    return maxValue;
                }
                reader.Close();
                conn.Close();
                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取某个字段的最小值，若有异常或者没有值，则返回null
        /// </summary>
        /// <param name="shpPath">shp文件路径</param>
        /// <param name="fldName">字段名称</param>
        /// <returns>最小值</returns>
        public static string GetMinValue(string shpPath, string fldName)
        {
            try
            {
                string temPath = Path.GetTempPath();
                string conStr = "provider=microsoft.jet.oledb.4.0;data source=" + temPath + ";extended properties='dbase 5.0;hdr=false';OLE DB Services=-4";
                string cmdStr = "select min(" + fldName + ") from temp.dbf";

                string dbfPath = shpPath.Substring(0, shpPath.Length - 3) + "dbf";
                if (File.Exists(dbfPath) == false)
                {
                    //MsgBox.ShowErro("属性记录文件*.dbf文件丢失");
                    return null;
                }
                //用oledb读取dbf文件对文件名有要求，不能包含某些字符，所以这里先做拷贝，重命名之，待优化
                File.Copy(dbfPath, temPath + "temp.dbf", true);

                OleDbConnection conn = new OleDbConnection(conStr);
                conn.Open();
                OleDbCommand cmd = new OleDbCommand(cmdStr, conn);
                OleDbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string maxValue = reader[0].ToString();
                    reader.Close();
                    conn.Close();
                    return maxValue;
                }
                reader.Close();
                conn.Close();
                return null;
            }
            catch
            {
                return null;
            }
        }
      
    }
}
