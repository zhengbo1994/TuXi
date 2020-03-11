using InfoEarthFrame.Application;
using InfoEarthFrame.Application.DataStyleApp;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using InfoEarthFrame.EntityFramework;
using InfoEarthFrame.ShpFileReadLogApp;
using InfoEarthFrame.ShpFileReadLogApp.Dtos;
using InfoEarthFrame.WebApi.Next.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class ChangePasswordDto
    {
        public string Ids { get; set; }

        public string Password { get; set; }
    }


    public class SystemManageController : BaseApiController
    {
        protected override string ModuleName
        {
            get
            {
                return "SystemManage";
            }
        }
        private readonly IAreaAppService _areaAppService;
        private readonly ISystemUserAppService _systemUserAppService;
        private readonly IOperateLogAppService _operateLogAppService;
        private readonly IDataManageAppService _dataManageAppService;
        private readonly ILayerManagerAppService _layerManagerAppService;
        private readonly IShpFileReadLogAppService _shpFileReadLogAppService;
        private readonly IUsersRightAppService _usersRightAppService;
        private readonly IDataStyleAppService _dataStyleAppService;
        public SystemManageController(IAreaAppService areaAppService, ISystemUserAppService systemUserAppService,
            IOperateLogAppService operateLogAppService,
            IDataManageAppService dataManageAppService,
            ILayerManagerAppService layerManagerAppService,
            IShpFileReadLogAppService shpFileReadLogAppService,
            IUsersRightAppService usersRightAppService,
            IDataStyleAppService dataStyleAppService)
        {
            this._areaAppService = areaAppService;
            this._systemUserAppService = systemUserAppService;
            this._operateLogAppService = operateLogAppService;
            this._dataManageAppService = dataManageAppService;
            this._layerManagerAppService = layerManagerAppService;
            this._shpFileReadLogAppService = shpFileReadLogAppService;
            this._usersRightAppService = usersRightAppService;
            this._dataStyleAppService = dataStyleAppService;
        }

        /// <summary>
        /// 获取区域范围树结构
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetAreaInfoByUserCode(string userCode)
        {
            var data = _areaAppService.GetAreaInfoByUserCode(userCode);
            var all = new List<AreaOutput>{
                new AreaOutput
            {
                Label =GetText(2000),
                Code = "00000",
                Children = data
            }};
            return Ok(GetResult(0, all));
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        /// <param name="userCode">用户编码</param>
        /// <param name="areaCode">区域编码</param>
        /// <param name="userName">用户名</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetAllPageListByCondition(string userCode, string areaCode, string userName, int? pageSize = 10, int? pageIndex = 1)
        {
            var dto = new SystemUserDto
            {
                UserCode = userCode,
                Department = areaCode ?? "",
                UserName = userName ?? ""
            };
            var result = await _systemUserAppService.GetAllPageListByCondition(dto, pageSize.Value, pageIndex.Value);
            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }


        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetUser(string id)
        {
            var model = new SystemUserOutputDto();
            if (!string.IsNullOrEmpty(id))
            {
                model = await _systemUserAppService.GetDetailById(id);
                if (model != null)
                {
                    var areaInfo = _areaAppService.GetAreaInfoByAreaCode(model.Department);
                    var current = GetCurrentArea(areaInfo, model.Department);
                    if (current == null)
                    {
                        model.AreaFullName = "";
                    }
                    else
                    {
                        model.AreaFullName = current.Label + "(" + current.Code + ")";
                    }
                }
            }
            return Ok(GetResult(0, model));
        }

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetUserByName(string username)
        {
            var model = new SystemUserOutputDto();
            if (!string.IsNullOrEmpty(username))
            {
                model = await _systemUserAppService.GetDetailByName(username);
                if (model != null)
                {
                    var areaInfo = _areaAppService.GetAreaInfoByAreaCode(model.Department);
                    var current = GetCurrentArea(areaInfo, model.Department);
                    if (current == null)
                    {
                        model.AreaFullName = "";
                    }
                    else
                    {
                        model.AreaFullName = current.Label + "(" + current.Code + ")";
                    }
                }
            }
            return Ok(GetResult(0, model));
        }

        public async Task<IHttpActionResult> GetService(string id)
        {
            var model = await _layerManagerAppService.GetDetailById(id);
            return Ok(GetResult(0, model));
        }

        private AreaOutput GetCurrentArea(IEnumerable<AreaOutput> areaInfo, string areaCode)
        {
            if (areaInfo != null && areaInfo.Any())
            {
                foreach (var item in areaInfo)
                {
                    if (item.Code == areaCode)
                    {
                        return item;
                    }

                    return GetCurrentArea(item.Children, areaCode);
                }
            }

            return null;
        }

        /// <summary>
        /// 获取用户编码状态
        /// </summary>
        /// <param name="oldUserCode"></param>
        /// <param name="userCode"></param>
        /// <returns>-2001代表分类编码已存在</returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetUserCodeStatus(string oldUserCode, string userCode)
        {
            var flag = true;

            //如果是新增或者（编辑并且原始usercode不等于页面上usercode时候)
            if (string.IsNullOrEmpty(oldUserCode)
                || (oldUserCode.ToLower() != userCode.ToLower()))
            {
                flag = !_systemUserAppService.IsUserCodeExists(userCode);
            }
            return Ok(GetResult(flag ? 0 : -2001));
        }


        /// <summary>
        /// 新增或编辑用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> AddOrEditUser([FromBody]SystemUserDto model)
        {
            var result = await (!string.IsNullOrEmpty(model.Id) ? _systemUserAppService.Update(model) : _systemUserAppService.Insert(model));
            return Ok(GetResult(0, result));
        }


        /// <summary>
        /// 新增或编辑服务
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> AddOrEditService([FromBody]Tbl_LayerManager dto)
        {
            try
            {

                if (string.IsNullOrEmpty(dto.Id))
                {
                    var flag = await _layerManagerAppService.Insert(dto);
                    return Ok(GetResult(flag));
                }
                else
                {
                    var flag = await _layerManagerAppService.Update(dto);
                    return Ok(GetResult(flag));
                }
            }
            catch (Exception ex)
            {
                var current = ex;
                while (current != null && !string.IsNullOrEmpty(current.Message))
                {
                    if (current.Message.Contains("UK_TBL_LAYERMANAGER_DATASERVERKEY"))
                    {
                        return Ok(GetResult(false, "KEY值已存在"));
                    }
                    current = current.InnerException;
                }
                throw;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemoveUser([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = await _systemUserAppService.Delete(idList);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// 删除用户组相关的用户
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult RemoveGroupUser([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = _usersRightAppService.DelGroupUser(idList);
            return Ok(GetResult(flag));
        }


        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemoveService([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = await _layerManagerAppService.Delete(idList);
            return Ok(GetResult(flag));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> ResetPassword([FromBody]ChangePasswordDto model)
        {
            var idList = (model.Ids ?? "").Split(',');
            var flag = await _systemUserAppService.ResetPassword(idList, model.Password);
            return Ok(GetResult(flag));
        }



        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetPageListByCondition(string UserName, string SystemFunc, string OperateType, DateTime? StartDate, DateTime? EndDate, int pageIndex, int pageSize)
        {
            //var pageIndex = input.pageIndex;
            //var pageSize = input.pageSize;

            QueryOperateLogInputParamDto input = new QueryOperateLogInputParamDto
            {
                UserName = UserName,
                SystemFunc = SystemFunc,
                OperateType = OperateType,
                StartDate = StartDate,
                EndDate = EndDate
            };


            var result = await _operateLogAppService.GetPageListByParamCondition(input, pageIndex, pageSize);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }

        public IHttpActionResult GetAllMainData(string keyword = "")
        {
            var data = _dataManageAppService.GetAllListByName(keyword).Select(p => new
            {
                name = p.Name,
                value = p.Id
            });
            return Ok(GetResult(0, data));
        }

        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetPublishPageListByCondition(string name, int pageIndex, int pageSize)
        {

            var input = new QueryLayerManagerInput
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Name = name
            };

            var result = await _layerManagerAppService.GetPageListAndCount(input);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }


        public IHttpActionResult GetServiceTreeData(string keyword = "")
        {
            var data = _layerManagerAppService.GetServiceSelectTreeData(keyword, CurrentUserId);
            return Ok(GetResult(0, data));
        }

        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetGeologgerPageListByCondition(string Createby, int Readstatus, string Message, DateTime? StartDate, DateTime? EndDate, int pageIndex, int pageSize)
        {
            //var pageIndex = input.pageIndex;
            //var pageSize = input.pageSize;

            QueryShpFileReadLogInputParamDto input = new QueryShpFileReadLogInputParamDto
            {
                Createby = Createby,
                Readstatus = Readstatus,
                Message = Message,
                StartDate = StartDate,
                EndDate = EndDate,
                pageIndex = pageIndex,
                pageSize = pageSize
            };


            var result = await _shpFileReadLogAppService.GetGeologgerPageListByCondition(input);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }

        /// <summary>
        /// 获取所有用户组
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetAllGroup()
        {
            var data = (await _usersRightAppService.GetAllGroup()).Select(p => new AreaOutput
            {
                Label = p.Name,
                Code = p.Id
            }).ToList();
            var all = new List<AreaOutput>{
                new AreaOutput
            {
                Label ="所有分组",
                Code = "00000",
                Children = data
            }};
            return Ok(GetResult(0, data));
        }

        public IHttpActionResult GetGroupMapPermession(string groupId)
        {
            var data = _usersRightAppService.GetUserGroupMapPermession(groupId);
            return Ok(GetResult(0, data));
        }

        public IHttpActionResult GetGroupMenuPermession(string groupId)
        {
            var data = _usersRightAppService.GetUserGroupMenuPermession(groupId);
            return Ok(GetResult(0, data));
        }



        public IHttpActionResult SetMapRight([FromBody]SetMapRightInput model)
        {
            _usersRightAppService.SetMapRight(model);
            return Ok(GetResult(0));
        }

        public IHttpActionResult SetMenuRight([FromBody]SetMenuRightInput model)
        {
            _usersRightAppService.SetMenuRight(model);
            return Ok(GetResult(0));
        }

        public IHttpActionResult SetMenuButtonRight([FromBody]SetMenuButtonRightInput model)
        {
            model.ButtonName = HttpUtility.UrlDecode(model.ButtonName);
            try
            {
                _usersRightAppService.SetMenuButtonRight(model);
                return Ok(GetResult(0));
            }
            catch (Exception ex)
            {
                return Ok(GetResult(-200,ex.Message));
            }
        }

        /// <summary>
        /// 根据用户组获取关联用户
        /// </summary>
        /// <param name="groupId">用户组ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ResponseType(typeof(LayuiGridResult))]
        public IHttpActionResult GetGroupRelatedUsers(string groupId, string userName, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = _usersRightAppService.GetUserGroupRelatedUsers(groupId, userName, pageSize.Value, pageIndex.Value);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }



        /// <summary>
        /// 根据用户组获取供选择的关联用户
        /// </summary>
        /// <param name="groupId">用户组ID</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [ResponseType(typeof(LayuiGridResult))]
        public IHttpActionResult GetGroupAlternativeUsers(string groupId, string userName, int? pageIndex = 1, int? pageSize = 10)
        {
            var result = _usersRightAppService.GetUserGroupAlternativeUsers(groupId, userName, pageSize.Value, pageIndex.Value);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }

        public IHttpActionResult IntsertGroupUser([FromBody]GroupUserDto dto)
        {
            _usersRightAppService.IntsertGroupUser(dto.GroupId, dto.UserIds);
            return Ok(GetResult(0));
        }
        public IHttpActionResult ClearData([FromBody]string password)
        {
            if (password != "1qaz@WSX")
            {
                return Ok(GetResult(-200, "管理员密码错误"));
            }

            using (var context = new InfoEarthFrameDbContext())
            {
                using (var trans = context.Database.BeginTransaction())
                {
                    try
                    {
                        //DrawingEntity
                        var sql = "delete from \"DrawingEntity\"";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_convertfile
                        sql = "delete from sdms_convertfile";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_datastyle
                        //sql = "delete from sdms_datastyle";
                        //context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_datatag
                        sql = "delete from sdms_tag_releation";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                        sql = "delete from sdms_datatag";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_datatype
                        sql = "delete from sdms_datatype";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_layer
                        sql = "delete from sdms_layerfielddict";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                        sql = "delete from sdms_layerfield";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                        sql = "delete from sdms_layer";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);


                        //sdms_layer_readlog
                        sql = "delete from sdms_layer_readlog";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_log
                        sql = "delete from sdms_log";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_map_releation
                        sql = "delete from sdms_map_releation";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_metadata
                        sql = "delete from sdms_metadata";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_operatelog
                        sql = "delete from sdms_operatelog";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //sdms_map
                        sql = "delete from sdms_map";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //TBL_DATAMANAGEFILE
                        sql = "delete from \"TBL_DATAMANAGEFILE\"";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //TBL_DATAMAIN
                        sql = "delete from \"TBL_DATAMAIN\"";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //TBL_LAYERMANAGER
                        sql = "delete from \"TBL_LAYERMANAGER\"";
                        context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        trans.Commit();

                        return Ok(GetResult(0));
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetDataStylePageListByCondition(string StyleName, string StyleDataType, string Createby, DateTime? StartDate, DateTime? EndDate, int pageIndex, int pageSize)
        {
            QueryDataStyleInputParamDto input = new QueryDataStyleInputParamDto
            {
                StyleName = StyleName,
                StyleType = StyleDataType,
                Createby = Createby,
                StartDate = StartDate,
                EndDate = EndDate,
                pageIndex = pageIndex,
                pageSize = pageSize,
            };

            var result = await _dataStyleAppService.GetDataStylePageListByCondition(input);

            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = result.Items
                },
                Status = 0,
                Total = result.TotalCount
            };
            return Ok(data);
        }

        /// <summary>
        /// 新增或编辑用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> AddOrEditStyle([FromBody]DataStyleInputDto model)
        {
            if (string.IsNullOrEmpty(model.Id))
            {
                model.CreateBy = CurrentUserName;
            }
            var result = await (!string.IsNullOrEmpty(model.Id) ? _dataStyleAppService.Update(model) : _dataStyleAppService.Insert(model));
            return Ok(GetResult(0, result));
        }


        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetDataStyleById(string id)
        {
            var model = new DataStyleOutputDto();
            if (!string.IsNullOrEmpty(id))
            {
                model = await _dataStyleAppService.GetDetailById(id);
            }
            return Ok(GetResult(0, model));
        }

        /// <summary>
        /// 删除样式
        /// </summary>
        /// <param name="ids">主键字符串，多个请用,号隔开，例如:1,2,3,4</param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> RemoveStyle([FromBody]string ids)
        {
            var idList = (ids ?? "").Split(',');
            var flag = false;
            var flagCount = 0;
            foreach (var id in idList)
            {
                var b = await _dataStyleAppService.Delete(id, CurrentUserName);
                if (b)
                {
                    flagCount++;
                }
            }

            if (flagCount > 0)
                flag = true;

            return Ok(GetResult(flag));
        }

        ///  <summary>
        ///  获取样式名称状态
        ///  </summary>
        ///  <param  name="oldUserCode"></param>
        ///  <param  name="userCode"></param>
        ///  <returns>-2002代表分类编码已存在</returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetDataStyleStatus(string oldStyleName, string styleName)
        {
            var flag = true;

            if (string.IsNullOrEmpty(oldStyleName)
                    || (oldStyleName.ToLower() != styleName.ToLower()))
            {
                flag = !_dataStyleAppService.IsDataStyleExists(styleName);
            }
            return Ok(GetResult(flag ? 0 : -2002));
        }

        #region  首页统计发布后的服务
        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetServerTypeForReleaseNumber()
        {
            int classNameCount1 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className1);
            int classNameCount2 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className2);
            int classNameCount3 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className3);
            int classNameCount4 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className4);
            int classNameCount5 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className5);

            List<ServerType> serverTypeList = new List<ServerType>()
            {
                new ServerType {name = classNameEnum.className1, value = classNameCount1},
                new ServerType {name = classNameEnum.className2, value = classNameCount2},
                new ServerType {name = classNameEnum.className3, value = classNameCount3},
                new ServerType {name = classNameEnum.className4, value = classNameCount4},
                new ServerType {name = classNameEnum.className5, value = classNameCount5}
            };

            return Ok(GetResult(0, serverTypeList));
        }

        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetServerTypeForReleaseNumber2()
        {
            int classNameCount11 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className11);
            int classNameCount12 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className12);
            int classNameCount13 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className13);

            List<ServerType> serverTypeList = new List<ServerType>()
            {
                new ServerType {name = classNameEnum.className11, value = classNameCount11},
                new ServerType {name = classNameEnum.className12, value = classNameCount12},
                new ServerType {name = classNameEnum.className13, value = classNameCount13}
            };

            return Ok(GetResult(0, serverTypeList));
        }

        [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetServerTypeForReleaseNumber3()
        {
            int classNameCount21 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className21);
            int classNameCount22 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className22);
            int classNameCount23 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className23);
            int classNameCount24 = _layerManagerAppService.GetServerTypeForReleaseNumber(classNameEnum.className24);

            List<ServerType> serverTypeList = new List<ServerType>()
            {
                new ServerType {name = classNameEnum.className21, value = classNameCount21},
                new ServerType {name = classNameEnum.className22, value = classNameCount22},
                new ServerType {name = classNameEnum.className23, value = classNameCount23},
                new ServerType {name = classNameEnum.className24, value = classNameCount24}
            };

            return Ok(GetResult(0, serverTypeList));
        }

        public struct classNameEnum
        {
            public const string className1 = "地质灾害类";
            public const string className2 = "地下水类";
            public const string className3 = "矿山地质环境类";
            public const string className4 = "地质遗迹类";
            public const string className5 = "地质环境条件类";

            //地质灾害类
            public const string className11 = "中国崩塌滑坡泥石流分布图";
            public const string className12 = "中国崩塌滑坡泥石流易发程度图";
            public const string className13 = "中国地面沉降现状图";

            //地下水类
            public const string className21 = "中国水文地质图";
            public const string className22 = "中国地下水资源图";
            public const string className23 = "中国地下水环境图";
            public const string className24 = "中国地热资源分布图";

            //矿山地质环境类
            public const string className31 = "中国矿山地质环境问题图";
            public const string className32 = "中国矿山地质环境保护与治理区划图";

            //地质遗迹类
            public const string className41 = "中国重要古生物化石产地分布图";
            public const string className42 = "中国重要地质遗迹资源分布图";

            //地质环境条件类
            public const string className51 = "中国地质环境分区图";
            public const string className52 = "中国工程地质图";
            public const string className53 = "中国岩溶环境地质图";
            public const string className54 = "中国及毗邻海区活动断裂分布图";
            public const string className55 = "中国地质环境安全程度图";
            public const string className56 = "中国荒漠化土地分布图";
            public const string className57 = "中国及毗邻海域主要沉积盆地二氧化碳地质储存适宜性评价图";
            public const string className58 = "中华人民共和国及其毗邻海区第四纪地质图";
            public const string className59 = "中国沿海地区环境地质图";
        }

        public class ServerType
        {
            public string name { get; set; }
            public int value { get; set; }
        }
        #endregion


        #region  菜单管理
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult AddOrEditMenu([FromBody]SetMenuRightPreamInput model)
        {
            _usersRightAppService.AddOrEditMenu(model);
            return Ok(GetResult(0));
        }

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult RemoveMenu([FromBody]string dataList)
        {
            JavaScriptSerializer Serializer = new JavaScriptSerializer();
            List<MenuIdList> MenuDataList = Serializer.Deserialize<List<MenuIdList>>(dataList);

            foreach (var item in MenuDataList)
            {
                _usersRightAppService.DeleteMenu(item.moduleId, item.menuId);
            }

            return Ok(GetResult(0));
        }

        public class MenuIdList
        {
            public string moduleId { get; set; }
            public string menuId { get; set; }
        }

        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult PostMenuInfo([FromBody]SetMenuRightPreamInput model)
        {
            InfoEarthFrame.Common.ModuleConfig.ParentMenu data = new InfoEarthFrame.Common.ModuleConfig.ParentMenu();
            data=_usersRightAppService.GetMenuInfo(model);
            return Ok(GetResult(0, data));
        }

        public IHttpActionResult SetMenuButton([FromBody]SetMenuButtonRightInput model)
        {
            model.ButtonName = HttpUtility.UrlDecode(model.ButtonName);
            try
            {
                _usersRightAppService.SetMenuButtonArea(model);
                return Ok(GetResult(0));
            }
            catch (Exception ex)
            {
                return Ok(GetResult(-200, ex.Message));
            }
        }

        #endregion
    }
}