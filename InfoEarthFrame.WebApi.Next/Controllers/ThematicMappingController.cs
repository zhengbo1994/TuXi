using InfoEarthFrame.Application;
using InfoEarthFrame.Common;
using InfoEarthFrame.Core;
using InfoEarthFrame.DrawingOutput;
using InfoEarthFrame.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfoEarthFrame.EntityFramework.Repositories;
using InfoEarthFrame.EntityFramework;
using Abp.EntityFramework.Repositories;
using System.Web.Http;
using Infoearth.Application.Entity.DrawingOutput.Dtos;
using Infoearth.Util;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Collections;
using System.Web.Http.Description;
using InfoEarthFrame.Application.MapApp;

namespace InfoEarthFrame.WebApi.Next.Controllers
{
    public class ThematicMappingController : BaseApiController
    {
        private readonly IDrawingEntityRepository _drawingEntityRepository;
        private readonly IMapAppService _mapService;
        public ThematicMappingController(IDrawingEntityRepository drawingEntityRepository,IMapAppService mapService)
        {
            this._drawingEntityRepository = drawingEntityRepository;
            this._mapService = mapService;
        }

        /// <summary>
        /// 保存制图信息
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public IHttpActionResult SaveDrawingInfo(DrawingConfigDTO info)
        {
            var entity = new DrawingEntity();
            entity.Id = Guid.NewGuid().ToString();
            if (!entity.CREATETIME.HasValue)
            {
                entity.CREATETIME = DateTime.Now;
            }
            if (info.Layers != null)
            {
                foreach (var item in info.Layers)
                {
                    if (string.IsNullOrEmpty(item.LayerUrl))
                    {
                        item.LayerUrl = ConfigContext.Current.DefaultConfig["geoserver:WMS"];
                    }
                    item.LayerType = "WMS";
                }
            }
            entity.DRAWINGNAME = info.DrawingName;
            entity.STAUE = "0";
            entity.USERID = CurrentUserId;
            entity.USERNAME = CurrentUserName;
            entity.PARA = JsonConvert.SerializeObject(info);

            var db = (InfoEarthFrameDbContext)_drawingEntityRepository.GetDbContext();

            db.DrawingEntities.Add(entity);
            var flag = db.SaveChanges() > 0;
            if (flag)
            {
                //消息队列发消息
                MqConfigInfo config = new MqConfigInfo();
                config.MQExChange = "DrawingOutput";
                config.MQQueueName = "DrawingOutput";
                config.MQRoutingKey = "DrawingOutput";

                try
                {
                    MqHelper heper = new MqHelper(config);
                    byte[] body = Encoding.UTF8.GetBytes(entity.Id);
                    heper.SendMsg(body);
                    //此时可以消息通知
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return Ok(GetResult(flag));
        }



        /// <summary>
        /// 查询制图输出信息
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <returns></returns>
       [ResponseType(typeof(LayuiGridResult))]
        public IHttpActionResult GetList(string name,DateTime? startDate,DateTime? endDate,int? pageSize = 10, int? pageIndex = 1)
        {
            var db = (InfoEarthFrameDbContext)_drawingEntityRepository.GetDbContext();
            var count = 0;
            var query = db.DrawingEntities.AsQueryable();
            query = query.Where(t => t.USERID == CurrentUserId||t.USERNAME==CurrentUserName);
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(t => t.DRAWINGNAME != null && t.DRAWINGNAME.ToLower() == name.ToLower());
            }
            if (startDate.HasValue)
            {
                query = query.Where(t => t.CREATETIME!=null&&t.CREATETIME.Value>=startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(t => t.CREATETIME != null && t.CREATETIME.Value <= endDate.Value);
            }

            count = query.Count();
            var result = query.OrderByDescending(t => t.CREATETIME).Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value).ToList();
            List<DrawingDTO> listResult = new List<DrawingDTO>();
            foreach (var entity in result)
            {
                DrawingDTO dto = new DrawingDTO()
                {
                    ID = entity.Id,
                    USERID = entity.USERID,
                    BZ = entity.BZ,
                    COMPLETE = entity.COMPLETE,
                    CREATETIME = entity.CREATETIME,
                    DRAWINGNAME = entity.DRAWINGNAME,
                    ERRORMSG = entity.ERRORMSG,
                    JD = entity.JD,
                    OUTPUTPATH = entity.OUTPUTPATH,
                    PARA = entity.PARA,
                    STAUE = entity.STAUE,
                    USERNAME = entity.USERNAME
                };
                listResult.Add(dto);
            }
            var data = new LayuiGridResult
            {
                Message = "",
                Rows = new LayuiGridData
                {
                    Items = listResult
                },
                Status = 0,
                Total = count
            };
            return Ok(data);
        }


       public async Task<IHttpActionResult> GetMapDetail(string id)
       {
           var data= await _mapService.GetDetailById(id);
           return Ok(GetResult(0, data));
       }
    }
}
