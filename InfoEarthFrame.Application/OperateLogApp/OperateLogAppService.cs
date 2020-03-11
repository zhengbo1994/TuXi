using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using InfoEarthFrame.DataManage.DTO;

namespace InfoEarthFrame.Application.OperateLogApp
{
    class OperateLogAppService : IApplicationService, IOperateLogAppService
    {
        #region 变量
        private readonly IOperateLogRepository _IOperateLogRepository;
        private readonly ISystemUserRepository _ISystemUserRepository;
        private readonly IAreaAppService _IAreaAppService;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public OperateLogAppService(IOperateLogRepository iOperateLogRepository,
            ISystemUserRepository iSystemUserRepository,
            IAreaAppService iAreaAppService)
        {
            _IOperateLogRepository = iOperateLogRepository;
            _ISystemUserRepository = iSystemUserRepository;
            _IAreaAppService = iAreaAppService;
        }


        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        public async Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByCondition(QueryOperateLogInputDto input, int pageSize, int pageIndex)
        {
            try
            {
                List<OperateLogEntity> query = GetAllListByCondition(input).Result;
                int count = 0;
                List<OperateLogEntity> result = null;
                if (query != null)
                {
                    count = query.Count();
                    result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }

                IReadOnlyList<OperateLogOutputDto> ir;
                if (result != null)
                {
                    ir = result.MapTo<List<OperateLogOutputDto>>();
                }
                else
                {
                    ir = new List<OperateLogOutputDto>();
                }
                PagedResultOutput<OperateLogOutputDto> outputList = new PagedResultOutput<OperateLogOutputDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据条件获取所有数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        private async Task<List<OperateLogEntity>> GetAllListByCondition(QueryOperateLogInputDto input)
        {
            try
            {
                // 打开Translate.xml文件获取翻译结果
                XElement xElement = XElement.Load(ConfigHelper.TranslateFilePath);
                XElement OperateLog = xElement.Descendants("Class").FirstOrDefault(s => s.Attribute("name").Value.Equals("OperateLog"));
                List<TranslateOutput> listTranslate = new List<TranslateOutput>();
                OperateLog.Descendants("word").ToList().ForEach(s =>
                {
                    listTranslate.Add(GetTranslateOutput(s));
                });
                bool bIsEnglish = ConfigHelper.IsEnglish == "1" ? true : false;

                // 系统功能
                string SystemFunc = null;
                switch (input.SystemFunc)
                {
                    case "1":
                        SystemFunc = GetTranslate(1001, bIsEnglish, listTranslate);
                        break;
                    case "2":
                        SystemFunc = GetTranslate(1002, bIsEnglish, listTranslate);
                        break;
                    case "3":
                        SystemFunc = GetTranslate(1003, bIsEnglish, listTranslate);
                        break;
                    default:
                        break;
                }

                // 操作类型
                string OperateType = null;
                switch (input.OperateType)
                {
                    case "1":
                        OperateType = GetTranslate(1101, bIsEnglish, listTranslate);
                        break;
                    case "2":
                        OperateType = GetTranslate(1102, bIsEnglish, listTranslate);
                        break;
                    case "3":
                        OperateType = GetTranslate(1103, bIsEnglish, listTranslate);
                        break;
                    case "4":
                        OperateType = GetTranslate(1104, bIsEnglish, listTranslate);
                        break;
                    case "5":
                        OperateType = GetTranslate(1105, bIsEnglish, listTranslate);
                        break;
                    case "6":
                        OperateType = GetTranslate(1106, bIsEnglish, listTranslate);
                        break;
                    default:
                        break;
                }

                // 操作时间
                DateTime? StartTime = string.IsNullOrEmpty(input.StartTime) ? null : (DateTime?)Convert.ToDateTime(input.StartTime + " 00:00:00");
                DateTime? EndTime = string.IsNullOrEmpty(input.EndTime) ? null : (DateTime?)Convert.ToDateTime(input.EndTime + " 23:59:59");

                // 查询用户
                List<string> UserCode = new List<string>();

                if (string.IsNullOrEmpty(input.QueryId))
                {
                    List<SystemUserEntity> queryUser = GetAllUserByUserAreaCode(input.OperateId, null).Result;
                    if (queryUser == null) return null;
                    if (queryUser.Count() == 0) return null;

                    foreach (SystemUserEntity element in queryUser)
                    {
                        UserCode.Add(element.UserCode);
                    }
                }
                else
                {
                    JavaScriptSerializer json = new JavaScriptSerializer();
                    UserCode = json.Deserialize<List<string>>(input.QueryId);
                }

                //List<OperateLogEntity> query = await _IOperateLogRepository.GetAllListAsync(s => (UserCode.Contains(s.UserCode)
                //    && (string.IsNullOrEmpty(SystemFunc) ? true : s.SystemFunc.Equals(SystemFunc))
                //    && (string.IsNullOrEmpty(OperateType) ? true : s.OperateType.Equals(OperateType))
                //    && (string.IsNullOrEmpty(input.StartTime) ? true : s.OperateTime > StartTime)
                //    && (string.IsNullOrEmpty(input.EndTime) ? true : s.OperateTime < EndTime)));

                var expression = LinqExtensions.True<OperateLogEntity>();
                expression = expression.And(s => UserCode.Contains(s.UserCode));
                if (!string.IsNullOrEmpty(SystemFunc))
                {
                    expression = expression.And(s => s.SystemFunc.Equals(SystemFunc));
                }
                if (!string.IsNullOrEmpty(OperateType))
                {
                    expression = expression.And(s => s.OperateType.Equals(OperateType));
                }
                if (!string.IsNullOrEmpty(input.StartTime))
                {
                    expression = expression.And(s => s.OperateTime > StartTime);
                }
                if (!string.IsNullOrEmpty(input.EndTime))
                {
                    expression = expression.And(s => s.OperateTime < EndTime);
                }

                List<OperateLogEntity> query = _IOperateLogRepository.GetAllList(expression);
                return query.OrderByDescending(s => s.OperateTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 根据LayerID获取分页数据
        /// </summary>
        /// <param name="layerID">图层ID</param>
        public async Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByLayerID(string layerID, int pageSize, int pageIndex)
        {
            try
            {
                List<OperateLogEntity> query = GetAllListByLayerID(layerID).Result;
                int count = query.Count();
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

                IReadOnlyList<OperateLogOutputDto> ir;
                if (result != null && result.Count > 0)
                {
                    ir = result.MapTo<List<OperateLogOutputDto>>();
                }
                else
                {
                    ir = new List<OperateLogOutputDto>();
                }
                PagedResultOutput<OperateLogOutputDto> outputList = new PagedResultOutput<OperateLogOutputDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据LayerID获取所有数据
        /// </summary>
        /// <param name="layerID">图层ID</param>
        private async Task<List<OperateLogEntity>> GetAllListByLayerID(string layerID)
        {
            try
            {
                //List<OperateLogEntity> query = await _IOperateLogRepository.GetAllListAsync(s => s.LayerID.Equals(layerID));
                List<OperateLogEntity> query = _IOperateLogRepository.GetAllList(s => s.LayerID.Equals(layerID));
                return query.OrderByDescending(s => s.OperateTime).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据AreaCode获取所有权限下用户
        /// </summary>
        /// <param name="areaCode">登陆人对应权限编号</param>
        public async Task<PagedResultOutput<SystemUserDto>> GetPageUserByAreaCode(QueryUserInputDto input, int pageSize, int pageIndex)
        {
            try
            {
                List<SystemUserEntity> query = GetAllUserByUserAreaCode(input.UserCode, input.AreaCode).Result;

                if (!string.IsNullOrEmpty(input.UserName))
                {
                    query = query.Where(s => s.UserName.Contains(input.UserName)).ToList();
                }

                int count = query.Count();
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

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
        /// 通过UserCode和AreaCode获取权限内所有用户
        /// </summary>
        /// <param name="userCode">登陆人ID</param>
        /// <param name="areaCode">区域编号（即Department）</param>
        private async Task<List<SystemUserEntity>> GetAllUserByUserAreaCode(string userCode, string areaCode)
        {
            try
            {
                List<string> listArea = GetAreaByUserAreaCode(userCode, areaCode);
                if (listArea == null) return null;
                if (listArea.Count == 0) return null;

                //List<SystemUserEntity> query = await _ISystemUserRepository.GetAllListAsync(s => listArea.Contains(s.Department));
                List<SystemUserEntity> query = _ISystemUserRepository.GetAllList(s => listArea.Contains(s.Department));

                if ((userCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMNAME) && string.IsNullOrWhiteSpace(areaCode))
                    || (string.IsNullOrWhiteSpace(userCode) && areaCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMCODE))
                    || (userCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMNAME) && areaCode.ToUpper().Equals(ConstHelper.CONST_SYSTEMCODE)))
                {
                    SystemUserEntity entity = new SystemUserEntity
                    {
                        Id = "",
                        UserName = string.IsNullOrWhiteSpace(userCode) ? ConstHelper.CONST_SYSTEMNAME : userCode,
                        UserCode = string.IsNullOrWhiteSpace(userCode) ? ConstHelper.CONST_SYSTEMNAME : userCode,
                        UserSex = "",
                        Password = "",
                        TelPhone = "",
                        Phone = "",
                        Department = "",
                        Position = "",
                        Remark = "",
                        CreateDT = null
                    };
                    query.Add(entity);
                }

                return query;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        /// <summary>
        /// 通过UserCode和AreaCode获取权限内所有AreaCode
        /// </summary>
        /// <param name="userCode">登陆人ID</param>
        /// <param name="areaCode">区域编号（即Department）</param>
        private List<string> GetAreaByUserAreaCode(string userCode, string areaCode)
        {
            if (!string.IsNullOrWhiteSpace(userCode) && string.IsNullOrWhiteSpace(areaCode))
            {
                List<Area> listAreaByUser = _IAreaAppService.GetAreaListInfoByUserCode(userCode);
                List<string> listCodeByUser = new List<string>();
                foreach (Area element in listAreaByUser)
                {
                    listCodeByUser.Add(element.Code);
                }
                return listCodeByUser;
            }
            else if (string.IsNullOrWhiteSpace(userCode) && !string.IsNullOrWhiteSpace(areaCode))
            {
                List<Area> listAreaByArea = _IAreaAppService.GetAreaListInfoByAreaCode(areaCode);
                List<string> listCodeByArea = new List<string>();
                foreach (Area element in listAreaByArea)
                {
                    listCodeByArea.Add(element.Code);
                }
                return listCodeByArea;
            }
            else if (!string.IsNullOrWhiteSpace(userCode) && !string.IsNullOrWhiteSpace(areaCode))
            {
                List<Area> listAreaByUser = _IAreaAppService.GetAreaListInfoByUserCode(userCode);
                List<string> listCodeByUser = new List<string>();
                foreach (Area element in listAreaByUser)
                {
                    listCodeByUser.Add(element.Code);
                }

                List<Area> listAreaByArea = _IAreaAppService.GetAreaListInfoByAreaCode(areaCode);
                List<string> listResult = new List<string>();
                foreach (Area element in listAreaByArea)
                {
                    if (listCodeByUser.Contains(element.Code))
                    {
                        listResult.Add(element.Code);
                    }
                }
                return listResult;
            }

            return null;
        }

        /// <summary>
        /// 通过UserCode获取权限内所有用户的UserCode
        /// </summary>
        /// <param name="userCode">登陆人ID</param>
        public List<string> GetAllUserCodeByUserCode(string userCode)
        {
            try
            {
                List<SystemUserEntity> query = GetAllUserByUserAreaCode(userCode, null).Result;
                if (query == null) return null;
                if (query.Count() == 0) return null;

                List<string> listUserCode = new List<string>();
                foreach (SystemUserEntity element in query)
                {
                    listUserCode.Add(element.UserCode);
                }
                return listUserCode;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public OperateLogOutputDto WriteOperateLog(string LayerID, string userCode, int systemFunc, int operateType, int result, int resultDescribe, string LayerName)
        {
                string name = string.Empty;
                if (!string.IsNullOrEmpty(userCode) && userCode.ToUpper() == ConstHelper.CONST_SYSTEMNAME)
                {
                    name = ConstHelper.CONST_SYSTEMNAME;
                }
                else
                {
                    //List<SystemUserEntity> queryUser = await _ISystemUserRepository.GetAllListAsync(s => s.UserCode.Equals(userCode));
                    List<SystemUserEntity> queryUser = _ISystemUserRepository.GetAllList(s => s.UserCode.Equals(userCode));
                    if (queryUser.Count > 0)
                    {
                        name = queryUser[0].UserName;
                    }
                }

                // 打开Translate.xml文件获取翻译结果
                XElement xElement = XElement.Load(ConfigHelper.TranslateFilePath);
                XElement OperateLog = xElement.Descendants("Class").FirstOrDefault(s => s.Attribute("name").Value.Equals("OperateLog"));
                List<TranslateOutput> listTranslate = new List<TranslateOutput>();
                OperateLog.Descendants("word").ToList().ForEach(s =>
                {
                    listTranslate.Add(GetTranslateOutput(s));
                });
                bool bIsEnglish = ConfigHelper.IsEnglish == "1" ? true : false;


                OperateLogEntity entity = new OperateLogEntity
                {
                    Id = System.Guid.NewGuid().ToString(),
                    LayerID = LayerID,
                    OperateTime = DateTime.Now,
                    UserName = name,
                    UserCode = userCode,
                    SystemFunc = GetTranslate(systemFunc, bIsEnglish, listTranslate),
                    OperateType = GetTranslate(operateType, bIsEnglish, listTranslate),
                    Result = GetTranslate(result, bIsEnglish, listTranslate),
                    ResultDescribe = GetTranslate(resultDescribe, bIsEnglish, listTranslate) + LayerName
                };

                var db =(InfoEarthFrameDbContext) _IOperateLogRepository.GetDbContext();
                db.OperateLog.Add(entity);
                db.SaveChanges();

                return entity.MapTo<OperateLogOutputDto>();
  
        }

        /// <summary>
        /// 根据元素节点,上级code 赋值对象
        /// </summary>
        /// <param name="data"></param>
        private TranslateOutput GetTranslateOutput(XElement data)
        {
            try
            {
                TranslateOutput obj = new TranslateOutput();
                obj.Code = data.Attribute("Code").Value;
                obj.Chinese = data.Attribute("Chinese").Value;
                obj.English = data.Attribute("English").Value;
                return obj;
            }
            catch { return null; }
        }


        /// <summary>
        /// 返回Code对应的中英文
        /// </summary>
        /// <param name="code"></param>
        /// <param name="bIsEnglish">是否输出英语</param>
        /// <param name="list">输入列表</param>
        private string GetTranslate(int code, bool bIsEnglish, List<TranslateOutput> list)
        {
            var result = list.FirstOrDefault(s => s.Code.Equals(Convert.ToString(code)));
            return bIsEnglish ? result.English : result.Chinese;
        }

        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        public async Task<PagedResultOutput<OperateLogOutputDto>> GetPageListByParamCondition(QueryOperateLogInputParamDto input, int pageIndex, int pageSize)
        {
            try
            {
                IEnumerable<OperateLogEntity> query = _IOperateLogRepository.GetAll();

                // 条件过滤
                if (input.UserName != null && input.UserName.Trim().Length > 0)
                {
                    query = query.Where(p => p.UserName.ToUpper().Contains(input.UserName.ToUpper()));
                }
                if (input.SystemFunc != null && input.SystemFunc.Trim().Length > 0)
                {
                    query = query.Where(p => p.SystemFunc.ToUpper().Contains(input.SystemFunc.ToUpper()));
                }
                if (input.OperateType != null && input.OperateType.Trim().Length > 0)
                {
                    query = query.Where(p => p.OperateType.ToUpper().Contains(input.OperateType.ToUpper()));
                }
                if (input.StartDate != null)
                {
                    query = query.Where(s => s.OperateTime >= input.StartDate.Value.AddHours(-1));
                }
                if (input.EndDate != null)
                {
                    query = query.Where(s => s.OperateTime <= input.EndDate.Value.AddDays(1).AddHours(-1));
                }

                int count = 0;
                List<OperateLogEntity> result = null;
                if (query != null)
                {
                    count = query.Count();
                    result = query.OrderByDescending(p=>p.OperateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                }

                IReadOnlyList<OperateLogOutputDto> ir;
                if (result != null)
                {
                    ir = result.MapTo<List<OperateLogOutputDto>>();
                }
                else
                {
                    ir = new List<OperateLogOutputDto>();
                }
                PagedResultOutput<OperateLogOutputDto> outputList = new PagedResultOutput<OperateLogOutputDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
