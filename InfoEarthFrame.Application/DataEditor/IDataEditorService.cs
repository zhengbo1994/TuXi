using InfoEarthFrame.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DataEditor
{
    public interface IDataEditorService
    {
        /// <summary>
        /// 获取图件图层树
        /// </summary>
        /// <param name="mappingTypeId"></param>
        /// <returns></returns>
        IList<ZTreeItem> GetMapLayerTree(string mappingTypeId,string userId);

        DataTable GetLayerElementAttrs(string layerId, string elementId);
    } 
}
