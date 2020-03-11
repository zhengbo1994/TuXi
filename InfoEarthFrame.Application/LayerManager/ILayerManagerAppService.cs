using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using AutoMapper;
using InfoEarthFrame.Common;
using InfoEarthFrame.Common.Model;
using InfoEarthFrame.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.Application
{
    public interface ILayerManagerAppService : IApplicationService
    {
        Task<object> GetAllList();
        Task<ListResultOutput<LayerManagerDto>> GetPageList(QueryLayerManagerInput input);
        Task<PagedResultOutput<LayerManagerDto>> GetPageListAndCount(QueryLayerManagerInput input);
        Task<int> GetPageCount(QueryLayerManagerInput input);
        Task<PagedResultOutput<LayerManagerDto>> Intsert(int pageIndex, int pageSize, LayerManagerInput input);
        Task<PagedResultOutput<LayerManagerDto>> Update(int pageIndex, int pageSize, LayerManagerDto input);
        Task<PagedResultOutput<LayerManagerDto>> Delete(string id, int pageIndex, int pageSize);
        Task<ListResultOutput<LayerManagerDto>> GetLayerManagerByPID();

        Task<bool> Insert(Tbl_LayerManager model);

        Task<bool> Update(Tbl_LayerManager model);

        Task<Tbl_LayerManager> GetDetailById(string id);

        Task<Tbl_LayerManager> GetDetailByMainId(string mainId);

        Task<bool> Delete(IEnumerable<string> ids);

        List<LayuiSelectItem> GetServiceSelectTreeData(string name,string userId);

        IList<ZTreeItem> GetServiceZTreeData(string userId);

        int GetServerTypeForReleaseNumber(string className);
    }
}
