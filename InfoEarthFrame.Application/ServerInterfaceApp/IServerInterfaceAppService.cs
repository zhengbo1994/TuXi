using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Core.Entities;
using InfoEarthFrame.ServerInterfaceApp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InfoEarthFrame.ServerInterfaceApp
{
    public interface IServerInterfaceAppService : IApplicationService
    {
        #region 数据接口
        byte[] GetProcessFile(string mapName, string tileMatrix, string tileCol, string tileRow);
        /// <summary>
        /// 根据地图名称查询图层列表
        /// </summary>
        /// <param name="mapName">地图名称</param>
        /// <returns></returns>
        ListResultOutput<LayerOutputDto> GetLayersByMapName(string mapName);
        /// <summary>
        /// 根据图层名称查询图层所有字段
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        ListResultOutput<FieldOutputDto> GetLayerFieldNameList(string layerName);
        /// <summary>
        /// 根据图层名称查询完整空间数据
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        string GetGeoJsonByLayerName(string layerName);

        /// <summary>
        /// 根据图层名称查询完整空间数据
        /// </summary>
        /// <param name="layerName"></param>
        /// <returns></returns>
        string GetGeoJsonByLayerName(string mainId,string layerName);
        /// <summary>
        /// 根据图层名称查询属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        string GetAttrByLayerName(string layerName);
        /// <summary>
        /// 根据图层名称查询属性信息[总数]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <returns></returns>
        string GetAttrByLayerNameCount(string layerName);
        /// <summary>
        /// 根据图层名称查询属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        string GetAttrByLayerNamePage(string layerName, int pageSize, int pageIndex);
        /// <summary>
        /// 根据图层名称查询属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        string GetAttrByCondition(string layerName, string condition);
        /// <summary>
        /// 根据图层名称查询属性信息[总数]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <returns></returns>
        string GetAttrByConditionCount(string layerName, string condition);
        /// <summary>
        /// 根据图层名称查询属性信息(分页)
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="condition">查询条件</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        string GetAttrByConditionPage(string layerName, string condition, int pageSize, int pageIndex);
        /// <summary>
        /// 根据点查询图层属性信息
        /// </summary>
        /// <param name="layerId">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        string GetAttrByPt(string layerName, float lon, float lat);
        /// <summary>
        /// 根据点查询图层属性信息[总数]
        /// </summary>
        /// <param name="layerId">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        string GetAttrByPtCount(string layerName, float lon, float lat);
        /// <summary>
        /// 根据点查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerId">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        string GetAttrByPtPage(string layerName, float lon, float lat, int pageSize, int pageIndex);
        /// <summary>
        /// 根据中心点查询图层属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        string GetAttrByPtTolerane(string layerName, float lon, float lat, float distance);
        /// <summary>
        /// 根据中心点查询图层属性信息[总数]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        string GetAttrByPtToleraneCount(string layerName, float lon, float lat, float distance);
        /// <summary>
        /// 根据中心点查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        string GetAttrByPtToleranePage(string layerName, float lon, float lat, float distance, int pageSize, int pageIndex);
        /// <summary>
        /// 根据矩形查询图层属性信息
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        string GetAttrByRect(string layerName, float minLon, float minLat, float maxLon, float maxLat);
        /// <summary>
        /// 根据矩形查询图层属性信息[总数]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        string GetAttrByRectCount(string layerName, float minLon, float minLat, float maxLon, float maxLat);
        /// <summary>
        /// 根据矩形查询图层属性信息[分页]
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页</param>
        /// <returns></returns>
        string GetAttrByRectPage(string layerName, float minLon, float minLat, float maxLon, float maxLat, int pageSize, int pageIndex);
        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <returns></returns>
        string GetAttrByNGon(string layerName, string wkt);
        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <returns></returns>
        string GetAttrByNGonCount(string layerName, string wkt);
        /// <summary>
        /// 根据多边形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="wkt"></param>
        /// <param name="pageSize">页记录数</param>
        /// <param name="pageIndex">查询页序号</param>
        /// <returns></returns>
        string GetAttrByNGonPage(string layerName, string wkt, int pageSize, int pageIndex);
        /// <summary>
        /// 根据点生成图层文件
        /// </summary>
        /// <param name="layerId">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        string GetShpFileByPt(string layerName, float lon, float lat);
        /// <summary>
        /// 根据中心点生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <param name="distance">半径</param>
        /// <returns></returns>
        string GetShpFileByPtTolerane(string layerName, float lon, float lat, float distance);
        /// <summary>
        /// 根据矩形生成图层文件
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="minLon">最小经度</param>
        /// <param name="maxLon">最大经度</param>
        /// <param name="minLat">最小纬度</param>
        /// <param name="maxLat">最大纬度</param>
        /// <returns></returns>
        string GetShpFileByRect(string layerName, float minLon, float minLat, float maxLon, float maxLat);
        /// <summary>
        /// 根据导入数据操作图层数据
        /// </summary>
        /// <param name="layerName">图层名称</param>
        /// <param name="dataType">insert,update,delete</param>
        /// <param name="layerData">数据json串</param>
        /// <returns>返回操作异常，无异常返回空</returns>
        string LayerDataImport(dynamic obje);

        #endregion

        #region 服务接口
        /// <summary>
        /// 获取类型下，相关地图服务接口
        /// </summary>
        /// <param name="mapIds">地图编号</param>
        /// <param name="serverTypes">服务类型集合</param>
        /// <returns></returns>
        string GetServerInterfaceDictionaryByTypes(string mapIds, string serverTypes);
        /// <summary>
        /// 查询提供的服务接口
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <param name="serverType">服务类型</param>
        /// <returns></returns>
        ListResultOutput<ServerInterfaceOutputDto> GetServerInterface(string mapId, string serverType);
        /// <summary>
        /// 查询提供的数据接口
        /// </summary>
        /// <param name="mapId">地图编号</param>
        /// <param name="serverType">服务类型</param>
        /// <returns></returns>
        ListResultOutput<ServerInterfaceOutputDto> GetDataInterface(string mapId, string serverType);
        #endregion

        #region [地下水服务接口]

        /// <summary>
        /// 查询MapGIS转换ArcGIS文件下载
        /// </summary>
        /// <param name="fileName">mapgis压缩包文件名</param>
        /// <returns>返回文件压缩包下载路径</returns>
        string GetMapGISToArcGIS(string fileName);

        /// <summary>
        /// ArcGIS文件入库
        /// </summary>
        /// <param name="fileName">arcgis压缩包文件名</param>
        /// <returns>返回成功提示</returns>
        string GetArcGISToDB(string fileName);

        /// <summary>
        /// MapGIS文件入库
        /// </summary>
        /// <param name="fileName">mapgis压缩包文件名</param>
        /// <returns>返回成功提示</returns>
        string GetMapGISToDB(string fileName);

        #endregion

        #region [其它特殊接口]

        /// <summary>
        /// ArcGIS文件入库
        /// </summary>
        /// <param name="filePath">arcgis压缩包物理路径</param>
        /// <returns>返回成功提示</returns>
        string GetArcgisFileToDB(string filePath);

        /// <summary>
        /// 删除一个图层要素
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="elementId"></param>
        /// <returns></returns>
        bool DeleteLayerElement(string userCode, string layerId, string elementId);

        /// <summary>
        /// 添加一个图层要素
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="cols"></param>
        /// <param name="values"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        bool AddLayerElement(string userCode,string layerId, string elementId, string[] values, string geometry);

        /// <summary>
        /// 编辑一个图层要素
        /// </summary>
        bool UpdateLayerElement(string userCode, string layerId, string elementId, string[] values, string geometry);

        /// <summary>
        /// 编辑一个要素的属性信息
        /// </summary>
        bool UpdateLayerElementAttribute(string userCode,string layerId, string elementId, string[] values);

        /// <summary>
        /// 编辑一个要素的空间信息
        /// </summary>
        /// <param name="layerId"></param>
        /// <param name="elementId"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        bool UpdateLayerElementGeometry(string layerId, string elementId, string geometry);

        /// <summary>
        /// mapgis文件转换arcgis文件接口
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        string[] Mapgis2Arcgis(string filename);

        /// <summary>
        /// 矢量裁剪
        /// </summary>
        /// <param name="sourceShp">原始shp文件</param>
        /// <param name="destShp">输出shp文件</param>
        /// <param name="cutshp">裁剪范围的shp文件</param>
        /// <returns></returns>
        bool ClipShp(string sourceShp, string destShp, string cutshp);

        /// <summary>
        /// 矢量裁剪
        /// </summary>
        /// <param name="sourceShp">原始shp文件</param>
        /// <param name="destShp">输出shp文件</param>
        /// <param name="wktString">裁剪范围的WKT字符串</param>
        /// <returns></returns>
        bool ClipShpByWKT(string sourceShp, string destShp, string wktString);

        /// <summary>
        /// 获取行政区划信息
        /// </summary>
        /// <param name="codeStr">行政区划编码，多个以逗号间隔</param>
        /// <param name="level">级别，省、市、县</param>
        /// <returns></returns>
        object GetDistrictWKTByCode(string codeStr, string level);
        /// <summary>
        /// 复制图层，属性数据不复制
        /// </summary>
        /// <param name="oldLayerId">图层编号</param>
        /// <param name="newLayerName">复制后的图层名称</param>
        /// <returns>新图层编号</returns>
        string CloneLayer(CloneLayerDto obj);
        /// <summary>
        /// 生成图层
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        string CreateLayer(CreateLayerDto dto);
        /// <summary>
        /// 发布地图
        /// </summary>
        /// <param name="mapDto">地图信息</param>
        /// <returns></returns>
        MapEntity PublishMap(PublishMapDto mapDto);
        #endregion
    }
}
