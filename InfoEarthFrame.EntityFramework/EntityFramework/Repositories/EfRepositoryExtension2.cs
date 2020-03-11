using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.EntityFramework.Repositories
{
    public static class EfRepositoryExtension2
    {
        public static string GenerateInsertSql<TEntity, TPrimaryKey, TDbEntity>(this IRepository<TEntity, TPrimaryKey> repository, TDbEntity model) 
            where TEntity : class, Abp.Domain.Entities.IEntity<TPrimaryKey>, new()
            where TDbEntity:Entity<string>
        {
            var type = typeof(TDbEntity);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var tableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            if (tableAttr == null)
            {
                throw new Exception("[" + type.Name + "]缺少特性[Table]");
            }

            var tableName = tableAttr.Name;
            var sql = new StringBuilder();
            if (properties != null
                && properties.Any())
            {
                sql.AppendFormat(" insert into \"{0}\" ({1})", tableName, string.Join(",", properties.Select(p => string.Format("\"{0}\"", p.Name))));
                sql.Append("values(");
                sql.Append(string.Join(",", properties.Select(p => GetValueString(p,model))));
                sql.Append(")");
            }
            return sql.ToString();
        }


        public static string GenerateUpdateSql<TEntity, TPrimaryKey, TDbEntity>(this IRepository<TEntity, TPrimaryKey> repository, TDbEntity model,string primaryKeyName="Id")
            where TEntity : class, Abp.Domain.Entities.IEntity<TPrimaryKey>, new()
            where TDbEntity : Entity<string>
        {
            var type = typeof(TDbEntity);
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var tableAttr = type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>();
            if (tableAttr == null)
            {
                throw new Exception("[" + type.Name + "]缺少特性[Table]");
            }

            var tableName = tableAttr.Name;
            var sql = new StringBuilder();
            var primaryKeyProp = properties.FirstOrDefault(p => p.Name.ToLower() == primaryKeyName.ToLower());
            if (primaryKeyProp == null)
            {
                throw new Exception("[" + type.Name + "]未找到主键字段["+primaryKeyName+"]");
            }

            if (properties != null
                && properties.Any())
            {
                sql.AppendFormat(" update \"{0}\" set {1}", tableName, string.Join(",", properties.Select(p => string.Format("\"{0}\"={1}", p.Name, GetValueString(p, model)))));
                sql.AppendFormat(" where \"{0}\"={1}", primaryKeyName, GetValueString(primaryKeyProp,model));
            }
            return sql.ToString();
        }


        private static string GetValueString(PropertyInfo prop,object obj)
        {
            var fullName=prop.PropertyType.FullName;
            switch (fullName)
            {
                case "System.String":
                    return "'" + prop.GetValue(obj) + "'";
                default:
                    if (fullName.Contains("System.DateTime"))
                    {
                        var value = prop.GetValue(obj);
                        var s = Convert.ToString(value);
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            return "null";
                        }
                        return "'" + DateTime.Parse(s).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                    }
                    else if(fullName.Contains("System.Int"))
                    {
                        var value = prop.GetValue(obj);
                        var s = Convert.ToString(value);
                        if (string.IsNullOrWhiteSpace(s))
                        {
                            return "null";
                        }
                        return s;
                    }
                    return "'" + prop.GetValue(obj) + "'";
            }
        }
    }
}
