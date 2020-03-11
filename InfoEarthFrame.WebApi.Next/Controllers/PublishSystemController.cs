using Abp.Application.Services.Dto;
using InfoEarthFrame.Application;
using InfoEarthFrame.Application.MapReleationApp;
using InfoEarthFrame.Common;
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
    public class PublishMapDto
    {
        public string MapId { get; set; }
    }
    public class PublishSystemController : BaseApiController
    {
        private readonly ILayerListAppService _layerListAppService;
        private readonly ILayerManagerAppService _layerManagerAppService;
        private readonly IMapReleationAppService _mapReleationAppService;
        public PublishSystemController(ILayerListAppService layerListAppService, ILayerManagerAppService layerManagerAppService, IMapReleationAppService mapReleationAppService)
        {
            this._layerListAppService = layerListAppService;
            this._layerManagerAppService = layerManagerAppService;
            this._mapReleationAppService = mapReleationAppService;
        }

        /// <summary>
        /// 获取发布后的地图列表
        /// </summary>
        /// <param name="mappingTypeID">地图种类ID</param>
        /// <param name="mappingClassName">地图种类名称</param>
        /// <param name="name">地图名称</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [ResponseType(typeof(PagedResultOutput<LayerListOutput>))]
        public IHttpActionResult GetPublishedMapList(string mappingTypeID, string mappingClassName,string name, int? pageSize = 10, int? pageIndex = 1)
        { 
            var dto=new LayerListInput{
             MappingTypeID=mappingTypeID,
              MappingClassName=mappingClassName,
               PageSize=pageSize.Value,
                PageIndex=pageIndex.Value,
                  Name=name
            };
            var result=_layerListAppService.GetPageListAndCount(CurrentUserId, dto);
            return Ok(GetResult(0, new
            {
                total = dto.Total,
                items = result
            }));
        }

        /// <summary>
        /// 获取发布后的地图树
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(IList<ZTreeItem>))]
        public IHttpActionResult GetPublishedMapTree()
        {
            var data= _layerManagerAppService.GetServiceZTreeData(CurrentUserId);
            return Ok(GetResult( 0, data));
        }

        /// <summary>
        /// 获取发布后的地图服务
        /// </summary>
        /// <param name="mainId">主数据ID</param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetPublishedMap(string mainId)
        {
            var data = await _layerManagerAppService.GetDetailByMainId(mainId);
            return Ok(GetResult(0, data));
        }

        public IHttpActionResult PublishMap([FromBody]PublishMapDto dto)
        {
            var flag = _mapReleationAppService.PublicMap(dto.MapId);
            return Ok(GetResult(flag));
        }
    }
}
