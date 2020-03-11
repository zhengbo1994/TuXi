using InfoEarthFrame.Application.DataStyleApp;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Common;
using InfoEarthFrame.WebApi.Next.Models;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class DataStyleController : BaseApiController
    {
        protected override string ModuleName
        {
            get
            {
                return "DataStyle";
            }
        }


        private readonly IDataStyleAppService _dataStyleAppService;
        private readonly ILog _logger = LogManager.GetLogger(typeof(DataStyleController));

        public DataStyleController(IDataStyleAppService dataStyleAppService)
        {
            this._dataStyleAppService = dataStyleAppService;
        }


        /// <summary>
        /// 根据条件获取分页数据
        /// </summary>
        /// <param name="input">查询条件的类</param>
        [ResponseType(typeof(LayuiGridResult))]
        public async Task<IHttpActionResult> GetDataStylePageListByCondition(string StyleName, string StyleType, string Createby, DateTime? StartDate, DateTime? EndDate, int pageIndex, int pageSize)
        {
            QueryDataStyleInputParamDto input = new QueryDataStyleInputParamDto
            {
                StyleName = StyleName,
                StyleType = StyleType,
                Createby = Createby,
                StartDate = StartDate,
                EndDate = EndDate,
                pageIndex = pageIndex,
                pageSize = pageSize
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
    }
}