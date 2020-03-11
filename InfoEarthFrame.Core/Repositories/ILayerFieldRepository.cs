using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Data;

namespace InfoEarthFrame.Core.Repositories
{
	public interface ILayerFieldRepository : IRepository<LayerFieldEntity, string>
	{
        bool ExecuteSql(string strSQL, string tableName1, string tableName2);

        DataTable GetLayerAttrs(string strSQL, string tableName);

        object ExecuteScalar(string strSQL);

        int ExecuteNonQuery(string strSQL);

    }
}

