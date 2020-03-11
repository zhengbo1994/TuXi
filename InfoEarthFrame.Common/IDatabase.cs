using System;
using System.Data;
using System.Collections.Generic;

namespace InfoEarthFrame.Data
{

    /// <summary>
    /// 数据库访问类型
    /// </summary>
    public enum AccessDBType
    {
        SQL,
        SQLCe,
        Oracle
    }

    /// <summary>
    /// 工厂类
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 用于得到数据库通用访问接口
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="Type">数据库访问类型</param>
        /// <returns></returns>
        public static IDatabase GetDBAccess(string connectionString, AccessDBType type)
        {
            IDatabase DBAccess = null;

            switch (type)
            {
                case AccessDBType.SQL:
                    DBAccess = new SqlDatabase(connectionString);
                    break;
                case AccessDBType.Oracle:
                    DBAccess = new OracleDatabase(connectionString);
                    break;
            }

            return DBAccess;
        }

        /// <summary>
        /// 用于得到数据库通用访问接口
        /// </summary>
        /// <param name="Type">数据库访问类型</param>
        /// <returns></returns>
        public static IDatabase GetDBAccess(AccessDBType type)
        {
            IDatabase DBAccess = null;

            switch (type)
            {
                case AccessDBType.SQL:
                    DBAccess = new SqlDatabase();
                    break;
                case AccessDBType.Oracle:
                    DBAccess = new OracleDatabase();
                    break;
            }

            return DBAccess;
        }
    }

    /// <summary>
    /// 数据库通用访问接口
    /// </summary>
    public interface IDatabase
    {
        #region 属性
        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnectionString
        {
            get;
            set;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 打开连接
        /// </summary>
        void Open();

        /// <summary>
        /// 关闭连接
        /// </summary>
        void Close();

        #region 执行命令
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>返回所影响的行数</returns>
        int ExcuteCommand(string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        DataSet GetDataSetFromExcuteCommand(string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        DataSet GetDataSetFromExcuteCommand(string cmdText, string tableName, IDataParameter[] paras = null);

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 查询执行命令通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, string tableName, IDataParameter[] paras = null);

        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        void ExcuteProc(string procName, IDataParameter[] paras = null);

        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        DataSet GetDataSetFromExcuteProc(string procName, IDataParameter[] paras = null);

        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        DataSet GetDataSetFromExcuteProc(string procName, string tableName, IDataParameter[] paras = null);

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, IDataParameter[] paras = null);

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, string tableName, IDataParameter[] paras = null);
        #endregion

        #region 执行事务
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="SQLList">要执行的SQL语句列表</param>
        bool ExcuteTransaction(List<string> SQLList);
        #endregion

        #region 获取第一行第一列的值
        /// <summary>
        /// 执行命令获取第一行第一列的值
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        object ExecuteCommandScalar(string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 执行存储过程获取第一行第一列的值
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        object ExecuteProcScalar(string procName, IDataParameter[] paras = null);
        #endregion

        #region 获取数据读取器
        /// <summary>
        /// 执行命令获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>数据读取器</returns>
        IDataReader GetDataReaderFromExcuteCommand(string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 执行命令通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        void GetDataReaderFromExcuteCommand(ref IDataReader dr, string cmdText, IDataParameter[] paras = null);

        /// <summary>
        /// 执行存储过程获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>数据读取器</returns>
        IDataReader GetDataReaderFromExcuteProc(string procName, IDataParameter[] paras = null);

        /// <summary>
        /// 执行存储过程通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        void GetDataReaderFromRefExcuteProc(ref IDataReader dr, string procName, IDataParameter[] paras = null);
        #endregion

        #endregion
    }
}
