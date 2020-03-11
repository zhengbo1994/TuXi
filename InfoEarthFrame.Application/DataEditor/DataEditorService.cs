using InfoEarthFrame.Application;
using InfoEarthFrame.Application.LayerContentApp;
using InfoEarthFrame.Common;
using InfoEarthFrame.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoEarthFrame.DataEditor
{
    public class DataEditorService : IDataEditorService
    {
        private readonly IDataManageAppService _dataManageAppService;
        private readonly ILayerContentAppService _layerContentAppService;
        public DataEditorService(IDataManageAppService dataManageAppService, ILayerContentAppService layerContentAppService)
        {
            this._dataManageAppService = dataManageAppService;
            this._layerContentAppService = layerContentAppService;
        }
        public IList<ZTreeItem> GetMapLayerTree(string mappingTypeId,string userId)
        {
            var result = new List<Common.ZTreeItem>();
            if (string.IsNullOrEmpty(mappingTypeId))
            {
                return result;
            }

            //首先获取图件
            var maps = _dataManageAppService.GetAllList(mappingTypeId,userId);

            //获取图层
            if (maps != null && maps.Count > 0)
            {
                foreach (var map in maps)
                {
                    var next= _layerContentAppService.GetLayers(map.Id, "").Select(p => new ZTreeItem
                        {
                            children = new List<ZTreeItem>(),
                            name = p.LayerName,
                            id = p.LayerAttrTable,
                            pId = map.Id,
                             isParent=false,
                             tag=p.Id,
                              tag1=p.LayerDefaultStyle,
                              tag2=p.MinX,
                              tag3=p.MinY
                        }).ToList();
                    var item = new ZTreeItem
                    {
                        id = map.Id,
                        name = map.Name,
                        children =next,
                        pId="",
                        isParent=next!=null&&next.Any()
                    };

                    result.Add(item);
                }
            }

            return result;
        }


        public DataTable GetLayerElementAttrs(string layerId, string elementId)
        {
            var sql = "select * from " + layerId + " where guid='" + elementId + "'";
            var db = new PostgrelVectorHelper();
            return db.getDataTable(sql);
        }
    }
}
