using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using InfoEarthFrame.Application.DataStyleApp.Dtos;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core.Entities;

namespace InfoEarthFrame.Application.DataStyleApp
{
    public interface IDataStyleAppService : IApplicationService
    {
        #region 自动生成
        ListResultOutput<DataStyleDto> GetAllList(string name);

        bool IsExists(string name);

        PagedResultOutput<DataStyleDto> GetAllListPage(DataStyleInputDto input, int PageSize, int PageIndex);

        Task<DataStyleOutputDto> GetDetailById(string id);

        Task<DataStyleOutputDto> GetDetailByLayerID(string layerID);

        Task<DataStyleDto> Insert(DataStyleInputDto input);

        Task<DataStyleDto> Update(DataStyleInputDto input);

        Task<bool> Delete(string id, string user);

        ListResultOutput<ColorRampInfo> GetRandomColorRamps();

        ColorRampInfo GetRandomColorRampsByName(string colorName, int count);

        ListResultOutput<ColorRampInfo> GetLinearColorRamps();

        /// <summary>
        /// 获取渐变颜色带
        /// </summary>
        /// <returns></returns>
        ColorRampInfo GetLinearColorRampsByName(string colorName, int count);

        /// <summary>
        /// 获取所有文件夹目录
        /// </summary>
        List<string> GetAllFolders();

        /// <summary>
        /// 新增文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        bool AddFolder(string folderName);

        /// <summary>
        /// 修改文件夹名称
        /// </summary>
        /// <param name="oldName">旧名称</param>
        /// <param name="newName">新名称</param>
        /// <returns></returns>
        bool UpdateFolder(string oldName, string newName);

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <returns></returns>
        bool DeleteFolder(string folderName);

        /// <summary>
        /// 获取指定目录所有文件
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <returns></returns>
        List<DataStyleOutputImgFileInfo> GetAllFiles(string folderName);

        /// <summary>
        /// 修改文件名称
        /// </summary>
        /// <param name="folderName">文件夹名称</param>
        /// <param name="oldName">旧文件名称</param>
        /// <param name="newName">新文件名称</param>
        /// <returns></returns>
        bool UpdateFile(string folderName, string oldName, string newName);

        /// <summary>
        ///  删除指定文件
        /// </summary>
        /// <param name="folder">文件夹名称</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        bool DeleteFile(string folderName, string fileName);
        #endregion

        /// <summary>
        /// 分页获取图层属性数据
        /// </summary>
        /// <param name="layerId">图层id</param>
        /// <param name="layerAttr">属性名</param>
        /// <param name="colorName">色带名称</param>
        /// <param name="colorType">色带类型</param>
        /// <param name="style">颜色字符串</param>
        /// <returns></returns>
        string GetDataAttributesPage(string layerId, string layerAttr, string colorName, string style, int PageSize, int PageIndex);

        /// <summary>
        /// 获取图层属性数据
        /// </summary>
        /// <param name="layerId">图层id</param>
        /// <param name="layerAttr">属性名</param>
        /// <param name="colorName">色带名称</param>
        /// <param name="colorType">色带类型</param>
        /// <param name="style">颜色字符串</param>
        /// <returns></returns>
        object GetDataAttributes(string layerId, string layerAttr, string colorName, string style);

        /// <summary>
        /// 获取图层属性数据
        /// </summary>
        /// <param name="styleId">样式ID</param>
        /// <returns></returns>
        object GetDataAttributesById(string styleId);
        string GetXmlContent(DataStyleInputDto input);

        void InsertGeoServerStyle(DataStyleInputDto input);

        void InsertStyle(string styleName, string styleContent);

        Task<PagedResultOutput<DataStyleOutputDto>> GetDataStylePageListByCondition(QueryDataStyleInputParamDto input);

        //Task<DataStyleDto> Insert(DataStyleDto input);
        //Task<DataStyleDto> Update(DataStyleDto input);

        string GetSLDContentByLayerIdOrTableName(string layerId,string tableName);

        bool IsDataStyleExists(string styleName);
    }
}

