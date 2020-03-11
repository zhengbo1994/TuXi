using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using InfoEarthFrame.Core;
using InfoEarthFrame.DataManage.DTO;

namespace InfoEarthFrame.Application
{
    public interface IDataManageAppService : IApplicationService
    {
        IList<DataMain> GetAllList(string mappingTypeId,string userId);

        IList<DataMain> GetAllListByName(string name);
        DataMain InsertDataMain(DataMainDto entity);

        Task<bool> Delete(IEnumerable<string> ids);

        Task<bool> Delete(PackageCategoryDto dto);

        DataMain UpdateDataMain(DataMainDto entity);
        DataMain GetDataMain(string id);
        //bool DeleteDataMain(List<string> DataMainIDList);
        void InsertDataManageFile(Core.DataManageFile entity);

        bool UploadLayer(UploadLayerContext dto);

        bool UploadFile(UploadFileContext dto);

        bool UploadPackage(UploadPackageContext context);

        bool ChangePackageStatus(string mainId);

        bool RemovePackage(string mainId);
        List<UploadFileResult> GetErrorMsg(string mainId);
        //void UploadFile(UploadFileParamDto paramDto);
        /// <summary>
        /// 获取上传文件列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        GetDataMainListResultDto GetDataMainList(GetDataMainListParamDto param);
        /// <summary>
        /// 获取上传文件内可显示的文件列表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        List<GetDataFileListResultDto> GetFileList(GetDataFileListParamDto param);

        /// <summary>
        /// 获取元数据
        /// </summary>
        /// <param name="mainId"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        InfoEarthFrame.Common.Metadata GetMetaData(string mainId, string folderName);

        bool SaveMetaData(InfoEarthFrame.Common.Metadata model);
        ///// <summary>
        ///// 获取文件下载地址
        ///// </summary>
        ///// <param name="FileID"></param>
        ///// <returns></returns>
        //string GetFile(string FileID);
        ///// <summary>
        ///// 获得ZIP文件路径
        ///// </summary>
        ///// <param name="DataMainId"></param>
        ///// <returns></returns>
        //string GetZipFile(string DataMainId);
        ///// <summary>
        ///// 获得成果图目录
        ///// </summary>
        ///// <param name="DataMainId"></param>
        ///// <returns></returns>
        //string GetResultChart(string DataMainId);
        ///// <summary>
        ///// 获得成果图路径
        ///// </summary>
        ///// <param name="DataMainId"></param>
        ///// <returns></returns>
        //string GetResultChartFile(string DataMainId);
        ///// <summary>
        ///// 获取TileGroup目录
        ///// </summary>
        ///// <param name="DataMainId"></param>
        ///// <returns></returns>
        //string GetThumbnailTileGroup(string DataMainId, int type);

        ///// <summary>
        ///// 获取大小
        ///// </summary>
        ///// <param name="DataMainId"></param>
        ///// <returns>WIDTH,HEIGHT</returns>
        //string GetThumbnailSize(string DataMainId, int type);




        //#region 统计

        ///// <summary>
        ///// 统计地质环境概况（线图）
        ///// </summary>
        ///// <returns>   
        ///// 字符串List ,第1行年份，第2行完成，第3行发布 如
        ///// 2014，2015，2016
        ///// 5,2,8
        ///// 2,0,1
        ///// </returns>
        //List<string> SummarizeLine();

        ///// <summary>
        ///// 全国（概况）柱状图
        ///// </summary>
        ///// <returns>
        ///// 字符串List ,第1行验收版，第2行最终版 如
        ///// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
        ///// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
        ///// </returns>
        //List<string> CountrySummarize();

        ///// <summary>
        ///// 区域（概况）柱状图
        ///// </summary>
        ///// <returns>
        ///// 字符串List ,第1行验收版，第2行最终版 如
        ///// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
        ///// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
        ///// </returns>
        //List<string> AreaSummarize();

        ///// <summary>
        ///// 省域（概况）柱状图
        ///// </summary>
        ///// <returns>
        ///// 字符串List ,第1行验收版，第2行最终版 如
        ///// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
        ///// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
        ///// </returns>
        //List<string> ProvinceSummarize();

        ///// <summary>
        ///// 省域（概况）柱状图
        ///// </summary>
        ///// <returns>
        ///// 字符串List ,第1行各省数量 如
        ///// 黑龙江省：3：702fd19f-4dc2-4490-a63b-bcd15593d4c7.ade5429f-ff11-438d-bb2b-2af2549c2a2f.068f95b9-0595-4d45-b79e-09a186f08e60，北京市：1：702fd19f-4dc2-4490-a63b-bcd15593d4c7.ade5429f-ff11-438d-bb2b-2af2549c2a2f.068f95b9-0595-4d45-b79e-09a186f08e60
        ///// </returns>
        //List<string> ProvinceMap();

        ///// <summary>
        ///// 省的详细信息
        ///// </summary>
        ///// <param name="Paths">路径，ProvinceMap返回值里获得</param>
        ///// <returns>
        ///// 字符串List ,第1行验收版，第2行最终版 如
        ///// 地质灾害类:4,地下水类:0,矿山地质环境类:5,地质遗迹类:1,地质环境条件类:9
        ///// 地质灾害类:2,地下水类:1,矿山地质环境类:0,地质遗迹类:0,地质环境条件类:6
        ///// </returns>
        //List<string> ProvinceInfo(string Paths);


        //#endregion
    }
}
