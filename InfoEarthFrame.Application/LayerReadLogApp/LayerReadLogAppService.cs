using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.LayerReadLogApp.Dtos;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.Core.Entities;
using System.Linq;
using System.Configuration;

namespace InfoEarthFrame.Application.LayerReadLogApp
{
    public class LayerReadLogAppService : IApplicationService, ILayerReadLogAppService
    {
        private readonly IShpFileReadLogRepository _ILayerReadLogRepository;
        private readonly ILayerContentRepository _ILayerContentRepository;
        private readonly IDicDataCodeRepository _IDicDataCodeRepository;

        /// <summary>
        /// 构造函数
        /// </summary>
        public LayerReadLogAppService(IShpFileReadLogRepository iLayerReadLogRepository, ILayerContentRepository iLayerContentRepository, IDicDataCodeRepository iDicDataCodeRepository)
        {
            _ILayerReadLogRepository = iLayerReadLogRepository;
            _ILayerContentRepository = iLayerContentRepository;
            _IDicDataCodeRepository = iDicDataCodeRepository;
        }

        #region 自动生成
        /// <summary>
        /// 获取所有数据
        /// </summary>
        public async Task<ListResultOutput<LayerReadLogDto>> GetAllList()
        {
            try
            {
                //var query = await _ILayerReadLogRepository.GetAllListAsync();
                var query =  _ILayerReadLogRepository.GetAllList();
                var list = new ListResultOutput<LayerReadLogDto>(query.MapTo<List<LayerReadLogDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据编号获取数据
        /// </summary>
        public async Task<LayerReadLogOutputDto> GetDetailById(string id)
        {
            try
            {
                var query = await _ILayerReadLogRepository.GetAsync(id);
                var result = query.MapTo<LayerReadLogOutputDto>();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 根据帐号和图层ID获取通知
        /// </summary>
        public ListResultOutput<LayerReadLogOutputDto> GetDetailByLayer(LayerReadLogOutputDto input)
        {
            try
            {
                string layerID = input.LayerID, user = input.CreateBy;
                ListResultOutput<LayerReadLogOutputDto> list;
                var layer = _ILayerContentRepository.GetAll();
                //var readLog = _ILayerReadLogRepository.GetAll().Where(q => (string.IsNullOrEmpty(layerID) ? true : q.LayerID.Contains(layerID)) && (string.IsNullOrEmpty(user) ? true : (q.CreateBy.Contains(user) && q.MsgStatus.Value.Equals(0))));

                var readLog = _ILayerReadLogRepository.GetAll();
                if (string.IsNullOrEmpty(layerID))
                {
                    readLog = readLog.Where(q => (q.CreateBy.Contains(user) && q.MsgStatus == 1));
                }
                else
                {
                    readLog = readLog.Where(q => q.LayerID.Contains(layerID) && (q.CreateBy.Contains(user) && q.MsgStatus == 1));
                }
                var query = (from r in readLog
                             join l in layer on r.LayerID equals l.Id into rl
                             from rre in rl.DefaultIfEmpty()
                             select new LayerReadLogOutputDto
                             {
                                 Id = r.Id,
                                 LayerID = r.LayerID,
                                 LayerName = (rre == null) ? "" : rre.LayerName,
                                 ShpFileName = r.ShpFileName,
                                 ReadStatus = r.ReadStatus,
                                 Message = r.Message,
                                 CreateDT = r.CreateDT,
                                 CreateBy = r.CreateBy,
                                 ReadStartDT = r.ReadStartDT,
                                 ReadEndDT = r.ReadEndDT,
                                 MsgStatus = r.MsgStatus
                             }).OrderByDescending(x => x.CreateDT).ToList();

                list = new ListResultOutput<LayerReadLogOutputDto>(query.MapTo<List<LayerReadLogOutputDto>>());
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 查询所有列表日志
        /// </summary>
        /// <param name="PageSize"></param>
        /// <param name="PageIndex"></param>
        /// <returns></returns>
        public string GetAllListByPage(int PageSize, int PageIndex)
        {
            string result = "";
            try
            {
                result += "[";
                var layerList = _ILayerContentRepository.GetAll().OrderByDescending(q => q.CreateDT);
                var dataType = _IDicDataCodeRepository.GetAll();
                var query = (from l in layerList
                             join d in dataType on l.DataType equals d.CodeName into tt
                             from de in tt.DefaultIfEmpty()
                             select new LayerContentEntity
                             {
                                 Id = l.Id,
                                 LayerName = l.LayerName,
                                 DataType = (de == null) ? "" : de.CodeName,
                                 LayerDesc = l.LayerDesc,
                                 CreateDT = l.CreateDT,
                                 UploadStatus = l.UploadStatus,
                                 CreateBy = l.CreateBy
                             });
                int count = layerList.Count();
                var lst = layerList.Skip((PageIndex - 1) * PageSize).Take(PageSize).ToList();

                int layerIndex = 0;
                foreach (var layer in lst)
                {
                    var readLogList = _ILayerReadLogRepository.GetAllList().Where(q => q.LayerID == layer.Id).ToList();

                    string num = "0";
                    if (readLogList.Count > 0)
                    {
                        num = readLogList.Count.ToString();
                    }

                    #region [组json输出]

                    result += "{";

                    #region [图层实体]

                    result += "\"" + "layerName" + "\":" + "\"" + layer.LayerName + "\",\"" + "layerType" + "\":\"" + layer.LayerType + "\",\"" + "count" + "\":" + num + ",\"" + "createDT" + "\":\"" + layer.CreateDT.ToString() + "\",\"" + "createBy" + "\":\"" + layer.CreateBy.ToString() + "\",";

                    #endregion

                    #region [日志实体]

                    result += "\"" + "data" + "\":";

                    if (readLogList.Count > 0)
                    {
                        result += "[";
                        int intIndex = 0;
                        foreach (var item in readLogList)
                        {
                            result += "{";
                            string status = "";
                            switch (item.ReadStatus)
                            {
                                case 0:
                                    status = "等待导入";
                                    break;
                                case 1:
                                    status = "正在导入";
                                    break;
                                case 2:
                                    status = "导入成功";
                                    break;
                                case 3:
                                    status = "导入失败";
                                    break;
                            }
                            result += "\"" + "fileName" + "\":\"" + item.ShpFileName + "\",\"" + "readStatus" + "\":\"" + status + "\",\"" + "createDT" + "\":\"" + item.CreateDT.ToString() + "\"";

                            if (intIndex == readLogList.Count - 1)
                            {
                                result += "}";
                            }
                            else
                            {
                                result += "},";
                            }
                            intIndex++;
                        }
                        result += "]";
                    }
                    else
                    {
                        result += "[]";
                    }

                    #endregion

                    if (layerIndex == lst.Count - 1)
                    {
                        result += "}";
                    }
                    else
                    {
                        result += "},";
                    }

                    layerIndex++;

                    #endregion
                }
                result += "]";

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        public async Task<LayerReadLogDto> Insert(LayerReadLogInputDto input)
        {
            try
            {
                ShpFileReadLogEntity entity = new ShpFileReadLogEntity
                {
                    Id = input.Id,
                    LayerID = input.LayerID,
                    ShpFileName = input.ShpFileName,
                    ReadStatus = input.ReadStatus,
                    Message = input.Message,
                    CreateDT = input.CreateDT,
                    CreateBy = input.CreateBy,
                    ReadStartDT = input.ReadStartDT,
                    ReadEndDT = input.ReadEndDT,
                    MsgStatus = input.MsgStatus
                };
                var query = await _ILayerReadLogRepository.InsertAsync(entity);
                var result = entity.MapTo<LayerReadLogDto>();
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
        public async Task<LayerReadLogDto> Update(LayerReadLogInputDto input)
        {
            try
            {
                ShpFileReadLogEntity entity = new ShpFileReadLogEntity
                {
                    Id = input.Id,
                    LayerID = input.LayerID,
                    ShpFileName = input.ShpFileName,
                    ReadStatus = input.ReadStatus,
                    Message = input.Message,
                    CreateDT = input.CreateDT,
                    CreateBy = input.CreateBy,
                    ReadStartDT = input.ReadStartDT,
                    ReadEndDT = input.ReadEndDT,
                    MsgStatus = input.MsgStatus
                };
                var query = await _ILayerReadLogRepository.UpdateAsync(entity);
                var result = entity.MapTo<LayerReadLogDto>();
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
                await _ILayerReadLogRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 消息读取状态更新
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdataMsgReadStatus(string user)
        {
            bool flag = false;
            try
            {
                var list = _ILayerReadLogRepository.GetAllList().Where(q => q.CreateBy == user);

                foreach (var item in list)
                {
                    item.MsgStatus = 1;
                    item.MsgReadDT = System.DateTime.Now;
                    var query = await _ILayerReadLogRepository.UpdateAsync(item);
                }
                flag = true;

                return flag;
            }
            catch (Exception ex)
            {
                return flag;
            }
        }

        /// <summary>
        ///Node JS Server配置
        /// </summary>
        /// <returns></returns>
        public string GetNodeJSServerConfig()
        {
            //string socketServer = ConfigurationManager.AppSettings["NodeJSSocketServer"];
            //string module = ConfigurationManager.AppSettings["NodeJSMsgModule"];

            //return socketServer + "##" + module;
            return string.Empty;
        }

        #endregion
    }
}

