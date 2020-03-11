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

namespace InfoEarthFrame.Application.SystemUserApp
{
    public class SystemUserAppService : IApplicationService, ISystemUserAppService
    {
        #region 变量
        private readonly ISystemUserRepository _ISystemUserRepository;
        private readonly IAreaAppService _IAreaAppService;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public SystemUserAppService(ISystemUserRepository iSystemUserRepository)
        {
            _ISystemUserRepository = iSystemUserRepository;
            _IAreaAppService = new AreaAppService(iSystemUserRepository);
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<SystemUserDto>> GetAllList()
        {
            try
            {
                //var query = await _ISystemUserRepository.GetAllListAsync();
                var query =  _ISystemUserRepository.GetAllList();
                var list = new ListResultOutput<SystemUserDto>(query.MapTo<List<SystemUserDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 通过areaCode查询数据结果
        /// </summary>
        /// <param name="areaCode">登陆人对应权限编号</param>
        /// <returns></returns>
        public async Task<ListResultOutput<SystemUserDto>> GetAllListByCondition(SystemUserDto input)
        {
            try
            {
                List<SystemUserEntity> query = GetDataByUserCodeAsync(input.UserCode.Trim(), input.Department.Trim(), input.UserName.Trim()).Result;
                return new ListResultOutput<SystemUserDto>(query.MapTo<List<SystemUserDto>>());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 通过areaCode查询数据结果
        /// </summary>
        /// <param name="areaCode">登陆人对应权限编号</param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<PagedResultOutput<SystemUserDto>> GetAllPageListByCondition(SystemUserDto input, int PageSize, int PageIndex)
        {
            try
            {
                List<SystemUserEntity> query = GetDataByUserCodeAsync(input.UserCode.Trim(),input.Department.Trim(), input.UserName.Trim()).Result;
                int count = query.Count();
                var result = query.OrderByDescending(p=>p.CreateDT).Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                IReadOnlyList<SystemUserDto> ir;
                if (result != null && result.Count > 0)
                {
                    ir = result.MapTo<List<SystemUserDto>>();
                }
                else
                {
                    ir = new List<SystemUserDto>();
                }
                PagedResultOutput<SystemUserDto> outputList = new PagedResultOutput<SystemUserDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 通过userCode查询数据结果
        /// </summary>
        /// <param name="userCode">登陆人对应权限编号</param>
        /// <param name="department">部门</param>
        /// <param name="userName">名称</param>
        /// <returns></returns>
        private async Task<List<SystemUserEntity>> GetDataByUserCodeAsync(string userCode, string department, string userName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userCode))
                {
                    return null;
                }

                List<SystemUserEntity> query = null;
                if (userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    #region 超级用户
                    //query = await _ISystemUserRepository.GetAllListAsync();
                    query =  _ISystemUserRepository.GetAllList();
                    #endregion
                }
                else
                {
                    query = await GetUserEntityByUserCodeAsync(userCode);
                }
                if (query != null && query.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(department))
                    {
                        if (department != ConstHelper.CONST_SYSTEMCODE)
                        {
                            var areaData = _IAreaAppService.GetAreaListInfoByAreaCode(department);
                            if (areaData != null && areaData.Count > 0)
                            {
                                var areaCodes = areaData.Select(s => s.Code).ToArray();
                                query = query.Where(s => areaCodes.Contains(s.Department)).ToList();
                            }
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(userName))
                    {
                        query = query.Where(s => s.UserName.Contains(userName)).ToList();
                    }
                }
                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<SystemUserDto>> GetUserDataByUserCodeAsync(string userCode)
        {
            try
            {
                List<SystemUserEntity> userQuery = await GetUserEntityByUserCodeAsync(userCode);
                return userQuery.MapTo<List<SystemUserDto>>();
            }
            catch
            {
                return null;
            }
        }

        private async Task<List<SystemUserEntity>> GetUserEntityByUserCodeAsync(string userCode)
        {
            List<SystemUserEntity> userQuery = null;
            try
            {
                var userInfo = _ISystemUserRepository.FirstOrDefault(s => s.UserCode.Equals(userCode));
                if (userInfo != null)
                {
                    var areaData = _IAreaAppService.GetAreaListInfoByUserCode(userCode);
                    if (areaData != null && areaData.Count > 0)
                    {
                        var areaCodes = areaData.Select(s => s.Code).ToArray();
                        //userQuery = await _ISystemUserRepository.GetAllListAsync(s =>
                        //    s.Department.Equals(userInfo.Department) ? s.UserCode.Equals(userCode) : areaCodes.Contains(s.Department));

                        userQuery =  _ISystemUserRepository.GetAllList(s =>
                                s.Department.Equals(userInfo.Department) ? s.UserCode.Equals(userCode) : areaCodes.Contains(s.Department));
                    }
                }
            }
            catch
            {
                return null;
            }
            return userQuery;
        }

        /// <summary>
        /// 通过name或code查询数据结果
        /// </summary>
        /// <param name="input"></param>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public async Task<PagedResultOutput<SystemUserDto>> GetAllListByName(SystemUserDto input, int PageSize, int PageIndex)
        {
            try
            {
                string name = input.UserName;
                //var query = await _ISystemUserRepository.GetAllListAsync(q => (string.IsNullOrEmpty(name) ? true : (q.UserName.Contains(name) || q.UserCode.Contains(name))));
                //var query = _ISystemUserRepository.GetAllList(q => (string.IsNullOrEmpty(name) ? true : (q.UserName.Contains(name) || q.UserCode.Contains(name))));

                var expression = LinqExtensions.True<SystemUserEntity>();
                if (!string.IsNullOrEmpty(name))
                {
                    expression = expression.And(q => (q.UserName.Contains(name) || q.UserCode.Contains(name)));
                }
                var query = _ISystemUserRepository.GetAllList(expression);

                int count = query.Count();
                var result = query.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                IReadOnlyList<SystemUserDto> ir;
                if (result != null && result.Count > 0)
                {
                    ir = result.MapTo<List<SystemUserDto>>();
                }
                else
                {
                    ir = new List<SystemUserDto>();
                }
                PagedResultOutput<SystemUserDto> outputList = new PagedResultOutput<SystemUserDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<SystemUserOutputDto> GetDetailById(string id)
        {
                var query = await _ISystemUserRepository.GetAsync(id);
                var result = query.MapTo<SystemUserOutputDto>();
                return result;
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<SystemUserOutputDto> GetDetailByName(string username)
        {

                var query = await _ISystemUserRepository.FirstOrDefaultAsync(p => p.UserCode.ToLower() == username.ToLower());
                var result = query.MapTo<SystemUserOutputDto>();
                return result;
        }


        /// <summary>
        /// 根据帐号和密码查询
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public SystemUserOutputDto GetDetailByNamePassword(SystemUserDto input)
        {
            SystemUserOutputDto suod = new SystemUserOutputDto();
            try
            {
                string administrator = ConfigurationManager.AppSettings["administrator"].ToString();

                if (!string.IsNullOrEmpty(administrator) && administrator.ToLower().Contains(input.UserCode.ToLower()))
                {
                    int index = administrator.IndexOf(":");
                    string account = administrator.Substring(0, index);
                    string password = administrator.Substring(index + 1);

                    if (password == input.Password && account.ToLower() == input.UserCode.ToLower())
                    {
                        suod.Id = account;
                        suod.UserName = account;
                        suod.UserCode = account;
                        suod.Password = password;
                    }
                }


                if (string.IsNullOrEmpty(suod.Password))
                {
                    var query = _ISystemUserRepository.GetAllList().Where(q => q.UserCode.ToLower() == input.UserCode.ToLower()).FirstOrDefault();
                    var db = (InfoEarthFrameDbContext)_ISystemUserRepository.GetDbContext();

                    if (!string.IsNullOrEmpty(query.Id) &query.Password == input.Password)
                    {
                        suod.Id = query.Id;
                        suod.UserName = query.UserName;
                        suod.UserCode = query.UserCode;
                        suod.UserSex = query.UserSex;
                        suod.Password = input.Password;
                        suod.TelPhone = query.TelPhone;
                        suod.Phone = query.Phone;
                        suod.Department = query.Department;
                        suod.Position = query.Position;
                        suod.Remark = query.Remark;
                        suod.CreateDT = query.CreateDT;
                        suod.GroupIds=db.GroupUserEntities.Where(p => p.UserId == suod.Id).Select(p => p.GroupId).Distinct().ToArray();
                    }

                }

                return suod;
            }
            catch (Exception ex)
            {
                return suod;
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<SystemUserDto> Insert(SystemUserDto input)
        {
            try
            {
                SystemUserEntity entity = new SystemUserEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = input.UserName,
                    UserCode = input.UserCode,
                    UserSex = input.UserSex,
                    Password = input.Password,
                    TelPhone = input.TelPhone,
                    Phone = input.Phone,
                    Department = input.Department,
                    Position = input.Position,
                    Remark = input.Remark,
                    CreateDT = DateTime.Now
                };

                var db = _ISystemUserRepository.GetDbContext();
                var sql = _ISystemUserRepository.GenerateInsertSql(entity);
                var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
                if (!flag)
                {
                    return null;
                }
                var result = entity.MapTo<SystemUserDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        public async Task<SystemUserDto> Update(SystemUserDto input)
        {
            try
            {
                var oldModel =await GetDetailById(input.Id);
                if(oldModel==null)
                {
                    throw new Exception("表[sdms_user]未找到[Id='" + input.Id + "']的数据");
                }

                oldModel.UserName = input.UserName;
                oldModel.UserCode = input.UserCode;
                oldModel.UserSex = input.UserSex;
                oldModel.Password = input.Password;
                oldModel.TelPhone = input.TelPhone;
                oldModel.Phone = input.Phone;
                oldModel.Department = input.Department;
                oldModel.Position = input.Position;
                oldModel.Remark = input.Remark;

                var entity = oldModel.MapTo<SystemUserEntity>();
                var sql = _ISystemUserRepository.GenerateUpdateSql(entity);
                var db = _ISystemUserRepository.GetDbContext();
                var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
                if (!flag)
                {
                    return null;
                }
                var result = entity.MapTo<SystemUserDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<SystemUserDto> UpdatePassword(SystemUserDto input)
        {
            try
            {
                SystemUserEntity entity = _ISystemUserRepository.Get(input.Id);
                if (!string.IsNullOrEmpty(input.Password) && input.UserCode == input.UserCode)
                {
                    entity.Password = input.Password;
                }
                var query = await _ISystemUserRepository.UpdateAsync(entity);
                var result = entity.MapTo<SystemUserDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        public async Task Delete(string id)
        {
            try
            {
                await _ISystemUserRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion


        public bool IsUserCodeExists(string userCode)
        {
            var userInfo = _ISystemUserRepository.FirstOrDefault(s =>!string.IsNullOrEmpty(s.UserCode)&&s.UserCode.ToLower()==userCode.ToLower());
            return userInfo != null;
        }


        public async Task<bool> Delete(IEnumerable<string> ids)
        {
            var sql = "delete from \"sdms_user\" where \"Id\" in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
            var db = _ISystemUserRepository.GetDbContext();
            var flag = await db.Database.ExecuteSqlCommandAsync(sql) > 0;
            return flag;
        }


        public async Task<bool> ResetPassword(IEnumerable<string> ids,string password)
        {
            var sql = "update \"sdms_user\" set \"Password\"='" + password + "' where \"Id\" in (" + string.Join(",", ids.Select(p => "'" + p + "'")) + ")";
            var db = _ISystemUserRepository.GetDbContext();
            var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
            return flag;
        }


        public string GetAccessToken(string username, string password,string apiUrl,out string errMsg)
        {
            errMsg = "";
            try
            {
                return HttpClinetHelper.PostResponse(apiUrl, string.Format("username={0}&password={1}&grant_type=password", username, password));
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return "";
            }
        }

        public bool GetLoginPasswordByUserCode(string password, string currentUserName)
        {
            var flag = false;

            var query = _ISystemUserRepository.GetAllList().Where(p => p.UserCode.Equals(currentUserName)).FirstOrDefault();
            if (query==null)
            {
                return flag;
            }
            else
            {
                var dataPassword = query.Password;
                if (dataPassword.ToLower() == password.ToLower())
                {
                    return true;
                }
            }
            return flag;
        }

        public async Task<bool> ChangePassword(string password, string currentUserName)
        {
            var sql = "update \"sdms_user\" set \"Password\"='" + password + "' where \"UserCode\" ='" + currentUserName + "'";
            var db = _ISystemUserRepository.GetDbContext();
            var flag = (await db.Database.ExecuteSqlCommandAsync(sql)) > 0;
            return flag;
        }
    }
}

