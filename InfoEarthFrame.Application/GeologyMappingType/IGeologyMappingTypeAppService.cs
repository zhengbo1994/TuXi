using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using InfoEarthFrame.Core;
using InfoEarthFrame.Common;

namespace InfoEarthFrame.Application
{
    public interface IGeologyMappingTypeAppService : IApplicationService
    {
        GeologyMappingTypeDto GetGeologyMappingType(string id);

        List<InfoEarthFrame.Core.GeologyMappingType> GetGeologyMappingTypeList();

        List<InfoEarthFrame.Core.GeologyMappingType> GetAreaGeologyMappingList(GeologyMappingTypeInput gmtInput);
        Task<List<GeologyMappingTypeOutput>> GetMappingTree(string userId);

        Tuple<bool, GeologyMappingType> Insert(GeologyMappingTypeDto dto);

        Tuple<bool, GeologyMappingType> Update(GeologyMappingTypeDto dto);

        bool IsClassNameExists(string parentId, string className);

        string GetClassName(string id);

        Task<bool> Delete(IEnumerable<string> ids);

        int ImportMappingType(RowData data);

        List<InfoEarthFrame.Core.GeologyMappingType> GetChildType(string parentId);
    }
}
