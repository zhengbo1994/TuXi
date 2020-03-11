using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using AutoMapper;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using Abp.AutoMapper;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;
using System.Data.Entity;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.UsersRight.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;

namespace InfoEarthFrame.Application
{
    public class UsersRightAppService : ApplicationService, IUsersRightAppService
    {
        private readonly IGroupRepository _iGroupRepository;
        private readonly IGroupRightRepository _iGroupRightRepository;
        private readonly IGroupUserRepository _iGroupUserRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="iSysMenuRepository"></param>
        public UsersRightAppService(IGroupRepository iGroupRepository, IGroupRightRepository iGroupRightRepository, IGroupUserRepository iGroupUserRepository)
        {
            _iGroupRepository = iGroupRepository;
            _iGroupRightRepository = iGroupRightRepository;
            _iGroupUserRepository = iGroupUserRepository;
        }

        /// <summary>
        /// 获取所有用户组
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupDto>> GetAllGroup()
        {
            var result = await _iGroupRepository.GetAllListAsync();
            var outputList = new List<GroupDto>(result.MapTo<List<GroupDto>>());
            return outputList;
        }

        /// <summary>
        /// 添加用户组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public GroupDto Intsert(GroupInput input)
        {
            GroupEntity group = new GroupEntity
            {
                Id = Guid.NewGuid().ToString(),
                Name = input.Name
            };
            var result = _iGroupRepository.Insert(group);
            GroupDto ret = new GroupDto();
            ret.Id = result.Id;
            ret.Name = result.Name;
            return ret;
        }

        /// <summary>
        /// 修改用户组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public void Update(GroupInput input)
        {
            GroupEntity group = new GroupEntity
            {
                Id = input.Id,
                Name = input.Name
            };
            _iGroupRepository.Update(group);
        }

        /// <summary>
        /// 删除用户组
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            _iGroupRepository.Delete(id);
            _iGroupUserRepository.Delete(q => q.GroupId == id);
            _iGroupRightRepository.Delete(q => q.GroupId == id);
        }


        /// <summary>
        /// 授权用户组
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public void SetRight(string GroupId, List<SetMapRightInput> input)
        {
            _iGroupRightRepository.Delete(x => x.GroupId == GroupId);
            foreach (SetMapRightInput item in input)
            {
                GroupRightEntity groupRight = new GroupRightEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = item.GroupId,
                    LayerId = item.LayerId,
                    IsDownload = Convert.ToInt32(item.IsDownload),
                    IsBrowse = Convert.ToInt32(item.IsBrowse)
                };
                _iGroupRightRepository.Insert(groupRight);
            }
        }

        /// <summary>
        /// 往组里添加用户
        /// </summary>
        /// <param name="Input"></param>
        public void IntsertGroupUser(GroupUserComparaInput input)
        {
            input.ExistUser.ForEach(x =>
            {
                input.InputArr.RemoveAll(aa => aa.UserId == x.UserId && aa.GroupId == x.GroupId);
            });

            foreach (GroupUserInput item in input.InputArr)
            {
                GroupUserEntity groupUser = new GroupUserEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupId = item.GroupId,
                    UserId = item.UserId
                };
                _iGroupUserRepository.Insert(groupUser);
            }

        }
        /// <summary>
        /// 删除组中用户
        /// </summary>
        /// <param name="Input"></param>
        public void DelGroupUser(List<GroupUserInput> input)
        {
            foreach (GroupUserInput item in input)
            {
                _iGroupUserRepository.Delete(x => x.GroupId == item.GroupId && x.UserId == item.UserId);
            }
        }
        /// <summary>
        /// 修改组中用户
        /// </summary>
        /// <param name="Input"></param>
        public void UpdateGroupUser(GroupUserInput input)
        {
            GroupUserEntity groupUser = new GroupUserEntity
            {
                GroupId = input.GroupId,
                UserId = input.UserId
            };
            _iGroupUserRepository.Update(groupUser);
        }

        /// <summary>
        /// 获取带权限的用户组
        /// </summary>
        /// <returns></returns>
        public List<GroupOutput> GetGroupWithRight()
        {
            var groupRight = _iGroupRightRepository.GetAll();
            var query = _iGroupRepository.Query(q => q.GroupJoin(groupRight, g => g.Id, r => r.GroupId, (g, r) => new { g, r }));
            var ret = query.ToList();
            List<GroupOutput> list = new List<GroupOutput>();

            #region [循环附值（授权信息）]
            if (ret.Count != 0)
            {
                ret.ForEach((x) =>
                {
                    GroupOutput output = new GroupOutput();
                    List<GroupWithRightEntity> eylist = new List<GroupWithRightEntity>();
                    if (x.r.Count() > 0)
                    {
                        foreach (var tb in x.r)
                        {
                            GroupWithRightEntity ey = new GroupWithRightEntity();
                            ey.Id = tb.Id;
                            ey.GroupId = x.g.Id;
                            ey.GroupName = x.g.Name;
                            ey.LayerId = tb.LayerId;
                            ey.IsDownload = tb.IsDownload;
                            ey.IsBrowse = tb.IsBrowse;
                            eylist.Add(ey);
                        }
                    }
                    output.Id = x.g.Id;
                    output.Name = x.g.Name;
                    output.RightInfo = eylist;
                    list.Add(output);
                });
            }
            #endregion

            return list;
        }
        /// <summary>
        /// 获取带用户的用户组
        /// </summary>
        /// <returns></returns>
        public List<GroupOutput> GetGroupWithUser()
        {
            throw new NotImplementedException();
            //#region [取sso帐号信息]

            //System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();
            //string url = ConfigurationManager.AppSettings["WebApiUrl"].ToString().Trim();
            //httpClient.BaseAddress = new Uri(url);
            //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserInfo.Token.ToString());
            ////httpClient.SetBearerToken();
            //var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("", "") });
            //content.Headers.ContentType = new MediaTypeHeaderValue("application/json") { CharSet = "utf-8" };
            //Task<HttpResponseMessage> task = httpClient.PostAsync("api/userListapi/fetchAll", content);
            //string result = string.Empty;
            //Task continuation = task.ContinueWith(t =>
            //{
            //    Task<string> task2 = t.Result.Content.ReadAsStringAsync();
            //    Task continuation2 = task2.ContinueWith(t1 =>
            //    {
            //        result = t1.Result;
            //    });
            //    continuation2.Wait();
            //});
            //continuation.Wait();
            //var userList = JsonConvert.DeserializeObject<List<UserInfoFromSSO>>(result);
            //#endregion

            //#region [取用户组信息]

            //var groupUser = _iGroupUserRepository.GetAll();
            //var query = _iGroupRepository.Query(q => q.GroupJoin(groupUser, g => g.Id, u => u.GroupId, (g, u) => new { g, u }));
            //var ret = query.ToList();
            //#endregion

            //List<GroupWithUserEntity> eList = new List<GroupWithUserEntity>();
            //#region [循环所有用户信息]            
            //userList.ForEach((x) =>
            //{
            //    GroupWithUserEntity ey = new GroupWithUserEntity();
            //    ey.UserId = x.ContactPeopleID;
            //    ey.UserName = x.UserName;
            //    ey.DeptName = x.DeptName;
            //    ey.LoginName = x.LoginName;
            //    eList.Add(ey);
            //});

            ////对全部的用户附值（组名）
            //if (eList.Count != 0)
            //{
            //    eList.ForEach((el) =>
            //    {
            //        if (ret.Count != 0)
            //        {
            //            ret.ForEach((gp) =>
            //            {
            //                if (gp.u.Count() != 0)
            //                {
            //                    foreach (var tbu in gp.u)
            //                    {
            //                        if (el.UserId == tbu.UserId)
            //                        {
            //                            el.GroupId += gp.g.Id + " ; ";
            //                            el.GroupName += gp.g.Name + "；　";
            //                        }
            //                    }
            //                }
            //            });
            //        }
            //    });
            //}
            //#endregion

            //List<GroupOutput> list = new List<GroupOutput>();

            ////添加所有用户
            //list.Add(new GroupOutput
            //{
            //    Id = "0",
            //    Name = "全部",
            //    UserInfo = eList,
            //});

            //#region [循环附值（用户信息）]

            //if (ret.Count!=0)
            //{
            //    ret.ForEach((x) =>
            //    {
            //        GroupOutput output = new GroupOutput();
            //        List<GroupWithUserEntity> eylist = new List<GroupWithUserEntity>();
            //        if (x.u.Count() > 0)
            //        {                       
            //            foreach(var tb in x.u){
            //               GroupWithUserEntity ey = new GroupWithUserEntity();
            //               ey.Id = tb.Id;
            //               ey.UserId = tb.UserId;
            //               ey.GroupId = x.g.Id;
            //               ey.GroupName = x.g.Name;
            //               var info = userList.Find(
            //                   delegate(UserInfoFromSSO u)
            //                   {
            //                       return tb.UserId == u.ContactPeopleID;
            //                   });
            //               if (info != null)
            //               {
            //                   ey.UserName = info.UserName;
            //                   ey.DeptName = info.DeptName;
            //                   ey.LoginName = info.LoginName;
            //               }
            //               eylist.Add(ey);
            //            }
            //        }
            //        output.Id = x.g.Id;
            //        output.Name = x.g.Name;
            //        output.UserInfo = eylist;
            //        list.Add(output); 
            //    });
            //}
            //#endregion            

            //return list;
        }


        public List<UsersRight.Dtos.UserGroupMapPermessionDto> GetUserGroupMapPermession(string groupId)
        {
            var list = new List<UsersRight.Dtos.UserGroupMapPermessionDto>();
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();

            //            var groupName = db.Database.SqlQuery<string>("				SELECT" +
            //"					\"Name\"" +
            //"				FROM" +
            //"					\"TBL_GROUP\"" +
            //"				WHERE" +
            //"					\"Id\" = \'" + groupId + "\'").FirstOrDefault();
            //            if (groupName == "全国用户组" || groupName == "区域用户组" || groupName == "省域用户组")
            //            {
            //                groupName = groupName.Substring(0, 2);
            //            }
            //根据用户组获取第一级
            var sql = "SELECT" +
"	T .\"Id\" AS MappingTypeId," +
"	T .\"ParentID\"," +
"	T .\"Paths\"," +
"	T .\"ClassName\" AS MappingTypeName," +
"	\'\' AS GroupId," +
"	r.\"IsBrowse\" AS CanBrowse," +
"	r.\"IsDownload\" AS CanDownload" +
" FROM" +
"	\"TBL_GEOLOGYMAPPINGTYPE\" T " +
"LEFT JOIN " +
"(select *from \"TBL_GROUP_RIGHT\" where \"GroupId\"='" + groupId + "')" +
"  r ON r.\"LayerId\" = T .\"Id\"";
            var all = db.Database.SqlQuery<UserGroupMapPermessionDto>(sql).ToList();
            var root = all.Where(p => string.IsNullOrEmpty(p.ParentId));
            if (root != null && root.Any())
            {
                foreach (var item in root)
                {
                    list.Add(new UserGroupMapPermessionDto
                    {
                        CanBrowse = item.CanBrowse,
                        CanDownload = item.CanDownload,
                        GroupId = item.GroupId,
                        MappingTypeId = item.MappingTypeId,
                        MappingTypeName = item.MappingTypeName,
                        ParentId = "",
                        Paths = item.MappingTypeId
                    });

                    //获取下一级
                    var children = GetUserGroupChildPermession( all,item.MappingTypeId, groupId, db);
                    list.AddRange(children);
                }
            }

            return list;
        }

        public List<UsersRight.Dtos.UserGroupMapPermessionDto> GetUserGroupChildPermession(List< UserGroupMapPermessionDto > all,string parentMappingTypeId, string groupId, DbContext db)
        {
            var list = new List<UsersRight.Dtos.UserGroupMapPermessionDto>();

            //根据用户组获取第一级
//            var sql = "SELECT" +
//"	T .\"Id\" AS MappingTypeId," +
//"	T .\"Paths\"," +
//"	T .\"ClassName\" AS MappingTypeName," +
//"	\'\' AS GroupId," +
//"	r.\"IsBrowse\" AS CanBrowse," +
//"	r.\"IsDownload\" AS CanDownload" +
//" FROM" +
//"	\"TBL_GEOLOGYMAPPINGTYPE\" T " +
//"LEFT JOIN " +
//"(select *from \"TBL_GROUP_RIGHT\" where \"GroupId\"='" + groupId + "')" +
//" r ON r.\"LayerId\" = T .\"Id\"" +
//" WHERE" +
//"	\"ParentID\" IN (" +
//"\'" + parentMappingTypeId + "\'" +
//"	) and \"ParentID\"!=T.\"Id\"";
            var children = all.Where(p => p.ParentId == parentMappingTypeId && p.ParentId != p.MappingTypeId);

            if (children != null && children.Any())
            {
                foreach (var item in children)
                {
                    list.Add(new UserGroupMapPermessionDto
                    {
                        CanBrowse = item.CanBrowse,
                        CanDownload = item.CanDownload,
                        GroupId = item.GroupId,
                        MappingTypeId = item.MappingTypeId,
                        MappingTypeName = item.MappingTypeName,
                        ParentId = parentMappingTypeId,
                        Paths = item.Paths
                    });

                    //获取下一级
                    list.AddRange(GetUserGroupChildPermession(all,item.MappingTypeId, groupId, db));
                }
            }

            return list;
        }


        public void SetMapRight(SetMapRightInput model)
        {
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();
            var currentMappingType = db.GeologyMappingType.FirstOrDefault(p => p.Id == model.LayerId);
            if (currentMappingType == null)
            {
                return;
            }

            //TODO:这里有个bug,如果点击父级的浏览下载，可能会覆盖子级的权限
            List<GroupRightEntity> insertData = null;
            var layerIds = new List<string>
            {
              model.LayerId
            };
            //判断是否存在多个子分类,如果有的话则加进来
            var mappingTypes = db.GeologyMappingType.Where(p => p.Paths.Contains(model.LayerId)).ToList();
            if (mappingTypes != null && mappingTypes.Any())
            {
                mappingTypes.ForEach(p =>
                {
                    layerIds.Add(p.Id);
                });
            }

            insertData = layerIds.Distinct().Select(p => new GroupRightEntity
            {
                GroupId = model.GroupId,
                Id = Guid.NewGuid().ToString(),
                IsBrowse = Convert.ToInt32(model.IsBrowse),
                IsDownload = Convert.ToInt32(model.IsDownload),
                LayerId = p
            }).ToList();

            //判断父级以及祖宗节点是否存在，如果不存在需要创建
            var parentIds = currentMappingType.Paths.Replace(model.LayerId, "").Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parentIds != null && parentIds.Any())
            {
                var sql = "select t.* from \"TBL_GEOLOGYMAPPINGTYPE\" t join \"TBL_GROUP_RIGHT\" r on r.\"LayerId\"=t.\"Id\" WHERE t.\"Id\" in (" + string.Join(",", parentIds.Select(p => "'" + p + "'")) + ") AND R.\"GroupId\"='" + model.GroupId + "' ";
                var parents = db.Database.SqlQuery<GeologyMappingType>(sql).ToDictionary(p => p.Id);
                if (parents == null || !parents.Any())
                {
                    insertData.AddRange(parentIds.Select(p => new GroupRightEntity
               {
                   GroupId = model.GroupId,
                   Id = Guid.NewGuid().ToString(),
                   IsBrowse = 1,
                   LayerId = p,
                   IsParent = true
               }));
                }
                else
                {
                    foreach (var parentId in parentIds)
                    {
                        foreach (var key in parents.Keys)
                        {
                            if (key != parentId)
                            {
                                if (!insertData.Any(p => p.LayerId == parentId))
                                {
                                    insertData.Add(new GroupRightEntity
                      {
                          GroupId = model.GroupId,
                          Id = Guid.NewGuid().ToString(),
                          IsBrowse = 1,
                          LayerId = parentId,
                          IsParent = true
                      });
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }

            //删除表中已经存在的记录
            var inStr = string.Join(",", layerIds.Select(p => "'" + p + "'"));
            var inStr1 = string.Join(",", insertData.Where(p => p.IsParent).Select(p => "'" + p.LayerId + "'"));
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    var sql = "";
                    if (!string.IsNullOrEmpty(inStr1))
                    {
                        sql = "delete from \"TBL_GROUP_RIGHT\" where \"GroupId\"='" + model.GroupId + "' and \"LayerId\" in (" + inStr1 + ")";
                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                    }
                    sql = "delete from \"TBL_GROUP_RIGHT\" where \"GroupId\"='" + model.GroupId + "' and \"LayerId\" in (" + inStr + ")";
                    db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                    //插入
                    if (insertData != null && insertData.Any())
                    {
                        foreach (var item in insertData)
                        {
                            sql = "insert into  \"TBL_GROUP_RIGHT\"(\"Id\",\"GroupId\",\"LayerId\",\"IsDownload\",\"IsBrowse\") values('" + item.Id + "','" + item.GroupId + "','" + item.LayerId + "','" + item.IsDownload + "','" + item.IsBrowse + "')";
                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw;
                }
            }
        }


        public void IntsertGroupUser(string groupId, IEnumerable<string> userIds)
        {
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();
            //删除表中已经存在的记录
            if (userIds != null && userIds.Any())
            {
                var inStr = string.Join(",", userIds.Select(p => "'" + p + "'"));
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        var sql = "delete from \"TBL_GROUP_USER\" where \"GroupId\"='" + groupId + "' and \"UserId\" in (" + inStr + ")";
                        db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);

                        //插入

                        foreach (var item in userIds)
                        {
                            sql = "insert into  \"TBL_GROUP_USER\"(\"Id\",\"GroupId\",\"UserId\") values('" + Guid.NewGuid().ToString() + "','" + groupId + "','" + item + "')";
                            db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql);
                        }

                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }


        public PagedResultOutput<SystemUserDto> GetUserGroupRelatedUsers(string groupId, string userName, int PageSize, int PageIndex)
        {
            var data = new PagedResultOutput<SystemUserDto>();
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();
            var query = from g in db.GroupUserEntities
                        join u in db.SysUser
                        on g.UserId equals u.Id into r
                        from row in r.DefaultIfEmpty()
                        select new SystemUserDto
                        {
                            CreateDT = row.CreateDT,
                            Department = row.Department,
                            Id = g.Id,
                            Password = row.Password,
                            Phone = row.Phone,
                            Position = row.Position,
                            Remark = row.Remark,
                            TelPhone = row.TelPhone,
                            UserCode = row.UserCode,
                            UserName = row.UserName,
                            UserSex = row.UserSex,
                            GroupId = g.GroupId
                        };
            if (!string.IsNullOrEmpty(groupId))
            {
                query = query.Where(p => p.GroupId != null && p.GroupId == groupId);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(p => p.UserName != null && p.UserName.ToLower().Contains(userName.ToLower()));
            }

            data.TotalCount = query.Count();
            data.Items = query.OrderByDescending(p => p.CreateDT).Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList();
            return data;
        }


        public PagedResultOutput<SystemUserDto> GetUserGroupAlternativeUsers(string groupId, string userName, int PageSize, int PageIndex)
        {
            var data = new PagedResultOutput<SystemUserDto>();
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();
            var where = "";
            if (!string.IsNullOrEmpty(userName))
            {
                where += " AND u.\"UserName\" like '%" + userName + "%'";
            }
            var sql = "SELECT" +
"	u.*, (" +
"		CASE" +
"		WHEN r.\"UserId\" != \'\'" +
"		AND r.\"UserId\" IS NOT NULL THEN" +
"			1" +
"		ELSE" +
"			0" +
"		END" +
"	) AS isRelated" +
" FROM" +
"	sdms_user u" +
" LEFT JOIN (" +
"	SELECT" +
"		\"UserId\"" +
"	FROM" +
"		\"TBL_GROUP_USER\" G" +
"	WHERE" +
"		G .\"GroupId\" = \'" + groupId + "\'" +
"	GROUP BY" +
"		G .\"GroupId\"," +
"		G .\"UserId\"" +
" ) r ON r.\"UserId\" = u.\"Id\"  where 1=1 " + where + " ";
            var query = db.Database.SqlQuery<SystemUserDto>(sql);

            data.TotalCount = query.Count();
            data.Items = query.OrderByDescending(p => p.CreateDT).Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList();
            return data;
        }


        public bool DelGroupUser(IEnumerable<string> ids)
        {
            var db = (InfoEarthFrameDbContext)_iGroupRepository.GetDbContext();
            //删除表中已经存在的记录
            if (ids != null && ids.Any())
            {
                var inStr = string.Join(",", ids.Select(p => "'" + p + "'"));
                var sql = "delete from \"TBL_GROUP_USER\" where \"Id\" in (" + inStr + ")";
                return db.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, sql) > 0;
            }

            return false;
        }


        public List<UserGroupMenuPermessionDto> GetUserGroupMenuPermession(string groupId)
        {
            var list = new List<UserGroupMenuPermessionDto>();
            foreach (var module in ConfigContext.Current.ModuleConfig.Modules)
            {
                list.Add(new UserGroupMenuPermessionDto
                {
                    HasPermession = ConfigContext.Current.PermessionConfig.HasModulePermession(groupId, module.ID),
                    Id = module.ID,
                    Name = module.Name,
                    ParentId = "",
                    Tag1 = "module",
                    Path = module.Path,
                    Icon = module.Icon
                });

                list.AddRange(module.Menus.Select(p => new UserGroupMenuPermessionDto
                {
                    Id = p.ID,
                    HasPermession = ConfigContext.Current.PermessionConfig.HasMenuPermession(groupId, module.ID, p.ID),
                    Name = p.Name,
                    ParentId = module.ID,
                    Tag = p.Url,
                    Tag1 = "menu",
                    Path = p.Path,
                    Icon = "",
                    Buttons = ConfigContext.Current.PermessionConfig.GetMenuButtonPermession(new string[]{groupId},p.Url)
                }));

                list.AddRange(GetUserGroupMenuChildPermession(groupId, module));
            }
            return list;
        }

        public List<UserGroupMenuPermessionDto> GetUserGroupMenuChildPermession(string groupId, Module parentModule)
        {
            var list = new List<UserGroupMenuPermessionDto>();


            foreach (var module in parentModule.Children)
            {
                list.Add(new UserGroupMenuPermessionDto
                {
                    HasPermession = ConfigContext.Current.PermessionConfig.HasModulePermession(groupId, module.ID),
                    Id = module.ID,
                    Name = module.Name,
                    ParentId = parentModule.ID,
                    Tag1 = "module",
                    Path = module.Path,
                    Icon = module.Icon
                });

                list.AddRange(module.Menus.Select(p => new UserGroupMenuPermessionDto
                {
                    Id = p.ID,
                    HasPermession = ConfigContext.Current.PermessionConfig.HasMenuPermession(groupId, module.ID, p.ID),
                    Name = p.Name,
                    ParentId = module.ID,
                    Tag = p.Url,
                    Tag1 = "menu",
                    Path = p.Path,
                    Icon = "",
                    Buttons = ConfigContext.Current.PermessionConfig.GetMenuButtonPermession(new string[] { groupId }, p.Url)
                }));

                list.AddRange(GetUserGroupMenuChildPermession(groupId, module));
            }


            return list;
        }

        public void SetMenuRight(SetMenuRightInput model)
        {
            //如果是模块授权
            if (model.CurrentType == "module")
            {
                //取消授权
                if (!model.HasPermession)
                {
                    //移除分组下的模块所有菜单权限(直接删掉当前模块)
                    ConfigContext.Current.PermessionConfig.RemoveModule(model.GroupId, model.Id, model.CurrentPath);
                    ConfigContext.Current.Save();
                }
                else
                {
                    ConfigContext.Current.PermessionConfig.AddModule(model.GroupId, model.Id, model.ParentId, model.CurrentPath);
                    ConfigContext.Current.Save();
                }
            }
            else
            {
                //取消授权
                if (!model.HasPermession)
                {
                    //移除分组下的模块单个菜单权限(直接删掉当前菜单)
                    ConfigContext.Current.PermessionConfig.RemoveMenu(model.GroupId, model.ParentId, model.Id);
                    ConfigContext.Current.Save();
                }
                else
                {
                    ConfigContext.Current.PermessionConfig.AddMenu(model.GroupId, model.ParentId, model.Id, model.CurrentPath);
                    ConfigContext.Current.Save();
                }
            }
        }

        public void AddOrEditMenu(SetMenuRightPreamInput model)
        {
            //如果是模块授权
            if (model.CurrentType == "module")
            {
                if (model.OperatingState.ToLower() == "insert")
                {
                    bool flag = ConfigContext.Current.ModuleConfig.AddModuleOperate(model.Id, model.ParentId, model.CurrentPath, model.Name, model.MenuValue, model.OperatingState);
                    if (flag)
                    {
                        ConfigContext.Current.Save();
                    }
                }
                else if (model.OperatingState.ToLower() == "update")
                {
                    bool flag = ConfigContext.Current.ModuleConfig.UpdateModuleOperate(model.Id, model.ParentId, model.CurrentPath, model.Name, model.MenuValue, model.OperatingState);
                    if (flag)
                    {
                        ConfigContext.Current.Save();
                    }
                }
            }
            else
            {
                if (model.OperatingState.ToLower() == "insert")
                {
                    bool flag = ConfigContext.Current.ModuleConfig.AddMenuOperate(model.Id, model.ParentId, model.CurrentPath, model.Name, model.MenuValue, model.OperatingState, model.SelectValueList);
                    if (flag)
                    {
                        ConfigContext.Current.Save();
                    }
                }
                else if (model.OperatingState.ToLower() == "update")
                {
                    bool flag = ConfigContext.Current.ModuleConfig.UpdateMenuOperate(model.Id, model.ParentId, model.CurrentPath, model.Name, model.MenuValue, model.OperatingState, model.SelectValueList);
                    if (flag)
                    {
                        ConfigContext.Current.Save();
                    }
                }
            }
        }

        public void DeleteMenu(string moduleId, string menuId)
        {
            ConfigContext.Current.ModuleConfig.DeleteMenu(moduleId, menuId);
            ConfigContext.Current.Save();
        }

        public InfoEarthFrame.Common.ModuleConfig.ParentMenu GetMenuInfo(SetMenuRightPreamInput model)
        {
            InfoEarthFrame.Common.ModuleConfig.ParentMenu data = new InfoEarthFrame.Common.ModuleConfig.ParentMenu();
            data = ConfigContext.Current.ModuleConfig.GetMenuDataInfo(model.Id, model.ParentId, model.CurrentPath, model.Name, model.MenuValue, model.OperatingState, model.CurrentType);
            return data;
        }


        public void SetMenuButtonRight(SetMenuButtonRightInput model)
        {
            ConfigContext.Current.PermessionConfig.SetMenuButton(model.GroupId, model.ModuleId, model.MenuId, model.ButtonName,model.HasPermession);
            ConfigContext.Current.Save();
        }

        public void SetMenuButtonArea(SetMenuButtonRightInput model)
        {
            bool flag= ConfigContext.Current.ModuleConfig.SetMenuButtonArea(model.ModuleId, model.MenuId, model.ButtonName, model.HasPermession);
            if (flag)
            {
                ConfigContext.Current.Save();
            }
        }
    }
}
