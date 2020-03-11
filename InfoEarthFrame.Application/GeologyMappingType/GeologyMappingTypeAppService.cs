using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using System.Configuration;
using InfoEarthFrame.Common;
using System.Collections;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using System.Net.Http;
using Newtonsoft.Json;
using InfoEarthFrame.Core;


namespace InfoEarthFrame.Application
{
    public class GeologyMappingTypeAppService : ApplicationService, IGeologyMappingTypeAppService
    {
        public readonly Core.IGeologyMappingTypeRepository _GeologyMappingType = null;

        public GeologyMappingTypeAppService(Core.IGeologyMappingTypeRepository dataGeologyMappingType)
        {
            _GeologyMappingType = dataGeologyMappingType;
        }


        public GeologyMappingTypeDto GetGeologyMappingType(string id)
        {
            var model = _GeologyMappingType.Get(id);
            var dto = AutoMapper.Mapper.Map<GeologyMappingTypeDto>(model);

            dto.ParentName = GetClassName(dto.ParentID);
            return dto;
        }

        /// <summary>
        /// 获取类型列表 
        /// </summary>
        /// <returns></returns>
        public List<InfoEarthFrame.Core.GeologyMappingType> GetGeologyMappingTypeList()
        {
            try
            {
                return _GeologyMappingType.GetAllList();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// 取区域图
        /// </summary>
        /// <param name="geologyMappingTypeInput"></param>
        /// <returns></returns>
        public List<InfoEarthFrame.Core.GeologyMappingType> GetAreaGeologyMappingList(GeologyMappingTypeInput gmtInput)
        {

            List<InfoEarthFrame.Core.GeologyMappingType> listGMTD = new List<InfoEarthFrame.Core.GeologyMappingType>();

            try
            {
                var listEntity = _GeologyMappingType.GetAllList();

                listGMTD = listEntity.FindAll(
                    delegate(InfoEarthFrame.Core.GeologyMappingType gmt)
                    {
                        return (gmt.Paths.Contains(gmtInput.Id) || gmt.Id.Contains(gmtInput.Id));
                    });
            }
            catch (Exception e)
            {
                throw e;
            }

            return listGMTD;
        }


        /// <summary>
        /// 最新---取区域图树结构数据
        /// </summary>
        /// <param name="geologyMappingTypeInput"></param>
        /// <returns></returns>
        public async Task<List<GeologyMappingTypeOutput>> GetMappingTree(string userId)
        {
            List<GeologyMappingTypeOutput> rootList = new List<GeologyMappingTypeOutput>();
            var result = await _GeologyMappingType.GetAllListAsync();
            List<GeologyMappingType> rightList = new List<GeologyMappingType>();
            string ProvinceID = System.Configuration.ConfigurationManager.AppSettings["ProvinceID"] ?? "";
            List<GeologyMappingType> returnList = result.FindAll(//根据ParentID == nul判断查找根节点
                delegate(GeologyMappingType gmt)
                {
                    if (ProvinceID == "0")
                    {
                        return (gmt.ParentID == null);
                    }
                    else
                    {
                        return (gmt.ParentID == ProvinceID);
                    }
                });

            returnList = returnList.OrderBy(s => s.Sn).ToList();
            if (userId.ToLower() != "admin")
            {
                using (var db = new InfoEarthFrameDbContext())
                {
                    var sql = "select * FROM" +
    "	\"TBL_GEOLOGYMAPPINGTYPE\" T where  EXISTS(" +
    "select \"LayerId\"  from \"TBL_GROUP_RIGHT\" r " +
    " where r.\"GroupId\" in(" +
    "select u.\"GroupId\"  from \"TBL_GROUP_USER\" u where u.\"UserId\"=\'" + userId + "\' and r.\"IsBrowse\"=1" +
    "group by u.\"GroupId\" " +
    ") and t.\"Id\"=r.\"LayerId\"" +
    ")";

                    rightList = db.Database.SqlQuery<GeologyMappingType>(sql).ToList();
                    var roots=rightList.Where(p=>string.IsNullOrEmpty(p.ParentID)).ToList();
                    returnList = returnList.Where(p => roots.Any(p1 => p1.Id == p.Id)).ToList();
                }
            }

                foreach (GeologyMappingType returnItem in returnList)
                {
                    GeologyMappingTypeOutput rootItem = new GeologyMappingTypeOutput();
                    rootItem.Id = returnItem.Id;
                    rootItem.Pid = returnItem.ParentID;
                    rootItem.Label = returnItem.ClassName;
                    rootItem.Paths = returnItem.Paths;
                    rootItem.Sn = returnItem.Sn;
                    rootItem.Children = CreatTree(result, rightList, returnItem.Id, userId);
                    rootList.Add(rootItem);
                }
                return rootList;
        }

        //递归创建树
        private List<GeologyMappingTypeOutput> CreatTree(List<GeologyMappingType> list, List<GeologyMappingType> rightList,string pid, string userId)
        {
            List<GeologyMappingTypeOutput> treeList = new List<GeologyMappingTypeOutput>();
            var childList = list.FindAll(
            delegate(GeologyMappingType gmt)
            {
                return (gmt.ParentID == pid);
            });

            if (userId.ToLower() != "admin")
            {
                    foreach (GeologyMappingType childItem in childList)
                    {
                    foreach (var right in rightList)
                        {
                            if (right.Id == childItem.Id)
                            {
                                GeologyMappingTypeOutput item = new GeologyMappingTypeOutput();
                                item.Id = childItem.Id;
                                item.Pid = childItem.ParentID;
                                item.Label = childItem.ClassName;
                                item.Paths = childItem.Paths;
                                item.Sn = childItem.Sn;
                                item.Children = CreatTree(list, rightList,item.Id, userId);
                                treeList.Add(item);
                                break;
                            }
                        }
            }}
            else
            {
                foreach (GeologyMappingType childItem in childList)
                {

                    GeologyMappingTypeOutput item = new GeologyMappingTypeOutput();
                    item.Id = childItem.Id;
                    item.Pid = childItem.ParentID;
                    item.Label = childItem.ClassName;
                    item.Paths = childItem.Paths;
                    item.Sn = childItem.Sn;
                    item.Children = CreatTree(list, rightList,item.Id, userId);
                    treeList.Add(item);
                }
            }

            return treeList;
        }


        public Tuple<bool, GeologyMappingType> Insert(GeologyMappingTypeDto dto)
        {
            var id = Guid.NewGuid().ToString();
            var db = _GeologyMappingType.GetDbContext();
            if (string.IsNullOrEmpty(dto.ParentID))
            {
                var model = new GeologyMappingType
                {
                    ClassName = dto.ClassName,
                    Id = id,
                    Paths = id
                };
                var sql = _GeologyMappingType.GenerateInsertSql(model);
                var flag = db.Database.ExecuteSqlCommand(sql) > 0;
                return new Tuple<bool, GeologyMappingType>(flag, model);
            }
            else
            {
                //获取父级
                var parent = GetGeologyMappingType(dto.ParentID);
                if (parent == null)
                {
                    return new Tuple<bool, GeologyMappingType>(false, null);
                }

                var model = new GeologyMappingType
                {
                    ClassName = dto.ClassName,
                    Id = id,
                    Paths = parent.Paths + "." + id,
                    ParentID = dto.ParentID
                };
                var sql = _GeologyMappingType.GenerateInsertSql(model);
                var flag = db.Database.ExecuteSqlCommand(sql) > 0;
                return new Tuple<bool, GeologyMappingType>(flag, model);
            }
        }

        public Tuple<bool, GeologyMappingType> Update(GeologyMappingTypeDto dto)
        {
            var model = GetGeologyMappingType(dto.Id);
            if (model == null)
            {
                return new Tuple<bool, GeologyMappingType>(false, null);
            }

            model.ParentID = dto.ParentID;

            //获取父级
            var parent = GetGeologyMappingType(dto.ParentID);
            if (parent == null)
            {
                return new Tuple<bool, GeologyMappingType>(false, null);
            }

            model.Paths = parent.Paths + "." + dto.Id;
            model.ClassName = dto.ClassName;

            var entity = model.MapTo<GeologyMappingType>();
            var db = _GeologyMappingType.GetDbContext();
            var sql = _GeologyMappingType.GenerateUpdateSql(entity);
            var flag = db.Database.ExecuteSqlCommand(sql) > 0;


            return new Tuple<bool, GeologyMappingType>(flag, entity);
        }


        public bool IsClassNameExists(string parentId, string className)
        {
            var sql = "select \"Id\"   from \"TBL_GEOLOGYMAPPINGTYPE\" t where t.\"ParentID\"='" + parentId + "' and t.\"ClassName\"='" + className + "' ";
            var obj = _GeologyMappingType.GetDbContext().Database.SqlQuery<string>(sql).FirstOrDefault();
            return !string.IsNullOrEmpty(obj);
        }

        public string GetClassName(string id)
        {
            var sql = "select \"ClassName\"   from \"TBL_GEOLOGYMAPPINGTYPE\" t where t.\"Id\"='" + id + "'";
            var obj = _GeologyMappingType.GetDbContext().Database.SqlQuery<string>(sql).FirstOrDefault();
            return obj;
        }


        public async Task<bool> Delete(IEnumerable<string> ids)
        {
            var sql = "delete from \"TBL_GEOLOGYMAPPINGTYPE\" where \"Id\" in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
            var db = _GeologyMappingType.GetDbContext();
            var flag = await db.Database.ExecuteSqlCommandAsync(sql) > 0;
            return flag;
        }


        public int ImportMappingType(RowData data)
        {
            var db = _GeologyMappingType.GetDbContext();
            var count=0;
            if (data.Rows != null && data.Rows.Any())
            {
                foreach (var row in data.Rows)
                {
                    var model = new GeologyMappingType
                    {
                        ClassName = row.ClassName,
                        Id = row.Id,
                        Paths = row.Paths,
                        ParentID = row.ParentID
                    };
                    var sql = _GeologyMappingType.GenerateInsertSql(model);
                    try
                    {
                        var flag = db.Database.ExecuteSqlCommand(sql) > 0;
                        if (flag)
                        {
                            count++;
                        }
                    }
                    catch
                    { 
                    
                    }
                }
            }
            return count;
        }


        public List<GeologyMappingType> GetChildType(string parentId)
        {
            var db = _GeologyMappingType.GetDbContext();
            var sql="select * from \"TBL_GEOLOGYMAPPINGTYPE\" where \"ParentID\"='"+parentId+"'";
            return db.Database.SqlQuery<GeologyMappingType>(sql).ToList();
        }
    }
}
