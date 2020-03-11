using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Application.OperateLogApp.Dtos;
using InfoEarthFrame.Application.SystemUserApp.Dtos;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.Core.Repositories;
using InfoEarthFrame.ShpFileReadLogApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace InfoEarthFrame.ShpFileReadLogApp
{
    class ShpFileReadLogAppService : IApplicationService,IShpFileReadLogAppService
    {
        #region 变量
        private readonly IShpFileReadLogRepository _IShpFileReadLogRepository;
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public ShpFileReadLogAppService(
            IShpFileReadLogRepository iShpFileReadLogRepository)
        {
            _IShpFileReadLogRepository = iShpFileReadLogRepository;
        }


        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        public async Task<PagedResultOutput<ShpFileReadLogOutputDto>> GetGeologgerPageListByCondition(QueryShpFileReadLogInputParamDto input)
        {
            try
            {
                IEnumerable<ShpFileReadLogEntity> query = _IShpFileReadLogRepository.GetAll();

                // 条件过滤
                if (input.Createby != null && input.Createby.Trim().Length > 0)
                {
                    query = query.Where(p => p.CreateBy.ToUpper().Contains(input.Createby.ToUpper()));
                }
                if (input.Readstatus!=0)
                {
                    query = query.Where(p => p.ReadStatus==input.Readstatus);
                }
                if (input.Message != null && input.Message.Trim().Length > 0)
                {
                    query = query.Where(p => p.Message.ToUpper().Contains(input.Message.ToUpper()));
                }
                if (input.StartDate != null)
                {
                    query = query.Where(s => s.CreateDT >= input.StartDate.Value.AddHours(-1));
                }
                if (input.EndDate != null)
                {
                    query = query.Where(s => s.CreateDT <= input.EndDate.Value.AddDays(1).AddHours(-1));
                }

                int count = 0;
                List<ShpFileReadLogEntity> result = null;
                if (query != null)
                {
                    count = query.Count();
                    result = query.OrderByDescending(p => p.CreateDT).Skip((input.pageIndex - 1) * input.pageSize).Take(input.pageSize).ToList();
                }

                IReadOnlyList<ShpFileReadLogOutputDto> ir;
                if (result != null)
                {
                    ir = result.MapTo<List<ShpFileReadLogOutputDto>>();
                }
                else
                {
                    ir = new List<ShpFileReadLogOutputDto>();
                }
                PagedResultOutput<ShpFileReadLogOutputDto> outputList = new PagedResultOutput<ShpFileReadLogOutputDto>(count, ir);

                return outputList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
