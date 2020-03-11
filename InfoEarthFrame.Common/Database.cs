using System;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Collections.Generic;

namespace InfoEarthFrame.Data
{
    #region SQL Server 数据库
    /// <summary>
    /// SQL Server 数据库
    /// </summary>
    class SqlDatabase : IDatabase
    {
        #region SQL数据库访问类字段

        private string connectionString;		// 数据库连接字符串
        private SqlConnection conn;				// 数据库连接对象

        #endregion

        #region SQL数据库访问构造方法
        /// <summary>
        /// 无参SQL数据库访问构造方法
        /// </summary>
        public SqlDatabase()
        {
            connectionString = "";
            conn = new SqlConnection();
        }

        /// <summary>
        /// 有参SQL数据库访问构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public SqlDatabase(string connectionString)
        {
            this.connectionString = connectionString;
            conn = new SqlConnection(connectionString);
        }
        #endregion

        #region 命令文本类型枚举
        private enum cmdTextType	// 命令文本类型
        {
            Command,	// 命令
            Procedure	// 存储过程
        }
        #endregion

        #region 获取命令对象
        /// <summary>
        /// 获取命令对象
        /// </summary>
        /// <param name="cmdText">命令文本</param>
        /// <param name="type">命令文本类型</param>
        /// <param name="paras">命令文本所带参数</param>
        /// <returns>命令对象</returns>
        private SqlCommand GetCommand(string cmdText, cmdTextType type, IDataParameter[] paras)
        {
            SqlCommand cmd = new SqlCommand(cmdText, conn);	// 生成命令对象

            switch (type)				// 指定命令类型
            {
                case cmdTextType.Command:
                    cmd.CommandType = CommandType.Text;
                    break;
                case cmdTextType.Procedure:
                    cmd.CommandType = CommandType.StoredProcedure;
                    break;
            }

            if (paras != null)		// 为命令添加参数
            {
                foreach (IDataParameter Para in paras)
                {
                    cmd.Parameters.Add(Para);
                }
            }

            return cmd;
        }
        #endregion

        #region 执行命令获取第一行第一列的数据
        /// <summary>
        /// 执行命令获取第一行第一列的数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private object GetScalar(SqlCommand cmd)
        {
            object obj = null;

            try
            {
                Open();						// 打开连接
                obj = cmd.ExecuteScalar();	// 执行命令获取第一行第一列数据
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取第一行第一列的数据错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }

            return obj;
        }
        #endregion

        #region 获取数据读取器
        /// <summary>
        /// 获取数据读取器
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <returns>数据读取器</returns>
        private SqlDataReader GetDataReader(SqlCommand cmd)
        {
            SqlDataReader Reader = null;

            try
            {
                Open();							// 打开连接
                Reader = cmd.ExecuteReader();	// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }

            return Reader;
        }
        #endregion

        #region IDataAccess 成员

        #region 数据库连接字符串属性
        public string ConnectionString
        {
            get
            {
                // TODO:  添加 SqlAccess.connectionString getter 实现
                return connectionString;
            }
            set
            {
                // TODO:  添加 SqlAccess.connectionString setter 实现
                connectionString = value;
                conn.ConnectionString = connectionString;
            }
        }
        #endregion

        #region 打开连接
        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            // TODO:  添加 SqlAccess.Open 实现
            try
            {
                if (conn.State != ConnectionState.Open)	// 判断数据库连接状态是否打开
                {
                    conn.Open();							// 打开数据库连接
                }
            }
            catch (Exception exp)
            {
                throw (new Exception("打开数据库连接错误：" + exp.Message));
            }
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            // TODO:  添加 SqlAccess.Close 实现
            try
            {
                if (conn.State != ConnectionState.Closed)	// 判断数据库连接是否关闭
                {
                    conn.Close();							// 关闭数据库连接
                }
            }
            catch (Exception exp)
            {
                throw (new Exception("关闭数据库连接错误：" + exp.Message));
            }
        }
        #endregion

        #region 执行命令
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>返回所影响的行数</returns>
        public int ExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExcuteCommand 实现
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令

            try
            {
                Open();						        // 打开连接
                return cmd.ExecuteNonQuery();		// 执行命令
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }
        #endregion

        #region 查询执行命令获取数据记录集
        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataSetFromExcuteCommand 实现
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);										// 创建数据适配器
            DataSet ds = new DataSet();															// 创建数据记录集

            try
            {
                da.Fill(ds);				// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据记录集错误：" + exp.Message));
            }

            return ds;
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteCommand(string cmdText, string tableName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataSetFromExcuteCommand 实现
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);										// 创建数据适配器
            DataSet ds = new DataSet();															// 创建数据记录集

            try
            {
                da.Fill(ds, tableName);				// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据记录集错误：" + exp.Message));
            }

            return ds;
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);										// 创建数据适配器

            try
            {
                return da.Fill(ds);				// 数据适配器填充数据记录集
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, string tableName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);										// 创建数据适配器

            try
            {
                return da.Fill(ds, tableName);				// 数据适配器填充数据记录集
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
        }

        #endregion

        #region 执行事务
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="SQLList">要执行的SQL语句列表</param>
        /// <returns>返回是否成功</returns>
        public bool ExcuteTransaction(List<string> SQLList)
        {
            // TODO:  添加 SqlAccess.ExcuteTransaction 实现

            SqlTransaction SQLTran = null;

            try
            {
                if (conn.State != ConnectionState.Open)	// 判断数据库连接状态是否打开
                {
                    conn.Open();							// 打开数据库连接
                }

                SQLTran = conn.BeginTransaction();

                for (int i = 0; i < SQLList.Count; i++)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQLList[i];
                    cmd.Transaction = SQLTran;
                    cmd.ExecuteNonQuery();
                }
                SQLTran.Commit();

                return true;
            }
            catch (Exception exp)
            {
                if (SQLTran != null)
                {
                    SQLTran.Rollback();
                }
                throw (new Exception(exp.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        public void ExcuteProc(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExcuteProc 实现
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令

            try
            {
                Open();					// 打开数据库连接
                cmd.ExecuteNonQuery();	// 执行命令
            }

            catch (Exception exp)
            {
                throw (new Exception("执行存储过程错误：" + exp.Message));
            }
            finally
            {
                Close();				// 关闭数据库连接
            }
        }
        #endregion

        #region 执行查询存储过程获取数据记录集
        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteProc(string procName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);			// 创建数据适配器
            DataSet ds = new DataSet();								// 创建数据记录集

            try
            {
                da.Fill(ds);							            // 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }

            return ds;
        }

        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteProc(string procName, string tableName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);			// 创建数据适配器
            DataSet ds = new DataSet();								// 创建数据记录集

            try
            {
                da.Fill(ds, tableName);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }

            return ds;
        }

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);			// 创建数据适配器

            try
            {
                return da.Fill(ds);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
        }

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, string tableName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            SqlDataAdapter da = new SqlDataAdapter(cmd);			// 创建数据适配器

            try
            {
                return da.Fill(ds, tableName);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
        }

        #endregion

        #region 执行命令获取第一行第一列的值
        /// <summary>
        /// 执行命令获取第一行第一列的值
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        public object ExecuteCommandScalar(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExecuteCommandScalar 实现
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            object obj = null;

            try
            {
                obj = GetScalar(cmd);		// 获取第一行第一列的值
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取第一行第一列的数据错误：" + exp.Message));
            }

            return obj;
        }
        #endregion

        #region 执行存储过程获取第一行第一列的值
        /// <summary>
        /// 执行存储过程获取第一行第一列的值
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        public object ExecuteProcScalar(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExecuteProcScalar 实现
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            object obj = null;

            try
            {
                obj = GetScalar(cmd);		// 获取第一行第一列的值
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取第一行第一列的数据错误：" + exp.Message));
            }

            return obj;
        }
        #endregion

        #region 执行命令获取数据读取器
        /// <summary>
        /// 执行命令获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>数据读取器</returns>
        public IDataReader GetDataReaderFromExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataReaderFromExcuteCommand 实现
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);		// 获取命令
            SqlDataReader Reader = null;

            try
            {
                Reader = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }

            return Reader;
        }

        /// <summary>
        /// 执行命令通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        public void GetDataReaderFromExcuteCommand(ref IDataReader dr, string cmdText, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);		// 获取命令

            try
            {
                dr = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }
        }

        #endregion

        #region 执行存储过程获取数据读取器
        /// <summary>
        /// 执行存储过程获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>数据读取器</returns>
        public IDataReader GetDataReaderFromExcuteProc(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataReaderFromExcuteProc 实现
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            SqlDataReader Reader = null;

            try
            {
                Reader = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据读取器错误：" + exp.Message));
            }

            return Reader;
        }

        /// <summary>
        /// 执行存储过程通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        public void GetDataReaderFromRefExcuteProc(ref IDataReader dr, string procName, IDataParameter[] paras)
        {
            SqlCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令

            try
            {
                dr = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据读取器错误：" + exp.Message));
            }
        }
        #endregion

        #endregion
    }

    #endregion

    #region Oracle 数据库
    /// <summary>
    /// SQL Server 数据库
    /// </summary>
    class OracleDatabase : IDatabase
    {
        #region SQL数据库访问类字段

        private string connectionString;		// 数据库连接字符串
        private OracleConnection conn;				// 数据库连接对象

        #endregion

        #region SQL数据库访问构造方法
        /// <summary>
        /// 无参SQL数据库访问构造方法
        /// </summary>
        public OracleDatabase()
        {
            connectionString = "";
            conn = new OracleConnection();
        }

        /// <summary>
        /// 有参SQL数据库访问构造方法
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public OracleDatabase(string connectionString)
        {
            this.connectionString = connectionString;
            conn = new OracleConnection(connectionString);
        }
        #endregion

        #region 命令文本类型枚举
        private enum cmdTextType	// 命令文本类型
        {
            Command,	// 命令
            Procedure	// 存储过程
        }
        #endregion

        #region 获取命令对象
        /// <summary>
        /// 获取命令对象
        /// </summary>
        /// <param name="cmdText">命令文本</param>
        /// <param name="type">命令文本类型</param>
        /// <param name="paras">命令文本所带参数</param>
        /// <returns>命令对象</returns>
        private OracleCommand GetCommand(string cmdText, cmdTextType type, IDataParameter[] paras)
        {
            OracleCommand cmd = new OracleCommand(cmdText, conn);	// 生成命令对象

            switch (type)				// 指定命令类型
            {
                case cmdTextType.Command:
                    cmd.CommandType = CommandType.Text;
                    break;
                case cmdTextType.Procedure:
                    cmd.CommandType = CommandType.StoredProcedure;
                    break;
            }

            if (paras != null)		// 为命令添加参数
            {
                foreach (IDataParameter Para in paras)
                {
                    cmd.Parameters.Add(Para);
                }
            }

            return cmd;
        }
        #endregion

        #region 执行命令获取第一行第一列的数据
        /// <summary>
        /// 执行命令获取第一行第一列的数据
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private object GetScalar(OracleCommand cmd)
        {
            object obj = null;

            try
            {
                Open();						// 打开连接
                obj = cmd.ExecuteScalar();	// 执行命令获取第一行第一列数据
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取第一行第一列的数据错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }

            return obj;
        }
        #endregion

        #region 获取数据读取器
        /// <summary>
        /// 获取数据读取器
        /// </summary>
        /// <param name="cmd">命令对象</param>
        /// <returns>数据读取器</returns>
        private OracleDataReader GetDataReader(OracleCommand cmd)
        {
            OracleDataReader Reader = null;

            try
            {
                Open();							// 打开连接
                Reader = cmd.ExecuteReader();	// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }

            return Reader;
        }
        #endregion

        #region IDataAccess 成员

        #region 数据库连接字符串属性
        public string ConnectionString
        {
            get
            {
                // TODO:  添加 OracleAccess.connectionString getter 实现
                return connectionString;
            }
            set
            {
                // TODO:  添加 OracleAccess.connectionString setter 实现
                connectionString = value;
                conn.ConnectionString = connectionString;
            }
        }
        #endregion

        #region 打开连接
        /// <summary>
        /// 打开连接
        /// </summary>
        public void Open()
        {
            // TODO:  添加 OracleAccess.Open 实现
            try
            {
                if (conn.State != ConnectionState.Open)	// 判断数据库连接状态是否打开
                {
                    conn.Open();							// 打开数据库连接
                }
            }
            catch (Exception exp)
            {
                throw (new Exception("打开数据库连接错误：" + exp.Message));
            }
        }
        #endregion

        #region 关闭连接
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            // TODO:  添加 OracleAccess.Close 实现
            try
            {
                if (conn.State != ConnectionState.Closed)	// 判断数据库连接是否关闭
                {
                    conn.Close();							// 关闭数据库连接
                }
            }
            catch (Exception exp)
            {
                throw (new Exception("关闭数据库连接错误：" + exp.Message));
            }
        }
        #endregion

        #region 执行命令
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>返回所影响的行数</returns>
        public int ExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 OracleAccess.ExcuteCommand 实现
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令

            try
            {
                Open();						        // 打开连接
                return cmd.ExecuteNonQuery();		// 执行命令
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }
        #endregion

        #region 查询执行命令获取数据记录集
        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 OracleAccess.GetDataSetFromExcuteCommand 实现
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);										// 创建数据适配器
            DataSet ds = new DataSet();															// 创建数据记录集

            try
            {
                da.Fill(ds);				// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return ds;
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>执行查询命令获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteCommand(string cmdText, string tableName, IDataParameter[] paras)
        {
            // TODO:  添加 OracleAccess.GetDataSetFromExcuteCommand 实现
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);										// 创建数据适配器
            DataSet ds = new DataSet();															// 创建数据记录集

            try
            {
                da.Fill(ds, tableName);				// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return ds;
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);										// 创建数据适配器

            try
            {
                return da.Fill(ds);				// 数据适配器填充数据记录集
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }

        /// <summary>
        /// 查询执行命令获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteCommand(ref DataSet ds, string cmdText, string tableName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);										// 创建数据适配器

            try
            {
                return da.Fill(ds, tableName);				// 数据适配器填充数据记录集
            }
            catch (Exception ex)
            {
                throw (new Exception(ex.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }

        #endregion

        #region 执行事务
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="SQLList">要执行的SQL语句列表</param>
        /// <returns>返回是否成功</returns>
        public bool ExcuteTransaction(List<string> SQLList)
        {
            // TODO:  添加 SqlAccess.ExcuteTransaction 实现

            OracleTransaction OracleTran = null;

            try
            {
                if (conn.State != ConnectionState.Open)	// 判断数据库连接状态是否打开
                {
                    conn.Open();							// 打开数据库连接
                }

                OracleTran = conn.BeginTransaction();

                for (int i = 0; i < SQLList.Count; i++)
                {
                    OracleCommand cmd = new OracleCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = SQLList[i];
                    cmd.Transaction = OracleTran;
                    cmd.ExecuteNonQuery();
                }
                OracleTran.Commit();

                return true;
            }
            catch (Exception exp)
            {
                if (OracleTran != null)
                {
                    OracleTran.Rollback();
                }
                throw (new Exception(exp.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        public void ExcuteProc(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExcuteProc 实现
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令

            try
            {
                Open();					// 打开数据库连接
                cmd.ExecuteNonQuery();	// 执行命令
            }

            catch (Exception exp)
            {
                throw (new Exception("执行存储过程错误：" + exp.Message));
            }
            finally
            {
                Close();				// 关闭数据库连接
            }
        }
        #endregion

        #region 执行查询存储过程获取数据记录集
        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteProc(string procName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);			// 创建数据适配器
            DataSet ds = new DataSet();								// 创建数据记录集

            try
            {
                da.Fill(ds);							            // 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return ds;
        }

        /// <summary>
        /// 执行查询存储过程获取数据记录集
        /// </summary>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>执行查询存储过程获取的数据记录集</returns>
        public DataSet GetDataSetFromExcuteProc(string procName, string tableName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);			// 创建数据适配器
            DataSet ds = new DataSet();								// 创建数据记录集

            try
            {
                da.Fill(ds, tableName);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return ds;
        }

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);			// 创建数据适配器

            try
            {
                return da.Fill(ds);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }

        /// <summary>
        /// 执行查询存储过程通过引用获取数据记录集
        /// </summary>
        /// <param name="ds">查询执行命令获取数据记录集</param>
        /// <param name="procName">查询存储过程名</param>
        /// <param name="tableName">查询表名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>返回在DataSet指定范围中添加或刷新行以匹配使用DataSet名称的数据源的行数</returns>
        public int GetDataSetFromRefExcuteProc(ref DataSet ds, string procName, string tableName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            OracleDataAdapter da = new OracleDataAdapter(cmd);			// 创建数据适配器

            try
            {
                return da.Fill(ds, tableName);							// 数据适配器填充数据记录集
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据记录集错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }

        #endregion

        #region 执行命令获取第一行第一列的值
        /// <summary>
        /// 执行命令获取第一行第一列的值
        /// </summary>
        /// <param name="cmdText">命令字符串</param>
        /// <param name="paras">命令所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        public object ExecuteCommandScalar(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExecuteCommandScalar 实现
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);	// 获取命令
            object obj = null;

            try
            {
                obj = GetScalar(cmd);		// 获取第一行第一列的值
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取第一行第一列的数据错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return obj;
        }
        #endregion

        #region 执行存储过程获取第一行第一列的值
        /// <summary>
        /// 执行存储过程获取第一行第一列的值
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">存储过程所带参数的数组</param>
        /// <returns>第一行第一列的值</returns>
        public object ExecuteProcScalar(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.ExecuteProcScalar 实现
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            object obj = null;

            try
            {
                obj = GetScalar(cmd);		// 获取第一行第一列的值
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取第一行第一列的数据错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return obj;
        }
        #endregion

        #region 执行命令获取数据读取器
        /// <summary>
        /// 执行命令获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        /// <returns>数据读取器</returns>
        public IDataReader GetDataReaderFromExcuteCommand(string cmdText, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataReaderFromExcuteCommand 实现
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);		// 获取命令
            OracleDataReader Reader = null;

            try
            {
                Reader = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return Reader;
        }

        /// <summary>
        /// 执行命令通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询命令字符串</param>
        /// <param name="paras">查询命令所带参数的数组</param>
        public void GetDataReaderFromExcuteCommand(ref IDataReader dr, string cmdText, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(cmdText, cmdTextType.Command, paras);		// 获取命令

            try
            {
                dr = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行命令获取数据读取器错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }

        #endregion

        #region 执行存储过程获取数据读取器
        /// <summary>
        /// 执行存储过程获取数据读取器
        /// </summary>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        /// <returns>数据读取器</returns>
        public IDataReader GetDataReaderFromExcuteProc(string procName, IDataParameter[] paras)
        {
            // TODO:  添加 SqlAccess.GetDataReaderFromExcuteProc 实现
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令
            OracleDataReader Reader = null;

            try
            {
                Reader = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据读取器错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
            return Reader;
        }

        /// <summary>
        /// 执行存储过程通过引用获取数据读取器
        /// </summary>
        /// <param name="dr">查询执行命令获取数据读取器</param>
        /// <param name="cmdText">查询存储过程名</param>
        /// <param name="paras">查询存储过程所带参数的数组</param>
        public void GetDataReaderFromRefExcuteProc(ref IDataReader dr, string procName, IDataParameter[] paras)
        {
            OracleCommand cmd = GetCommand(procName, cmdTextType.Procedure, paras);	// 获取命令

            try
            {
                dr = GetDataReader(cmd);		// 获取数据读取器
            }
            catch (Exception exp)
            {
                throw (new Exception("执行存储过程获取数据读取器错误：" + exp.Message));
            }
            finally
            {
                Close();					// 关闭连接
            }
        }
        #endregion

        #endregion
    }

    #endregion
}
