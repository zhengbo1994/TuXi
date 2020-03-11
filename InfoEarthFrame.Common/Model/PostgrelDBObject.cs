using System;
using System.Collections.Generic;
using System.Text;

namespace InfoEarthFrame.Common.Data
{
    public class PostgrelDBObject
    {
        /// <summary>
        /// 数据库名称
        /// </summary>
        private string _sqlDB = "gisdatamanage";

        public string SqlDB
        {
            get { return _sqlDB; }
            set { _sqlDB = value; }
        }

        /// <summary>
        /// 用户名
        /// </summary>
        private string _sqlUser = "postgres";

        public string SqlUser
        {
            get { return _sqlUser; }
            set { _sqlUser = value; }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        private string _sqlPwd = "123456";

        public string SqlPwd
        {
            get { return _sqlPwd; }
            set { _sqlPwd = value; }
        }

        /// <summary>
        /// 数据库服务IP地址(可选)
        /// </summary>
        private string _sqlServer = "192.168.1.63";

        public string SqlServer
        {
            get { return _sqlServer; }
            set { _sqlServer = value; }
        }

        /// <summary>
        /// 数据库服务端口(可选)
        /// </summary>
        private string _sqlPort = "5432";

        public string SqlPort
        {
            get { return _sqlPort; }
            set { _sqlPort = value; }
        }

        /// <summary>
        /// SQL数据库连接字符串
        /// </summary>
        private string _connStr = null;

        public string ConnStr
        {
            set { _connStr = value; }
            get
            {
                if (string.IsNullOrEmpty(_connStr))
                {
                    return String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4}", _sqlServer, _sqlPort, _sqlUser, _sqlPwd, _sqlDB);
                }
                else
                {
                    return _connStr;
                }
            }
        }

        /// <summary>
        /// 构造函数（SQL Server用户验证方式）
        /// </summary>
        /// <param name="db">数据库名称</param>
        /// <param name="user">用户名</param>
        /// <param name="psw">密码</param>
        /// <param name="server">服务器IP</param>
        public PostgrelDBObject(string db, string user, string psw, string server)
        {
            _sqlDB = db;
            _sqlUser = user;
            _sqlPwd = psw;
            _sqlServer = server;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public PostgrelDBObject() { }
    }
}
