using InfoEarthFrame.Application.DataStyleApp;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Application.LayerContentApp;
using InfoEarthFrame.Application.LayerContentApp.Dtos;
using InfoEarthFrame.Application.LayerFieldApp;
using InfoEarthFrame.Application.OperateLogApp;
using InfoEarthFrame.Common;
using InfoEarthFrame.DataEditor;
using InfoEarthFrame.ServerInterfaceApp;
using InfoEarthFrame.ServerInterfaceApp.Dtos;
using InfoEarthFrame.WebApi.Next.Models;
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

    public class DataEditorController : BaseApiController
    {
        private readonly ILayerContentAppService _layerContentService;
        private readonly IDataEditorService _dataEditorService;
        private readonly IServerInterfaceAppService _serverInterfaceAppService;
        private readonly IDataStyleAppService _dataStyleAppService;
        private readonly ILayerFieldAppService _layerFieldAppService;
        private readonly IOperateLogAppService _operateLogAppService;
        public DataEditorController(ILayerContentAppService layerContentService, IDataEditorService dataEditorService, 
            IServerInterfaceAppService serverInterfaceAppService,
            IDataStyleAppService dataStyleAppService,
            ILayerFieldAppService layerFieldAppService,
            IOperateLogAppService operateLogAppService)
        {
            this._layerContentService = layerContentService;
            this._dataEditorService = dataEditorService;
            this._serverInterfaceAppService = serverInterfaceAppService;
            this._dataStyleAppService=dataStyleAppService;
            this._layerFieldAppService = layerFieldAppService;
            this._operateLogAppService=operateLogAppService;
        }

       /// <summary>
        /// 获取图层列表
       /// </summary>
       /// <param name="mainId">主数据ID</param>
       /// <param name="layerName">图层名称</param>
       /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetLayers(string mainId, string layerName="")
        { 
            var layers=_layerContentService.GetLayers(mainId, layerName);

            return Ok(GetResult(0, layers));
        }

        /// <summary>
        /// 获取图层树
        /// </summary>
        /// <param name="mappingTypeId">图件分类Id</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
        public IHttpActionResult GetLayerTree(string mappingTypeId)
        {
            var tree = _dataEditorService.GetMapLayerTree(mappingTypeId,CurrentUserId);

            return Ok(GetResult(0, tree));
        }

        /// <summary>
        /// 根据图层名称获取GEOJson
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
         public IHttpActionResult GetGeoJsonByLayerName(string mainId, string layerName)
         {
             var geoJson = _serverInterfaceAppService.GetGeoJsonByLayerName(mainId, layerName);

             return Ok(geoJson);
         }

        /// <summary>
        /// 获取样式
        /// </summary>
        /// <param name="createBy">创建人</param>
        /// <param name="styleName">样式名称</param>
        /// <param name="styleType">样式类型</param>
        /// <returns></returns>
        [ResponseType(typeof(LayuiGridResult))]
         public IHttpActionResult GetStyles(string createBy,string styleName,string styleType,string defaultStyleId,int? pageSize = 10, int? pageIndex = 1)
         {
             var dto = new DataStyleInputDto
             {
                 CreateBy = createBy,
                 StyleName = styleName,
                 StyleType = styleType,
                 DefaultStyleId = defaultStyleId
             };
           var result= _dataStyleAppService.GetAllListPage(dto, pageSize.Value, pageIndex.Value);
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
        /// 修改图层默认样式
        /// </summary>
        /// <param name="layerID">图层ID</param>
        /// <param name="styleID"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [ResponseType(typeof(ApiResult))]
        public IHttpActionResult UpdateDefaultStyle([FromBody]UpdateDefaultStyleDto dto)
        {
            var model = _layerContentService.UpdateDefaultStyle(dto.layerID, dto.styleID, dto.user);
          return Ok(GetResult(model != null));
        }

        /// <summary>
        /// 获取图层详细信息
        /// </summary>
        /// <param name="layerId">图层ID</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
        public async Task<IHttpActionResult> GetDetailById(string layerId)
        {
            var data =await _layerContentService.GetDetailById(layerId);
            return Ok(GetResult(data != null, data));
        }

        /// <summary>
        /// 获取图层字段
        /// </summary>
        /// <param name="layerId">图层ID</param>
        /// <returns></returns>
         [ResponseType(typeof(ApiResult))]
         public IHttpActionResult GetLayerFieldsById(string layerId)
         {
             var result = _layerFieldAppService.GetDetailByLayerID(layerId);
             var data = new LayuiGridResult
             {
                 Message = "",
                 Rows = new LayuiGridData
                 {
                     Items = result.Items
                 },
                 Status = 0,
                 Total = result.Items.Count
             };
             return Ok(data);
         }

        /// <summary>
         /// 获取图层属性表
        /// </summary>
        /// <param name="layerId">图层ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
         public IHttpActionResult GetLayerAttrTabledDetail(string layerId, int? pageSize = 10, int? pageIndex = 1)
         {
             int total;
             var result =_layerContentService.GetLayerAttrTabledDetail(layerId,pageSize.Value,pageIndex.Value,out total);
             var data = new LayuiGridResult
             {
                 Message = "",
                 Rows = new LayuiGridData
                 {
                     Items = result
                 },
                 Status = 0,
                 Total = total,
             };
             return Ok(data);
         }


        /// <summary>
        /// 新增图层要素属性
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
         public IHttpActionResult AddLayerElementAttribute([FromBody] LayerElementAttributeInputDto dto)
         {
            var flag= _serverInterfaceAppService.AddLayerElement(CurrentUserName, dto.LayerId,Guid.NewGuid().ToString(), dto.Values,dto.Geometry);
            return Ok(GetResult(flag));
         }

         /// <summary>
         /// 编辑图层要素属性
         /// </summary>
         /// <param name="dto"></param>
         /// <returns></returns>
         public IHttpActionResult UpdateLayerElementAttribute([FromBody] LayerElementAttributeInputDto dto)
         {
             var flag = _serverInterfaceAppService.UpdateLayerElementAttribute(CurrentUserName, dto.LayerId, dto.ElementId, dto.Values);
             return Ok(GetResult(flag));
         }

         /// <summary>
         /// 删除图层要素
         /// </summary>
         /// <param name="dto"></param>
         /// <returns></returns>
         public IHttpActionResult RemoveLayerElement([FromBody] LayerElementAttributeInputDto dto)
         {
             var flag = _serverInterfaceAppService.DeleteLayerElement(CurrentUserName,dto.LayerId, dto.ElementId);
             return Ok(GetResult(flag));
         }

        /// <summary>
        /// 获取图层日志
        /// </summary>
        /// <param name="layerId">图层ID</param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetLogsByLayerID(string layerId, int? pageSize = 10, int? pageIndex = 1)
        {
             int total;
             var result =await _operateLogAppService.GetPageListByLayerID(layerId, pageSize.Value, pageIndex.Value);
             var data = new LayuiGridResult
             {
                 Message = "",
                 Rows = new LayuiGridData
                 {
                     Items = result.Items
                 },
                 Status = 0,
                 Total = result.TotalCount,
             };
             return Ok(data);
        }


        public IHttpActionResult GetLayerElementAttrs(string layerId, string elementId)
        {
            var data = _dataEditorService.GetLayerElementAttrs(layerId, elementId);
            return Ok(GetResult(0, data));
        }


        public IHttpActionResult GetSLDContentByLayerIdOrTableName(string layerId="", string tableName="")
        {
            var data = _dataStyleAppService.GetSLDContentByLayerIdOrTableName(layerId, tableName);
            return Ok(GetResult(0, data));
        }
    }
}