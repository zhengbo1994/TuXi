using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using InfoEarthFrame.Core;

namespace InfoEarthFrame.Application
{
    public interface IDataConvertAppService : IApplicationService
    {

        List<InfoEarthFrame.Core.ConvertFile> GetConvertFilesList(string UserID);
        InfoEarthFrame.Core.ConvertFile GetConvertFiles(string Id);

        int GetConvertFilesNum(string UserID);
        void Insert(InfoEarthFrame.Core.ConvertFile entity);

        void UpdateState(string Id);

        /// <summary>
        /// /文件转换
        /// </summary>
        /// <param name="Files">文件列表</param>
        /// <param name="type">类型</param>
        /// <param name="IsZip">结果是否压缩</param>
        /// <returns></returns>

        ConvertResult DataConvert1(List<string> Files, string UserID, int type, string OutputCoordName, bool IsZip, string ConvertKey);
        ConvertResult DataConvert(List<ConvertFileList> fileList, string UserID, string OutputCoordName, bool IsZip);

        List<string> GetCoordList(List<string> fileList);

        List<string> GetlyrTypeList(List<string> fileList);

        /// <summary>
        /// 数据检查
        /// </summary>
        /// <param name="fileList">检查的文件列表</param>
        /// <returns></returns>

        DataCheckResult DataCheck(List<string> fileList, bool IsExcel = true);
        string[] Mapgis2Arcgis(string filename);

    }
}
