using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Uow;
using Abp.AutoMapper;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using System.Net.Http;
using Newtonsoft.Json;
using System.Data.Entity;
using InfoEarthFrame.Common.Model;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    //[AbpAuthorize]
    public class LayerManagerAppService : ApplicationService, ILayerManagerAppService
    {
        private readonly ILayerManagerRepository _iLayerManagerRepository;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iLayerManagerRepository"></param>
        public LayerManagerAppService(ILayerManagerRepository iLayerManagerRepository)
        {
            _iLayerManagerRepository = iLayerManagerRepository;
        }

        /// <summary>
        /// 创建树
        /// </summary>
        /// <param name="list"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private List<Gislayer> CreatTree(List<LayerManagerDto> list, string pid)
        {
            List<Gislayer> treeList = new List<Gislayer>();
            var childList = list.FindAll(x => x.PID == pid);
            foreach (LayerManagerDto childItem in childList)
            {
                Gislayer item = new Gislayer();
                item.Id = childItem.Id;
                item.PID = childItem.PID;
                item.LABEL = childItem.LABEL;
                item.ZOOMLEVEL = childItem.ZOOMLEVEL;
                item.ZEROLEVELSIZE = childItem.ZEROLEVELSIZE;
                item.DATASERVERKEY = childItem.DATASERVERKEY;
                item.URL = childItem.URL;
                item.TILESIZE = childItem.TILESIZE;
                item.PICTYPE = childItem.PICTYPE;
                item.showCheckbox = true;
                item.@checked = false;
                item.children = CreatTree(list, item.Id);
                treeList.Add(item);
            }
            return treeList;
        }
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        public async Task<object> GetAllList()
        {
            //var result = await _iLayerManagerRepository.GetAllListAsync();
            var result = _iLayerManagerRepository.GetAllList();
            var list = result.MapTo<List<LayerManagerDto>>();

            LayerManagerDto map1 = list.First(x => x.PID == "1");//天地图
            LayerManagerDto note1 = list.First(x => x.PID == "2");//天地图标注
            LayerManagerDto map2 = list.First(x => x.PID == "3");//影像图
            LayerManagerDto note2 = list.First(x => x.PID == "4");//影像图标注
            List<Gislayer> gislist = CreatTree(list, "0000");//gis图层树

            return new
            {
                BaseMap = new { map = map1, note = note1 },
                StatelliteMap = new { map = map2, note = note1 },
                GisLayer = gislist
            };
        }

        /// <summary>
        /// 获取不带count的分页列表
        /// </summary>
        /// <param name="PageIndex"></param>
        /// <param name="PageSize"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ListResultOutput<LayerManagerDto>> GetPageList(QueryLayerManagerInput input)
        {
            var result = await _iLayerManagerRepository.GetPageList(input.Name, input.PageIndex, input.PageSize);
            var outputList = new ListResultOutput<LayerManagerDto>(result.MapTo<List<LayerManagerDto>>());
            return outputList;
        }
        /// <summary>
        /// 获取带count的分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultOutput<LayerManagerDto>> GetPageListAndCount(QueryLayerManagerInput input)
        {
            var result = await _iLayerManagerRepository.GetPageList(input.Name, input.PageIndex, input.PageSize);

            IReadOnlyList<LayerManagerDto> ir = result.MapTo<List<LayerManagerDto>>();
            int count = _iLayerManagerRepository.Count();
            PagedResultOutput<LayerManagerDto> outputList = new PagedResultOutput<LayerManagerDto>(count, ir);
            return outputList;
        }
        /// <summary>
        /// 获取符合的数据条数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> GetPageCount(QueryLayerManagerInput input)
        {
            int counts = await _iLayerManagerRepository.CountAsync();
            return counts;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public async Task<PagedResultOutput<LayerManagerDto>> Intsert(int pageIndex, int pageSize, LayerManagerInput input)
        {
            throw new NotImplementedException();
            //Tbl_LayerManager lm = new Tbl_LayerManager
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    PID = input.PID,
            //    LABEL = input.LABEL,
            //    ZOOMLEVEL = input.ZOOMLEVEL,
            //    URL = input.URL,
            //    DATASERVERKEY = input.DATASERVERKEY,
            //    TILESIZE = 512,
            //    ZEROLEVELSIZE = input.ZEROLEVELSIZE,
            //    PICTYPE = input.PICTYPE,
            //    CREATETIME = DateTime.Now
            //};
            //_iLayerManagerRepository.Insert(lm);
            //QueryLayerManagerInput queryInput = new QueryLayerManagerInput
            //{
            //    PageIndex = pageIndex,
            //    PageSize = pageSize
            //};

            //return await GetPageListAndCount(queryInput);
        }



        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [UnitOfWork(IsDisabled = true)]
        public async Task<PagedResultOutput<LayerManagerDto>> Update(int pageIndex, int pageSize, LayerManagerDto input)
        {
            //Tbl_LayerManager lm = _iLayerManagerRepository.Get(input.Id);
            ////lm.PID = input.PID;
            ////lm.LABEL = input.LABEL;
            ////lm.ZOOMLEVEL = input.ZOOMLEVEL;
            ////lm.URL = input.URL;
            ////lm.DATASERVERKEY = input.DATASERVERKEY;
            ////lm.TILESIZE = 512;
            ////lm.ZEROLEVELSIZE = input.ZEROLEVELSIZE;
            ////lm.PICTYPE = input.PICTYPE;
            ////_iLayerManagerRepository.Update(lm);
            ////QueryLayerManagerInput queryInput = new QueryLayerManagerInput
            ////{
            ////    PageIndex = pageIndex,
            ////    PageSize = pageSize
            ////};
            //return await GetPageListAndCount(queryInput);
            throw new NotImplementedException();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <param name="PageSize"></param>
        /// <returns>PageData</returns>
        [UnitOfWork(IsDisabled = true)]
        public async Task<PagedResultOutput<LayerManagerDto>> Delete(string id, int pageIndex, int pageSize)
        {
            _iLayerManagerRepository.Delete(id);
            QueryLayerManagerInput queryInput = new QueryLayerManagerInput
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            return await GetPageListAndCount(queryInput);
        }

        /// <summary>
        /// 绑定下拉框 根据父ID查询
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public async Task<ListResultOutput<LayerManagerDto>> GetLayerManagerByPID()
        {
            var result = _iLayerManagerRepository.GetAll().Where(q => q.PID == "0000");
            var outputList = new ListResultOutput<LayerManagerDto>(result.MapTo<List<LayerManagerDto>>());
            return outputList;
        }


        public async Task<bool> Insert(Tbl_LayerManager model)
        {
            model.Id = Guid.NewGuid().ToString();
            model.CreateTime = DateTime.Now;
            model.TILESIZE = 512;
            var db = (InfoEarthFrameDbContext)_iLayerManagerRepository.GetDbContext();
            db.LayerManagers.Add(model);
            return await db.SaveChangesAsync() > 0;
        }

        public async Task<bool> Update(Tbl_LayerManager model)
        {
            var db = (InfoEarthFrameDbContext)_iLayerManagerRepository.GetDbContext();

            var oldModel = db.LayerManagers.FirstOrDefault(p => p.Id == model.Id);
            if (oldModel == null)
            {
                throw new Exception("未找到记录");
            }

            oldModel.INDEXID = model.INDEXID;
            oldModel.TEXT = model.TEXT;
            oldModel.URL = model.URL;
            oldModel.DATASERVERKEY = model.DATASERVERKEY;
            oldModel.TILESIZE = model.TILESIZE ?? 512;
            oldModel.ZEROLEVELSIZE = model.ZEROLEVELSIZE;
            oldModel.PICTYPE = model.PICTYPE;
            oldModel.DataMainID = model.DataMainID;
            oldModel.PID = model.PID;

            db.Entry(oldModel).State = EntityState.Modified;
            return await db.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<Tbl_LayerManager> GetDetailById(string id)
        {
            var query = await _iLayerManagerRepository.GetAsync(id);
            return query;
        }


        public async Task<Tbl_LayerManager> GetDetailByMainId(string mainId)
        {
            var query = await _iLayerManagerRepository.FirstOrDefaultAsync(p => p.DataMainID == mainId);
            return query;
        }
        public async Task<bool> Delete(IEnumerable<string> ids)
        {
            var sql = "delete from \"TBL_LAYERMANAGER\" where \"Id\" in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
            var db = _iLayerManagerRepository.GetDbContext();
            var flag = await db.Database.ExecuteSqlCommandAsync(sql) > 0;
            return flag;
        }


        public List<Common.Model.LayuiSelectItem> GetServiceSelectTreeData(string name, string userId)
        {
            var list = new List<LayuiSelectItem>();
            var db = (InfoEarthFrameDbContext)_iLayerManagerRepository.GetDbContext();
            var query = db.LayerManagers.AsQueryable();
            var parents=new List<Tbl_LayerManager>();
            if (userId.ToLower() == "admin")
            {
                if (!string.IsNullOrEmpty(name))
                {
                    query = query.Where(p => p.TEXT != null && p.TEXT.ToLower().Contains(name.ToLower()));
                }
                parents = query.Where(p => string.IsNullOrEmpty(p.PID)).ToList();
            }
            else
            {
                var where1 = "";
                if (!string.IsNullOrEmpty(name))
                {
                    where1 += "			AND \"TEXT\" like '%" + name + "%'";
                }
                var sql = "SELECT" +
    "	r.*" +
    "FROM" +
    "	\"TBL_LAYERMANAGER\" r" +
    " JOIN (" +
    "	SELECT" +
    "		K .\"Id\"," +
    "		K .\"MapId\"" +
    "	FROM" +
    "		\"TBL_GROUP_RIGHT\" r" +
    "	LEFT JOIN (" +
    "		SELECT" +
    "			P .\"Id\"," +
    "			l.\"Id\" AS \"MapId\"" +
    "		FROM" +
    "			\"TBL_LAYERMANAGER\" l" +
    "		LEFT JOIN \"TBL_DATAMAIN\" M ON l.\"DataMainID\" = M .\"Id\"" +
    "		LEFT JOIN \"TBL_GEOLOGYMAPPINGTYPE\" P ON P .\"Id\" = M .\"MappingTypeID\"" +
    "	) K ON K .\"Id\" = r.\"LayerId\"" +
    "	WHERE" +
    "		r.\"GroupId\" IN (" +
    "			SELECT" +
    "				gu.\"GroupId\"" +
    "			FROM" +
    "				\"TBL_GROUP_USER\" gu" +
    "			LEFT JOIN sdms_user u ON u.\"Id\" = gu.\"UserId\"" +
    "			WHERE 1=1" +
        "AND	u.\"Id\" = \'" + userId + "\'" +
    "		)" +
    "	AND K .\"Id\" IS NOT NULL" +
       " AND \"IsBrowse\"=1" +
    ") T ON r.\"Id\" = T .\"MapId\"" +
    "where (\"PID\" is  null or(\"PID\"='')) " + where1 + "";
                parents = db.Database.SqlQuery<Tbl_LayerManager>(sql).ToList();
            }
            if (parents != null && parents.Any())
            {
                foreach (var p in parents)
                {
                    var current = new LayuiSelectItem
                    {
                        name = p.TEXT,
                        value = p.Id,
                        tag = p.DATASERVERKEY,
                        tag1 = p.URL,
                        tag2 = p.ZEROLEVELSIZE,
                        tag3 = p.TILESIZE,
                        tag4 = p.SERVICETYPE,
                        children = GetChildServiceSelectTreeData(db, p.Id, userId)
                    };

                    list.Add(current);
                }
            }

            return list;
        }

        public List<Common.Model.LayuiSelectItem> GetChildServiceSelectTreeData(InfoEarthFrameDbContext db, string id, string userId)
        {
            var list = new List<LayuiSelectItem>();
            var parents = new List<Tbl_LayerManager>();
            if (userId.ToLower() == "admin")
            {
                parents = db.LayerManagers.Where(p => p.PID == id).ToList();
            }
            else
            {
                var sql = "SELECT" +
         "	r.*" +
         "FROM" +
         "	\"TBL_LAYERMANAGER\" r" +
         " JOIN (" +
         "	SELECT" +
         "		K .\"Id\"," +
         "		K .\"MapId\"" +
         "	FROM" +
         "		\"TBL_GROUP_RIGHT\" r" +
         "	LEFT JOIN (" +
         "		SELECT" +
         "			P .\"Id\"," +
         "			l.\"Id\" AS \"MapId\"" +
         "		FROM" +
         "			\"TBL_LAYERMANAGER\" l" +
         "		LEFT JOIN \"TBL_DATAMAIN\" M ON l.\"DataMainID\" = M .\"Id\"" +
         "		LEFT JOIN \"TBL_GEOLOGYMAPPINGTYPE\" P ON P .\"Id\" = M .\"MappingTypeID\"" +
         "	) K ON K .\"Id\" = r.\"LayerId\"" +
         "	WHERE" +
         "		r.\"GroupId\" IN (" +
         "			SELECT" +
         "				gu.\"GroupId\"" +
         "			FROM" +
         "				\"TBL_GROUP_USER\" gu" +
         "			LEFT JOIN sdms_user u ON u.\"Id\" = gu.\"UserId\"" +
         "			WHERE 1=1" +
"			AND	u.\"Id\" = \'" + userId + "\'" +
         "		)" +
         "	AND K .\"Id\" IS NOT NULL" +
            " AND \"IsBrowse\"=1" +
         ") T ON r.\"Id\" = T .\"MapId\"" +
         "where (\"PID\"='" + id + "')";

                parents = db.Database.SqlQuery<Tbl_LayerManager>(sql).ToList();
            }
            if (parents != null && parents.Any())
            {
                foreach (var p in parents)
                {
                    var current = new LayuiSelectItem
                    {
                        name = p.TEXT,
                        value = p.Id,
                        children = GetChildServiceSelectTreeData(db, p.Id, userId),
                        tag = p.DATASERVERKEY,
                        tag1 = p.URL,
                        tag2 = p.ZEROLEVELSIZE,
                        tag3 = p.TILESIZE,
                        tag4 = p.SERVICETYPE
                    };
                    list.Add(current);
                }
            }

            return list;
        }


        public IList<Common.ZTreeItem> GetServiceZTreeData(string userId)
        {
            var result = new List<Common.ZTreeItem>();
            var parents = new List<Tbl_LayerManager>();
            var db = (InfoEarthFrameDbContext)_iLayerManagerRepository.GetDbContext();
           
            if (userId.ToLower() == "admin")
            {
                parents = db.LayerManagers.Where(p => string.IsNullOrEmpty(p.PID)).ToList();
            }
            else
            {
                var sql = "SELECT" +
    "	r.*" +
    "FROM" +
    "	\"TBL_LAYERMANAGER\" r" +
    " JOIN (" +
    "	SELECT" +
    "		K .\"Id\"," +
    "		K .\"MapId\"" +
    "	FROM" +
    "		\"TBL_GROUP_RIGHT\" r" +
    "	LEFT JOIN (" +
    "		SELECT" +
    "			P .\"Id\"," +
    "			l.\"Id\" AS \"MapId\"" +
    "		FROM" +
    "			\"TBL_LAYERMANAGER\" l" +
    "		LEFT JOIN \"TBL_DATAMAIN\" M ON l.\"DataMainID\" = M .\"Id\"" +
    "		LEFT JOIN \"TBL_GEOLOGYMAPPINGTYPE\" P ON P .\"Id\" = M .\"MappingTypeID\"" +
    "	) K ON K .\"Id\" = r.\"LayerId\"" +
    "	WHERE" +
    "		r.\"GroupId\" IN (" +
    "			SELECT" +
    "				gu.\"GroupId\"" +
    "			FROM" +
    "				\"TBL_GROUP_USER\" gu" +
    "			LEFT JOIN sdms_user u ON u.\"Id\" = gu.\"UserId\"" +
    "			WHERE 1=1" +
    "			AND	u.\"Id\" = \'" + userId + "\'" +
    "		)" +
    "	AND K .\"Id\" IS NOT NULL" +
   " AND \"IsBrowse\"=1"+
    ") T ON r.\"Id\" = T .\"MapId\"" +
    "where \"PID\" is  null or(\"PID\"='')";
                 parents = db.Database.SqlQuery<Tbl_LayerManager>(sql).ToList();
            }

            if (parents != null && parents.Count > 0)
            {
                foreach (var p in parents)
                {
                    var nexts = GetChildServiceZTreeData(db, p.Id, userId);
                    var item = new ZTreeItem
                    {
                        id = p.Id,
                        name = p.TEXT,
                        children = nexts,
                        pId = "",
                        isParent = nexts != null && nexts.Any(),
                        tag = p.DATASERVERKEY,
                        tag1 = p.URL,
                        tag2 = p.ZEROLEVELSIZE,
                        tag3 = p.TILESIZE,
                        tag4 = p.SERVICETYPE
                    };

                    result.Add(item);
                }
            }

            return result;
        }


        public List<Common.ZTreeItem> GetChildServiceZTreeData(InfoEarthFrameDbContext db, string id, string userId)
        {
            var result = new List<Common.ZTreeItem>();
            var children = new List<Tbl_LayerManager>();
            if (userId.ToLower() == "admin")
            {
                children = db.LayerManagers.Where(p => p.PID == id).ToList();
            }
            else
            {
                children = db.Database.SqlQuery<Tbl_LayerManager>("SELECT" +
    "	r.*" +
    "FROM" +
    "	\"TBL_LAYERMANAGER\" r" +
    " JOIN (" +
    "	SELECT" +
    "		K .\"Id\"," +
    "		K .\"MapId\"" +
    "	FROM" +
    "		\"TBL_GROUP_RIGHT\" r" +
    "	LEFT JOIN (" +
    "		SELECT" +
    "			P .\"Id\"," +
    "			l.\"Id\" AS \"MapId\"" +
    "		FROM" +
    "			\"TBL_LAYERMANAGER\" l" +
    "		LEFT JOIN \"TBL_DATAMAIN\" M ON l.\"DataMainID\" = M .\"Id\"" +
    "		LEFT JOIN \"TBL_GEOLOGYMAPPINGTYPE\" P ON P .\"Id\" = M .\"MappingTypeID\"" +
    "	) K ON K .\"Id\" = r.\"LayerId\"" +
    "	WHERE" +
    "		r.\"GroupId\" IN (" +
    "			SELECT" +
    "				gu.\"GroupId\"" +
    "			FROM" +
    "				\"TBL_GROUP_USER\" gu" +
    "			LEFT JOIN sdms_user u ON u.\"Id\" = gu.\"UserId\"" +
    "			WHERE 1=1" +
    "			AND	u.\"Id\" = \'" + userId + "\'" +
    "		)" +
    "	AND K .\"Id\" IS NOT NULL" +
       " AND \"IsBrowse\"=1" +
    ") T ON r.\"Id\" = T .\"MapId\"" +
    "where (\"PID\"='" + id + "')").ToList();
            }
            //获取图层
            if (children != null && children.Count > 0)
            {
                foreach (var p in children)
                {
                    var nexts = GetChildServiceZTreeData(db, p.Id, userId);
                    var item = new ZTreeItem
                    {
                        id = p.Id,
                        name = p.TEXT,
                        children = nexts,
                        pId = id,
                        isParent = nexts != null && nexts.Any(),
                        tag = p.DATASERVERKEY,
                        tag1 = p.URL,
                        tag2 = p.ZEROLEVELSIZE,
                        tag3 = p.TILESIZE,
                        tag4 = p.SERVICETYPE
                    };

                    result.Add(item);
                }
            }

            return result;
        }



        public int GetServerTypeForReleaseNumber(string className)
        {
            try
            {
                var sql = string.Format("SELECT COUNT(1) from \"TBL_LAYERMANAGER\" a " +
                          "JOIN \"TBL_DATAMAIN\" b ON a.\"DataMainID\"=b.\"Id\"" +
                          "JOIN \"TBL_GEOLOGYMAPPINGTYPE\" c ON b.\"MappingTypeID\"=c.\"Id\"" +
                          "where 1=1 AND c.\"ClassName\"={0}", "'" + className + "'");

                var db = (InfoEarthFrameDbContext)_iLayerManagerRepository.GetDbContext();
                int count = db.Database.SqlQuery<int>(sql).FirstOrDefault();
                return count;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
