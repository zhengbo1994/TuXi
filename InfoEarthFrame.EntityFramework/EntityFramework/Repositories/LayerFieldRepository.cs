using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.EntityFramework;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public class LayerFieldRepository : InfoEarthFrameRepositoryBase<LayerFieldEntity, string>, ILayerFieldRepository
    {
        private IDbContextProvider<InfoEarthFrameDbContext> _dbContextProvider;
        public LayerFieldRepository(IDbContextProvider<InfoEarthFrameDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;

        }

        public bool ExecuteSql(string strSQL, string tableName1, string tableName2)
        {
            try
            {
                //string strSql = "CREATE TABLE @tableName "
                //    + "("
                //    + "`Id` varchar(36) NOT NULL COMMENT '数据类型ID'"
                //    + "," + "`TypeName` varchar(50) DEFAULT NULL COMMENT '类型名称'"
                //    + "," + "`TypeDesc` varchar(100) DEFAULT NULL COMMENT '类型描述'"
                //    + "," + "`TypeCode` varchar(36) DEFAULT NULL COMMENT '类型编码'"
                //    + "," + "`TypeSort` int(11) DEFAULT NULL COMMENT '类型排序'"
                //    + "," + "`ParentID` varchar(36) DEFAULT NULL COMMENT '父数据类型ID'"
                //    + "," + "`Keywords` varchar(100) DEFAULT NULL COMMENT '关键字'"
                //    + "," + "PRIMARY KEY (`Id`)"
                //    + ")"
                //   + "ENGINE=InnoDB DEFAULT CHARSET=utf8 COMMENT='基本代码字典类型表';";
                strSQL = strSQL.Replace("@tableName1", tableName1);
                strSQL = strSQL.Replace("@tableName2", tableName2);
                var ctx = new InfoEarthFrameDbContext();
                var test = ctx.Database.ExecuteSqlCommand(strSQL);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public DataTable GetLayerAttrs(string strSQL, string tableName)
        {
            try
            {
                var ctx = new InfoEarthFrameDbContext();
                var query = ctx.Database.SqlQuery<DataTable>(strSQL, new[]
                {
                    new[] { new SqlParameter("tableName", tableName) }
                });
                foreach (var item in query)
                {
                    return item;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }

        public object ExecuteScalar(string strSQL)
        {
            try
            {
                var ctx = new InfoEarthFrameDbContext();
                return ctx.Database.SqlQuery<object>(strSQL);
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

        public int ExecuteNonQuery(string strSQL)
        {
            try
            {
                var ctx = new InfoEarthFrameDbContext();
                return ctx.Database.ExecuteSqlCommand(strSQL);
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

    }
}

